using UnityEngine;
using UnityEngine.SceneManagement;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Domain.States;

namespace PawnSurvivors.Managers
{
    /// <summary>
    /// 게임 전체 흐름을 중앙에서 관리하는 컨트롤러입니다.
    /// Stage → Shop → Stage 순환과 GameOver 처리를 담당합니다.
    ///
    /// 책임:
    /// - StageFlowUseCase 이벤트 구독 및 화면 전환 처리
    /// - 씬 전환 로직 중앙화
    /// - 게임 상태 흐름 관리
    /// </summary>
    public class GameFlowController : MonoBehaviour
    {
        public static GameFlowController Instance { get; private set; }

        private StageFlowUseCase _stageFlowUseCase;
        private bool _isInitialized = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// GameManager에서 UseCase가 준비된 후 호출합니다.
        /// </summary>
        public void Initialize(StageFlowUseCase stageFlowUseCase)
        {
            if (_isInitialized)
            {
                // 이미 초기화된 경우 이벤트만 재구독
                UnsubscribeEvents();
            }

            _stageFlowUseCase = stageFlowUseCase;
            SubscribeEvents();
            _isInitialized = true;

            LogManager.LogInfo(LogCategory.System, "[GameFlowController] 초기화 완료");
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (_stageFlowUseCase == null) return;

            _stageFlowUseCase.OnStageStartRequested += HandleStageStartRequested;
            _stageFlowUseCase.OnStageCompletedToShop += HandleStageCompletedToShop;
            _stageFlowUseCase.OnGameOver += HandleGameOver;
            _stageFlowUseCase.OnAllStagesCleared += HandleAllStagesCleared;
        }

        private void UnsubscribeEvents()
        {
            if (_stageFlowUseCase == null) return;

            _stageFlowUseCase.OnStageStartRequested -= HandleStageStartRequested;
            _stageFlowUseCase.OnStageCompletedToShop -= HandleStageCompletedToShop;
            _stageFlowUseCase.OnGameOver -= HandleGameOver;
            _stageFlowUseCase.OnAllStagesCleared -= HandleAllStagesCleared;
        }

        #region Event Handlers

        /// <summary>
        /// 스테이지 시작 요청 처리
        /// </summary>
        private void HandleStageStartRequested(string stageName)
        {
            LogManager.LogInfo(LogCategory.Stage, $"[GameFlowController] 스테이지 시작 요청: {stageName}");

            // 이미 StageScene에 있으면 씬 전환하지 않음
            if (SceneManager.GetActiveScene().name == "StageScene")
            {
                LogManager.LogInfo(LogCategory.Stage, "[GameFlowController] 이미 StageScene에 있음. 씬 전환 스킵.");
                return;
            }

            // StageState로 전환 (씬 전환 포함)
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.TransitionTo<StageState>();
            }
        }

        /// <summary>
        /// 스테이지 완료 → 상점으로 이동
        /// </summary>
        private void HandleStageCompletedToShop()
        {
            LogManager.LogInfo(LogCategory.Stage, "[GameFlowController] 스테이지 완료 → 상점으로 이동");

            // ShopState로 전환
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.TransitionTo<ShopState>();
            }
        }

        /// <summary>
        /// 게임 오버 처리
        /// </summary>
        private void HandleGameOver()
        {
            LogManager.LogInfo(LogCategory.Stage, "[GameFlowController] 게임 오버 → GameOverScene으로 이동");

            // GameOverState로 전환
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.TransitionTo<GameOverState>();
            }
        }

        /// <summary>
        /// 모든 스테이지 클리어 처리
        /// </summary>
        private void HandleAllStagesCleared()
        {
            LogManager.LogInfo(LogCategory.Stage, "[GameFlowController] 모든 스테이지 클리어! → GameOverScene으로 이동 (승리)");

            // GameOverState로 전환 (승리 상태)
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.TransitionTo<GameOverState>();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 새 게임 시작 (캐릭터 선택 후 호출)
        /// </summary>
        public void StartNewGame(string firstStageName = null)
        {
            if (_stageFlowUseCase == null)
            {
                LogManager.LogError(LogCategory.Stage, "[GameFlowController] StageFlowUseCase가 null입니다.");
                return;
            }

            _stageFlowUseCase.StartNewGame(firstStageName);
        }

        /// <summary>
        /// 현재 스테이지 재시작
        /// </summary>
        public void RestartCurrentStage()
        {
            if (_stageFlowUseCase == null)
            {
                LogManager.LogError(LogCategory.Stage, "[GameFlowController] StageFlowUseCase가 null입니다.");
                return;
            }

            _stageFlowUseCase.RestartStage();
        }

        /// <summary>
        /// 다음 스테이지로 이동 (상점에서 호출)
        /// </summary>
        public void ProceedToNextStage()
        {
            if (_stageFlowUseCase == null)
            {
                LogManager.LogError(LogCategory.Stage, "[GameFlowController] StageFlowUseCase가 null입니다.");
                return;
            }

            _stageFlowUseCase.ProceedToNextStage();
        }

        /// <summary>
        /// 스테이지 완료 처리 (StageScreen에서 호출)
        /// </summary>
        public void CompleteCurrentStage()
        {
            if (_stageFlowUseCase == null)
            {
                LogManager.LogError(LogCategory.Stage, "[GameFlowController] StageFlowUseCase가 null입니다.");
                return;
            }

            // CurrentState 확인 및 강제 설정
            if (_stageFlowUseCase.CurrentState != StageFlowUseCase.StageState.InProgress)
            {
                LogManager.LogWarning(LogCategory.Stage,
                    $"[GameFlowController] CurrentState가 InProgress가 아님 ({_stageFlowUseCase.CurrentState}). StartStage로 강제 설정 후 완료 처리.");

                string currentStageName = _stageFlowUseCase.GetCurrentStageName();
                _stageFlowUseCase.StartStage(currentStageName, resetSession: false);
            }

            _stageFlowUseCase.CompleteStage();
        }

        /// <summary>
        /// 스테이지 실패 처리 (플레이어 사망 시 호출)
        /// </summary>
        public void FailCurrentStage()
        {
            if (_stageFlowUseCase == null)
            {
                LogManager.LogError(LogCategory.Stage, "[GameFlowController] StageFlowUseCase가 null입니다.");
                return;
            }

            _stageFlowUseCase.FailStage();
        }

        /// <summary>
        /// 메인 메뉴로 돌아가기
        /// </summary>
        public void ReturnToMainMenu()
        {
            if (_stageFlowUseCase != null)
            {
                _stageFlowUseCase.ResetState();
            }

            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.TransitionTo<CharacterSelectState>();
            }
        }

        /// <summary>
        /// 다음 스테이지 이름 가져오기
        /// </summary>
        public string GetNextStageName()
        {
            return _stageFlowUseCase?.GetNextStageName();
        }

        /// <summary>
        /// 현재 스테이지 이름 가져오기
        /// </summary>
        public string GetCurrentStageName()
        {
            return _stageFlowUseCase?.GetCurrentStageName() ?? "Unknown";
        }

        /// <summary>
        /// 다음 스테이지가 있는지 확인
        /// </summary>
        public bool HasNextStage()
        {
            return !string.IsNullOrEmpty(GetNextStageName());
        }

        #endregion
    }
}

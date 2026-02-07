using UnityEngine;
using PawnSurvivors.Managers;
using PawnSurvivors.Domain.States;
using PawnSurvivors.UI.Factory;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 게임오버 화면입니다. Builder를 사용하여 UI를 생성합니다.
    /// </summary>
    public class GameOverScreen : MonoBehaviour
    {
        #region State
        private bool _isVictory = false;
        #endregion

        #region References
        private Canvas _canvas;
        #endregion

        #region UI
        private MenuUIBuilder.GameOverLayout _layout;
        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            SetupCanvas();
        }

        private void Start()
        {
            DetermineVictory();
            CreateUI();
            LoadStats();

            // 게임 일시정지
            if (GameManager.Instance?.LifecycleManager != null && !GameManager.Instance.LifecycleManager.IsPaused)
            {
                GameManager.Instance.LifecycleManager.TogglePause();
            }
        }

        private void OnDestroy()
        {
            // 게임 재개
            if (GameManager.Instance?.LifecycleManager != null && GameManager.Instance.LifecycleManager.IsPaused)
            {
                GameManager.Instance.LifecycleManager.TogglePause();
            }
        }

        #endregion

        #region Initialization

        private void SetupCanvas()
        {
            _canvas = UIFactory.SetupCanvas(gameObject);
        }

        private void DetermineVictory()
        {
            // StageFlowUseCase의 상태로 승패 판단
            if (GameManager.Instance?.StageFlowUseCase != null)
            {
                var state = GameManager.Instance.StageFlowUseCase.CurrentState;
                _isVictory = state == Domain.Usecases.StageFlowUseCase.StageState.AllCleared;
            }
        }

        private void CreateUI()
        {
            _layout = MenuUIBuilder.CreateGameOverLayout(
                transform,
                _isVictory,
                onRetry: OnRetryClicked,
                onMainMenu: OnMainMenuClicked
            );
        }

        private void LoadStats()
        {
            if (GameManager.Instance == null) return;

            // 통계 가져오기
            int kills = GameManager.Instance.KillTrackingUseCase?.GetTotalKills() ?? 0;
            float survivalTime = GameManager.Instance.SurvivalTimeTrackingUseCase?.GetCurrentSurvivalTime() ?? 0f;
            int gold = GameManager.Instance.CurrencyUseCase?.GetGold() ?? 0;

            _layout.SetStats(kills, survivalTime, gold);
        }

        #endregion

        #region Event Handlers

        private void OnRetryClicked()
        {
            // 게임 재개
            if (GameManager.Instance?.LifecycleManager != null && GameManager.Instance.LifecycleManager.IsPaused)
            {
                GameManager.Instance.LifecycleManager.TogglePause();
            }

            // 같은 캐릭터로 재시작
            GameManager.Instance?.ResetForRetry();

            // 첫 스테이지로 이동
            string firstStageName = "Stage_01";
            GameManager.Instance?.StageManagementUseCase?.PrepareStageStart(firstStageName, shouldResetSession: true);

            // StageState로 전환
            GameStateManager.Instance?.GoToStage();
        }

        private void OnMainMenuClicked()
        {
            // 게임 재개
            if (GameManager.Instance?.LifecycleManager != null && GameManager.Instance.LifecycleManager.IsPaused)
            {
                GameManager.Instance.LifecycleManager.TogglePause();
            }

            // 완전 초기화
            GameManager.Instance?.ResetForNewGame();

            // 캐릭터 선택 화면으로
            GameStateManager.Instance?.TransitionTo<CharacterSelectState>();
        }

        #endregion
    }
}

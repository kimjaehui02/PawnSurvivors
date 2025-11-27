using UnityEngine;
using PawnSurvivors.UI;
using PawnSurvivors.Domain.Usecases;

namespace PawnSurvivors.Managers
{
    /// <summary>
    /// 게임 상태를 관리하는 상태 머신입니다.
    /// 명확한 상태 전환과 상태별 동작을 보장합니다.
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        public enum GameState
        {
            Title,      // 타이틀 화면
            MainMenu,   // 메인 메뉴 (스테이지 선택)
            Stage,      // 스테이지 플레이 중
            Paused,     // 일시정지
            Shop,       // 상점
            GameOver,   // 게임 오버
            StageClear  // 스테이지 클리어
        }

        [SerializeField] private GameState _currentState = GameState.Title;
        public GameState CurrentState => _currentState;

        private GameState _previousState = GameState.Title;

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

        private void Start()
        {
            // UIManager가 먼저 초기화되도록 약간의 지연 후 초기 상태 설정
            // 또는 UIManager.Start()에서 GameStateManager가 있으면 초기화하지 않도록 처리
            if (UIManager.Instance != null)
            {
                ChangeState(GameState.Title);
            }
        }

        /// <summary>
        /// 상태를 변경합니다. 유효한 전환만 허용합니다.
        /// </summary>
        public bool ChangeState(GameState newState)
        {
            if (!IsValidTransition(_currentState, newState))
            {
                return false;
            }

            _previousState = _currentState;
            _currentState = newState;

            // 상태별 동작 실행
            OnStateEnter(_currentState);
            OnStateExit(_previousState);

            return true;
        }

        /// <summary>
        /// 이전 상태로 돌아갑니다.
        /// </summary>
        public void ReturnToPreviousState()
        {
            ChangeState(_previousState);
        }

        /// <summary>
        /// 유효한 상태 전환인지 확인합니다.
        /// </summary>
        private bool IsValidTransition(GameState from, GameState to)
        {
            // 같은 상태로 전환 불가
            if (from == to) return false;

            // 상태 전환 규칙
            switch (from)
            {
                case GameState.Title:
                    return to == GameState.MainMenu;

                case GameState.MainMenu:
                    return to == GameState.Stage || to == GameState.Title;

                case GameState.Stage:
                    return to == GameState.Paused || to == GameState.Shop || 
                           to == GameState.GameOver || to == GameState.StageClear || 
                           to == GameState.MainMenu;

                case GameState.Paused:
                    return to == GameState.Stage || to == GameState.MainMenu;

                case GameState.Shop:
                    return to == GameState.Stage || to == GameState.MainMenu;

                case GameState.GameOver:
                    return to == GameState.MainMenu || to == GameState.Stage;

                case GameState.StageClear:
                    return to == GameState.Shop || to == GameState.MainMenu;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 상태 진입 시 동작을 처리합니다.
        /// </summary>
        private void OnStateEnter(GameState state)
        {
            if (UIManager.Instance == null)
            {
                Debug.LogWarning("[GameStateManager] UIManager.Instance가 null입니다.");
                return;
            }

            switch (state)
            {
                case GameState.Title:
                    UIManager.Instance.ShowTitleScreen();
                    break;

                case GameState.MainMenu:
                    UIManager.Instance.ShowMainMenuScreen();
                    // 게임 상태 초기화
                    if (GameManager.Instance != null)
                    {
                        CleanupGameState();
                    }
                    break;

                case GameState.Stage:
                    UIManager.Instance.ShowStageScreen();
                    // 일시정지 해제 (일시정지나 상점에서 올 때)
                    if (_previousState == GameState.Paused || _previousState == GameState.Shop)
                    {
                        if (GameManager.Instance?.LifecycleManager != null)
                        {
                            if (GameManager.Instance.LifecycleManager.IsPaused)
                            {
                                GameManager.Instance.LifecycleManager.TogglePause();
                            }
                        }
                    }
                    
                    // 스테이지 시작 (일시정지에서 돌아오는 게 아닐 때만)
                    if (_previousState != GameState.Paused)
                    {
                        if (GameManager.Instance != null)
                        {
                            string stageName = GameManager.Instance?.StageManagementUseCase?.GetCurrentStageName() ?? "Stage1";
                            bool resetSession = _previousState == GameState.MainMenu || _previousState == GameState.Title;
                            GameManager.Instance.StartStage(stageName, resetSession);
                        }
                    }
                    break;

                case GameState.Paused:
                    UIManager.Instance.ShowPauseMenu();
                    // 게임 일시정지
                    if (GameManager.Instance?.LifecycleManager != null)
                    {
                        if (!GameManager.Instance.LifecycleManager.IsPaused)
                        {
                            GameManager.Instance.LifecycleManager.TogglePause();
                        }
                    }
                    break;

                case GameState.Shop:
                    UIManager.Instance.ShowShopScreen();
                    // 스테이지 종료 처리 (GameManager를 통해 UseCase 사용)
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.EndStage();
                    }
                    // 상점으로 갈 때 게임 일시정지
                    if (GameManager.Instance?.LifecycleManager != null)
                    {
                        if (!GameManager.Instance.LifecycleManager.IsPaused)
                        {
                            GameManager.Instance.LifecycleManager.TogglePause();
                        }
                    }
                    break;

                case GameState.GameOver:
                    UIManager.Instance.ShowGameOverScreen();
                    break;

                case GameState.StageClear:
                    UIManager.Instance.ShowStageClearScreen();
                    break;
            }
        }

        /// <summary>
        /// 상태 종료 시 동작을 처리합니다.
        /// </summary>
        private void OnStateExit(GameState state)
        {
            switch (state)
            {
                case GameState.Paused:
                    // 일시정지 해제는 스테이지로 돌아갈 때만 (OnStateEnter에서 처리)
                    // 여기서는 UI만 숨김
                    UIManager.Instance?.HidePauseMenu();
                    break;
            }
        }

        /// <summary>
        /// 게임 상태를 정리합니다 (메인 메뉴로 돌아갈 때).
        /// </summary>
        private void CleanupGameState()
        {
            // 일시정지 해제
            if (GameManager.Instance.LifecycleManager != null && 
                GameManager.Instance.LifecycleManager.IsPaused)
            {
                GameManager.Instance.LifecycleManager.TogglePause();
            }

            // 모든 Pawn 파괴
            if (GameManager.Instance.CreationManager != null)
            {
                GameManager.Instance.CreationManager.DestroyAllPawns();
            }

            // 스테이지 종료 (GameManager를 통해 UseCase 사용)
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndStage();
            }
        }

        /// <summary>
        /// ESC 키 입력을 처리합니다.
        /// </summary>
        public void HandleEscapeKey()
        {
            switch (_currentState)
            {
                case GameState.Paused:
                    // 일시정지 해제 -> 스테이지로
                    ChangeState(GameState.Stage);
                    break;

                case GameState.Stage:
                    // 일시정지 메뉴 열기
                    ChangeState(GameState.Paused);
                    break;

                case GameState.Shop:
                    // 상점에서 ESC -> 메인 메뉴로
                    ChangeState(GameState.MainMenu);
                    break;

                case GameState.GameOver:
                case GameState.StageClear:
                    // 메인 메뉴로
                    ChangeState(GameState.MainMenu);
                    break;
            }
        }

        // 편의 메서드들
        public void StartStage(string stageName = "Stage1")
        {
            if (GameManager.Instance != null)
            {
                // UseCase를 통해 스테이지 이름 설정 (리셋 없이)
                GameManager.Instance?.StageManagementUseCase?.PrepareStageStart(stageName, shouldResetSession: false);
            }
            ChangeState(GameState.Stage);
        }

        public void PauseGame()
        {
            if (_currentState == GameState.Stage)
            {
                ChangeState(GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            if (_currentState == GameState.Paused)
            {
                ChangeState(GameState.Stage);
            }
        }

        public void GoToShop()
        {
            ChangeState(GameState.Shop);
        }

        public void GoToMainMenu()
        {
            ChangeState(GameState.MainMenu);
        }
    }
}


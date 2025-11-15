using UnityEngine;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// UI 화면 전환을 관리하는 매니저입니다.
    /// GameManager와 같은 GameObject에 컴포넌트로 추가하세요.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Screens")]
        [SerializeField] private GameObject titleScreen;
        [SerializeField] private GameObject mainMenuScreen;
        [SerializeField] private GameObject stageScreen;
        [SerializeField] private GameObject pauseMenuScreen;
        [SerializeField] private GameObject gameOverScreen;
        [SerializeField] private GameObject stageClearScreen;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // 시작 시 타이틀 화면만 표시
            ShowTitleScreen();
        }

        private void Update()
        {
            // ESC 키 입력을 중앙에서 관리
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleEscapeKey();
            }
        }

        private void HandleEscapeKey()
        {
            Debug.Log("[UIManager] ESC key pressed");
            
            // 일시정지 메뉴가 열려있으면 닫기
            if (pauseMenuScreen != null && pauseMenuScreen.activeSelf)
            {
                Debug.Log("[UIManager] Pause menu is active, hiding it");
                HidePauseMenu();
            }
            // 스테이지 화면이 활성화되어 있으면 일시정지 메뉴 열기
            else if (stageScreen != null && stageScreen.activeSelf)
            {
                Debug.Log("[UIManager] Stage screen is active, showing pause menu");
                ShowPauseMenu();
            }
            else
            {
                Debug.Log($"[UIManager] No action - pauseMenu active: {pauseMenuScreen?.activeSelf}, stage active: {stageScreen?.activeSelf}");
            }
        }

        #region Screen Transition Methods
        
        /// <summary>
        /// 타이틀 화면을 표시합니다.
        /// </summary>
        public void ShowTitleScreen()
        {
            HideAllScreens();
            if (titleScreen != null)
                titleScreen.SetActive(true);
        }

        /// <summary>
        /// 메인 메뉴 화면을 표시합니다. (스테이지 선택)
        /// </summary>
        public void ShowMainMenuScreen()
        {
            HideAllScreens();
            if (mainMenuScreen != null)
                mainMenuScreen.SetActive(true);
        }

        /// <summary>
        /// 스테이지 화면을 표시합니다. (게임 플레이 HUD)
        /// </summary>
        public void ShowStageScreen()
        {
            HideAllScreens();
            if (stageScreen != null)
                stageScreen.SetActive(true);
        }

        /// <summary>
        /// 일시정지 메뉴를 표시합니다.
        /// </summary>
        public void ShowPauseMenu()
        {
            Debug.Log($"[UIManager] ShowPauseMenu called - pauseMenuScreen is {(pauseMenuScreen == null ? "NULL" : pauseMenuScreen.activeSelf ? "already active" : "inactive")}");
            
            if (pauseMenuScreen != null)
            {
                pauseMenuScreen.SetActive(true);
                Debug.Log("[UIManager] PauseMenuScreen SetActive(true) completed");
                
                // LifecycleManager를 통해 일시정지
                if (GameManager.Instance?.LifecycleManager != null)
                {
                    if (!GameManager.Instance.LifecycleManager.IsPaused)
                    {
                        GameManager.Instance.LifecycleManager.TogglePause();
                        Debug.Log("[UIManager] Game paused");
                    }
                    else
                    {
                        Debug.Log("[UIManager] Game was already paused");
                    }
                }
            }
            else
            {
                Debug.LogError("[UIManager] pauseMenuScreen is NULL! Please assign it in the Inspector.");
            }
        }

        /// <summary>
        /// 일시정지 메뉴를 숨기고 게임을 재개합니다.
        /// </summary>
        public void HidePauseMenu()
        {
            if (pauseMenuScreen != null)
            {
                pauseMenuScreen.SetActive(false);
                
                // LifecycleManager를 통해 게임 재개
                if (GameManager.Instance?.LifecycleManager != null)
                {
                    if (GameManager.Instance.LifecycleManager.IsPaused)
                    {
                        GameManager.Instance.LifecycleManager.TogglePause();
                    }
                }
            }
        }

        /// <summary>
        /// 게임 오버 화면을 표시합니다.
        /// </summary>
        public void ShowGameOverScreen()
        {
            HideAllScreens();
            if (gameOverScreen != null)
                gameOverScreen.SetActive(true);
        }

        /// <summary>
        /// 스테이지 클리어 화면을 표시합니다.
        /// </summary>
        public void ShowStageClearScreen()
        {
            HideAllScreens();
            if (stageClearScreen != null)
                stageClearScreen.SetActive(true);
        }

        /// <summary>
        /// 메인 메뉴로 돌아갑니다. (게임 종료)
        /// </summary>
        public void ReturnToMainMenu()
        {
            // 게임 상태 초기화
            if (GameManager.Instance != null)
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

                // 스테이지 종료
                if (GameManager.Instance.StageManager != null)
                {
                    GameManager.Instance.StageManager.EndStage();
                }
            }

            ShowMainMenuScreen();
        }

        private void HideAllScreens()
        {
            if (titleScreen != null) titleScreen.SetActive(false);
            if (mainMenuScreen != null) mainMenuScreen.SetActive(false);
            if (stageScreen != null) stageScreen.SetActive(false);
            if (pauseMenuScreen != null) pauseMenuScreen.SetActive(false);
            if (gameOverScreen != null) gameOverScreen.SetActive(false);
            if (stageClearScreen != null) stageClearScreen.SetActive(false);
        }

        #endregion
    }
}


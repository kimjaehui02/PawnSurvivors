using UnityEngine;
using PawnSurvivors.Managers;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// UI 화면 전환을 관리하는 매니저입니다.
    /// GameManager와 같은 GameObject에 컴포넌트로 추가하세요.
    /// GameStateManager와 함께 작동합니다.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Screens")]
        [SerializeField] private GameObject titleScreen;
        [SerializeField] private GameObject campaignSelectScreen;
        [SerializeField] private GameObject stageScreen;
        [SerializeField] private GameObject pauseMenuScreen;
        [SerializeField] private GameObject gameOverScreen;
        [SerializeField] private GameObject stageClearScreen;
        [SerializeField] private GameObject shopScreen;
        [SerializeField] private GameObject characterSelectScreen;

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
            ShowTitleScreen();
        }

        private void Update()
        {
            // ESC 키 입력 처리
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleEscapeKey();
            }
        }

        private void HandleEscapeKey()
        {
            // 일시정지 메뉴가 열려있으면 닫기
            if (pauseMenuScreen != null && pauseMenuScreen.activeSelf)
            {
                HidePauseMenu();
            }
            // 스테이지 화면이 활성화되어 있으면 일시정지 메뉴 열기
            else if (stageScreen != null && stageScreen.activeSelf)
            {
                ShowPauseMenu();
            }
            // 상점 화면이 활성화되어 있으면 일시정지 메뉴 열기
            else if (shopScreen != null && shopScreen.activeSelf)
            {
                ShowPauseMenu();
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
        public void ShowCampaignSelectScreen()
        {
            HideAllScreens();
            
            if (campaignSelectScreen == null)
            {
                GameObject campaignObj = new GameObject("CampaignSelectScreen");
                campaignObj.AddComponent<CampaignSelectScreen>();
                campaignSelectScreen = campaignObj;
                campaignSelectScreen.transform.SetParent(transform);
                campaignSelectScreen.SetActive(true);
            }
            else
            {
                campaignSelectScreen.SetActive(true);
            }
        }

        /// <summary>
        /// 캐릭터 선택 화면을 표시합니다.
        /// </summary>
        public void ShowCharacterSelectScreen()
        {
            HideAllScreens();
            
            if (characterSelectScreen == null)
            {
                GameObject selectObj = new GameObject("CharacterSelectScreen");
                CharacterSelectScreen screenComponent = selectObj.AddComponent<CharacterSelectScreen>();
                characterSelectScreen = selectObj;
                characterSelectScreen.transform.SetParent(transform);
                characterSelectScreen.SetActive(true);
            }
            else
            {
                characterSelectScreen.SetActive(true);
            }
            
            CharacterSelectScreen screen = characterSelectScreen.GetComponent<CharacterSelectScreen>();
            if (screen != null)
            {
                screen.Show();
            }
        }

        /// <summary>
        /// 스테이지 화면을 표시합니다. (게임 플레이 HUD)
        /// </summary>
        public void ShowStageScreen()
        {
            HideAllScreens();
            
            // 게임 재개 (일시정지 상태면 해제)
            if (GameManager.Instance?.LifecycleManager != null)
            {
                if (GameManager.Instance.LifecycleManager.IsPaused)
                {
                    GameManager.Instance.LifecycleManager.TogglePause();
                }
            }
            
            if (stageScreen != null)
                stageScreen.SetActive(true);
        }

        /// <summary>
        /// 일시정지 메뉴를 표시합니다.
        /// </summary>
        public void ShowPauseMenu()
        {
            if (pauseMenuScreen != null)
            {
                pauseMenuScreen.SetActive(true);
                
                // LifecycleManager를 통해 일시정지
                if (GameManager.Instance?.LifecycleManager != null)
                {
                    if (!GameManager.Instance.LifecycleManager.IsPaused)
                    {
                        GameManager.Instance.LifecycleManager.TogglePause();
                    }
                }
            }
            else
            {
                LogManager.LogError(LogCategory.UI, "pauseMenuScreen is NULL! Please assign it in the Inspector.");
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
            {
                gameOverScreen.SetActive(true);
                GameOverScreen screen = gameOverScreen.GetComponent<GameOverScreen>();
                if (screen != null)
                {
                    screen.Show(0);
                }
            }
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
        /// 상점 화면을 표시합니다.
        /// </summary>
        public void ShowShopScreen()
        {
            HideAllScreens();
            
            // 게임 일시정지 (상점 진입 시)
            if (GameManager.Instance?.LifecycleManager != null)
            {
                if (!GameManager.Instance.LifecycleManager.IsPaused)
                {
                    GameManager.Instance.LifecycleManager.TogglePause();
                }
            }
            
            // 상점 화면이 없으면 자동 생성
            if (shopScreen == null)
            {
                GameObject shopObj = new GameObject("BrotatoShopScreen");
                shopScreen = shopObj.AddComponent<BrotatoShopScreen>().gameObject;
                shopScreen.transform.SetParent(transform);
            }
            
            if (shopScreen != null)
            {
                shopScreen.SetActive(true);
            }
        }

        /// <summary>
        /// 메인 메뉴로 돌아갑니다.
        /// </summary>
        public void ReturnToMainMenu()
        {
            // 일시정지 해제
            if (GameManager.Instance?.LifecycleManager != null && 
                GameManager.Instance.LifecycleManager.IsPaused)
            {
                GameManager.Instance.LifecycleManager.TogglePause();
            }

            // 모든 Pawn 파괴
            if (GameManager.Instance?.CreationManager != null)
            {
                GameManager.Instance.CreationManager.DestroyAllPawns();
            }

            // 스테이지 종료
            if (GameManager.Instance?.StageManager != null)
            {
                GameManager.Instance.EndStage();
            }

            ShowCharacterSelectScreen();
        }

        private void HideAllScreens()
        {
            if (titleScreen != null) titleScreen.SetActive(false);
            if (campaignSelectScreen != null) campaignSelectScreen.SetActive(false);
            if (characterSelectScreen != null) characterSelectScreen.SetActive(false);
            if (stageScreen != null) stageScreen.SetActive(false);
            if (pauseMenuScreen != null) pauseMenuScreen.SetActive(false);
            if (gameOverScreen != null) gameOverScreen.SetActive(false);
            if (stageClearScreen != null) stageClearScreen.SetActive(false);
            if (shopScreen != null) shopScreen.SetActive(false);
            if (characterSelectScreen != null) characterSelectScreen.SetActive(false);
        }

        #endregion
    }
}


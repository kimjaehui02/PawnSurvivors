using UnityEngine;
using PawnSurvivors.Managers;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 오버레이 UI(PauseMenu, OptionsScreen)를 관리하는 매니저입니다.
    /// 씬 기반 아키텍처에서는 각 씬의 Screen이 독립적으로 작동하므로,
    /// UIManager는 씬 위에 표시되는 오버레이 UI만 관리합니다.
    /// GameManager와 같은 GameObject에 컴포넌트로 추가하세요.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Overlay UI (씬 위에 표시되는 UI)")]
        [SerializeField] private GameObject _pauseMenuScreen;
        [SerializeField] private GameObject _optionsScreen;
        
        // GameStateManager에서 오버레이 UI 상태 확인용
        public GameObject pauseMenuScreen => _pauseMenuScreen;
        public GameObject optionsScreen => _optionsScreen;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 씬 전환 시 이벤트 구독
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDestroy()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        /// <summary>
        /// 씬 전환 시 오버레이 UI를 숨깁니다.
        /// </summary>
        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            // 모든 오버레이 UI 숨기기
            if (_pauseMenuScreen != null && _pauseMenuScreen.activeSelf)
            {
                _pauseMenuScreen.SetActive(false);
            }
            
            if (_optionsScreen != null && _optionsScreen.activeSelf)
            {
                _optionsScreen.SetActive(false);
            }
            
            // AudioSettingsScreen도 찾아서 숨기기
            GameObject audioSettingsScreen = GameObject.Find("AudioSettingsScreen");
            if (audioSettingsScreen != null && audioSettingsScreen.activeSelf)
            {
                audioSettingsScreen.SetActive(false);
            }
        }

        private void Update()
        {
            // StageScene에서는 StageScreen이 직접 ESC 키를 처리하므로 여기서는 처리하지 않음
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (currentSceneName == "StageScene")
            {
                return;
            }
            
            // ESC 키 입력 처리 (오버레이 UI만 처리)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleEscapeKey();
            }
        }

        private void HandleEscapeKey()
        {
            // StageScene에서는 StageScreen이 직접 ESC 키를 처리하므로 여기서는 처리하지 않음
            // 다른 씬에서만 오버레이 UI 처리
            
            // 현재 씬이 StageScene인지 확인
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "StageScene")
            {
                // StageScreen에서 처리하므로 여기서는 아무것도 하지 않음
                return;
            }
            
            // 최우선: 소리 설정 화면 (3개 슬라이더) 찾기
            GameObject audioSettingsScreen = GameObject.Find("AudioSettingsScreen");
            if (audioSettingsScreen != null && audioSettingsScreen.activeSelf)
            {
                // 소리 설정 → 옵션으로
                audioSettingsScreen.SetActive(false);
                ShowOptionsScreen();
                return;
            }
            
            // 옵션 화면이 열려있으면 → 일시정지로
            if (_optionsScreen != null && _optionsScreen.activeSelf)
            {
                _optionsScreen.SetActive(false);
                ShowPauseMenu();
                return;
            }
            
            // 일시정지 메뉴가 열려있으면 닫기
            if (_pauseMenuScreen != null && _pauseMenuScreen.activeSelf)
            {
                HidePauseMenu();
                return;
            }
            
            // 오버레이 UI가 없으면 GameStateManager에 위임
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.HandleEscapeKey();
            }
        }

        #region Overlay UI Methods

        /// <summary>
        /// 일시정지 메뉴를 표시합니다.
        /// </summary>
        public void ShowPauseMenu()
        {
            if (_pauseMenuScreen == null)
            {
                // 코드로 생성 (프리팹 없이)
                _pauseMenuScreen = new GameObject("PauseMenuScreen");
                _pauseMenuScreen.transform.SetParent(transform);
                _pauseMenuScreen.AddComponent<PauseMenuScreen>();
                _pauseMenuScreen.SetActive(false); // 생성 시 비활성화
            }
            
            _pauseMenuScreen.SetActive(true);
            
            // LifecycleManager를 통해 일시정지
            if (GameManager.Instance?.LifecycleManager != null)
            {
                if (!GameManager.Instance.LifecycleManager.IsPaused)
                {
                    GameManager.Instance.LifecycleManager.TogglePause();
                }
            }
        }

        /// <summary>
        /// 일시정지 메뉴를 숨기고 게임을 재개합니다.
        /// </summary>
        /// <param name="updateState">GameStateManager의 상태도 업데이트할지 여부 (기본값: true)</param>
        public void HidePauseMenu(bool updateState = true)
        {
            if (_pauseMenuScreen != null)
            {
                _pauseMenuScreen.SetActive(false);
                
                // LifecycleManager를 통해 게임 재개
                if (GameManager.Instance?.LifecycleManager != null)
                {
                    if (GameManager.Instance.LifecycleManager.IsPaused)
                    {
                        GameManager.Instance.LifecycleManager.TogglePause();
                    }
                }
                
                // GameStateManager의 상태도 Stage로 전환 (동기화)
                // 단, PausedState.OnExit()에서 호출될 때는 updateState=false로 호출하여 무한 루프 방지
                if (updateState && GameStateManager.Instance != null && 
                    GameStateManager.Instance.CurrentStateName == "Paused")
                {
                    GameStateManager.Instance.GoToStage();
                }
            }
        }


        /// <summary>
        /// 옵션 화면을 표시합니다.
        /// </summary>
        public void ShowOptionsScreen()
        {
            if (_optionsScreen == null)
            {
                // 코드로 생성 (프리팹 없이)
                _optionsScreen = new GameObject("OptionsScreen");
                _optionsScreen.transform.SetParent(transform);
                _optionsScreen.AddComponent<OptionsScreen>();
                _optionsScreen.SetActive(false); // 생성 직후 비활성화 (Awake 호출 후)
            }
            
            _optionsScreen.SetActive(true);
        }


        #endregion
    }
}


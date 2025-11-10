using System.Collections.Generic;
using UnityEngine;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 모든 UI 화면을 관리하고 게임 상태에 따라 적절한 화면을 표시하는 매니저입니다.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Screens")]
        [SerializeField] private TitleScreen titleScreen;
        [SerializeField] private MainMenuScreen mainMenuScreen;
        [SerializeField] private GameplayHUD gameplayHUD;
        [SerializeField] private PauseMenuScreen pauseMenuScreen;
        [SerializeField] private GameOverScreen gameOverScreen;
        [SerializeField] private StageClearScreen stageClearScreen;

        private Dictionary<GameState, UIScreen> _screenMap;
        private GameState _currentState;
        private UIScreen _currentScreen;

        public GameState CurrentState => _currentState;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeScreenMap();
        }

        private void Start()
        {
            // 게임 시작 시 타이틀 화면 표시
            ChangeState(GameState.Title);
        }

        /// <summary>
        /// 화면 맵을 초기화합니다.
        /// </summary>
        private void InitializeScreenMap()
        {
            _screenMap = new Dictionary<GameState, UIScreen>
            {
                { GameState.Title, titleScreen },
                { GameState.MainMenu, mainMenuScreen },
                { GameState.Playing, gameplayHUD },
                { GameState.Paused, pauseMenuScreen },
                { GameState.GameOver, gameOverScreen },
                { GameState.StageClear, stageClearScreen }
            };

            // 모든 화면을 숨김 상태로 초기화
            foreach (var screen in _screenMap.Values)
            {
                if (screen != null)
                {
                    screen.Hide();
                }
            }
        }

        /// <summary>
        /// 게임 상태를 변경하고 해당하는 UI 화면을 표시합니다.
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (_currentState == newState) return;

            // 이전 화면 숨기기
            if (_currentScreen != null)
            {
                _currentScreen.Hide();
            }

            _currentState = newState;

            // 새 화면 표시
            if (_screenMap.TryGetValue(newState, out UIScreen newScreen) && newScreen != null)
            {
                _currentScreen = newScreen;
                _currentScreen.Show();
            }
            else
            {
                Debug.LogWarning($"UIManager: No screen found for state {newState}");
            }

            Debug.Log($"Game State Changed: {newState}");
        }

        /// <summary>
        /// 게임을 시작합니다 (타이틀 -> 메인 메뉴).
        /// </summary>
        public void StartGame()
        {
            ChangeState(GameState.MainMenu);
        }

        /// <summary>
        /// 스테이지를 시작합니다 (메인 메뉴 -> 게임 플레이).
        /// </summary>
        public void StartStage(string stageName = "Stage1")
        {
            ChangeState(GameState.Playing);
            
            // GameManager에게 스테이지 시작을 알림
            if (GameManager.Instance != null)
            {
                // 플레이어 생성
                var playerRecipe = GameManager.Instance.CreationManager.GetRecipe("Player");
                if (playerRecipe != null)
                {
                    GameManager.Instance.CreationManager.CreatePawn(playerRecipe, Vector3.zero, Quaternion.identity);
                }

                // 스테이지 로드 및 시작
                var stageData = GameManager.Instance.LoadStage(stageName);
                if (stageData != null && GameManager.Instance.StageManager != null)
                {
                    GameManager.Instance.StageManager.Initialize(GameManager.Instance.CreationManager, stageData);
                    GameManager.Instance.StageManager.StartStage();
                }
            }
        }

        /// <summary>
        /// 게임을 일시정지합니다.
        /// </summary>
        public void PauseGame()
        {
            if (_currentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
                Time.timeScale = 0f;
            }
        }

        /// <summary>
        /// 게임을 재개합니다.
        /// </summary>
        public void ResumeGame()
        {
            if (_currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
                Time.timeScale = 1f;
            }
        }

        /// <summary>
        /// 게임 오버 화면을 표시합니다.
        /// </summary>
        public void ShowGameOver()
        {
            ChangeState(GameState.GameOver);
            Time.timeScale = 0f;
        }

        /// <summary>
        /// 스테이지 클리어 화면을 표시합니다.
        /// </summary>
        public void ShowStageClear()
        {
            ChangeState(GameState.StageClear);
            Time.timeScale = 0f;
        }

        /// <summary>
        /// 메인 메뉴로 돌아갑니다.
        /// </summary>
        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            ChangeState(GameState.MainMenu);
            
            // 모든 Pawn 제거
            ClearAllPawns();
        }

        /// <summary>
        /// 타이틀 화면으로 돌아갑니다.
        /// </summary>
        public void ReturnToTitle()
        {
            Time.timeScale = 1f;
            ChangeState(GameState.Title);
            
            // 모든 Pawn 제거
            ClearAllPawns();
        }

        /// <summary>
        /// 게임을 종료합니다.
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// 모든 Pawn을 제거합니다.
        /// </summary>
        private void ClearAllPawns()
        {
            var allPawns = new List<PawnManager>(PawnManager.AllPawnManagers);
            foreach (var pawn in allPawns)
            {
                if (pawn != null)
                {
                    Destroy(pawn.gameObject);
                }
            }
        }
    }
}


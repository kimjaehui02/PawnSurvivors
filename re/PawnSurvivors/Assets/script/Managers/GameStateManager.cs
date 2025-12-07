using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using PawnSurvivors.Domain.States;
using PawnSurvivors.Managers;
using PawnSurvivors.UI;

namespace PawnSurvivors.Managers
{
    /// <summary>
    /// 게임 상태를 관리하는 상태 머신입니다.
    /// State 패턴을 사용하여 각 게임 단계를 독립적으로 관리합니다.
    /// State 간 직접 호출을 금지하고, 오직 Manager를 통해서만 전환합니다.
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        private IGameState _currentState;
        private IGameState _previousState;
        
        // State 인스턴스 캐싱 (재사용)
        private Dictionary<System.Type, IGameState> _stateCache = new Dictionary<System.Type, IGameState>();
        
        // State → Scene 매핑
        private Dictionary<System.Type, string> _stateToSceneMap = new Dictionary<System.Type, string>();

        public IGameState CurrentState => _currentState;
        public string CurrentStateName => _currentState?.StateName ?? "None";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // State → Scene 매핑 초기화
            InitializeSceneMapping();
        }

        private void Start()
        {
            // 현재 씬에 맞는 State를 자동으로 설정
            // 씬이 이미 로드되어 있으면 그 씬에 맞는 State로 설정
            InitializeStateFromCurrentScene();
        }
        
        /// <summary>
        /// 현재 씬에 맞는 State를 자동으로 설정합니다.
        /// </summary>
        private void InitializeStateFromCurrentScene()
        {
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            // 씬 이름에 맞는 State 찾기
            foreach (var kvp in _stateToSceneMap)
            {
                if (kvp.Value == currentSceneName)
                {
                    // 해당 State로 전환 (씬은 이미 로드되어 있으므로 씬 로드 스킵)
                    IGameState state = GetOrCreateState(kvp.Key);
                    if (state != null)
                    {
                        _currentState = state;
                        _currentState.OnEnter();
                        LogManager.LogInfo(LogCategory.System, 
                            $"현재 씬 '{currentSceneName}'에 맞는 State '{state.StateName}'로 자동 설정됨");
                        return;
                    }
                }
            }
            
            // 매칭되는 씬이 없으면 TitleState로 (기본값)
            LogManager.LogWarning(LogCategory.System, 
                $"현재 씬 '{currentSceneName}'에 매칭되는 State가 없습니다. TitleState로 설정합니다.");
            TransitionTo<TitleState>();
        }
        
        /// <summary>
        /// State 타입으로 State 인스턴스를 가져오거나 생성합니다.
        /// </summary>
        private IGameState GetOrCreateState(System.Type stateType)
        {
            if (!_stateCache.ContainsKey(stateType))
            {
                _stateCache[stateType] = System.Activator.CreateInstance(stateType) as IGameState;
            }
            return _stateCache[stateType];
        }
        
        /// <summary>
        /// State와 Scene 매핑을 초기화합니다.
        /// </summary>
        private void InitializeSceneMapping()
        {
            _stateToSceneMap[typeof(TitleState)] = "TitleScene";
            _stateToSceneMap[typeof(CharacterSelectState)] = "CharacterSelectScene";
            _stateToSceneMap[typeof(StageSelectState)] = "StageSelectScene";
            _stateToSceneMap[typeof(StageState)] = "StageScene";
            _stateToSceneMap[typeof(ShopState)] = "ShopScene";
            _stateToSceneMap[typeof(GameOverState)] = "GameOverScene";
            // PausedState는 오버레이이므로 씬 전환 없음
        }

        private void Update()
        {
            // 현재 State 업데이트
            _currentState?.OnUpdate();
        }

        /// <summary>
        /// 특정 State 타입으로 전환합니다.
        /// </summary>
        public bool TransitionTo<T>() where T : IGameState, new()
        {
            IGameState nextState = GetOrCreateState<T>();
            return TransitionTo(nextState);
        }

        /// <summary>
        /// 특정 State 인스턴스로 전환합니다.
        /// </summary>
        public bool TransitionTo(IGameState nextState)
        {
            if (nextState == null)
            {
                LogManager.LogError(LogCategory.System, "[GameStateManager] 전환하려는 State가 null입니다.");
                return false;
            }

            // 같은 State로 전환 시도 시, OnEnter()만 다시 호출 (전환은 허용)
            if (_currentState != null && _currentState.GetType() == nextState.GetType())
            {
                LogManager.LogInfo(LogCategory.System, 
                    $"[GameStateManager] 같은 State로 전환 시도: {nextState.StateName}. OnEnter()만 재호출.");
                _currentState.OnEnter();
                return true;
            }

            // 전환 가능 여부 확인
            if (_currentState != null && !_currentState.CanTransitionTo(nextState))
            {
                LogManager.LogWarning(LogCategory.System, 
                    $"[GameStateManager] {_currentState.StateName} → {nextState.StateName} 전환이 허용되지 않습니다.");
                UnityEngine.Debug.LogWarning($"[GameStateManager] {_currentState.StateName} → {nextState.StateName} 전환이 허용되지 않습니다.");
                return false;
            }

            // 이전 State 종료
            if (_currentState != null)
            {
                _currentState.OnExit();
            }

            // State 전환
            _previousState = _currentState;
            _currentState = nextState;

            // 씬 전환 (해당 State에 씬이 매핑되어 있는 경우)
            // 이미 같은 씬이면 로드하지 않음
            string sceneName = GetSceneNameForState(nextState);
            if (!string.IsNullOrEmpty(sceneName))
            {
                string currentSceneName = SceneManager.GetActiveScene().name;
                if (currentSceneName != sceneName)
                {
                    SceneManager.LoadScene(sceneName);
                }
            }

            // 새 State 진입
            _currentState.OnEnter();

            LogManager.LogInfo(LogCategory.System, 
                $"[GameStateManager] State 전환: {_previousState?.StateName ?? "None"} → {_currentState.StateName}");

            return true;
        }
        
        /// <summary>
        /// State에 해당하는 씬 이름을 가져옵니다.
        /// </summary>
        private string GetSceneNameForState(IGameState state)
        {
            if (state == null) return null;
            
            System.Type stateType = state.GetType();
            if (_stateToSceneMap.ContainsKey(stateType))
            {
                return _stateToSceneMap[stateType];
            }
            
            return null; // PausedState 등 오버레이 State는 씬 전환 없음
        }
        

        /// <summary>
        /// 이전 상태로 돌아갑니다.
        /// </summary>
        public void ReturnToPreviousState()
        {
            if (_previousState != null)
            {
                TransitionTo(_previousState);
            }
        }

        /// <summary>
        /// State 인스턴스를 가져오거나 생성합니다. (캐싱)
        /// </summary>
        private T GetOrCreateState<T>() where T : IGameState, new()
        {
            System.Type stateType = typeof(T);
            
            if (!_stateCache.ContainsKey(stateType))
            {
                T newState = new T();
                _stateCache[stateType] = newState;
            }
            
            return (T)_stateCache[stateType];
        }


        /// <summary>
        /// ESC 키 입력을 처리합니다.
        /// </summary>
        public void HandleEscapeKey()
        {
            if (_currentState == null) return;

            switch (_currentState.StateName)
            {
                case "Paused":
                    // 일시정지 해제 -> 스테이지로
                    GoToStage();
                    break;

                case "Stage":
                    // 일시정지 메뉴 열기
                    TransitionTo<PausedState>();
                    break;

                case "Shop":
                    // 상점에서 ESC -> 캐릭터 선택으로
                    TransitionTo<CharacterSelectState>();
                    break;

                case "GameOver":
                    // 게임오버에서 ESC -> 캐릭터 선택으로
                    TransitionTo<CharacterSelectState>();
                    break;
            }
        }

        // ========================================
        // 편의 메서드들 (State 전환 요청)
        // ========================================

        /// <summary>
        /// 타이틀 화면으로 전환합니다.
        /// </summary>
        public void GoToTitle()
        {
            TransitionTo<TitleState>();
        }

        /// <summary>
        /// 캐릭터 선택 화면으로 전환합니다.
        /// </summary>
        public void GoToCharacterSelect()
        {
            TransitionTo<CharacterSelectState>();
        }

        /// <summary>
        /// 스테이지 선택 화면으로 전환합니다.
        /// </summary>
        public void GoToStageSelect()
        {
            TransitionTo<StageSelectState>();
        }

        /// <summary>
        /// 스테이지로 전환합니다.
        /// </summary>
        public void GoToStage()
        {
            TransitionTo<StageState>();
        }

        /// <summary>
        /// 상점으로 전환합니다.
        /// </summary>
        public void GoToShop()
        {
            TransitionTo<ShopState>();
        }

        /// <summary>
        /// 게임오버 화면으로 전환합니다.
        /// </summary>
        public void GoToGameOver()
        {
            TransitionTo<GameOverState>();
        }

        /// <summary>
        /// 일시정지 상태로 전환합니다.
        /// </summary>
        public void PauseGame()
        {
            if (_currentState is StageState)
            {
                TransitionTo<PausedState>();
            }
        }

        /// <summary>
        /// 일시정지를 해제하고 스테이지로 돌아갑니다.
        /// </summary>
        public void ResumeGame()
        {
            if (_currentState is PausedState)
            {
                GoToStage();
            }
        }
    }
}

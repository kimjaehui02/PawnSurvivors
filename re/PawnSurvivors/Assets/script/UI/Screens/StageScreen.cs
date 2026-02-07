using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using PawnSurvivors.Managers;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Domain.States;
using PawnSurvivors.UI.Factory;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 스테이지 플레이 화면입니다. Builder를 사용하여 UI를 생성합니다.
    /// </summary>
    public class StageScreen : MonoBehaviour
    {
        #region Settings
        [SerializeField] private string _selectedStage = "Stage1";
        [SerializeField] private string tileSpriteFolderPath = "Sprites/tiles";
        [SerializeField] private Vector2Int mapSize = new Vector2Int(50, 50);
        #endregion

        #region State
        private float _stageTime = 0f;
        private float _stageDuration = 60f;
        private bool _stageEnded = false;
        private bool _isPaused = false;
        #endregion

        #region References
        private StageManager _stageManager;
        private Canvas _canvas;
        private Grid _grid;
        private Tilemap _backgroundTilemap;
        #endregion

        #region UI Components (Builder로 생성)
        private StageUIBuilder.StageHUD _hud;
        private StageUIBuilder.PauseMenu _pauseMenu;
        private StageUIBuilder.PlayerStatusContainer _playerStatusContainer;
        private List<StageUIBuilder.PlayerStatusBar> _playerStatusBars = new();
        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            SetupCanvas();
        }

        private void Start()
        {
            CreateUI();
            InitializeStageManager();
            CreateBackgroundTilemap();
            StartStageIfNeeded();
        }

        private void Update()
        {
            if (_stageEnded || _isPaused) return;

            UpdateStageTimer();
            UpdateHUD();
            UpdatePlayerStatus();
            HandleInput();
        }

        private void OnDestroy()
        {
            // 정리
        }

        #endregion

        #region Initialization

        private void SetupCanvas()
        {
            _canvas = UIFactory.SetupCanvas(gameObject);
        }

        private void CreateUI()
        {
            // HUD 생성
            _hud = StageUIBuilder.CreateStageHUD(transform);
            UpdateRoundDisplay();

            // 플레이어 상태 컨테이너
            _playerStatusContainer = StageUIBuilder.CreatePlayerStatusContainer(transform);

            // 일시정지 메뉴
            _pauseMenu = StageUIBuilder.CreatePauseMenu(
                transform,
                onResume: OnResumeClicked,
                onOptions: OnOptionsClicked,
                onMainMenu: OnMainMenuClicked
            );

            // 골드 UI
            if (GetComponent<CurrencyUI>() == null)
            {
                var currencyUI = gameObject.AddComponent<CurrencyUI>();
                currencyUI.anchorPosition = new Vector2(0.95f, 0.95f);
            }
        }

        private void InitializeStageManager()
        {
            _stageManager = GetComponent<StageManager>() ?? gameObject.AddComponent<StageManager>();

            if (GameManager.Instance != null)
            {
                var stageDataSource = new PawnSurvivors.Data.DataSources.StageDataSource();
                stageDataSource.LoadStages("StreamingAssets/Stages");

                _stageManager.Initialize(
                    GameManager.Instance.CreationManager,
                    stageDataSource,
                    GameManager.Instance.StageManagementUseCase
                );
            }
        }

        private void CreateBackgroundTilemap()
        {
            // Grid 생성
            var gridObj = new GameObject("BackgroundGrid");
            _grid = gridObj.AddComponent<Grid>();
            _grid.cellSize = new Vector3(1, 1, 0);

            // Tilemap 생성
            var tilemapObj = new GameObject("Tilemap");
            tilemapObj.transform.SetParent(gridObj.transform);
            _backgroundTilemap = tilemapObj.AddComponent<Tilemap>();
            var renderer = tilemapObj.AddComponent<TilemapRenderer>();
            renderer.sortingOrder = -10;

            // 타일 로드 및 배치
            var tiles = Resources.LoadAll<Sprite>(tileSpriteFolderPath);
            if (tiles.Length > 0)
            {
                FillTilemap(tiles);
            }
        }

        private void FillTilemap(Sprite[] tiles)
        {
            int halfX = mapSize.x / 2;
            int halfY = mapSize.y / 2;

            for (int x = -halfX; x < halfX; x++)
            {
                for (int y = -halfY; y < halfY; y++)
                {
                    var tile = ScriptableObject.CreateInstance<Tile>();
                    tile.sprite = tiles[Random.Range(0, tiles.Length)];
                    _backgroundTilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }

        private void StartStageIfNeeded()
        {
            if (GameManager.Instance == null) return;

            // 선택된 스테이지 가져오기
            string stageName = GameManager.Instance.StageManagementUseCase?.GetCurrentStageName() ?? _selectedStage;
            _selectedStage = stageName;

            // 스테이지 데이터에서 duration 가져오기
            var stageData = GameManager.Instance.LoadStage(stageName);
            if (stageData != null)
            {
                _stageDuration = stageData.stageDuration;
            }

            // PlayerController 생성
            if (GameManager.Instance.PlayerController == null)
            {
                GameManager.Instance.CreatePlayerController();
            }
            else
            {
                GameManager.Instance.PrepareExistingPlayers();
            }

            // 스테이지 시작
            _stageManager?.StartStage(stageName, resetSession: false);
            _stageTime = _stageDuration;
            _stageEnded = false;

            // 플레이어 상태바 생성
            CreatePlayerStatusBars();

            // BGM 재생
            if (stageData != null && !string.IsNullOrEmpty(stageData.bgmName))
            {
                GameManager.Instance.PlayBGM(stageData.bgmName);
            }
        }

        #endregion

        #region Update Logic

        private void UpdateStageTimer()
        {
            float deltaTime = GetGameDeltaTime();
            _stageTime -= deltaTime;

            if (_stageTime <= 0)
            {
                _stageTime = 0;
                OnStageClear();
            }
        }

        private void UpdateHUD()
        {
            _hud?.SetTimer(_stageTime);

            // 킬 수 업데이트
            int killCount = GameManager.Instance?.KillTrackingUseCase?.GetTotalKills() ?? 0;
            _hud?.SetKillCount(killCount);

            // 골드 업데이트
            int gold = GameManager.Instance?.CurrencyUseCase?.GetGold() ?? 0;
            _hud?.SetGold(gold);
        }

        private void UpdateRoundDisplay()
        {
            if (_hud == null || GameManager.Instance?.StageManagementUseCase == null) return;

            int current = GameManager.Instance.StageManagementUseCase.GetCurrentRound();
            int total = GameManager.Instance.StageManagementUseCase.GetTotalRounds();
            _hud.SetRound(current, total);
        }

        private void UpdatePlayerStatus()
        {
            if (GameManager.Instance?.PlayerController == null) return;

            var pawns = GameManager.Instance.PlayerController.playerPawns;
            for (int i = 0; i < _playerStatusBars.Count && i < pawns.Count; i++)
            {
                var pawn = pawns[i];
                var bar = _playerStatusBars[i];

                if (pawn == null) continue;

                var pawnManager = pawn.GetComponent<PawnManager>();
                if (pawnManager?.PawnData?.healthData != null)
                {
                    var health = pawnManager.PawnData.healthData;
                    bar.SetHealth(health.currentHealth, health.maxHealth);
                }

                var levelUp = pawn.GetComponent<LevelUpSubManager>();
                if (levelUp != null)
                {
                    // 진행도 기반으로 표시 (0~1 값을 백분율로)
                    float progress = levelUp.GetCurrentProgress();
                    bar.SetExp(progress, 1f);
                }
            }
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        private float GetGameDeltaTime()
        {
            return GameManager.Instance?.LifecycleManager?.GameDeltaTime ?? Time.deltaTime;
        }

        #endregion

        #region Player Status

        private void CreatePlayerStatusBars()
        {
            _playerStatusContainer?.Clear();
            _playerStatusBars.Clear();

            if (GameManager.Instance?.PlayerController == null) return;

            var pawns = GameManager.Instance.PlayerController.playerPawns;
            for (int i = 0; i < pawns.Count; i++)
            {
                var pawn = pawns[i];
                if (pawn == null) continue;

                var pawnManager = pawn.GetComponent<PawnManager>();
                string name = pawnManager?.PawnData?.recipeName ?? $"Player {i + 1}";

                var bar = _playerStatusContainer.AddPlayer(i, name);
                _playerStatusBars.Add(bar);
            }
        }

        #endregion

        #region Pause Menu

        private void TogglePause()
        {
            if (_pauseMenu.IsVisible)
            {
                OnResumeClicked();
            }
            else
            {
                _isPaused = true;
                _pauseMenu.Show();
                if (GameManager.Instance?.LifecycleManager != null && !GameManager.Instance.LifecycleManager.IsPaused)
                {
                    GameManager.Instance.LifecycleManager.TogglePause();
                }
            }
        }

        private void OnResumeClicked()
        {
            _isPaused = false;
            _pauseMenu.Hide();
            if (GameManager.Instance?.LifecycleManager != null && GameManager.Instance.LifecycleManager.IsPaused)
            {
                GameManager.Instance.LifecycleManager.TogglePause();
            }
        }

        private void OnOptionsClicked()
        {
            // 옵션 화면 열기
            UIManager.Instance?.ShowOptionsScreen();
        }

        private void OnMainMenuClicked()
        {
            _pauseMenu.Hide();
            if (GameManager.Instance?.LifecycleManager != null && GameManager.Instance.LifecycleManager.IsPaused)
            {
                GameManager.Instance.LifecycleManager.TogglePause();
            }
            GameFlowController.Instance?.ReturnToMainMenu();
        }

        #endregion

        #region Stage Events

        public void OnStageClear()
        {
            if (_stageEnded) return;
            _stageEnded = true;

            DestroyAllEnemies();

            if (GameFlowController.Instance != null)
            {
                GameFlowController.Instance.CompleteCurrentStage();
            }
            else if (GameStateManager.Instance != null)
            {
                GameManager.Instance?.EndStage();
                GameStateManager.Instance.GoToShop();
            }
        }

        public void OnStageFailed()
        {
            if (_stageEnded) return;
            _stageEnded = true;

            DestroyAllEnemies();

            if (GameFlowController.Instance != null)
            {
                GameFlowController.Instance.FailCurrentStage();
            }
            else if (GameManager.Instance?.StageFlowUseCase != null)
            {
                GameManager.Instance.StageFlowUseCase.FailStage();
            }
        }

        private void DestroyAllEnemies()
        {
            var allPawns = PawnManager.AllPawnManagers.ToArray();
            foreach (var pm in allPawns)
            {
                if (pm != null && pm.gameObject.CompareTag("Enemy"))
                {
                    Destroy(pm.gameObject);
                }
            }

            // 적 투사체 파괴
            var projectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");
            foreach (var proj in projectiles)
            {
                Destroy(proj);
            }
        }

        #endregion

        #region Public API

        public StageManager GetStageManager() => _stageManager;

        public void StageStart()
        {
            GameManager.Instance?.StartStage(_selectedStage);
        }

        #endregion
    }
}

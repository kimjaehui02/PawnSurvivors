using UnityEngine;
using System.Collections.Generic;
using PawnSurvivors.Managers;
using PawnSurvivors.Data;
using PawnSurvivors.Data.Repositories;
using PawnSurvivors.Domain.Repositories;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Player;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public LifecycleManager LifecycleManager { get; private set; }
    public CreationManager CreationManager { get; private set; }
    public StageManager StageManager { get; private set; }
    
    public FloatingEffectManager FloatingEffectManager { get; private set; }
    public BackgroundTilemapManager BackgroundTilemapManager { get; private set; }
    // public AspectRatioManager AspectRatioManager { get; private set; }
    
    /// <summary>
    /// 현재 게임 세션의 런타임 데이터 (Data 계층 내부용, 외부 접근 불가)
    /// </summary>
    private GameSessionData _sessionData;
    
    /// <summary>
    /// 세션 데이터 Repository (Data 계층 내부용, UseCase에서만 사용)
    /// </summary>
    private ISessionDataRepository SessionDataRepository { get; set; }
    
    /// <summary>
    /// 아이템 Repository (Data 계층 내부용, UseCase에서만 사용)
    /// </summary>
    public IItemRepository ItemRepository { get; private set; }
    
    /// <summary>
    /// 레시피 Repository (Data 계층 내부용, UseCase에서만 사용)
    /// </summary>
    public IRecipeRepository RecipeRepository { get; private set; }
    
    /// <summary>
    /// UseCase 인스턴스들
    /// </summary>
    public DamageTrackingUseCase DamageTrackingUseCase { get; private set; }
    public KillTrackingUseCase KillTrackingUseCase { get; private set; }
    public SurvivalTimeTrackingUseCase SurvivalTimeTrackingUseCase { get; private set; }
    public SessionManagementUseCase SessionManagementUseCase { get; private set; }
    public CurrencyUseCase CurrencyUseCase { get; private set; }
    public StageManagementUseCase StageManagementUseCase { get; private set; }
    public PawnPersistenceUseCase PawnPersistenceUseCase { get; private set; }
    public ItemManagementUseCase ItemManagementUseCase { get; private set; }
    public PawnStatCalculator PawnStatCalculator { get; private set; }
    public FloatingEffectUseCase FloatingEffectUseCase { get; private set; }
    public ItemPoolUseCase ItemPoolUseCase { get; private set; }
    public ShopUseCase ShopUseCase { get; private set; }
    public CharacterSelectionUseCase CharacterSelectionUseCase { get; private set; }
    public CharacterUpgradeUseCase CharacterUpgradeUseCase { get; private set; }
    public StageFlowUseCase StageFlowUseCase { get; private set; }
    
    /// <summary>
    /// 플레이어 컨트롤러 (입력 받는 중심 오브젝트)
    /// </summary>
    public PlayerController PlayerController { get; private set; }
    
    /// <summary>
    /// 배경 음악 AudioSource
    /// </summary>
    public AudioSource BGMSource { get; private set; }
    
    /// <summary>
    /// 오디오 설정 (볼륨 관리)
    /// </summary>
    public PawnSurvivors.Data.GameAudioSettings AudioSettings { get; private set; }
    
    /// <summary>
    /// 선택된 캐릭터 목록
    /// </summary>
    private List<string> _selectedCharacters = new List<string> { "PlayerErpin", "PlayerButter", "PlayerOpal" };
    
    /// <summary>
    /// 아이템 풀 데이터 소스 (아이템 JSON 파일 로드)
    /// </summary>
    private PawnSurvivors.Data.DataSources.ItemPoolDataSource _itemPoolDataSource;
    
    /// <summary>
    /// 스테이지 데이터 소스 (스테이지 JSON 파일 로드)
    /// </summary>
    private PawnSurvivors.Data.DataSources.StageDataSource _stageDataSource;
    
    /// <summary>
    /// 스테이지 리스트 데이터 소스 (스테이지 순서 정의)
    /// </summary>
    private PawnSurvivors.Data.DataSources.StageListDataSource _stageListDataSource;

    /// <summary>
    /// PlayerController를 생성하고 초기화합니다.
    /// </summary>
    public void CreatePlayerController()
    {
        if (PlayerController != null) return;

        GameObject playerControllerObj = new GameObject("PlayerController");
        PlayerController = playerControllerObj.AddComponent<PlayerController>();
        
        // 메인 카메라를 PlayerController의 자식으로 설정
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(PlayerController.transform);
            mainCamera.transform.localPosition = new Vector3(0f, 0f, -10f); // 2D 게임용
            LogManager.LogInfo(LogCategory.System, "메인 카메라가 PlayerController에 붙었습니다.");
        }
        
        // ========== 초기 플레이어 생성 ==========
        // 선택된 캐릭터들 생성 (첫 스테이지에만 호출됨)
        foreach (string characterName in _selectedCharacters)
        {
            AddPlayerPawn(characterName);
        }
        
        LogManager.LogInfo(LogCategory.System, $"총 {_selectedCharacters.Count}명의 플레이어 생성 완료");
        // ========== 초기 플레이어 생성 끝 ==========
    }

    private void Awake()
    {
        // 강제 로그 출력 (LogManager 초기화 전에도 작동)
        UnityEngine.Debug.Log("[GameManager] Awake() 시작");
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        UnityEngine.Debug.Log("[GameManager] Instance 설정 완료");
        
        // 세션 데이터 초기화 (Data 계층 내부)
        _sessionData = new GameSessionData();
        
        // Repository 초기화 (Data 계층 구현체)
        SessionDataRepository = new SessionDataRepository(_sessionData);
        ItemRepository = new ItemRepository(_sessionData);
        
        // RecipeRepository는 CreationManager 초기화 후 설정
        
        // StageDataSource 초기화 (UseCase보다 먼저 초기화 필요)
        // Resources 폴더 경로 사용 (WebGL 호환)
        _stageDataSource = new PawnSurvivors.Data.DataSources.StageDataSource();
        string stagesPath = "StreamingAssets/Stages";
        _stageDataSource.LoadStages(stagesPath);
        
        // StageListDataSource 초기화
        _stageListDataSource = new PawnSurvivors.Data.DataSources.StageListDataSource();
        string stageListPath = "StreamingAssets/Stages/StageList";
        _stageListDataSource.LoadStageList(stageListPath);

        // UseCase 초기화
        DamageTrackingUseCase = new DamageTrackingUseCase(SessionDataRepository);
        KillTrackingUseCase = new KillTrackingUseCase(SessionDataRepository);
        SurvivalTimeTrackingUseCase = new SurvivalTimeTrackingUseCase(SessionDataRepository, null); // LifecycleManager는 나중에 설정
        SessionManagementUseCase = new SessionManagementUseCase(SessionDataRepository);
        CurrencyUseCase = new CurrencyUseCase(SessionDataRepository);
        StageManagementUseCase = new StageManagementUseCase(SessionDataRepository, _stageListDataSource);
        PawnPersistenceUseCase = new PawnPersistenceUseCase(_sessionData);
        ItemManagementUseCase = new ItemManagementUseCase(ItemRepository, SessionDataRepository, CurrencyUseCase);
        PawnStatCalculator = new PawnStatCalculator(ItemRepository);
        FloatingEffectUseCase = new FloatingEffectUseCase();

        // 동일한 GameObject에서 구성 요소 가져오기
        LifecycleManager = GetComponent<LifecycleManager>();
        
        // FloatingEffectManager 초기화
        FloatingEffectManager = GetComponent<FloatingEffectManager>();
        if (FloatingEffectManager == null)
        {
            FloatingEffectManager = gameObject.AddComponent<FloatingEffectManager>();
        }
        
        // BackgroundTilemapManager 초기화
        BackgroundTilemapManager = GetComponent<BackgroundTilemapManager>();
        if (BackgroundTilemapManager == null)
        {
            BackgroundTilemapManager = gameObject.AddComponent<BackgroundTilemapManager>();
        }
        
        // AspectRatioManager 초기화 (레터박스)
        // AspectRatioManager = GetComponent<AspectRatioManager>();
        // if (AspectRatioManager == null)
        // {
        //     AspectRatioManager = gameObject.AddComponent<AspectRatioManager>();
        // }
        
        CreationManager = GetComponent<CreationManager>();
        StageManager = GetComponent<StageManager>();
        
        // 오디오 설정 로드
        AudioSettings = new PawnSurvivors.Data.GameAudioSettings();
        AudioSettings.Load();
        
        // BGM AudioSource 생성
        InitializeBGM();
        
        // LifecycleManager 설정 후 UseCase 업데이트
        if (LifecycleManager != null)
        {
            SurvivalTimeTrackingUseCase = new SurvivalTimeTrackingUseCase(SessionDataRepository, LifecycleManager);
        }

        // StageDataSource는 위에서 이미 초기화됨

        // ItemPoolDataSource 초기화
        // Resources 폴더 경로 사용 (WebGL 호환)
        _itemPoolDataSource = new PawnSurvivors.Data.DataSources.ItemPoolDataSource();
        string itemsPath = "StreamingAssets/Recipes/Items";
        _itemPoolDataSource.LoadItems(itemsPath);
        
        // ItemPoolUseCase 초기화
        var itemPoolRepository = new PawnSurvivors.Data.Repositories.ItemPoolRepository(_itemPoolDataSource);
        ItemPoolUseCase = new ItemPoolUseCase(itemPoolRepository, ItemRepository);
        
        // RecipeRepository 초기화 (CreationManager의 RecipeDataSource 사용)
        if (CreationManager != null)
        {
            var recipeDataSource = CreationManager.GetType()
                .GetField("_recipeDataSource", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(CreationManager) as PawnSurvivors.Data.DataSources.RecipeDataSource;
            
            if (recipeDataSource != null)
            {
                RecipeRepository = new PawnSurvivors.Data.Repositories.RecipeRepository(recipeDataSource);
            }
            else
            {
                LogManager.LogError(LogCategory.System, "RecipeDataSource를 CreationManager에서 가져올 수 없습니다.");
            }
        }
        
        // ShopUseCase 초기화 (아이템 + 캐릭터 랜덤 선택 기능 포함)
        ShopUseCase = new ShopUseCase(ItemPoolUseCase, ItemRepository, RecipeRepository, CurrencyUseCase);
        
        // CharacterSelectionUseCase 초기화
        CharacterSelectionUseCase = new CharacterSelectionUseCase(SessionDataRepository);
        
        // CharacterUpgradeUseCase 초기화 (RecipeRepository와 SessionDataRepository 필요)
        CharacterUpgradeUseCase = new CharacterUpgradeUseCase(RecipeRepository, SessionDataRepository);
        
        // StageFlowUseCase 초기화
        StageFlowUseCase = new StageFlowUseCase(SessionDataRepository, _stageListDataSource);
        StageFlowUseCase.OnStageStartRequested += HandleStageStartRequested;
        StageFlowUseCase.OnStageCompletedToShop += HandleStageCompletedToShop;
        StageFlowUseCase.OnGameOver += HandleGameOver;
        StageFlowUseCase.OnAllStagesCleared += HandleAllStagesCleared;

        // StageManager 초기화 (의존성 주입)
        if (StageManager != null)
        {
            StageManager.Initialize(CreationManager, _stageDataSource, StageManagementUseCase);
        }

        if (LifecycleManager == null)
        {
            LogManager.LogError(LogCategory.System, "LifecycleManager component not found on the same GameObject.");
        }
        if (CreationManager == null)
        {
            LogManager.LogError(LogCategory.System, "CreationManager component not found on the same GameObject.");
        }
        if (StageManager == null)
        {
            LogManager.LogError(LogCategory.System, "StageManager component not found on the same GameObject.");
        }
    }

    /// <summary>
    /// 스테이지를 시작합니다.
    /// StageManager에 모든 로직을 위임합니다.
    /// </summary>
    /// <param name="stageName">시작할 스테이지 이름 (기본값: DebugStage)</param>
    /// <param name="resetSession">세션 데이터를 리셋할지 여부 (기본값: true, 상점에서 올 때는 false)</param>
    public void StartStage(string stageName = "DebugStage", bool resetSession = true)
    {
        if (StageManager != null)
        {
            StageManager.StartStage(stageName, resetSession);
        }
        else
        {
            LogManager.LogError(LogCategory.System, "StageManager가 없습니다!");
        }
    }

    /// <summary>
    /// 플레이어블 폰을 추가합니다.
    /// </summary>
    public GameObject AddPlayerPawn(string recipeName, Vector3? worldPosition = null)
    {
        if (PlayerController == null)
        {
            LogManager.LogError(LogCategory.System, "PlayerController가 없습니다!");
            return null;
        }
        
        var recipe = CreationManager.GetRecipe(recipeName);
        if (recipe == null)
        {
            LogManager.LogError(LogCategory.System, $"Recipe '{recipeName}'를 찾을 수 없습니다!");
            return null;
        }
        
        Vector3 spawnPos = worldPosition ?? PlayerController.transform.position;
        GameObject pawn = CreationManager.CreatePawn(recipe, spawnPos, Quaternion.identity);
        
        if (pawn != null)
        {
            PlayerController.AddPlayerPawn(pawn);
            LogManager.LogInfo(LogCategory.Pawn, $"플레이어 폰 추가: {recipeName}");
        }
        
        return pawn;
    }
    
    /// <summary>
    /// 여러 플레이어블 폰을 한 번에 추가합니다.
    /// </summary>
    public void AddMultiplePlayerPawns(string[] recipeNames)
    {
        foreach (string recipeName in recipeNames)
        {
            AddPlayerPawn(recipeName);
        }
    }
    
    /// <summary>
    /// 스테이지를 로드합니다.
    /// </summary>
    public StageData LoadStage(string stageName)
    {
        return _stageDataSource?.GetStage(stageName);
    }

    /// <summary>
    /// 모든 스테이지 이름 목록을 가져옵니다.
    /// </summary>
    public List<string> GetAllStageNames()
    {
        var names = _stageDataSource?.GetAllStageNames();
        return names != null ? new List<string>(names) : new List<string>();
    }

    public void PauseStage(string stageName)
    {
        LifecycleManager.TogglePause();
    }

    public void SetSelectedCharacters(List<string> characters)
    {
        _selectedCharacters = new List<string>(characters);
    }

    public List<string> GetSelectedCharacters()
    {
        return new List<string>(_selectedCharacters);
    }

    /// <summary>
    /// BGM AudioSource를 초기화합니다.
    /// </summary>
    private void InitializeBGM()
    {
        GameObject bgmObj = new GameObject("BGM");
        bgmObj.transform.SetParent(transform);
        BGMSource = bgmObj.AddComponent<AudioSource>();
        BGMSource.loop = true;
        BGMSource.playOnAwake = false;
        UpdateBGMVolume(); // 설정된 볼륨 적용
    }
    
    /// <summary>
    /// BGM을 재생합니다.
    /// </summary>
    /// <param name="bgmPath">Resources 폴더 기준 경로 (예: "Audio/BGM")</param>
    public void PlayBGM(string bgmPath)
    {
        if (BGMSource == null || string.IsNullOrEmpty(bgmPath)) return;
        
        AudioClip bgm = Resources.Load<AudioClip>(bgmPath);
        if (bgm == null)
        {
            LogManager.LogWarning(LogCategory.System, $"BGM '{bgmPath}' not found in Resources.");
            return;
        }
        
        // 이미 같은 BGM이 재생 중이면 스킵
        if (BGMSource.clip == bgm && BGMSource.isPlaying)
        {
            return;
        }
        
        BGMSource.clip = bgm;
        UpdateBGMVolume(); // 볼륨 적용
        BGMSource.Play();
        LogManager.LogInfo(LogCategory.System, $"BGM 재생: {bgmPath}");
    }
    
    /// <summary>
    /// BGM 볼륨을 업데이트합니다.
    /// 설정 변경 시 호출하면 즉시 반영됩니다.
    /// </summary>
    public void UpdateBGMVolume()
    {
        if (BGMSource != null && AudioSettings != null)
        {
            BGMSource.volume = AudioSettings.EffectiveBGMVolume;
        }
    }
    
    /// <summary>
    /// 기존 플레이어들을 다음 스테이지에 대비합니다.
    /// 죽은 플레이어는 부활시키고, 모든 플레이어의 공격 상태를 리셋합니다.
    /// </summary>
    public void PrepareExistingPlayers()
    {
        if (PlayerController == null) return;
        
        ReviveAllPlayers(); // 죽은 플레이어 부활
        ResetAllPlayerAttackStates(); // 모든 플레이어 공격 리셋
        
        LogManager.LogInfo(LogCategory.System, $"{PlayerController.playerPawns.Count}명의 플레이어 준비 완료");
    }
    
    /// <summary>
    /// 모든 플레이어를 부활시킵니다 (다음 스테이지 시작 시).
    /// </summary>
    private void ReviveAllPlayers()
    {
        if (PlayerController == null) return;

        int revivedCount = 0;
        foreach (var pawn in PlayerController.playerPawns)
        {
            if (pawn != null && !pawn.activeInHierarchy)
            {
                pawn.SetActive(true);
                
                // 체력 회복
                var pawnManager = pawn.GetComponent<PawnManager>();
                if (pawnManager?.PawnData?.healthData != null)
                {
                    pawnManager.PawnData.healthData.currentHealth = pawnManager.PawnData.healthData.maxHealth;
                }
                
                revivedCount++;
            }
        }
        
        if (revivedCount > 0)
        {
            LogManager.LogInfo(LogCategory.System, $"{revivedCount}명의 플레이어 부활");
        }
    }
    
    /// <summary>
    /// 모든 플레이어의 공격 상태를 리셋합니다.
    /// 살아있는 플레이어도 다음 스테이지에서 공격할 수 있도록 합니다.
    /// </summary>
    private void ResetAllPlayerAttackStates()
    {
        if (PlayerController == null) return;

        int resetCount = 0;
        foreach (var pawn in PlayerController.playerPawns)
        {
            if (pawn != null && pawn.activeInHierarchy)
            {
                // AttackSubManager가 있으면 리셋
                var attackSubManager = pawn.GetComponent<AttackSubManager>();
                if (attackSubManager != null)
                {
                    attackSubManager.ResetForNewStage(); // ✅ 명확한 의도
                    resetCount++;
                }
            }
        }
        
        if (resetCount > 0)
        {
            LogManager.LogInfo(LogCategory.System, $"{resetCount}개의 공격 SubManager 상태 리셋");
        }
    }

    /// <summary>
    /// 스테이지를 종료합니다.
    /// </summary>
    public void EndStage()
    {
        // UseCase를 통해 스테이지 종료 준비
        StageManagementUseCase.PrepareStageEnd();
        
        // StageManager 종료 처리
        if (StageManager != null)
        {
            StageManager.EndStage();
        }
        
        LogManager.LogInfo(LogCategory.Stage, "스테이지 종료");
    }

    private void HandleStageStartRequested(string stageName)
    {
        if (StageManager != null)
        {
            StageManager.StartStage(stageName, resetSession: false);
        }
        if (PawnSurvivors.UI.UIManager.Instance != null)
        {
            PawnSurvivors.UI.UIManager.Instance.ShowStageScreen();
        }
    }
    
    private void HandleStageCompletedToShop()
    {
        EndStage();
        if (PawnSurvivors.UI.UIManager.Instance != null)
        {
            PawnSurvivors.UI.UIManager.Instance.ShowShopScreen();
        }
    }
    
    private void HandleGameOver()
    {
        // 게임오버 시 캐릭터 선택 상태 저장
        if (CharacterSelectionUseCase != null)
        {
            CharacterSelectionUseCase.SaveSelectedCharacters();
        }
        
        if (PawnSurvivors.UI.UIManager.Instance != null)
        {
            PawnSurvivors.UI.UIManager.Instance.ShowGameOverScreen();
        }
    }
    
    private void HandleAllStagesCleared()
    {
        if (PawnSurvivors.UI.UIManager.Instance != null)
        {
            PawnSurvivors.UI.UIManager.Instance.ShowGameOverScreen();
        }
    }
}

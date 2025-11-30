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
    
    /// <summary>
    /// 플레이어 컨트롤러 (입력 받는 중심 오브젝트)
    /// </summary>
    public PlayerController PlayerController { get; private set; }
    
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
        // 각 캐릭터 1개씩
        AddPlayerPawn("Player");
        AddPlayerPawn("PlayerButter");
        AddPlayerPawn("PlayerOpal");
        
        LogManager.LogInfo(LogCategory.System, "총 3명의 플레이어 생성 완료 (Player x1, PlayerButter x1, PlayerOpal x1)");
        // ========== 초기 플레이어 생성 끝 ==========
        
        // ========== 테스트용 아이템 추가 ==========
        // Pawn들이 생성된 후 아이템 추가
        AddTestItems();
        // ========== 테스트용 아이템 추가 끝 ==========
    }
    
    /// <summary>
    /// 테스트용 아이템을 추가합니다.
    /// </summary>
    private void AddTestItems()
    {
        if (ItemManagementUseCase == null || ItemRepository == null) return;
        
        // 1. 전역 아이템: 공격력 +1 (모든 Pawn에 적용)
        var globalDamageItem = new PawnSurvivors.Data.ItemData
        {
            itemId = "TestGlobalDamagePlus1",
            itemName = "전역 공격력 +1",
            itemType = PawnSurvivors.Domain.ItemType.Global,
            description = "모든 캐릭터의 공격력이 1 증가합니다.",
            cost = 0
        };
        globalDamageItem.SetStatModifier(PawnSurvivors.Domain.StatKey.Damage, 1f);
        
        // 골드 없이 강제로 추가 (테스트용)
        ItemRepository.SaveItem(globalDamageItem);
        LogManager.LogInfo(LogCategory.Item, "테스트 전역 아이템 추가: 공격력 +1 (모든 Pawn에 적용)");
        
        // 2. 장착 아이템: 공격력 2배 (1번째 Pawn에 장착)
        var equippedDamageItem = new PawnSurvivors.Data.ItemData
        {
            itemId = "TestEquippedDamageX2",
            itemName = "공격력 2배",
            itemType = PawnSurvivors.Domain.ItemType.Equipped,
            description = "장착한 캐릭터의 공격력이 2배가 됩니다.",
            cost = 0
        };
        equippedDamageItem.SetStatMultiplier(PawnSurvivors.Domain.StatKey.Damage, 2f);
        
        // 골드 없이 강제로 추가 (테스트용)
        ItemRepository.SaveItem(equippedDamageItem);
        
        // 1번째 Pawn (playerIndex = 0)에 장착
        if (PlayerController != null && PlayerController.playerPawns != null && PlayerController.playerPawns.Count > 0)
        {
            var firstPawn = PlayerController.playerPawns[0];
            if (firstPawn != null)
            {
                var pawnManager = firstPawn.GetComponent<PawnManager>();
                if (pawnManager != null && pawnManager.PawnData != null)
                {
                    int playerIndex = pawnManager.PawnData.playerIndex;
                    if (playerIndex >= 0)
                    {
                        ItemManagementUseCase.EquipItemToPawn(equippedDamageItem.itemId, playerIndex);
                        LogManager.LogInfo(LogCategory.Item, $"테스트 장착 아이템 추가: 공격력 2배 → Pawn {playerIndex} ({firstPawn.name})");
                    }
                    else
                    {
                        LogManager.LogWarning(LogCategory.Item, "첫 번째 Pawn의 playerIndex가 설정되지 않았습니다.");
                    }
                }
            }
        }
        else
        {
            LogManager.LogWarning(LogCategory.Item, "PlayerController 또는 playerPawns가 없어 테스트 아이템을 장착할 수 없습니다.");
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // 세션 데이터 초기화 (Data 계층 내부)
        _sessionData = new GameSessionData();
        
        // Repository 초기화 (Data 계층 구현체)
        SessionDataRepository = new SessionDataRepository(_sessionData);
        ItemRepository = new ItemRepository(_sessionData);
        
        // StageDataSource 초기화 (UseCase보다 먼저 초기화 필요)
        _stageDataSource = new PawnSurvivors.Data.DataSources.StageDataSource();
        string stagesPath = Path.Combine(Application.streamingAssetsPath, "Stages");
        _stageDataSource.LoadStages(stagesPath);
        
        // StageListDataSource 초기화
        _stageListDataSource = new PawnSurvivors.Data.DataSources.StageListDataSource();
        string stageListPath = Path.Combine(Application.streamingAssetsPath, "Stages", "StageList.json");
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
        CreationManager = GetComponent<CreationManager>();
        StageManager = GetComponent<StageManager>();
        
        // LifecycleManager 설정 후 UseCase 업데이트
        if (LifecycleManager != null)
        {
            SurvivalTimeTrackingUseCase = new SurvivalTimeTrackingUseCase(SessionDataRepository, LifecycleManager);
        }

        // StageDataSource는 위에서 이미 초기화됨

        // ItemPoolDataSource 초기화
        _itemPoolDataSource = new PawnSurvivors.Data.DataSources.ItemPoolDataSource();
        string itemsPath = Path.Combine(Application.streamingAssetsPath, "Recipes", "Items");
        _itemPoolDataSource.LoadItems(itemsPath);
        
        // ItemPoolUseCase 초기화
        var itemPoolRepository = new PawnSurvivors.Data.Repositories.ItemPoolRepository(_itemPoolDataSource);
        ItemPoolUseCase = new ItemPoolUseCase(itemPoolRepository, ItemRepository);
        
        // ShopUseCase 초기화
        ShopUseCase = new ShopUseCase(ItemPoolUseCase, ItemRepository, CurrencyUseCase);

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
        return _stageDataSource?.GetAllStageNames() ?? new List<string>();
    }

    public void PauseStage(string stageName)
    {
        LifecycleManager.TogglePause();
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
}

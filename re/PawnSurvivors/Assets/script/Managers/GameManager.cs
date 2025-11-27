using UnityEngine;
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
    public StageLoader _stageLoader;
    
    /// <summary>
    /// 현재 게임 세션의 런타임 데이터 (Data 계층 내부용, 외부 접근 불가)
    /// </summary>
    private GameSessionData _sessionData;
    
    /// <summary>
    /// 세션 데이터 Repository (Data 계층 내부용, UseCase에서만 사용)
    /// </summary>
    private ISessionDataRepository SessionDataRepository { get; set; }
    
    /// <summary>
    /// UseCase 인스턴스들
    /// </summary>
    public DamageTrackingUseCase DamageTrackingUseCase { get; private set; }
    public KillTrackingUseCase KillTrackingUseCase { get; private set; }
    public SurvivalTimeTrackingUseCase SurvivalTimeTrackingUseCase { get; private set; }
    public SessionManagementUseCase SessionManagementUseCase { get; private set; }
    public CurrencyUseCase CurrencyUseCase { get; private set; }
    public StageManagementUseCase StageManagementUseCase { get; private set; }
    
    /// <summary>
    /// 플레이어 컨트롤러 (입력 받는 중심 오브젝트)
    /// </summary>
    public PlayerController PlayerController { get; private set; }

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
            Debug.Log("[GameManager] 메인 카메라가 PlayerController에 붙었습니다.");
        }
        
        // ========== 초기 플레이어 생성 ==========
        // 각 캐릭터 1개씩
        AddPlayerPawn("Player");
        AddPlayerPawn("PlayerButter");
        AddPlayerPawn("PlayerOpal");
        
        Debug.Log("[GameManager] 총 3명의 플레이어 생성 완료 (Player x1, PlayerButter x1, PlayerOpal x1)");
        // ========== 초기 플레이어 생성 끝 ==========
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
        
        // UseCase 초기화
        DamageTrackingUseCase = new DamageTrackingUseCase(SessionDataRepository);
        KillTrackingUseCase = new KillTrackingUseCase(SessionDataRepository);
        SurvivalTimeTrackingUseCase = new SurvivalTimeTrackingUseCase(SessionDataRepository, null); // LifecycleManager는 나중에 설정
        SessionManagementUseCase = new SessionManagementUseCase(SessionDataRepository);
        CurrencyUseCase = new CurrencyUseCase(SessionDataRepository);
        StageManagementUseCase = new StageManagementUseCase(SessionDataRepository);

        // 동일한 GameObject에서 구성 요소 가져오기
        LifecycleManager = GetComponent<LifecycleManager>();
        CreationManager = GetComponent<CreationManager>();
        StageManager = GetComponent<StageManager>();
        
        // LifecycleManager 설정 후 UseCase 업데이트
        if (LifecycleManager != null)
        {
            SurvivalTimeTrackingUseCase = new SurvivalTimeTrackingUseCase(SessionDataRepository, LifecycleManager);
        }

        _stageLoader = new StageLoader();
        string stagesPath = Path.Combine(Application.streamingAssetsPath, "Stages");
        _stageLoader.LoadStages(stagesPath);

        // StageManager 초기화 (의존성 주입)
        if (StageManager != null)
        {
            StageManager.Initialize(CreationManager, _stageLoader, StageManagementUseCase);
        }

        if (LifecycleManager == null)
        {
            Debug.LogError("GameManager: LifecycleManager component not found on the same GameObject.");
        }
        if (CreationManager == null)
        {
            Debug.LogError("GameManager: CreationManager component not found on the same GameObject.");
        }
        if (StageManager == null)
        {
            Debug.LogError("GameManager: StageManager component not found on the same GameObject.");
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
            Debug.LogError("[GameManager] StageManager가 없습니다!");
        }
    }

    /// <summary>
    /// 플레이어블 폰을 추가합니다.
    /// </summary>
    public GameObject AddPlayerPawn(string recipeName, Vector3? worldPosition = null)
    {
        if (PlayerController == null)
        {
            Debug.LogError("[GameManager] PlayerController가 없습니다!");
            return null;
        }
        
        var recipe = CreationManager.GetRecipe(recipeName);
        if (recipe == null)
        {
            Debug.LogError($"[GameManager] Recipe '{recipeName}'를 찾을 수 없습니다!");
            return null;
        }
        
        Vector3 spawnPos = worldPosition ?? PlayerController.transform.position;
        GameObject pawn = CreationManager.CreatePawn(recipe, spawnPos, Quaternion.identity);
        
        if (pawn != null)
        {
            PlayerController.AddPlayerPawn(pawn);
            Debug.Log($"[GameManager] 플레이어 폰 추가: {recipeName}");
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
        return _stageLoader.GetStage(stageName);
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
        
        Debug.Log("[GameManager] 스테이지 종료");
    }
}

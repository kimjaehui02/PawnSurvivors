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
    /// 현재 게임 세션의 런타임 데이터
    /// </summary>
    public GameSessionData SessionData { get; private set; }
    
    /// <summary>
    /// 세션 데이터 Repository (Domain 계층용)
    /// </summary>
    public ISessionDataRepository SessionDataRepository { get; private set; }
    
    /// <summary>
    /// UseCase 인스턴스들
    /// </summary>
    public DamageTrackingUseCase DamageTrackingUseCase { get; private set; }
    public KillTrackingUseCase KillTrackingUseCase { get; private set; }
    public SurvivalTimeTrackingUseCase SurvivalTimeTrackingUseCase { get; private set; }
    
    /// <summary>
    /// 플레이어 컨트롤러 (입력 받는 중심 오브젝트)
    /// </summary>
    public PlayerController PlayerController { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // 세션 데이터 초기화
        SessionData = new GameSessionData();
        
        // Repository 초기화
        SessionDataRepository = new SessionDataRepository(SessionData);
        
        // UseCase 초기화
        DamageTrackingUseCase = new DamageTrackingUseCase(SessionDataRepository);
        KillTrackingUseCase = new KillTrackingUseCase(SessionDataRepository);
        SurvivalTimeTrackingUseCase = new SurvivalTimeTrackingUseCase(SessionDataRepository, null); // LifecycleManager는 나중에 설정

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
    /// </summary>
    /// <param name="stageName">시작할 스테이지 이름 (기본값: DebugStage)</param>
    /// <param name="resetSession">세션 데이터를 리셋할지 여부 (기본값: true, 상점에서 올 때는 false)</param>
    public void StartStage(string stageName = "DebugStage", bool resetSession = true)
    {
        // 세션 데이터 리셋 (새 게임 시작 시에만)
        if (resetSession)
        {
            SessionData.Reset();
        }
        SessionData.currentStageName = stageName;
        
        Debug.Log($"[GameManager] 스테이지 시작: {stageName} (세션 리셋: {resetSession})");
        
        // PlayerController가 없으면 생성 (상점에서 올 때는 기존 것 유지)
        if (PlayerController == null)
        {
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
            // 기본 플레이어 3개
            for (int i = 0; i < 3; i++)
            {
                AddPlayerPawn("Player");
            }
            
            // 버터 캐릭터 3개
            for (int i = 0; i < 3; i++)
            {
                AddPlayerPawn("PlayerButter");
            }
            
            Debug.Log("[GameManager] 총 6명의 플레이어 생성 완료 (Player x3, PlayerButter x3)");
            // ========== 초기 플레이어 생성 끝 ==========
        }

        // 스테이지 로드 및 시작
        StageData stageData = _stageLoader.GetStage(stageName);
        if (stageData != null)
        {
            StageManager.Initialize(CreationManager, stageData);
            StageManager.StartStage();
            Debug.Log($"Stage '{stageName}' started.");
        }
        else
        {
            Debug.LogError($"StageData for '{stageName}' not found! Cannot start stage.");
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


    public void EndStage(string stageName)
    {
        // StageManager.EndStage();
    }
}

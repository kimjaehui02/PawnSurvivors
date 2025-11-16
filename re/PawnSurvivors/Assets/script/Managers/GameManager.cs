using UnityEngine;
using PawnSurvivors.Managers;
using PawnSurvivors.Data;
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

        // 동일한 GameObject에서 구성 요소 가져오기
        LifecycleManager = GetComponent<LifecycleManager>();
        CreationManager = GetComponent<CreationManager>();
        StageManager = GetComponent<StageManager>();

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
    /// <param name="stageName">시작할 스테이지 이름 (기본값: Stage1)</param>
    public void StartStage(string stageName = "Stage1")
    {
        // 세션 데이터 리셋 (새 게임 시작)
        SessionData.Reset();
        SessionData.currentStageName = stageName;
        
        Debug.Log($"[GameManager] 새 게임 세션 시작: {SessionData}");
        
        // PlayerController 생성 (입력 받는 중심 오브젝트)
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
        
        // ========== 인원 테스트 ==========
        // 이 숫자를 1~6 사이로 변경해서 대열 테스트!
        int testPlayerCount = 3;
        
        for (int i = 0; i < testPlayerCount; i++)
        {
            AddPlayerPawn("Player");
        }
        // ========== 인원 테스트 끝 ==========

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

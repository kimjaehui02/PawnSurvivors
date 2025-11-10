using UnityEngine;
using PawnSurvivors.Managers;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public LifecycleManager LifecycleManager { get; private set; }
    public CreationManager CreationManager { get; private set; }
    public StageManager StageManager { get; private set; }
    private StageLoader _stageLoader;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

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
        // 플레이어 폰 생성
        PawnCore.Recipes.Json.PawnRecipeData playerRecipe = CreationManager.GetRecipe("Player");
        if (playerRecipe != null)
        {
            CreationManager.CreatePawn(playerRecipe, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Player recipe not found! Cannot create player pawn.");
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
    /// 스테이지를 로드합니다.
    /// </summary>
    public StageData LoadStage(string stageName)
    {
        return _stageLoader.GetStage(stageName);
    }
}

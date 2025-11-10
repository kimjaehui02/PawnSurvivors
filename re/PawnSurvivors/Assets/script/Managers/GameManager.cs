using UnityEngine;
using PawnSurvivors.Managers;
using PawnSurvivors.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public LifecycleManager LifecycleManager { get; private set; }
    public CreationManager CreationManager { get; private set; }
    public StageManager StageManager { get; private set; }
    public UIManager UIManager { get; private set; }
    private StageLoader _stageLoader;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 동일한 GameObject에서 구성 요소 가져오기
        LifecycleManager = GetComponent<LifecycleManager>();
        CreationManager = GetComponent<CreationManager>();
        StageManager = GetComponent<StageManager>();
        UIManager = GetComponent<UIManager>();

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
        if (UIManager == null)
        {
            Debug.LogWarning("GameManager: UIManager component not found on the same GameObject. UI functionality will be limited.");
        }
    }

    private void Start()
    {
        // UI 시스템이 있으면 타이틀 화면부터 시작
        // UI 시스템이 없으면 기존처럼 바로 게임 시작 (테스트용)
        if (UIManager != null)
        {
            Debug.Log("GameManager: UI System detected. Starting from Title Screen.");
            // UIManager가 자동으로 타이틀 화면을 표시함
        }
        else
        {
            Debug.LogWarning("GameManager: No UI System. Starting game directly (Test Mode).");
            StartGameDirectly();
        }
    }

    /// <summary>
    /// UI 없이 게임을 직접 시작합니다 (테스트용).
    /// </summary>
    public void StartGameDirectly()
    {
        // "Player" 레시피를 사용하여 플레이어 폰 생성
        PawnCore.Recipes.Json.PawnRecipeData playerRecipe = CreationManager.GetRecipe("Player");
        if (playerRecipe != null)
        {
            CreationManager.CreatePawn(playerRecipe, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Player recipe not found! Cannot create player pawn.");
        }

        // StageManager를 초기화하고 스테이지 시작
        StageData currentStage = _stageLoader.GetStage("Stage1"); // 기본 스테이지 이름 "Stage1"으로 가정
        if (currentStage != null)
        {
            StageManager.Initialize(CreationManager, currentStage);
            StageManager.StartStage();
        }
        else
        {
            Debug.LogError("StageData for Stage1 not found! Cannot start stage.");
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

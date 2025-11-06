using UnityEngine;
using PawnSurvivors.Managers;

public class StageManager : MonoBehaviour
{
    private CreationManager _creationManager;
    private StageData _currentStageData;

    private float _spawnTimer = 0f;
    private float _spawnInterval; // StageData에서 초기화
    private float _spawnRadius; // StageData에서 초기화
    private float _spawnDistanceFromCamera; // StageData에서 초기화

    public void Initialize(CreationManager creationManager, StageData stageData)
    {
        _creationManager = creationManager;
        _currentStageData = stageData;

        // Use stageData to initialize variables
        _spawnInterval = _currentStageData.spawnInterval;
        _spawnRadius = _currentStageData.spawnRadius;
        _spawnDistanceFromCamera = _currentStageData.spawnDistanceFromCamera;

        // Set initial spawn timer based on interval
        _spawnTimer = _spawnInterval; 
    }

    public void StartStage()
    {
        Debug.Log("Stage Started!");
    }

    public void UpdateStage()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f)
        {
            SpawnEnemy();
            _spawnTimer = _spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        if (_creationManager == null) return;

        PawnCore.Recipes.Json.PawnRecipeData enemyRecipe = _creationManager.GetRecipe("Enemy");
        if (enemyRecipe != null)
        {
            Vector3 spawnPosition = GetCircularSpawnPosition();
            _creationManager.CreatePawn(enemyRecipe, spawnPosition, Quaternion.identity);
            Debug.Log($"Spawned Enemy at {spawnPosition}");
        }
        else
        {
            Debug.LogError("Enemy recipe not found for spawning!");
        }
    }

    private Vector3 GetCircularSpawnPosition()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Cannot calculate circular spawn position. Defaulting to camera Y position.");
            return new Vector3(0, 0, 0); // Default to origin if camera not found
        }

        // 카메라 뷰포트의 바깥쪽에서 원형으로 스폰 위치 계산
        float angle = Random.Range(0f, 2f * Mathf.PI);

        Vector3 cameraCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane));

        // 스폰 반지름을 카메라 뷰포트 크기와 distanceFromCamera를 기반으로 조정
        float viewportHalfWidth = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, mainCamera.nearClipPlane)).x - cameraCenter.x;
        float viewportHalfHeight = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1, mainCamera.nearClipPlane)).y - cameraCenter.y;
        float effectiveRadius = Mathf.Max(viewportHalfWidth, viewportHalfHeight) + _spawnDistanceFromCamera;

        // 원형 위치 계산
        float x = cameraCenter.x + Mathf.Cos(angle) * effectiveRadius;
        float y = cameraCenter.y + Mathf.Sin(angle) * effectiveRadius;

        return new Vector3(x, y, 0);
    }
}

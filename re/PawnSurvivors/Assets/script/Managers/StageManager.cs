using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PawnSurvivors.Managers;

public class StageManager : MonoBehaviour
{
    private CreationManager _creationManager;
    private StageData _currentStageData;

    private float _stageElapsedTime = 0f;
    private float _spawnRadius;
    private float _spawnDistanceFromCamera;
    
    // 웨이브별 타이머 관리
    private Dictionary<EnemyWave, float> _waveTimers = new Dictionary<EnemyWave, float>();
    private Dictionary<EnemyWave, bool> _waveSpawnedOnce = new Dictionary<EnemyWave, bool>();

    public void Initialize(CreationManager creationManager, StageData stageData)
    {
        _creationManager = creationManager;
        _currentStageData = stageData;

        // stageData를 사용하여 변수 초기화
        _spawnRadius = _currentStageData.spawnRadius;
        _spawnDistanceFromCamera = _currentStageData.spawnDistanceFromCamera;

        // 웨이브 타이머 초기화
        _waveTimers.Clear();
        _waveSpawnedOnce.Clear();
        if (_currentStageData.enemyWaves != null)
        {
            foreach (var wave in _currentStageData.enemyWaves)
            {
                _waveTimers[wave] = wave.spawnInterval; // 첫 스폰까지의 시간
                _waveSpawnedOnce[wave] = false;
            }
        }

        _stageElapsedTime = 0f;
    }

    public void StartStage()
    {
        Debug.Log($"Stage '{_currentStageData.stageName}' Started!");
        _stageElapsedTime = 0f;
        
        // LifecycleManager의 게임 시간도 리셋
        if (GameManager.Instance?.LifecycleManager != null)
        {
            GameManager.Instance.LifecycleManager.ResetGameTime();
        }
    }

    public void UpdateStage()
    {
        if (_currentStageData == null || _creationManager == null) return;

        float deltaTime = GetGameDeltaTime();
        _stageElapsedTime += deltaTime;

        // 각 웨이브 처리
        if (_currentStageData.enemyWaves != null)
        {
            foreach (var wave in _currentStageData.enemyWaves)
            {
                ProcessWave(wave, deltaTime);
            }
        }
    }
    
    private float GetGameDeltaTime()
    {
        if (GameManager.Instance?.LifecycleManager != null)
        {
            return GameManager.Instance.LifecycleManager.GameDeltaTime;
        }
        return Time.deltaTime; // 폴백
    }

    private void ProcessWave(EnemyWave wave, float deltaTime)
    {
        // 웨이브 시간 범위 체크
        if (_stageElapsedTime < wave.startTime) return;
        if (wave.endTime >= 0f && _stageElapsedTime > wave.endTime) return;

        // 한 번만 소환하는 경우 체크
        if (wave.spawnOnce && _waveSpawnedOnce[wave]) return;

        // 타이머 업데이트
        _waveTimers[wave] -= deltaTime;
        
        if (_waveTimers[wave] <= 0f)
        {
            SpawnEnemyFromWave(wave);
            _waveTimers[wave] = wave.spawnInterval;
            
            if (wave.spawnOnce)
            {
                _waveSpawnedOnce[wave] = true;
            }
        }
    }

    private void SpawnEnemyFromWave(EnemyWave wave)
    {
        if (wave.enemyTypes == null || wave.enemyTypes.Count == 0)
        {
            Debug.LogWarning($"Wave has no enemy types defined!");
            return;
        }

        // 가중치 기반으로 적 선택
        string selectedEnemyRecipe = SelectEnemyByWeight(wave.enemyTypes);
        
        if (string.IsNullOrEmpty(selectedEnemyRecipe))
        {
            Debug.LogError("No valid enemy recipe selected!");
            return;
        }

        PawnCore.Recipes.Json.PawnRecipeData enemyRecipe = _creationManager.GetRecipe(selectedEnemyRecipe);
        if (enemyRecipe != null)
        {
            Vector3 spawnPosition = GetCircularSpawnPosition();
            _creationManager.CreatePawn(enemyRecipe, spawnPosition, Quaternion.identity);
            Debug.Log($"Spawned {selectedEnemyRecipe} at {spawnPosition}");
        }
        else
        {
            Debug.LogError($"Enemy recipe '{selectedEnemyRecipe}' not found for spawning!");
        }
    }

    private string SelectEnemyByWeight(List<EnemyType> enemyTypes)
    {
        int totalWeight = enemyTypes.Sum(e => e.weight);
        int randomValue = Random.Range(0, totalWeight);
        
        int currentWeight = 0;
        foreach (var enemyType in enemyTypes)
        {
            currentWeight += enemyType.weight;
            if (randomValue < currentWeight)
            {
                return enemyType.recipeName;
            }
        }
        
        // 폴백: 첫 번째 적
        return enemyTypes[0].recipeName;
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

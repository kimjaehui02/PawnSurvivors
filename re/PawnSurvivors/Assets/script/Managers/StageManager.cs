using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using PawnSurvivors.Managers;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Player;

public class StageManager : MonoBehaviour
{
    private CreationManager _creationManager;
    private PawnSurvivors.Data.DataSources.StageDataSource _stageDataSource;
    private StageManagementUseCase _stageManagementUseCase;
    private StageData _currentStageData;

    private float _stageElapsedTime = 0f;
    private float _spawnRadius;
    private float _spawnDistanceFromCamera;
    
    // 웨이브별 타이머 관리
    private Dictionary<EnemyWave, float> _waveTimers = new Dictionary<EnemyWave, float>();
    private Dictionary<EnemyWave, bool> _waveSpawnedOnce = new Dictionary<EnemyWave, bool>();

    /// <summary>
    /// StageManager를 초기화합니다. (의존성 주입)
    /// </summary>
    public void Initialize(CreationManager creationManager, PawnSurvivors.Data.DataSources.StageDataSource stageDataSource, StageManagementUseCase stageManagementUseCase)
    {
        _creationManager = creationManager;
        _stageDataSource = stageDataSource;
        _stageManagementUseCase = stageManagementUseCase;
    }


    /// <summary>
    /// 스테이지를 시작합니다. (전체 라이프사이클 담당)
    /// </summary>
    /// <param name="stageName">시작할 스테이지 이름</param>
    /// <param name="resetSession">세션 데이터를 리셋할지 여부</param>
    public void StartStage(string stageName, bool resetSession = true)
    {
        // UseCase를 통해 스테이지 시작 준비
        if (_stageManagementUseCase != null)
        {
            _stageManagementUseCase.PrepareStageStart(stageName, resetSession);
        }
        
        LogManager.LogInfo(LogCategory.Stage, $"스테이지 시작: {stageName} (세션 리셋: {resetSession})");
        
        // 배경 타일맵 생성
        if (GameManager.Instance != null && GameManager.Instance.BackgroundTilemapManager != null)
        {
            GameManager.Instance.BackgroundTilemapManager.CreateBackgroundTilemap();
        }
        
        // PlayerController가 없으면 생성 (상점에서 올 때는 기존 것 유지)
        if (GameManager.Instance != null && GameManager.Instance.PlayerController == null)
        {
            GameManager.Instance.CreatePlayerController();
        }

        // 스테이지 로드 및 초기화
        if (_stageDataSource != null)
        {
            StageData stageData = _stageDataSource.GetStage(stageName);
            if (stageData != null)
            {
                InitializeStageData(stageData);
                StartStageInternal();
                LogManager.LogInfo(LogCategory.Stage, $"Stage '{stageName}' started.");
            }
            else
            {
                LogManager.LogError(LogCategory.Stage, $"StageData for '{stageName}' not found! Cannot start stage.");
            }
        }
        else
        {
            LogManager.LogError(LogCategory.Stage, "StageDataSource가 초기화되지 않았습니다!");
        }
    }

    /// <summary>
    /// 스테이지 데이터로 초기화합니다.
    /// </summary>
    private void InitializeStageData(StageData stageData)
    {
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

    /// <summary>
    /// 스테이지 시작 내부 로직 (시간 리셋, ProjectileShooter 리셋 등)
    /// </summary>
    private void StartStageInternal()
    {
        LogManager.LogInfo(LogCategory.Stage, $"Stage '{_currentStageData.stageName}' Started!");
        _stageElapsedTime = 0f;
        
        // LifecycleManager의 게임 시간도 리셋
        if (GameManager.Instance?.LifecycleManager != null)
        {
            GameManager.Instance.LifecycleManager.ResetGameTime();
        }
        
        // 모든 플레이어 Pawn의 ProjectileShooterSubManager의 _nextFireTime 리셋
        ResetAllProjectileShooters();
    }
    
    /// <summary>
    /// 모든 플레이어 Pawn의 ProjectileShooterSubManager의 _nextFireTime을 리셋합니다.
    /// </summary>
    private void ResetAllProjectileShooters()
    {
        foreach (var pawnManager in PawnManager.AllPawnManagers)
        {
            if (pawnManager == null) continue;
            
                // 플레이어 태그를 가진 Pawn만 처리
                if (pawnManager.gameObject.CompareTag("Player"))
                {
                    var shooter = pawnManager.GetComponent<ProjectileShooterSubManager>();
                    if (shooter != null)
                    {
                        // 리플렉션을 사용하여 private 필드 _nextFireTime 리셋
                        var field = typeof(ProjectileShooterSubManager).GetField("_nextFireTime", 
                            BindingFlags.NonPublic | BindingFlags.Instance);
                        if (field != null)
                        {
                            field.SetValue(shooter, 0f);
                        }
                    }
                }
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
            LogManager.LogWarning(LogCategory.Stage, $"Wave has no enemy types defined!");
            return;
        }

        // 가중치 기반으로 적 선택
        string selectedEnemyRecipe = SelectEnemyByWeight(wave.enemyTypes);
        
        if (string.IsNullOrEmpty(selectedEnemyRecipe))
        {
            LogManager.LogError(LogCategory.Stage, "No valid enemy recipe selected!");
            return;
        }

        PawnSurvivors.Data.Recipes.PawnRecipeData enemyRecipe = _creationManager.GetRecipe(selectedEnemyRecipe);
        if (enemyRecipe != null)
        {
            Vector3 spawnPosition = GetCircularSpawnPosition();
            _creationManager.CreatePawn(enemyRecipe, spawnPosition, Quaternion.identity);
            LogManager.LogDebug(LogCategory.Stage, $"Spawned {selectedEnemyRecipe} at {spawnPosition}");
        }
        else
        {
            LogManager.LogError(LogCategory.Stage, $"Enemy recipe '{selectedEnemyRecipe}' not found for spawning!");
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
            LogManager.LogError(LogCategory.Stage, "Main Camera not found! Cannot calculate circular spawn position. Defaulting to camera Y position.");
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

    /// <summary>
    /// 스테이지를 종료합니다.
    /// </summary>
    public void EndStage()
    {
        LogManager.LogInfo(LogCategory.Stage, $"Stage '{_currentStageData?.stageName}' Ended!");
        
        // 웨이브 타이머 초기화
        _waveTimers.Clear();
        _waveSpawnedOnce.Clear();
        
        // 현재 스테이지 데이터 클리어
        _currentStageData = null;
        _stageElapsedTime = 0f;
    }
}

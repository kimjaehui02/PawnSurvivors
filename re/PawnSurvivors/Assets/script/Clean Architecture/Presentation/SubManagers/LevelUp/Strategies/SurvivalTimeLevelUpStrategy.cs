using UnityEngine;
using PawnSurvivors.Domain.Usecases;

/// <summary>
/// 생존 시간을 기준으로 레벨업하는 전략입니다.
/// </summary>
public class SurvivalTimeLevelUpStrategy : LevelUpStrategyBase
{
    /// <summary>레벨업에 필요한 생존 시간 (초)</summary>
    public float requiredSeconds = 30f;

    private float _timeAtStart = 0f;
    private SurvivalTimeTrackingUseCase _survivalTimeTrackingUseCase;

    public override void Init(PawnManager pawnManager)
    {
        base.Init(pawnManager);

        // UseCase 가져오기
        if (GameManager.Instance?.SurvivalTimeTrackingUseCase != null)
        {
            _survivalTimeTrackingUseCase = GameManager.Instance.SurvivalTimeTrackingUseCase;
            _timeAtStart = _survivalTimeTrackingUseCase.GetCurrentSurvivalTime();
        }
        else
        {
            Debug.LogWarning($"[SurvivalTimeLevelUpStrategy] {pawnManager.name} - SurvivalTimeTrackingUseCase를 찾을 수 없습니다.", this);
        }

        Debug.Log($"[SurvivalTimeLevelUpStrategy] {pawnManager.name} 초기화 - 목표: {requiredSeconds}초 생존", this);
    }

    public override bool CheckCondition()
    {
        if (_survivalTimeTrackingUseCase == null) return false;

        float elapsedTime = _survivalTimeTrackingUseCase.GetElapsedTimeSinceStart(_timeAtStart);
        return elapsedTime >= requiredSeconds;
    }

    public override float GetProgress()
    {
        if (_survivalTimeTrackingUseCase == null) return 0f;

        float elapsedTime = _survivalTimeTrackingUseCase.GetElapsedTimeSinceStart(_timeAtStart);
        return Mathf.Clamp01(elapsedTime / requiredSeconds);
    }

    public override string GetProgressText()
    {
        if (_survivalTimeTrackingUseCase == null) return "0/0";

        float elapsedTime = Mathf.Min(_survivalTimeTrackingUseCase.GetElapsedTimeSinceStart(_timeAtStart), requiredSeconds);
        return $"{elapsedTime:F1}/{requiredSeconds:F0}초 생존";
    }
}


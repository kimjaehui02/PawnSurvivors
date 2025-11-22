using UnityEngine;
using PawnSurvivors.Domain.Usecases;

/// <summary>
/// 적 처치 수를 기준으로 레벨업하는 전략입니다.
/// </summary>
public class KillCountLevelUpStrategy : LevelUpStrategyBase
{
    /// <summary>레벨업에 필요한 처치 수</summary>
    public int requiredKills = 3;

    private int _killsAtStart = 0;
    private KillTrackingUseCase _killTrackingUseCase;

    public override void Init(PawnManager pawnManager)
    {
        base.Init(pawnManager);

        // UseCase 가져오기
        if (GameManager.Instance?.KillTrackingUseCase != null)
        {
            _killTrackingUseCase = GameManager.Instance.KillTrackingUseCase;
            _killsAtStart = _killTrackingUseCase.GetTotalKills();
        }
        else
        {
            Debug.LogWarning($"[KillCountLevelUpStrategy] {pawnManager.name} - KillTrackingUseCase를 찾을 수 없습니다.", this);
        }

        Debug.Log($"[KillCountLevelUpStrategy] {pawnManager.name} 초기화 - 목표: {requiredKills}킬 (현재: {_killsAtStart})", this);
    }

    public override bool CheckCondition()
    {
        if (_killTrackingUseCase == null) return false;

        int killsSinceStart = _killTrackingUseCase.GetKillsSinceStart(_killsAtStart);
        return killsSinceStart >= requiredKills;
    }

    public override float GetProgress()
    {
        if (_killTrackingUseCase == null) return 0f;

        int killsSinceStart = _killTrackingUseCase.GetKillsSinceStart(_killsAtStart);
        return Mathf.Clamp01(killsSinceStart / (float)requiredKills);
    }

    public override string GetProgressText()
    {
        if (_killTrackingUseCase == null) return "0/0";

        int killsSinceStart = Mathf.Min(_killTrackingUseCase.GetKillsSinceStart(_killsAtStart), requiredKills);
        return $"{killsSinceStart}/{requiredKills} 처치";
    }
}


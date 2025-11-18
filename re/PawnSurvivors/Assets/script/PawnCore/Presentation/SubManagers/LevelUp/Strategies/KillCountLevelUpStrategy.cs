using UnityEngine;

/// <summary>
/// 적 처치 수를 기준으로 레벨업하는 전략입니다.
/// </summary>
public class KillCountLevelUpStrategy : LevelUpStrategyBase
{
    /// <summary>레벨업에 필요한 처치 수</summary>
    public int requiredKills = 3;

    private int _killsAtStart = 0;

    public override void Init(PawnManager pawnManager)
    {
        base.Init(pawnManager);

        // 현재 처치 수를 기준점으로 저장
        if (GameManager.Instance?.SessionData != null)
        {
            _killsAtStart = GameManager.Instance.SessionData.GetInt("enemiesKilled");
        }

        Debug.Log($"[KillCountLevelUpStrategy] {pawnManager.name} 초기화 - 목표: {requiredKills}킬 (현재: {_killsAtStart})", this);
    }

    public override bool CheckCondition()
    {
        if (GameManager.Instance?.SessionData == null) return false;

        int currentKills = GameManager.Instance.SessionData.GetInt("enemiesKilled");
        int killsSinceStart = currentKills - _killsAtStart;

        return killsSinceStart >= requiredKills;
    }

    public override float GetProgress()
    {
        if (GameManager.Instance?.SessionData == null) return 0f;

        int currentKills = GameManager.Instance.SessionData.GetInt("enemiesKilled");
        int killsSinceStart = currentKills - _killsAtStart;

        return Mathf.Clamp01(killsSinceStart / (float)requiredKills);
    }

    public override string GetProgressText()
    {
        if (GameManager.Instance?.SessionData == null) return "0/0";

        int currentKills = GameManager.Instance.SessionData.GetInt("enemiesKilled");
        int killsSinceStart = Mathf.Min(currentKills - _killsAtStart, requiredKills);

        return $"{killsSinceStart}/{requiredKills} 처치";
    }
}


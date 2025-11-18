using UnityEngine;

/// <summary>
/// 생존 시간을 기준으로 레벨업하는 전략입니다.
/// </summary>
public class SurvivalTimeLevelUpStrategy : LevelUpStrategyBase
{
    /// <summary>레벨업에 필요한 생존 시간 (초)</summary>
    public float requiredSeconds = 30f;

    private float _timeAtStart = 0f;

    public override void Init(PawnManager pawnManager)
    {
        base.Init(pawnManager);

        // 현재 게임 시간을 기준점으로 저장 (pause-aware time)
        if (GameManager.Instance?.LifecycleManager != null)
        {
            _timeAtStart = GameManager.Instance.LifecycleManager.GameTime;
        }

        Debug.Log($"[SurvivalTimeLevelUpStrategy] {pawnManager.name} 초기화 - 목표: {requiredSeconds}초 생존", this);
    }

    public override bool CheckCondition()
    {
        if (GameManager.Instance?.LifecycleManager == null) return false;

        float currentTime = GameManager.Instance.LifecycleManager.GameTime;
        float elapsedTime = currentTime - _timeAtStart;

        return elapsedTime >= requiredSeconds;
    }

    public override float GetProgress()
    {
        if (GameManager.Instance?.LifecycleManager == null) return 0f;

        float currentTime = GameManager.Instance.LifecycleManager.GameTime;
        float elapsedTime = currentTime - _timeAtStart;

        return Mathf.Clamp01(elapsedTime / requiredSeconds);
    }

    public override string GetProgressText()
    {
        if (GameManager.Instance?.LifecycleManager == null) return "0/0";

        float currentTime = GameManager.Instance.LifecycleManager.GameTime;
        float elapsedTime = Mathf.Min(currentTime - _timeAtStart, requiredSeconds);

        return $"{elapsedTime:F1}/{requiredSeconds:F0}초 생존";
    }
}


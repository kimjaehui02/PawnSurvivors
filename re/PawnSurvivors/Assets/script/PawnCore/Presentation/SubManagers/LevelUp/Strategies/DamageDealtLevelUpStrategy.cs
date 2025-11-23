using UnityEngine;

/// <summary>
/// 누적 데미지를 기준으로 레벨업하는 전략입니다.
/// DamageableSubManager가 ExperienceData.currentProgress를 증가시키고,
/// 이 전략은 그 값을 읽어서 레벨업 조건을 확인합니다.
/// </summary>
public class DamageDealtLevelUpStrategy : LevelUpStrategyBase
{
    /// <summary>레벨업에 필요한 누적 데미지</summary>
    public float requiredDamage = 500f;

    /// <summary>전략 활성화 시점의 진행도</summary>
    private float _damageAtStart = 0f;

    public override void Init(PawnManager pawnManager)
    {
        base.Init(pawnManager);

        // ExperienceData는 LevelUpSubManager.SubStart()에서 이미 생성됨
        // 전략 활성화 시점의 진행도 저장
        if (_pawnManager.PawnData?.experienceData != null)
        {
            _damageAtStart = _pawnManager.PawnData.experienceData.currentProgress;
        }

        Debug.Log($"[DamageDealtLevelUpStrategy] {pawnManager.name} 초기화 - 목표: {requiredDamage} 데미지, 현재 진행도: {_damageAtStart:F0}", this);
    }


    public override bool CheckCondition()
    {
        if (_pawnManager?.PawnData?.experienceData == null) return false;
        
        float progressSinceStart = _pawnManager.PawnData.experienceData.currentProgress - _damageAtStart;
        return progressSinceStart >= requiredDamage;
    }

    public override float GetProgress()
    {
        if (_pawnManager?.PawnData?.experienceData == null) return 0f;
        
        float progressSinceStart = _pawnManager.PawnData.experienceData.currentProgress - _damageAtStart;
        return Mathf.Clamp01(progressSinceStart / requiredDamage);
    }

    public override string GetProgressText()
    {
        if (_pawnManager?.PawnData?.experienceData == null) return "0/0";
        
        float progressSinceStart = Mathf.Min(_pawnManager.PawnData.experienceData.currentProgress - _damageAtStart, requiredDamage);
        return $"{progressSinceStart:F0}/{requiredDamage:F0} 데미지";
    }
}


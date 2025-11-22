namespace PawnSurvivors.Domain
{
    /// <summary>
    /// 세션 데이터의 정수 값 키를 정의하는 enum입니다.
    /// 타입 안정성과 IDE 자동완성을 제공합니다.
    /// </summary>
    public enum SessionDataIntKey
    {
        // 통계
        EnemiesKilled,
        
        // 재화
        Gold,
        
        // 레벨/경험치
        Level,
        Experience,
        
        // 기타
        Wave,
        ProjectilesFired,
    }

    /// <summary>
    /// 세션 데이터의 실수 값 키를 정의하는 enum입니다.
    /// 타입 안정성과 IDE 자동완성을 제공합니다.
    /// </summary>
    public enum SessionDataFloatKey
    {
        // 데미지 통계
        TotalDamageDealt,
        TotalDamageTaken,
        
        // 시간 통계
        SurvivalTime,
        
        // 기타
        DamageDealt,
    }
}


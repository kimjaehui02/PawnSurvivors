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

    /// <summary>
    /// 아이템 타입을 정의하는 enum입니다.
    /// </summary>
    public enum ItemType
    {
        /// <summary>모든 Pawn에 자동 적용되는 전역 아이템</summary>
        Global,
        
        /// <summary>개별 Pawn에 장착하는 아이템</summary>
        Equipped,
    }

    /// <summary>
    /// 스탯 키를 정의하는 enum입니다.
    /// 아이템의 statModifiers에서 사용됩니다.
    /// </summary>
    public enum StatKey
    {
        // 전투 관련
        Damage,              // 데미지
        MaxHealth,           // 최대 체력
        FireRate,            // 공격 속도
        ProjectileSpeed,     // 투사체 속도
        
        // 이동 관련
        MoveSpeed,           // 이동 속도
        
        // 방어 관련
        Armor,               // 방어력
        Shield,              // 방어막
        
        // 기타
        CritChance,          // 치명타 확률
        CritDamage,          // 치명타 데미지
        LifeSteal,           // 생명력 흡수
        Regeneration,        // 재생력
        Range,               // 사거리
    }

    /// <summary>
    /// 업그레이드 키를 정의하는 enum입니다.
    /// 아이템의 upgradeModifiers에서 사용됩니다.
    /// </summary>
    public enum UpgradeKey
    {
        AttackSpeed,         // 공격 속도 업그레이드
        Damage,              // 데미지 업그레이드
        Health,              // 체력 업그레이드
        Speed,               // 이동 속도 업그레이드
    }

    /// <summary>
    /// 플레이어 캐릭터를 정의하는 enum입니다.
    /// 타입 안정성을 위해 문자열 대신 enum을 사용합니다.
    /// </summary>
    public enum PlayerCharacter
    {
        PlayerBeni,
        PlayerButter,
        PlayerElena,
        PlayerEpica,
        PlayerErpin,
        PlayerOpal,
        PlayerRufo,
        PlayerSpeaki,
        PlayerTig,
        PlayerUi,
    }

    /// <summary>
    /// 게임 설정 상수입니다.
    /// </summary>
    public static class GameConstants
    {
        /// <summary>
        /// 최대 보유 가능한 캐릭터 종류 수 (1 메인 + 6 서포트)
        /// </summary>
        public const int MAX_CHARACTER_TYPES = 7;
    }

    /// <summary>
    /// 아이템 기능 타입을 정의하는 enum입니다.
    /// 아이템의 itemFunctionType에서 사용됩니다.
    /// </summary>
    public enum ItemFunctionType
    {
        None,           // 기능 없음
        OnKill,         // 적 처치 시
        OnHit,          // 공격 시
        OnDamageTaken,  // 피해 받을 시
    }
}


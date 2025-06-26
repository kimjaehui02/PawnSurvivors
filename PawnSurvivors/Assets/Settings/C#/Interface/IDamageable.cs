using System;

/// <summary>
/// 피해를 입을 수 있는 모든 게임 오브젝트가 구현해야 하는 인터페이스입니다.
/// 이 인터페이스를 통해 체력 관리 및 피해 처리 로직에 접근할 수 있습니다.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// 현재 체력을 가져옵니다. (읽기 전용)
    /// </summary>
    float CurrentHealth { get; }

    /// <summary>
    /// 최대 체력을 가져옵니다. (읽기 전용)
    /// </summary>
    float MaxHealth { get; }

    event Action<float> OnDamaged; // 객체가 피해를 입었을 때 발생합니다.

    /// <summary>
    /// 지정된 양의 피해를 '받습니다'.
    /// </summary>
    /// <param name="amount">'받을' 피해량입니다.</param>
    void TakeDamage(float amount);

}
using System;
using UnityEngine;

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

    event Action<float> OnHit;

    /// <summary>
    /// 지정된 양의 피해를 입힙니다.
    /// </summary>
    /// <param name="amount">입힐 피해량입니다.</param>
    void TakeDamage(float amount); // <-- 이런 메서드가 추가될 수 있습니다.

    // 추가적으로 체력 변경 이벤트도 포함될 수 있습니다.
    // event Action<float, float> OnHealthChanged; // (currentHealth, maxHealth)
    // event Action OnDeath;

}


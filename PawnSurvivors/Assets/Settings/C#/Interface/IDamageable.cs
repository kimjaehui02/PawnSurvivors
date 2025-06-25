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

    /// <summary>
    /// 대상에게 피해를 입히는 메서드입니다.
    /// </summary>
    /// <param name="amount">입힐 피해량</param>
    void TakeDamage(float amount);

    /// <summary>
    /// 대상의 체력을 회복시키는 메서드입니다.
    /// </summary>
    /// <param name="amount">회복할 체력량</param>
    void Heal(float amount); // 새로 추가되는 체력 회복 함수

    /// <summary>
    /// 대상이 사망했을 때 호출되는 메서드입니다.
    /// </summary>
    void Die();

    // 선택적으로 추가할 수 있는 이벤트 (예: 체력이 변경될 때, 사망할 때)
    // event System.Action<float> OnHealthChanged;
    // event System.Action OnDied;
}


using System;


/// <summary>
/// 피해를 입힐 수 있는 모든 게임 오브젝트가 구현해야 하는 인터페이스입니다.
/// 이 인터페이스를 통해 피해량 정보에 접근할 수 있습니다.
/// </summary>
public interface IDamageDealer
{
    /// <summary>
    /// 이 객체가 입힐 수 있는 피해량입니다. (읽기 전용)
    /// </summary>
    float DamageAmount { get; }

    event Action<float> OnDamageDealt;

    /// <summary>
    /// 지정된 양의 피해를 입힙니다.
    /// </summary>
    /// <param name="target">입힐 피해량입니다.</param>
    void DealDamage(IDamageable target); // <-- 이런 메서드가 추가될 수 있습니다.


}
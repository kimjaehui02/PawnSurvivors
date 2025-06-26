using System;
using UnityEngine;

public class DamageDealerComponent : MonoBehaviour, IDamageDealer
{
    [SerializeField] private float _damageAmount = 10f;
    public float DamageAmount => _damageAmount;

    public event Action<float> OnDamageDealt;

    public void DealDamage(IDamageable target)
    {
        if (target == null || target.CurrentHealth <= 0) return; // 유효하지 않거나 이미 죽은 대상은 스킵

        // 실제 피해량 계산 (방어력, 치명타 등)
        float actualDamage = _damageAmount; // 예시: 간단하게 기본 피해량 사용
        // 실제로는 여기서 target의 방어력 등을 고려하여 finalDamage를 계산

        target.TakeDamage(actualDamage); // 대상에게 피해를 입히도록 명령
        OnDamageDealt?.Invoke(actualDamage); // 내가 실제로 얼마의 피해를 입혔는지 알림
    }
}
using UnityEngine;
using System; // Action 이벤트를 위해 추가

/// <summary>
/// IDamageable 인터페이스를 실제로 구현하여 체력 관리 및 피해/회복 처리 로직을 담당하는 컴포넌트입니다.
/// 이 컴포넌트를 플레이어, 몬스터 등 체력이 필요한 GameObject에 붙여서 사용합니다.
/// </summary>
public class DamageableComponent : MonoBehaviour, IDamageable // 클래스 이름 변경
{
    private float _maxHealth;
    public float MaxHealth => _maxHealth;

    public float CurrentHealth { get; private set; }

    public event Action<float> OnDamaged;

    public void TakeDamage(float amount)
    {
        Debug.Log(amount);
        OnDamaged?.Invoke(amount);
    }
}
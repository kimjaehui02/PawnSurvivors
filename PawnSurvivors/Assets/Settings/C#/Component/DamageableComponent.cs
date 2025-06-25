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

    // 사망 시 발생할 이벤트 (Pawn 클래스 등이 구독)
    public event Action<GameObject> OnDiedGameObject;

    /// <summary>
    /// DamageableComponent를 초기화하는 메서드 (PawnSpawner에서 호출).
    /// 이 메서드에서 최대 체력을 설정하고 현재 체력을 초기화합니다.
    /// </summary>
    /// <param name="initialMaxHealth">초기 설정할 최대 체력</param>
    public void InitializeDamageable(float initialMaxHealth) // 초기화 메서드 이름도 변경
    {
        _maxHealth = initialMaxHealth;
        CurrentHealth = _maxHealth;
        Debug.Log($"<color=green>{gameObject.name}의 DamageableComponent가 Max Health: {_maxHealth}로 초기화되었습니다.</color>");
    }

    /// <summary>
    /// IDamageable 인터페이스의 TakeDamage 메서드 구현.
    /// 대상의 체력을 감소시키고, 체력이 0 이하가 되면 Die 메서드를 호출합니다.
    /// </summary>
    /// <param name="amount">입힐 피해량</param>
    public void TakeDamage(float amount)
    {
        if (amount <= 0) return;

        CurrentHealth -= amount;
        Debug.Log($"{gameObject.name}이(가) {amount} 피해를 입었습니다. 현재 체력: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die();
        }
    }

    /// <summary>
    /// IDamageable 인터페이스의 Heal 메서드 구현.
    /// 대상의 체력을 증가시킵니다. 최대 체력을 넘지 않습니다.
    /// </summary>
    /// <param name="amount">회복할 체력량</param>
    public void Heal(float amount)
    {
        if (amount <= 0) return;

        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        Debug.Log($"{gameObject.name}이(가) {amount} 체력을 회복했습니다. 현재 체력: {CurrentHealth}");
    }

    /// <summary>
    /// IDamageable 인터페이스의 Die 메서드 구현.
    /// 대상이 사망했을 때의 로직을 처리하고 사망 이벤트를 발생시킵니다.
    /// </summary>
    public void Die()
    {
        Debug.Log($"<color=red>{gameObject.name}이(가) 사망했습니다!</color>");
        gameObject.SetActive(false); // 예시: 게임 오브젝트 비활성화 (Destroy 대신 풀링을 위해)

        // 사망 이벤트 발생!
        OnDiedGameObject?.Invoke(this.gameObject);
    }
}
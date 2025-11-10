using UnityEngine;
using PawnCore.Domain;
using PawnCore.Domain.Events;

/// <summary>
/// Pawn이 데미지를 받을 수 있도록 하는 SubManager입니다.
/// DamageEvent를 구독하여 데미지를 처리하고, 체력이 0이 되면 PawnDeathEvent를 발행합니다.
/// </summary>
public class DamageableSubManager : PawnSubManager
{
    private PawnData _pawnData;

    public override void SubStart()
    {
        _pawnData = _pawnManager.PawnData;
        _pawnData.currentHealth = _pawnData.maxHealth;

        // DamageEvent 구독
        _pawnManager.Subscribe<DamageEvent>(HandleDamageEvent);
    }

    private void OnDisable()
    {
        if (_pawnManager != null)
        {
            _pawnManager.Unsubscribe<DamageEvent>(HandleDamageEvent);
        }
    }

    public override void SubUpdate()
    {
        // 체력은 기본적으로 프레임별 업데이트가 필요하지 않습니다.
    }

    /// <summary>
    /// DamageEvent를 처리합니다.
    /// </summary>
    private void HandleDamageEvent(DamageEvent evt)
    {
        // 이 Pawn을 대상으로 한 데미지인지 확인
        if (evt.Target != _pawnManager) return;

        // 데미지 적용
        _pawnData.currentHealth -= evt.Amount;
        _pawnData.currentHealth = Mathf.Max(_pawnData.currentHealth, 0);

        Debug.Log($"{_pawnManager.name} took {evt.Amount} damage. Current health: {_pawnData.currentHealth}");

        // 체력이 0 이하가 되면 사망 이벤트 발행
        if (_pawnData.currentHealth <= 0)
        {
            Debug.Log($"{_pawnManager.name} has run out of health and will be destroyed.");
            _pawnManager.Publish(new PawnDeathEvent(_pawnManager, evt.Attacker));
        }
    }

    /// <summary>
    /// 외부에서 직접 데미지를 주기 위한 메서드 (하위 호환성)
    /// 내부적으로 DamageEvent를 발행합니다.
    /// </summary>
    public void TakeDamage(float amount, GameObject attacker = null)
    {
        _pawnManager.Publish(new DamageEvent(_pawnManager, amount, attacker));
    }
}

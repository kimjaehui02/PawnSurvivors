using UnityEngine;
using PawnCore.Domain;

public class DamageableSubManager : PawnSubManager
{
    private PawnData _pawnData;

    public override void SubStart()
    {
        _pawnData = _pawnManager.PawnData;
        _pawnData.currentHealth = _pawnData.maxHealth;
    }

    public override void SubUpdate()
    {
        // 체력은 기본적으로 프레임별 업데이트가 필요하지 않습니다.
    }

    public void TakeDamage(float amount)
    {
        CombatUsecases.ApplyDamage(_pawnManager, amount);
    }
}

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
        // Health doesn't need a per-frame update by default.
    }

    public void TakeDamage(float amount)
    {
        CombatUsecases.ApplyDamage(_pawnData, amount);
    }
}

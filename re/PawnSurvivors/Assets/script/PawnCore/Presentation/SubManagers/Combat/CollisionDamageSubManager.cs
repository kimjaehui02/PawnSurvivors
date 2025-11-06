using UnityEngine;
using PawnCore.Domain;

public class CollisionDamageSubManager : PawnSubManager
{
    private PawnData _pawnData;

    public override void SubStart()
    {
        _pawnData = _pawnManager.PawnData;
    }

    public override void SubUpdate()
    {
        // No specific update logic needed
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CombatUsecases.HandleCollisionDamage(gameObject, other, _pawnData.damage);
    }
}

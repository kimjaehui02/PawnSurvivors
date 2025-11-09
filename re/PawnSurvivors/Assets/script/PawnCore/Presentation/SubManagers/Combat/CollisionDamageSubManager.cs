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
        // 특정 업데이트 로직이 필요하지 않음
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CombatUsecases.HandleCollisionDamage(gameObject, other, _pawnData.damage);
    }
}

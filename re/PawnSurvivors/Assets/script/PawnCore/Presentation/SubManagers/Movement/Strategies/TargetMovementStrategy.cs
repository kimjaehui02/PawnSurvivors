using UnityEngine;
using PawnCore.Domain;

public class TargetMovementStrategy : MovementStrategyBase
{
    [Header("Target Settings")]
    public Transform target;
    private PawnData _pawnData;

    public override void Init(PawnManager pawnManager)
    {
        base.Init(pawnManager);
        _pawnData = pawnManager.PawnData;
    }

    public override void Move()
    {
        if (_pawnManager == null || target == null) return;

        Transform currentTransform = _pawnManager.transform;
        Vector3 direction = (target.position - currentTransform.position);
        MovementUsecases.MoveInDirection(currentTransform, direction, _pawnData);
    }
}

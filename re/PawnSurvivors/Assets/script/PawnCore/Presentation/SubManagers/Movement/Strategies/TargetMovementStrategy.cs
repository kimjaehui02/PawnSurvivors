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
        
        // MovableData가 없으면 생성
        if (_pawnData.movableData == null)
        {
            _pawnData.movableData = new PawnCore.Domain.MovableData();
        }
    }

    public override void Move()
    {
        if (_pawnManager == null || target == null) return;
        if (_pawnData?.movableData?.targetMovement == null) return;

        Vector3 direction = (target.position - _pawnManager.transform.position).normalized;
        MovementUsecases.MoveInDirection(_pawnManager.transform, direction, _pawnData.movableData.targetMovement.speed, _pawnData);
    }
}

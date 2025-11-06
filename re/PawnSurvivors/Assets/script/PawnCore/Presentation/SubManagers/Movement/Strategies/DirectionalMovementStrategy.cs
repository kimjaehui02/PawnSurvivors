using UnityEngine;
using PawnCore.Domain;

public class DirectionalMovementStrategy : MovementStrategyBase
{
    private PawnData _pawnData;
    private float _age = 0f;

    public override void Init(PawnManager pawnManager)
    {
        base.Init(pawnManager);
        _pawnData = pawnManager.PawnData;
    }

    public override void Move()
    {
        if (_pawnManager == null) return;

        // Initialize moveDirection if not set
        if (_pawnData.movableData.directionalMovement.moveDirection == Vector3.zero)
        {
            _pawnData.movableData.directionalMovement.moveDirection = _pawnManager.transform.up;
        }

        MovementUsecases.MoveInDirection(_pawnManager.transform, _pawnData.movableData.directionalMovement.moveDirection, _pawnData);
        MovementUsecases.HandleLifetime(_pawnManager.gameObject, _pawnData, ref _age, Time.deltaTime);
    }
}

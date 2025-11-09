using UnityEngine;
using PawnCore.Domain;

public class DirectionalMovementStrategy : MovementStrategyBase
{
    private PawnData _pawnData;
    private float _currentAge = 0f;

    public override void Init(PawnManager pawnManager)
    {
        base.Init(pawnManager);
        _pawnData = pawnManager.PawnData;
    }

    public override void Move()
    {
        if (_pawnManager == null) return;

        // 설정되지 않은 경우 moveDirection 초기화
        if (_pawnData.movableData.directionalMovement.moveDirection == Vector3.zero)
        {
            _pawnData.movableData.directionalMovement.moveDirection = _pawnManager.transform.up;
        }

        Vector3 direction = _pawnData.movableData.directionalMovement.moveDirection.normalized;
        MovementUsecases.MoveInDirection(_pawnManager.transform, direction, _pawnData.movableData.directionalMovement.speed, _pawnData);

        _currentAge += Time.deltaTime;
        MovementUsecases.HandleLifetime(_pawnManager.gameObject, _pawnData, ref _currentAge, Time.deltaTime);
    }
}

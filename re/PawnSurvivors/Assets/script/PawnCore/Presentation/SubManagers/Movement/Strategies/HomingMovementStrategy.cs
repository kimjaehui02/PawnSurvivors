using UnityEngine;
using System.Linq;
using PawnCore.Domain;
using PawnCore.Domain.Usecases;

public class HomingMovementStrategy : MovementStrategyBase
{
    private PawnData _pawnData;
    private Transform _currentTarget;
    private float _nextRetargetTime;

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
        if (_pawnManager == null) return;
        if (_pawnData?.movableData?.homingMovement == null) return;

        if (Time.time >= _nextRetargetTime)
        {
            _nextRetargetTime = Time.time + _pawnData.movableData.homingMovement.retargetFrequency;
            _currentTarget = TargetingUsecases.FindClosestTargetByTag(_pawnManager.transform.position, _pawnData.movableData.homingMovement.targetTag, _pawnData.movableData.homingMovement.detectionRange);
        }

        if (_currentTarget != null)
        {
            Vector3 direction = (_currentTarget.position - _pawnManager.transform.position);
            MovementUsecases.MoveInDirection(_pawnManager.transform, direction, _pawnData.movableData.homingMovement.speed, _pawnData);
        }
    }
}

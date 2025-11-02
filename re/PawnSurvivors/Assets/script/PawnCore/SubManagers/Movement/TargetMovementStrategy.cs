using UnityEngine;

public class TargetMovementStrategy : MovementStrategyBase
{
    [Header("Target Settings")]
    public Transform target;
    public float speed = 5f;

    public override void Move()
    {
        if (_pawnManager == null || target == null) return;

        Transform currentTransform = _pawnManager.transform;
        Vector3 direction = (target.position - currentTransform.position);
        MovementUsecases.MoveInDirection(currentTransform, direction, speed);
    }
}


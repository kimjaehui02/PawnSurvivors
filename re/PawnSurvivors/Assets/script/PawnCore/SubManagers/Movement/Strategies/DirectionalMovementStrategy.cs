using UnityEngine;

public class DirectionalMovementStrategy : MovementStrategyBase
{
    public float speed = 20f;
    public float lifetime = 5f;
    public Vector3 moveDirection;
    private float _age = 0f;

    public override void Move()
    {
        if (_pawnManager == null) return;

        // Initialize moveDirection if not set
        if (moveDirection == Vector3.zero)
        {
            moveDirection = _pawnManager.transform.up;
        }

        MovementUsecases.MoveInDirection(_pawnManager.transform, moveDirection, speed);
        MovementUsecases.HandleLifetime(_pawnManager.gameObject, lifetime, ref _age, Time.deltaTime);
    }
}

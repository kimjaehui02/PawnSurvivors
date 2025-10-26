using UnityEngine;

public class DirectionalMovementSubManager : PawnSubManager
{
    public float speed = 20f;
    public float lifetime = 5f;
    public Vector3 moveDirection;
    private float _age = 0f;

    public override void SubStart()
    {
        _age = 0f; // Initialize age when the sub-manager starts
        if (moveDirection == Vector3.zero)
        {
            moveDirection = transform.up;
        }
    }

    public override void SubUpdate()
    {
        MovementUsecases.MoveInDirection(transform, moveDirection, speed);
        MovementUsecases.HandleLifetime(gameObject, lifetime, ref _age, Time.deltaTime);
    }
}

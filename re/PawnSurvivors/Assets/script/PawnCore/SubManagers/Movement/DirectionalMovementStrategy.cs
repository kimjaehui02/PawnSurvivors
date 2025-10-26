using UnityEngine;

public class DirectionalMovementStrategy : IMovementStrategy
{
    public float speed = 20f;
    public float lifetime = 5f;
    public Vector3 moveDirection;
    private float _age = 0f; // Age is now managed by the strategy itself

    /// <summary>
    /// Executes directional movement logic for the Pawn.
    /// </summary>
    /// <param name="pawnManager">The PawnManager instance controlling the Pawn.</param>
    /// <param name="deltaTime">The time elapsed since the last frame.</param>
    public void Move(PawnManager pawnManager, float deltaTime)
    {
        if (pawnManager == null || pawnManager.gameObject == null) return;

        // Initialize moveDirection if not set
        if (moveDirection == Vector3.zero)
        {
            moveDirection = pawnManager.transform.up;
        }

        MovementUsecases.MoveInDirection(pawnManager.transform, moveDirection, speed);
        MovementUsecases.HandleLifetime(pawnManager.gameObject, lifetime, ref _age, deltaTime);
    }
}

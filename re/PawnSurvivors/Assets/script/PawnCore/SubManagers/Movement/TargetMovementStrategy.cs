using UnityEngine;

public class TargetMovementStrategy : IMovementStrategy
{
    public Transform target;
    public float speed = 5f; // A reasonable default speed

    /// <summary>
    /// Executes target-seeking movement logic for the Pawn.
    /// </summary>
    /// <param name="pawnManager">The PawnManager instance controlling the Pawn.</param>
    /// <param name="deltaTime">The time elapsed since the last frame.</param>
    public void Move(PawnManager pawnManager, float deltaTime)
    {
        if (pawnManager == null || pawnManager.gameObject == null || target == null) return;

        // Calculate the direction from the current position to the target's position.
        Vector3 direction = (target.position - pawnManager.transform.position);

        // Call the use case to move the object in the calculated direction.
        // The object's rotation will not be affected.
        MovementUsecases.MoveInDirection(pawnManager.transform, direction, speed);
    }
}

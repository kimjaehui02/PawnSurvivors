using UnityEngine;
using UnityEngine.InputSystem; // Needed for Keyboard input

public class KeyboardMovementStrategy : IMovementStrategy
{
    public float moveSpeed = 5f;

    /// <summary>
    /// Executes keyboard-based movement logic for the Pawn.
    /// </summary>
    /// <param name="pawnManager">The PawnManager instance controlling the Pawn.</param>
    /// <param name="deltaTime">The time elapsed since the last frame.</param>
    public void Move(PawnManager pawnManager, float deltaTime)
    {
        if (pawnManager == null || pawnManager.gameObject == null) return;

        Vector2 inputDirection = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) inputDirection.y += 1;
            if (Keyboard.current.sKey.isPressed) inputDirection.y -= 1;
            if (Keyboard.current.aKey.isPressed) inputDirection.x -= 1;
            if (Keyboard.current.dKey.isPressed) inputDirection.x += 1;
        }

        inputDirection = inputDirection.normalized;
        Vector3 movement = moveSpeed * deltaTime * (Vector3)inputDirection;
        pawnManager.transform.position += movement;
    }
}

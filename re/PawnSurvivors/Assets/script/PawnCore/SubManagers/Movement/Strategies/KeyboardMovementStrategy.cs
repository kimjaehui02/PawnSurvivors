using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardMovementStrategy : MovementStrategyBase
{
    public float moveSpeed = 5f;

    public override void Move()
    {
        if (_pawnManager == null) return;

        Vector2 inputDirection = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) inputDirection.y += 1;
            if (Keyboard.current.sKey.isPressed) inputDirection.y -= 1;
            if (Keyboard.current.aKey.isPressed) inputDirection.x -= 1;
            if (Keyboard.current.dKey.isPressed) inputDirection.x += 1;
        }

        inputDirection = inputDirection.normalized;
        Vector3 movement = moveSpeed * Time.deltaTime * (Vector3)inputDirection;
        _pawnManager.transform.position += movement;
    }
}

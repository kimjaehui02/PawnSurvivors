using UnityEngine;
using UnityEngine.InputSystem;

public static class Usecase
{
    public static void Move(GameObject obj, float speed)
    {
        if (obj == null) return;

        Vector2 inputDirection = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) inputDirection.y += 1;
            if (Keyboard.current.sKey.isPressed) inputDirection.y -= 1;
            if (Keyboard.current.aKey.isPressed) inputDirection.x -= 1;
            if (Keyboard.current.dKey.isPressed) inputDirection.x += 1;
        }

        inputDirection = inputDirection.normalized;
        Vector3 movement = speed * Time.deltaTime * (Vector3)inputDirection;
        obj.transform.position += movement;
    }

    public static void ApplyDamage(DamageableSubManager damageable, float amount)
    {
        if (damageable == null) return;

        damageable.currentHealth -= amount;
        damageable.currentHealth = Mathf.Max(damageable.currentHealth, 0);

        Debug.Log($"{damageable.gameObject.name} took {amount} damage. Current health: {damageable.currentHealth}");
    }
}

using UnityEngine;
public static class MovementUsecases
{
    public static void MoveWithInput(Transform transform, Vector2 inputDirection, float speed)
    {
        if (transform == null) return;

        Vector3 movement = speed * Time.deltaTime * (Vector3)inputDirection.normalized;
        transform.position += movement;
    }

    public static void MoveInDirection(Transform transform, Vector3 direction, float speed)
    {
        if (transform == null) return;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }

    public static void HandleLifetime(GameObject self, float lifetime, ref float currentAge, float deltaTime)
    {
        if (lifetime <= 0) return; // Do nothing if lifetime is not set

        currentAge += deltaTime;
        if (currentAge >= lifetime)
        {
            Object.Destroy(self);
        }
    }
}

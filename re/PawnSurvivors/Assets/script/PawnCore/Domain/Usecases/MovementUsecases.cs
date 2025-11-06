using UnityEngine;
using PawnCore.Domain;

public static class MovementUsecases
{
    public static void MoveWithInput(Transform transform, Vector2 inputDirection, PawnData pawnData)
    {
        if (transform == null) return;

        Vector3 movement = pawnData.movableData.keyboardMovement.moveSpeed * Time.deltaTime * (Vector3)inputDirection.normalized;
        transform.position += movement;
    }

    public static void MoveInDirection(Transform transform, Vector3 direction, PawnData pawnData)
    {
        if (transform == null) return;
        // Use the speed from the appropriate movement data within PawnData
        // For directional movement, we assume it's coming from directionalMovement data
        transform.Translate(direction.normalized * pawnData.movableData.directionalMovement.speed * Time.deltaTime, Space.World);
    }

    public static void HandleLifetime(GameObject self, PawnData pawnData, ref float currentAge, float deltaTime)
    {
        if (pawnData.movableData.directionalMovement.lifetime <= 0) return; // Do nothing if lifetime is not set

        currentAge += deltaTime;
        if (currentAge >= pawnData.movableData.directionalMovement.lifetime)
        {
            if (self.TryGetComponent<PawnManager>(out var pawnManager))
            {
                pawnManager.DestroyPawn();
            }
            else
            {
                Object.Destroy(self); // Fallback for objects without a PawnManager
            }
        }
    }
}
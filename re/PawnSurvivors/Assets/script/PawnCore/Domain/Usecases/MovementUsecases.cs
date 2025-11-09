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

    public static void MoveInDirection(Transform transform, Vector3 direction, float speed, PawnData pawnData)
    {
        if (transform == null) return;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }

    public static void HandleLifetime(GameObject self, PawnData pawnData, ref float currentAge, float deltaTime)
    {
        if (pawnData.movableData.directionalMovement.lifetime <= 0) return; // lifetime이 설정되지 않은 경우 아무것도 하지 않음

        currentAge += deltaTime;
        if (currentAge >= pawnData.movableData.directionalMovement.lifetime)
        {
            if (self.TryGetComponent<PawnManager>(out var pawnManager))
            {
                pawnManager.DestroyPawn();
            }
            else
            {
                Object.Destroy(self); // PawnManager가 없는 개체에 대한 대체
            }
        }
    }
}
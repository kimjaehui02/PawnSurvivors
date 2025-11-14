using UnityEngine;
using PawnCore.Domain;

public static class MovementUsecases
{
    public static void MoveWithInput(Transform transform, Vector2 inputDirection, PawnData pawnData)
    {
        if (transform == null) return;

        float deltaTime = GetGameDeltaTime();
        Vector3 movement = pawnData.movableData.keyboardMovement.moveSpeed * deltaTime * (Vector3)inputDirection.normalized;
        transform.position += movement;
    }

    public static void MoveInDirection(Transform transform, Vector3 direction, float speed, PawnData pawnData)
    {
        if (transform == null) return;
        float deltaTime = GetGameDeltaTime();
        transform.Translate(direction.normalized * speed * deltaTime, Space.World);
    }
    
    private static float GetGameDeltaTime()
    {
        if (GameManager.Instance?.LifecycleManager != null)
        {
            return GameManager.Instance.LifecycleManager.GameDeltaTime;
        }
        return Time.deltaTime; // 폴백
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
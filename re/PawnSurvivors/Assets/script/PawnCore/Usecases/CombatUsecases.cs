using UnityEngine;

public static class CombatUsecases
{
    public static void ApplyDamage(DamageableSubManager damageable, float amount)
    {
        if (damageable == null) return;

        damageable.currentHealth -= amount;
        damageable.currentHealth = Mathf.Max(damageable.currentHealth, 0);

        Debug.Log($"{damageable.gameObject.name} took {amount} damage. Current health: {damageable.currentHealth}");
    }

    public static void FireProjectile(GameObject projectilePrefab, Vector3 position, Quaternion rotation)
    {
        GameManager.Instance.CreationManager.CreatePawn(projectilePrefab, position, rotation);
    }
}

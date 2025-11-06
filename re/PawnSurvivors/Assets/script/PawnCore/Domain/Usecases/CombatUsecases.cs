using UnityEngine;
using PawnCore.Domain;
using PawnCore.Recipes.Json;
using PawnCore.Domain.Usecases;

public static class CombatUsecases
{
    public static void ApplyDamage(PawnData pawnData, float amount)
    {
        if (pawnData == null) return;

        pawnData.currentHealth -= amount;
        pawnData.currentHealth = Mathf.Max(pawnData.currentHealth, 0);

        Debug.Log($"{pawnData} took {amount} damage. Current health: {pawnData.currentHealth}");
    }

    public static void FireProjectile(PawnRecipeData projectileRecipe, Vector3 position, Quaternion rotation, Vector3 direction)
    {
        // Ensure the projectile is created facing the correct direction
        GameManager.Instance.CreationManager.CreatePawn(projectileRecipe, position, Quaternion.LookRotation(Vector3.forward, direction));
    }

    public static void HandleCollisionDamage(GameObject self, Collider2D other, float damage)
    {
        // Avoid hitting other objects with the same component
        if (other.GetComponent<CollisionDamageSubManager>() != null)
        {
            return;
        }

        if (other.TryGetComponent<DamageableSubManager>(out var damageable))
        {
            damageable.TakeDamage(damage);
            
            if (self.TryGetComponent<PawnManager>(out var pawnManager))
            {
                pawnManager.DestroyPawn(); // Use controlled destruction
            }
            else
            {
                Object.Destroy(self); // Fallback for objects without a PawnManager
            }
        }
    }

    public static void HandleProjectileAttack(ref float nextFireTime, float fireRate, PawnRecipeData projectileRecipe, Transform firePoint)
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;

            Transform closestEnemy = TargetingUsecases.FindClosestTargetByTag(firePoint.position, "Enemy", 0); // 0 means infinite range
            Vector3 direction = firePoint.up; // Default direction

            if (closestEnemy != null)
            {
                direction = (closestEnemy.position - firePoint.position).normalized;
                Debug.Log($"Found closest enemy at {closestEnemy.position}. Projectile direction set to {direction}");
            }
            else
            {
                Debug.Log("No enemy found. Projectile will fire in default direction (firePoint.up).");
            }
            
            GameManager.Instance.CreationManager.CreatePawn(projectileRecipe, firePoint.position, firePoint.rotation, direction);
        }
    }
}

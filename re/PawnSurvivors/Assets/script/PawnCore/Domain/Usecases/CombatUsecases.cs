using UnityEngine;
using PawnCore.Domain;
using PawnCore.Recipes.Json;

public static class CombatUsecases
{
    public static void ApplyDamage(PawnData pawnData, float amount)
    {
        if (pawnData == null) return;

        pawnData.currentHealth -= amount;
        pawnData.currentHealth = Mathf.Max(pawnData.currentHealth, 0);

        Debug.Log($"{pawnData} took {amount} damage. Current health: {pawnData.currentHealth}");
    }

    public static void FireProjectile(PawnRecipeData projectileRecipe, Vector3 position, Quaternion rotation)
    {
        GameManager.Instance.CreationManager.CreatePawn(projectileRecipe, position, rotation);
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
            FireProjectile(projectileRecipe, firePoint.position, firePoint.rotation);
        }
    }
}

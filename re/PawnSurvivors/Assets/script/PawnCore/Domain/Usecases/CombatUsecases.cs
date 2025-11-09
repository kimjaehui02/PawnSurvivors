using UnityEngine;
using PawnCore.Domain;
using PawnCore.Recipes.Json;
using PawnCore.Domain.Usecases;

public static class CombatUsecases
{
    public static void ApplyDamage(PawnManager pawnManager, float amount)
    {
        if (pawnManager == null || pawnManager.PawnData == null) return;

        PawnData pawnData = pawnManager.PawnData;
        pawnData.currentHealth -= amount;
        pawnData.currentHealth = Mathf.Max(pawnData.currentHealth, 0);

        Debug.Log($"{pawnManager.name} took {amount} damage. Current health: {pawnData.currentHealth}");

        if (pawnData.currentHealth <= 0)
        {
            Debug.Log($"{pawnManager.name} has run out of health and will be destroyed.");
            pawnManager.DestroyPawn();
        }
    }

    public static void FireProjectile(PawnRecipeData projectileRecipe, Vector3 position, Quaternion rotation, Vector3 direction)
    {
        // 발사체가 올바른 방향을 향하도록 생성되었는지 확인
        GameManager.Instance.CreationManager.CreatePawn(projectileRecipe, position, Quaternion.LookRotation(Vector3.forward, direction));
    }

    public static void HandleCollisionDamage(GameObject self, Collider2D other, float damage)
    {
        // 동일한 구성 요소를 가진 다른 개체에 부딪히지 않도록 방지
        if (other.GetComponent<CollisionDamageSubManager>() != null)
        {
            return;
        }

        if (other.TryGetComponent<DamageableSubManager>(out var damageable))
        {
            damageable.TakeDamage(damage);
            
            if (self.TryGetComponent<PawnManager>(out var pawnManager))
            {
                pawnManager.DestroyPawn(); // 제어된 파괴 사용
            }
            else
            {
                Object.Destroy(self); // PawnManager가 없는 개체에 대한 대체 처리
            }
        }
    }

    public static void HandleProjectileAttack(ref float nextFireTime, float fireRate, PawnRecipeData projectileRecipe, Transform firePoint)
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;

            Transform closestEnemy = TargetingUsecases.FindClosestTargetByTag(firePoint.position, "Enemy", 0); // 0은 무한 범위를 의미합니다.
            Vector3 direction = firePoint.up; // 기본 방향

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

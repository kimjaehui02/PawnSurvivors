using UnityEngine;
using PawnCore.Domain;
using PawnCore.Domain.Events;
using PawnCore.Recipes.Json;
using PawnCore.Domain.Usecases;

/// <summary>
/// 전투 관련 비즈니스 로직을 담당하는 Usecase 클래스입니다.
/// 이제 이벤트 버스 시스템을 활용하여 느슨한 결합을 유지합니다.
/// </summary>
public static class CombatUsecases
{
    /// <summary>
    /// 발사체를 발사합니다.
    /// </summary>
    public static void FireProjectile(PawnRecipeData projectileRecipe, Vector3 position, Quaternion rotation, Vector3 direction)
    {
        // 발사체가 올바른 방향을 향하도록 생성되었는지 확인
        GameManager.Instance.CreationManager.CreatePawn(projectileRecipe, position, Quaternion.LookRotation(Vector3.forward, direction));
    }

    /// <summary>
    /// 발사체 공격을 처리합니다.
    /// </summary>
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

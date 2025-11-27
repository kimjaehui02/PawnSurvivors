using System.Collections.Generic;
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
    /// <param name="projectileRecipe">발사체 레시피</param>
    /// <param name="position">발사 위치</param>
    /// <param name="rotation">발사 회전</param>
    /// <param name="direction">발사 방향</param>
    /// <param name="owner">발사자 PawnManager (선택적)</param>
    public static void FireProjectile(PawnRecipeData projectileRecipe, Vector3 position, Quaternion rotation, Vector3 direction, PawnManager owner = null)
    {
        // 발사체가 올바른 방향을 향하도록 생성되었는지 확인
        GameManager.Instance.CreationManager.CreatePawn(projectileRecipe, position, Quaternion.LookRotation(Vector3.forward, direction), null, owner);
    }

    /// <summary>
    /// 발사체 공격을 처리합니다.
    /// </summary>
    /// <param name="nextFireTime">다음 발사 시간 (ref)</param>
    /// <param name="fireRate">발사 속도</param>
    /// <param name="projectileRecipe">발사체 레시피</param>
    /// <param name="firePoint">발사 지점</param>
    /// <param name="gameTime">현재 게임 시간</param>
    /// <param name="owner">발사자 PawnManager (선택적)</param>
    /// <param name="projectileDamage">발사자의 데미지 (투사체에 전달)</param>
    /// <param name="projectileSpeed">투사체 속도 (0이면 레시피 기본값 사용)</param>
    public static void HandleProjectileAttack(ref float nextFireTime, float fireRate, PawnRecipeData projectileRecipe, Transform firePoint, float gameTime, PawnManager owner = null, float projectileDamage = 0f, float projectileSpeed = 0f)
    {
        if (gameTime >= nextFireTime)
        {
            nextFireTime = gameTime + 1f / fireRate;

            // 타겟 태그 결정: CombatData에 설정되어 있으면 사용, 없으면 자동 결정
            string targetTag;
            if (owner != null && owner.PawnData?.combatData != null && !string.IsNullOrEmpty(owner.PawnData.combatData.targetTag))
            {
                // 설정에서 지정된 타겟 태그 사용
                targetTag = owner.PawnData.combatData.targetTag;
            }
            else
            {
                // 자동 결정: 발사자의 태그에 따라 타겟 결정
                // Player 태그면 Enemy를 타겟, Enemy 태그면 Player를 타겟
                targetTag = (owner != null && owner.gameObject.CompareTag("Player")) ? "Enemy" : "Player";
            }
            
            Transform closestTarget = TargetingUsecases.FindClosestTargetByTag(firePoint.position, targetTag, 0); // 0은 무한 범위를 의미합니다.
            Vector3 direction = firePoint.up; // 기본 방향

            if (closestTarget != null)
            {
                direction = (closestTarget.position - firePoint.position).normalized;
                // Debug.Log($"Found closest target at {closestTarget.position}. Projectile direction set to {direction}");
            }
            else
            {
                // Debug.Log("No target found. Projectile will fire in default direction (firePoint.up).");
            }
            
            // 투사체 속도 사용 (파라미터로 전달받은 값 사용, 0이면 레시피 기본값)
            // 파라미터로 전달받은 projectileSpeed가 0이 아니면 사용, 0이면 owner의 기본값 확인
            float finalProjectileSpeed = projectileSpeed;
            if (finalProjectileSpeed <= 0f && owner != null && owner.PawnData?.combatData != null && owner.PawnData.combatData.projectileSpeed > 0f)
            {
                finalProjectileSpeed = owner.PawnData.combatData.projectileSpeed;
            }
            
            GameManager.Instance.CreationManager.CreatePawn(projectileRecipe, firePoint.position, firePoint.rotation, direction, owner, projectileDamage, finalProjectileSpeed);
        }
    }

    /// <summary>
    /// 데미지를 적용하고 결과를 반환합니다.
    /// </summary>
    /// <param name="healthData">체력 데이터</param>
    /// <param name="damageAmount">데미지 양</param>
    /// <returns>(실제 적용된 데미지, 적용 후 체력, 치명타 여부)</returns>
    public static (float actualDamage, float newHealth, bool isFatal) ApplyDamage(HealthData healthData, float damageAmount)
    {
        if (healthData == null)
        {
            Debug.LogWarning("[CombatUsecases] HealthData is null. Cannot apply damage.");
            return (0f, 0f, false);
        }

        float healthBefore = healthData.currentHealth;
        
        // 데미지 적용 (최소값 0)
        healthData.currentHealth = Mathf.Max(healthData.currentHealth - damageAmount, 0f);
        
        // 결과 계산
        float actualDamage = healthBefore - healthData.currentHealth;
        bool isFatal = healthData.currentHealth <= 0f;
        
        return (actualDamage, healthData.currentHealth, isFatal);
    }

    /// <summary>
    /// 쿨다운을 고려하여 데미지를 줄 수 있는지 확인합니다.
    /// </summary>
    /// <param name="target">대상 PawnManager</param>
    /// <param name="currentTime">현재 시간</param>
    /// <param name="cooldown">쿨다운 시간</param>
    /// <param name="lastDamageTimes">마지막 데미지 시간 딕셔너리</param>
    /// <returns>데미지를 줄 수 있으면 true</returns>
    public static bool CanDealDamageWithCooldown(
        PawnManager target, 
        float currentTime, 
        float cooldown, 
        Dictionary<PawnManager, float> lastDamageTimes)
    {
        if (lastDamageTimes.TryGetValue(target, out float lastTime))
        {
            if (currentTime - lastTime < cooldown)
            {
                return false; // 쿨다운 중
            }
        }
        
        // 쿨다운이 지났거나 처음 공격하는 경우
        lastDamageTimes[target] = currentTime;
        return true;
    }
}

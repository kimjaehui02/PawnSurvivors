using UnityEngine;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Data.Recipes;

namespace PawnSurvivors.Domain.Combat
{
    /// <summary>
    /// 투사체를 발사하는 공격 방식입니다.
    /// </summary>
    public class ProjectileAttackMethod : IAttackMethod
    {
        public bool Execute(
            PawnData pawnData,
            Transform attackPoint,
            string targetTag,
            float damage,
            float attackRange,
            GameObject attacker,
            PawnManager owner)
        {
            // CombatData 확인
            if (pawnData?.combatData == null)
            {
                Debug.LogWarning("[ProjectileAttackMethod] CombatData is null.");
                return false;
            }

            // 투사체 레시피 확인
            if (string.IsNullOrEmpty(pawnData.combatData.projectileRecipeName))
            {
                Debug.LogWarning("[ProjectileAttackMethod] Projectile recipe name is not set.");
                return false;
            }

            // 레시피 가져오기
            PawnRecipeData projectileRecipe = GameManager.Instance.CreationManager.GetRecipe(pawnData.combatData.projectileRecipeName);
            if (projectileRecipe == null)
            {
                Debug.LogWarning($"[ProjectileAttackMethod] Recipe '{pawnData.combatData.projectileRecipeName}' not found.");
                return false;
            }

            // 타겟 찾기
            Transform closestTarget = TargetingUsecases.FindClosestTargetByTag(attackPoint.position, targetTag, attackRange);
            
            // 타겟이 없으면 발사하지 않음
            if (closestTarget == null)
            {
                return false;
            }
            
            Vector3 direction = (closestTarget.position - attackPoint.position).normalized;

            // 투사체 속도 계산
            float projectileSpeed = 0f;
            if (GameManager.Instance?.PawnStatCalculator != null)
            {
                projectileSpeed = GameManager.Instance.PawnStatCalculator.GetEffectiveProjectileSpeed(pawnData);
            }

            // 투사체 생성
            GameManager.Instance.CreationManager.CreatePawn(
                projectileRecipe, 
                attackPoint.position, 
                attackPoint.rotation, 
                direction, 
                owner, 
                damage, 
                projectileSpeed
            );

            return true; // 투사체 발사 성공
        }
    }
}


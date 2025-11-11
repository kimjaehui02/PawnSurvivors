using UnityEngine;
using PawnCore.Domain;
using PawnCore.Domain.Events;
using PawnCore.Domain.Usecases;
using PawnCore.Recipes.Json;

/// <summary>
/// 발사체를 발사하는 SubManager입니다.
/// AttackInputEvent를 구독하여 공격을 처리합니다.
/// </summary>
public class ProjectileShooterSubManager : PawnSubManager
{
    private PawnData _pawnData;
    public Transform firePoint;
    private float _nextFireTime = 0f;

    public override void SubStart()
    {
        _pawnData = _pawnManager.PawnData;

        // CombatData가 없으면 생성
        if (_pawnData.combatData == null)
        {
            _pawnData.combatData = new PawnCore.Domain.CombatData();
        }

        if (string.IsNullOrEmpty(_pawnData.combatData.projectileRecipeName))
        {
            Debug.LogError("Projectile Recipe is not assigned in PawnData.", this);
            this.enabled = false;
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning("Fire Point is not assigned. Using this GameObject's transform as default.", this);
            firePoint = this.transform;
        }
        
        // 클릭 발사 임시 비활성화
        // _pawnManager.Subscribe<AttackInputEvent>(HandleAttackInput);
    }

    private void OnDisable()
    {
        // 클릭 발사 임시 비활성화
        // if (_pawnManager != null)
        // {
        //     _pawnManager.Unsubscribe<AttackInputEvent>(HandleAttackInput);
        // }
    }

    public override void SubUpdate()
    {
        // CombatData가 없으면 무시
        if (_pawnData?.combatData == null) return;

        // 적이 범위 내에 있으면 자동 발사
        Transform closestEnemy = TargetingUsecases.FindClosestTargetByTag(firePoint.position, "Enemy", 0);
        if (closestEnemy != null)
        {
            PerformAttack();
        }
    }

    private void HandleAttackInput(AttackInputEvent evt)
    {
        if (evt.Attacker == this.gameObject)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        PawnRecipeData projectileRecipe = GameManager.Instance.CreationManager.GetRecipe(_pawnData.combatData.projectileRecipeName);
        if (projectileRecipe != null)
        {
            CombatUsecases.HandleProjectileAttack(ref _nextFireTime, _pawnData.combatData.fireRate, projectileRecipe, firePoint);
        }
    }
}

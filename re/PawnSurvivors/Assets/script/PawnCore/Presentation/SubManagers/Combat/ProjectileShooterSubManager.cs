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

        // CombatData 가져오기 또는 생성
        _pawnData.GetOrCreateCombatData();

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

        // 타겟 태그 결정: 설정되어 있으면 사용, 없으면 자동 결정
        string targetTag;
        if (!string.IsNullOrEmpty(_pawnData.combatData.targetTag))
        {
            // 설정에서 지정된 타겟 태그 사용
            targetTag = _pawnData.combatData.targetTag;
        }
        else
        {
            // 자동 결정: 자신의 태그에 따라 타겟 결정
            // Player 태그면 Enemy를 타겟, Enemy 태그면 Player를 타겟
            targetTag = _pawnManager.gameObject.CompareTag("Player") ? "Enemy" : "Player";
        }
        
        Transform closestTarget = TargetingUsecases.FindClosestTargetByTag(firePoint.position, targetTag, 0);
        if (closestTarget != null)
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
            float currentGameTime = GetGameTime(); // 게임 시간 사용 (정지 시 멈춤)
            // 발사자 정보와 데미지 전달 (탄환이 발사자를 추적할 수 있도록)
            float projectileDamage = _pawnData.combatData.damage; // 발사자의 데미지
            CombatUsecases.HandleProjectileAttack(ref _nextFireTime, _pawnData.combatData.fireRate, projectileRecipe, firePoint, currentGameTime, _pawnManager, projectileDamage);
        }
    }
}

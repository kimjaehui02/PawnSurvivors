using UnityEngine;
using PawnCore.Domain;
using PawnCore.Recipes.Json;

public class ProjectileShooterSubManager : PawnSubManager
{
    private PawnData _pawnData;
    public Transform firePoint;
    private float _nextFireTime = 0f;

    public override void SubStart()
    {
        _pawnData = _pawnManager.PawnData;

        if (string.IsNullOrEmpty(_pawnData.projectileRecipeName))
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
        
        _pawnManager.Subscribe<AttackInputEvent>(HandleAttackInput);
    }

    private void OnDisable()
    {
        if (_pawnManager != null)
        {
            _pawnManager.Unsubscribe<AttackInputEvent>(HandleAttackInput);
        }
    }

    public override void SubUpdate()
    {
        // Not used for this attack type
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
        PawnRecipeData projectileRecipe = GameManager.Instance.CreationManager.GetRecipe(_pawnData.projectileRecipeName);
        if (projectileRecipe != null)
        {
            CombatUsecases.HandleProjectileAttack(ref _nextFireTime, _pawnData.fireRate, projectileRecipe, firePoint);
        }
    }
}

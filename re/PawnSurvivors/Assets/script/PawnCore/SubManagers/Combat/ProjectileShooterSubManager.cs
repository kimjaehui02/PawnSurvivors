using UnityEngine;

public class ProjectileShooterSubManager : PawnSubManager
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Firing Rate")]
    public float fireRate = 2f; // Shots per second
    private float _nextFireTime = 0f;

    public override void SubStart()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab is not assigned.", this);
            this.enabled = false;
            return; // Stop initialization if prefab is missing
        }
        if (firePoint == null)
        {
            Debug.LogWarning("Fire Point is not assigned. Using this GameObject's transform as default.", this);
            firePoint = this.transform;
        }
        
        // Subscribe to the attack input event
        _pawnManager.Subscribe<AttackInputEvent>(HandleAttackInput);
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
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
        // Check if the event was triggered by this pawn
        if (evt.Attacker == this.gameObject)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        CombatUsecases.HandleProjectileAttack(ref _nextFireTime, fireRate, projectilePrefab, firePoint);
    }
}


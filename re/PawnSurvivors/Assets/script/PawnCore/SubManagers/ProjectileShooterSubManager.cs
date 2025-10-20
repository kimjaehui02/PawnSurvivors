using UnityEngine;

public class ProjectileShooterSubManager : PawnSubManager, IAttackable
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
        }
        if (firePoint == null)
        {
            Debug.LogWarning("Fire Point is not assigned. Using this GameObject's transform as default.", this);
            firePoint = this.transform;
        }
    }

    public override void SubUpdate()
    {
        // Not used for this attack type
    }

    public void Attack()
    {
        if (Time.time >= _nextFireTime)
        {
            _nextFireTime = Time.time + 1f / fireRate;
            CombatUsecases.FireProjectile(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }
}

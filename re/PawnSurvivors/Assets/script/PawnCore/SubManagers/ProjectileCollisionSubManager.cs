using UnityEngine;

public class ProjectileCollisionSubManager : PawnSubManager
{
    public float damage = 10f;

    public override void SubStart()
    {
        // No specific start logic needed
    }

    public override void SubUpdate()
    {
        // No specific update logic needed
    }

    private void OnTriggerEnter(Collider other)
    {
        // Avoid hitting other projectiles
        if (other.GetComponent<ProjectileCollisionSubManager>() != null)
        {
            return;
        }

        if (other.TryGetComponent<DamageableSubManager>(out var damageable))
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject); // Destroy projectile on impact
        }
    }
}

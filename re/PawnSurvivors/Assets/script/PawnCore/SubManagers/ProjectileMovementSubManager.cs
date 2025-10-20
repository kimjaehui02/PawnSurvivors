using UnityEngine;

public class ProjectileMovementSubManager : PawnSubManager
{
    public float speed = 20f;
    public float lifetime = 5f;
    private float _age = 0f;

    public override void SubStart()
    {
        // Lifetime is now managed in SubUpdate, so no Destroy call here.
        _age = 0f; // Initialize age when the sub-manager starts
    }

    public override void SubUpdate()
    {
        // Move the projectile forward.
        transform.Translate(transform.up * speed * Time.deltaTime, Space.World);

        // Update age and destroy if lifetime exceeded
        _age += Time.deltaTime;
        if (_age >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}

using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 5f;

    void Start()
    {
        // Destroy the projectile after its lifetime expires.
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the projectile forward.
        // For a 2D game, you might want to use Vector3.up if your sprite is oriented upwards.
        transform.Translate(transform.up * speed * Time.deltaTime, Space.World);
    }

    // Optional: Add collision detection to apply damage.
    private void OnTriggerEnter(Collider other)
    {
        // Example of how you could apply damage
        // if (other.TryGetComponent<DamageableSubManager>(out var damageable))
        // {
        //     damageable.TakeDamage(10); // Example damage value
        //     Destroy(gameObject); // Destroy projectile on impact
        // }
    }
}

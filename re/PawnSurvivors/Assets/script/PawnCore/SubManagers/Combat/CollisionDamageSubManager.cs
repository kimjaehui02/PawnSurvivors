using UnityEngine;

public class CollisionDamageSubManager : PawnSubManager
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



    private void OnTriggerEnter2D(Collider2D other)
    {
        CombatUsecases.HandleCollisionDamage(gameObject, other, damage);
    }
}

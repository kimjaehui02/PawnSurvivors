using UnityEngine;

public class DamageableSubManager : PawnSubManager
{
    public float maxHealth = 100f;
    public float currentHealth;

    public override void SubStart()
    {
        currentHealth = maxHealth;
    }

    public override void SubUpdate()
    {
        // Health doesn't need a per-frame update by default.
    }

    public void TakeDamage(float amount)
    {
        Usecase.ApplyDamage(this, amount);
    }
}

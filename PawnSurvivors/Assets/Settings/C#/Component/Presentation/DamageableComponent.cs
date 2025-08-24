using UnityEngine;
using Game.Core;
using Game.Core.Base;
using Game.Core.Configs;
using Game.Core.Enums;
using Game.Core.Contexts;

public class DamageableComponent : PawnBase
{
    [SerializeField]
    private DamageableConfig damageableConfig = new();

    public float MaxHealth => damageableConfig.MaxHealth;

    public float CurrentHealth
    {
        get => damageableConfig.CurrentHealth;
        set => damageableConfig.CurrentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }

    public override void RegisterAbilities()
    {
        AddAction(Acts.OnDamaged, TakeDamage);
    }

    public void TakeDamage(AbilityContext abilityContext)
    {
        if (abilityContext == null)
        {
            Debug.LogWarning("TakeDamage 호출 시 AbilityContext가 null입니다.");
            return;
        }

        float damage = abilityContext.DamageAmount ?? 0f;
        CurrentHealth -= damage;

        Debug.Log($"DamageableComponent: {name}이(가) {damage} 피해를 받았습니다. CurrentHealth={CurrentHealth}");
    }
    public override void Initialize(IBaseConfig config)
    {
        if (config is DamageableConfig dmgConfig)
        {
            damageableConfig = dmgConfig;
        }
        else
        {
            Debug.LogWarning($"{name}: Initialize 호출 시 잘못된 config 타입입니다. 기대한 타입: DamageableConfig");
        }
    }

}

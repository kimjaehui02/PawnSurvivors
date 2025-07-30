using UnityEngine;
using System;
using Game.Core; // Acts 및 AbilityContext를 위해 추가



/// <summary>
/// IDamageable 인터페이스를 실제로 구현하여 체력 관리 및 피해/회복 처리 로직을 담당하는 컴포넌트입니다.
/// 이 컴포넌트를 플레이어, 몬스터 등 체력이 필요한 GameObject에 붙여서 사용합니다.
/// </summary>
public class DamageableComponent : PawnBase
{
    #region Fields & Properties

    private DamageableConfig Config
    {
        get
        {
            // Debug.Log($"Accessing _config. Current baseConfig type: {baseConfig?.GetType().Name ?? "null"}");
            return baseConfig as DamageableConfig;
        }
        set
        {
            // Debug.Log($"Setting _config. New value type: {value?.GetType().Name ?? "null"}");
            baseConfig = value;
        }
    }

    /// <summary>
    /// 최대 체력입니다. DamageableConfig에서 값을 가져옵니다.
    /// </summary>
    public float MaxHealth
    {
        get
        {
            // _config가 null인 경우를 대비하여 방어 코드 추가
            if (Config == null)
            {
                Debug.LogError($"DamageableComponent({name}): DamageableConfig가 할당되지 않았습니다. MaxHealth 기본값 0을 반환합니다.");
                return 0f;
            }
            return Config.maxHealth;
        }
    }

    /// <summary>
    /// 현재 체력입니다. DamageableConfig에서 값을 가져오고 설정합니다.
    /// 체력 변경은 TakeDamage 또는 별도의 힐 메서드를 통해서만 이루어집니다.
    /// </summary>
    public float CurrentHealth
    {
        get
        {
            if (Config == null)
            {
                Debug.LogError($"DamageableComponent({name}): DamageableConfig가 할당되지 않았습니다. CurrentHealth 기본값 0을 반환합니다.");
                return 0f;
            }
            return Config.currentHealth;
        }
        private set
        {
            if (Config == null)
            {
                Debug.LogError($"DamageableComponent({name}): DamageableConfig가 할당되지 않아 체력 설정 불가.");
                return;
            }
            // 체력을 0과 MaxHealth 사이로 클램프하여 _config.currentHealth에 저장
            // MaxHealth는 _config.maxHealth에서 가져오므로 일관성이 유지됩니다.
            Config.currentHealth = Mathf.Clamp(value, 0, MaxHealth);
            // TODO: 체력이 변경될 때마다 UI 업데이트 등의 이벤트를 발생시킬 수 있습니다.
            // OnHealthChanged?.Invoke(_config.currentHealth, _config.maxHealth);
        }
    }

    #endregion

    #region Ability Registration

    /// <summary>
    /// PawnAction의 RegisterAbilities를 오버라이드하여
    /// 이 컴포넌트의 능력을 Pawn의 델리게이트 시스템에 등록합니다.
    /// </summary>
    public override void RegisterAbilities()
    {

        // Acts.OnDamaged 이벤트가 발생했을 때 TakeDamage 메서드를 호출하도록 등록합니다.
        // AbilityContext를 통해 피해량 정보를 받아 처리합니다.
        AddAction(Acts.OnDamaged, TakeDamage);
    }



    #endregion

    #region Public Methods



    /// <summary>
    /// 피해를 입었을 때 호출되는 메서드입니다.
    /// Acts.OnDamaged 델리게이트에 연결됩니다.
    /// </summary>
    /// <param name="abilityContext">피해량 정보(damageAmount)를 포함하는 컨텍스트.</param>
    public void TakeDamage(AbilityContext abilityContext)
    {
        // TODO: AbilityContext에 damageAmount 필드가 있다고 가정하고 피해를 적용합니다.
         CurrentHealth -= abilityContext.DamageAmount ?? 0f;

        // 디버그: 전달받은 AbilityContext 값을 로그로 출력합니다.
        Debug.Log($"DamageableComponent: {name}이(가) AbilityContext로부터 피해를 받았습니다. Context: {abilityContext}");

        // TODO: 체력이 0 이하가 되었을 때 사망 처리 로직을 트리거할 수 있습니다.
        // if (CurrentHealth <= 0) { RequestAction(Acts.OnDeath, new AbilityContext()); }
    }

    #endregion
}

namespace Game.Core
{
    /// <summary>
    /// 체력 관리에 필요한 설정과 현재 상태를 담는 클래스입니다.
    /// 이 데이터는 DamageableComponent에서 사용되며 JSON 직렬화/역직렬화의 대상이 됩니다.
    /// </summary>
    [Serializable]
    public class DamageableConfig : BaseConfig // BaseConfig를 상속받습니다.
    {
        // === 중요 변경: 필드 이름 컨벤션과 역할 명확화 ===
        // DefaultMaxHealth -> maxHealth (config의 핵심 설정)
        // currentHealth 필드를 추가하여 현재 상태를 여기에 저장
        public float maxHealth = 100f;   // 이 개체의 최대 체력 설정값
        public float currentHealth = 100f; // 이 개체의 현재 체력 상태값

        // TODO: 필요한 다른 체력 관련 설정이나 상태 필드를 여기에 추가할 수 있습니다.
        // public float defenseModifier = 0f;
        // public bool isInvincible = false;
    }
}
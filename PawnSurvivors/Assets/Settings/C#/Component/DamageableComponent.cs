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

    [SerializeField]
    private DamageableConfig damageableConfig = new();

    /// <summary>
    /// 최대 체력 프로퍼티. 외부에서 읽기만 가능 (get).
    /// </summary>
    public float MaxHealth
    {
        get { return damageableConfig.MaxHealth; }
        // set을 private으로 설정하여 내부에서만 수정 가능하게 할 수 있습니다.
        private set { damageableConfig.MaxHealth = value; }
    }

    /// <summary>
    /// 현재 체력 프로퍼티. 외부에서 읽기/쓰기 가능 (get, set).
    /// </summary>
    public float CurrentHealth
    {
        get { return damageableConfig.CurrentHealth; }
        set { damageableConfig.CurrentHealth = value; }
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
    
}
using System;
using UnityEngine;
using Game.Core;
using Game.Core.Base;
using Game.Core.Configs;
using Game.Core.Enums;
using Game.Core.Contexts; // Acts 및 AbilityContext 사용을 위해 추가

/// <summary>
/// 특정 대상에게 피해를 입히는 로직을 담당하는 컴포넌트입니다.
/// 공격 스킬, 투사체 등 피해를 발생시키는 GameObject에 부착하여 사용합니다.
/// </summary>
public class DamageDealerComponent : PawnBase // PawnAction을 상속하여 Pawn 시스템과 통합
{
    #region Fields & Properties

    [SerializeField]
    DamageDealerConfig damageDealerConfig;

    public float DamageAmount => damageDealerConfig.DamageAmount;

    public override void Initialize(IBaseConfig config)
    {
        if (config is DamageDealerConfig dmgConfig)
        {
            damageDealerConfig = dmgConfig;
        }
        else
        {
            Debug.LogWarning($"{name}: Initialize 호출 시 잘못된 config 타입입니다. 기대한 타입: DamageDealerConfig");
        }
    }

    #endregion

    #region Ability Registration

    /// <summary>
    /// PawnAction의 RegisterAbilities를 오버라이드하여
    /// 이 컴포넌트의 능력을 Pawn의 델리게이트 시스템에 등록합니다.
    /// 현재는 특정 Acts에 직접 연결되지 않고, 다른 컴포넌트에서 직접 호출될 수 있습니다.
    /// </summary>
    public override void RegisterAbilities()
    {
        AddAction(Acts.OnDamaged, DealDamage);

    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 지정된 IDamageable 대상에게 피해를 입힙니다.
    /// 이 메서드는 다른 컴포넌트(예: 공격 스킬 컴포넌트)에서 직접 호출될 수 있습니다.
    /// </summary>
    /// <param name="target">피해를 입힐 IDamageable 인터페이스를 구현한 대상입니다.</param>
    public void DealDamage(AbilityContext abilityContext)
    {
        //abilityContext.DamageAmount = _damageAmount;


        abilityContext.DamageAmount = DamageAmount;

    }



    #endregion
}

namespace Game.Core
{

}
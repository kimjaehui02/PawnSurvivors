using Game.Core;
using Game.Core.Base;
using Game.Core.Configs;
using Game.Core.Contexts;
using Game.Core.Enums;
using System;
using UnityEngine;

/// <summary>
/// 이동 로직을 담당하는 컴포넌트입니다.
/// 플레이어, 몬스터 등 움직임이 필요한 GameObject에 부착하여 사용합니다.
/// </summary>
public class MoveableComponent : PawnBase
{
    #region Fields & Properties

    [SerializeField]
    MoveableConfig moveableConfig = new(); // 이동 설정을 담는 구성 객체
    public float MoveSpeed => moveableConfig.MoveSpeed;

    public override void Initialize(IBaseConfig config)
    {
        if (config is MoveableConfig dmgConfig)
        {
            moveableConfig = dmgConfig;
            // 초기화 후 MoveSpeed가 설정되었는지 확인
            //Debug.LogWarning($"MoveableConfig가 설정되었습니다 스피드는{MoveSpeed}");
            //Debug.LogWarning($"MoveableConfig가 설정되었습니다 입력받은 스피드는{dmgConfig.MoveSpeed}");

        }
        else
        {
            Debug.LogWarning($"{name}: Initialize 호출 시 잘못된 config 타입입니다. 기대한 타입: MoveableConfig");
        }
    }

    #endregion

    #region Ability Registration

    /// <summary>
    /// PawnAction의 RegisterAbilities를 오버라이드하여
    /// 이 컴포넌트의 이동 능력을 Pawn의 델리게이트 시스템에 등록합니다.
    /// </summary>
    public override void RegisterAbilities()
    {
        // Acts.OnMove 이벤트가 발생했을 때 Move 메서드를 호출하도록 등록합니다.
        // AbilityContext를 통해 이동 방향 정보를 받아 처리합니다.

        AddAction(Acts.OnMove, Move);
    }

    #endregion

    #region Public Methods



    /// <summary>
    /// Acts.OnMove 델리게이트에 연결되어 실제 오브젝트 이동을 처리합니다.
    /// </summary>
    /// <param name="abilityContext">이동 방향 정보를 포함하는 컨텍스트 (AbilityContext.inputDirection 사용).</param>
    public void Move(AbilityContext abilityContext)
    {
        // 컨텍스트에서 받은 방향과 현재 속도, Time.deltaTime을 곱하여 오브젝트를 이동시킵니다.
        Vector3 movement = MoveSpeed * Time.deltaTime * (abilityContext.InputDirection ?? Vector3.zero);
        transform.position += movement;
    }

    #endregion
}


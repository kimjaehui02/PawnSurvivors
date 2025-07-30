using Game.Core;
using System;
using UnityEngine;

/// <summary>
/// 이동 로직을 담당하는 컴포넌트입니다.
/// 플레이어, 몬스터 등 움직임이 필요한 GameObject에 부착하여 사용합니다.
/// </summary>
public class MoveableComponent : PawnBase
{
    #region Fields & Properties


    private MoveableConfig Config
    {
        get
        {
            // Debug.Log($"Accessing _config. Current baseConfig type: {baseConfig?.GetType().Name ?? "null"}");
            return baseConfig as MoveableConfig;
        }
        set
        {
            // Debug.Log($"Setting _config. New value type: {value?.GetType().Name ?? "null"}");
            baseConfig = value;
        }
    }
    //public float MoveSpeed => moveSpeed;
    /// <summary>
    /// 최대 체력입니다. DamageableConfig에서 값을 가져옵니다.
    /// </summary>
    public float MoveSpeed
    {
        get
        {
            // _config가 null인 경우를 대비하여 방어 코드 추가
            if (Config == null)
            {
                return 5f;
            }
            return Config.moveSpeed;
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


namespace Game.Core
{
    /// <summary>
    /// 체력 관리에 필요한 설정과 현재 상태를 담는 클래스입니다.
    /// 이 데이터는 DamageableComponent에서 사용되며 JSON 직렬화/역직렬화의 대상이 됩니다.
    /// </summary>
    [Serializable]
    public class MoveableConfig : BaseConfig // BaseConfig를 상속받습니다.
    {
        public float moveSpeed = 1f; // 이동 속도 (유니티 에디터에서 설정 가능)

        /// <summary>
        /// 현재 이동 속도 값을 외부에 노출합니다.
        /// </summary>
        //public float MoveSpeed => moveSpeed;
    }
}
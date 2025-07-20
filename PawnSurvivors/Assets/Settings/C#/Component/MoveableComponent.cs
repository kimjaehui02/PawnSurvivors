using UnityEngine;
using Game.Core;

/// <summary>
/// 이동 로직을 담당하는 컴포넌트입니다.
/// 플레이어, 몬스터 등 움직임이 필요한 GameObject에 부착하여 사용합니다.
/// </summary>
public class MoveableComponent : PawnAction
{
    #region Fields & Properties

    [SerializeField] private float moveSpeed = 5f; // 이동 속도 (유니티 에디터에서 설정 가능)

    /// <summary>
    /// 현재 이동 속도 값을 외부에 노출합니다.
    /// </summary>
    public float MoveSpeed => moveSpeed;

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
    /// 이 컴포넌트의 이동 속도를 초기 설정하거나 변경합니다.
    /// </summary>
    /// <param name="initialMoveSpeed">설정할 초기 이동 속도.</param>
    public void InitializeMovement(float initialMoveSpeed)
    {
        moveSpeed = initialMoveSpeed;
    }

    /// <summary>
    /// Acts.OnMove 델리게이트에 연결되어 실제 오브젝트 이동을 처리합니다.
    /// </summary>
    /// <param name="abilityContext">이동 방향 정보를 포함하는 컨텍스트 (AbilityContext.inputDirection 사용).</param>
    public void Move(AbilityContext abilityContext)
    {
        // 컨텍스트에서 받은 방향과 현재 속도, Time.deltaTime을 곱하여 오브젝트를 이동시킵니다.
        Vector3 movement = moveSpeed * Time.deltaTime * (abilityContext.InputDirection ?? Vector3.zero);
        transform.position += movement;
    }

    #endregion
}
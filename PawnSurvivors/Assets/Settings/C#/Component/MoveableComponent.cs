using Game.Core;
using UnityEngine;

/// <summary>
/// 이동 로직을 담당하는 컴포넌트입니다.
/// 플레이어, 몬스터 등 움직임이 필요한 GameObject에 부착하여 사용합니다.
/// </summary>
public class MoveableComponent : PawnAction
{
    [SerializeField] private float moveSpeed = 5f; // 이동 속도

    // 현재 이동 속도 값을 외부에 노출합니다.
    public float MoveSpeed => moveSpeed;

    public override void RegisterAbilities()
    {
        AddAction(Acts.OnMove, Move);
    }

    /// <summary>
    /// 초기 이동 속도를 설정합니다.
    /// </summary>
    public void InitializeMovement(float initialMoveSpeed)
    {
        moveSpeed = initialMoveSpeed;
    }

    /// <summary>
    /// Acts.OnMove 델리게이트에 연결되어 실제 이동을 처리합니다.
    /// </summary>
    /// <param name="abilityContext">이동 방향 정보를 포함하는 컨텍스트</param>
    public void Move(AbilityContext abilityContext)
    {
        // 컨텍스트에서 받은 방향과 현재 속도로 오브젝트를 이동시킵니다.
        Vector3 movement = abilityContext.direction * moveSpeed * Time.deltaTime;
        transform.position += movement;
    }


}
using UnityEngine;
using static UnityEditorInternal.VersionControl.ListControl;

/// <summary>
/// 이동 로직을 담당하는 컴포넌트입니다. IMoveable 인터페이스를 구현합니다.
/// 이 컴포넌트를 플레이어, 몬스터 등 움직임이 필요한 GameObject에 붙여서 사용합니다.
/// </summary>
public class MoveableComponent : PawnAbility, IMoveable
{
    public override void RegisterAbilities()
    {
        //_actionDelegates += Move;
    }

    [SerializeField] private float moveSpeed = 5f; // 인스펙터에서 설정할 이동 속도
    public float MoveSpeed => moveSpeed; // IMoveable 인터페이스 구현

    /// <summary>
    /// 이속변경이 항상 필요한건 아님
    /// </summary>
    /// <param name="initialMoveSpeed"></param>
    public void InitializeMovement(float initialMoveSpeed)
    {
        moveSpeed = initialMoveSpeed;
        //Debug.Log($"{gameObject.name}의 MoveableComponent가 Move Speed: {moveSpeed}로 초기화되었습니다.");
    }

    /// <summary>
    /// IMoveable 인터페이스의 Move 메서드 구현.
    /// 대상의 Transform을 사용하여 실제로 이동을 처리합니다.
    /// </summary>
    /// <param name="direction">이동할 방향 벡터 (이미 정규화된 상태로 가정)</param>
    public void Move(Vector3 direction)
    {
        // 방향 벡터(direction)는 이미 정규화(normalized)된 상태로 넘어온다고 가정합니다.
        // 예를 들어, PlayerMoverComponent나 GetPawnMovementDirection 함수에서 정규화를 수행합니다.
        // 따라서 여기서 direction.normalized를 다시 호출할 필요가 없습니다.
        // 만약 direction이 정규화되지 않은 상태로 넘어올 수도 있는 경우라면,
        // Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime; 로 변경하여
        // 이 메서드 내에서 항상 정규화된 방향을 사용하도록 보장할 수 있습니다.
        Vector3 movement = direction * moveSpeed * Time.deltaTime;
        transform.position += movement;

        // 디버그 로그 추가 (선택 사항): 실제 적용되는 movement 값 확인
        //Debug.Log($"Applied movement: {movement}");

        // 여기에서 애니메이션 트리거, 이동 사운드 재생 등 추가 로직을 넣을 수 있습니다.
    }


}
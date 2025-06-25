using UnityEngine;

/// <summary>
/// 이동 로직을 담당하는 컴포넌트입니다. IMoveable 인터페이스를 구현합니다.
/// 이 컴포넌트를 플레이어, 몬스터 등 움직임이 필요한 GameObject에 붙여서 사용합니다.
/// </summary>
public class MoveableComponent : MonoBehaviour, IMoveable
{
    [SerializeField] private float moveSpeed = 5f; // 인스펙터에서 설정할 이동 속도
    public float MoveSpeed => moveSpeed; // IMoveable 인터페이스 구현

    // 이 메서드는 외부에서 데이터 주입을 받아 초기화하는 용도로도 사용할 수 있습니다.
    // 예를 들어, PawnSpawner에서 PawnData.MoveSpeed 값을 받아 초기화.
    public void InitializeMovement(float initialMoveSpeed)
    {
        moveSpeed = initialMoveSpeed;
        Debug.Log($"{gameObject.name}의 MoveableComponent가 Move Speed: {moveSpeed}로 초기화되었습니다.");
    }

    /// <summary>
    /// IMoveable 인터페이스의 Move 메서드 구현.
    /// 대상의 Transform을 사용하여 실제로 이동을 처리합니다.
    /// </summary>
    /// <param name="direction">이동할 방향 벡터</param>
    public void Move(Vector3 direction)
    {
        // Vector3.normalized는 방향 벡터의 크기를 1로 만듭니다.
        // 이는 대각선 이동 시 속도 증가를 방지합니다.
        Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
        transform.position += movement;

        // Debug.Log($"{gameObject.name}이(가) {direction} 방향으로 이동 중입니다.");

        // 여기에서 애니메이션 트리거, 이동 사운드 재생 등 추가 로직을 넣을 수 있습니다.
    }


}
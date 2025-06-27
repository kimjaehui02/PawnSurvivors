using UnityEngine;

/// <summary>
/// 플레이어의 입력을 받아 이동 방향을 결정하는 역할만 수행합니다.
/// 실제 이동 명령은 이 컴포넌트를 사용하는 상위 컨트롤러(예: Player.cs)가 내립니다.
/// </summary>
public class PlayerMoverComponent : MonoBehaviour, IPlayerMover
{
    // Awake()나 Update()에서 직접 MoveableComponent를 제어하는 로직은 없습니다.

    /// <summary>
    /// IPlayerControllable 인터페이스 구현.
    /// 플레이어의 키보드 입력을 받아 이동 방향 벡터를 반환합니다.
    /// </summary>
    /// <returns>플레이어의 입력에 따른 이동 방향 벡터</returns>
    public Vector3 GetPlayerMovementInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // A/D 또는 좌/우 화살표
        float verticalInput = Input.GetAxisRaw("Vertical");   // W/S 또는 상/하 화살표

        // 2D 평면(X-Z)에서의 이동을 위해 Y축은 0으로 설정합니다.
        // normalized를 통해 대각선 이동 시 속도가 빨라지는 것을 방지합니다.
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        return moveDirection;
    }
}
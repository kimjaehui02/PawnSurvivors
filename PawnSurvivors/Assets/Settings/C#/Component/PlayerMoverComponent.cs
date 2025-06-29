using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoverComponent : MonoBehaviour, IPlayerMover
{
    private PlayerInputActions playerInputActions;
    private Vector2 currentMoveInput;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();

        playerInputActions.Player.Move.performed += ctx =>
        {
            currentMoveInput = ctx.ReadValue<Vector2>();
            // 디버그 로그 추가: performed 될 때 입력 값을 확인
            //Debug.Log($"Input Performed: X={currentMoveInput.x}, Y={currentMoveInput.y}");
        };
        playerInputActions.Player.Move.canceled += ctx =>
        {
            currentMoveInput = Vector2.zero;
            // 디버그 로그 추가: canceled 될 때 입력 값을 확인
            //Debug.Log($"Input Canceled: X={currentMoveInput.x}, Y={currentMoveInput.y}");
        };
    }

    void OnEnable()
    {
        playerInputActions.Enable();
    }

    void OnDisable()
    {
        playerInputActions.Disable();
    }

    public Vector3 GetPlayerMovementInput()
    {
        Vector3 moveDirection = new Vector3(currentMoveInput.x, currentMoveInput.y, 0f).normalized;

        // 디버그 로그 추가: 최종 반환되는 이동 방향 벡터 확인
        // 이 로그는 Update()나 FixedUpdate()마다 계속 출력됩니다.
        // W나 S를 눌렀을 때 moveDirection.z 값이 변하는지 확인하세요.
        //Debug.Log($"Movement Input: X={moveDirection.x}, Z={moveDirection.z}");

        return moveDirection;
    }
}
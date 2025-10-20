using UnityEngine;
using UnityEngine.InputSystem;

public class Input : PawnSub
{
    #region Fields
    public PawnUseCase pawnUseCase;
    private InputActions _inputActions;
    private Vector2 currentMoveInput;             // 현재 입력된 이동 벡터 값

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// MonoBehaviour의 Awake 메서드입니다.
    /// Input System 액션 맵을 초기화하고 입력 이벤트를 구독합니다.
    /// </summary>
    void Awake()
    {
        _inputActions = new InputActions();

        // 이동 입력(Move) 액션에 performed (입력 시작/유지) 이벤트 구독
        _inputActions.Player.Move.performed += ctx =>
        {
            currentMoveInput = ctx.ReadValue<Vector2>();
            // 디버그: 입력 값 확인 (필요시 주석 해제)
            // Debug.Log($"Input Performed: X={currentMoveInput.x}, Y={currentMoveInput.y}");
        };

        // 이동 입력(Move) 액션에 canceled (입력 종료) 이벤트 구독
        _inputActions.Player.Move.canceled += ctx =>
        {
            currentMoveInput = Vector2.zero; // 입력이 없으면 0으로 설정
            // 디버그: 입력 값 확인 (필요시 주석 해제)
            // Debug.Log($"Input Canceled: X={currentMoveInput.x}, Y={currentMoveInput.y}");
        };
    }
    #endregion
    public void Start()
    {


        // 이동 관련 액션을 델리게이트 맵에 추가
        //myMap.Add(EnumActions.GetPlayerMovementInput, PawnUseCase.GetPlayerMovementInput);
        //myMap.Add(EnumActions.GetPlayerMovementInput, context =>
        //{
        //    // Movable 변수 적용
        //    context.MoveSpeed = this.MoveSpeed;
        //});
    }

}

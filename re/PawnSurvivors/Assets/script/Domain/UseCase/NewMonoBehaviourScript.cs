using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private PlayerInputActions playerInputActions; // Input System 액션 맵 인스턴스
    private Vector2 currentMoveInput;
    public float MoveSpeed = 1f;
    #region Unity Lifecycle

    /// <summary>
    /// MonoBehaviour의 Awake 메서드입니다.
    /// Input System 액션 맵을 초기화하고 입력 이벤트를 구독합니다.
    /// </summary>
    void Awake()
    {
        playerInputActions = new PlayerInputActions();

        // 이동 입력(Move) 액션에 performed (입력 시작/유지) 이벤트 구독
        playerInputActions.Player.Move.performed += ctx =>
        {
            currentMoveInput = ctx.ReadValue<Vector2>();
            // 디버그: 입력 값 확인 (필요시 주석 해제)
            // Debug.Log($"Input Performed: X={currentMoveInput.x}, Y={currentMoveInput.y}");
        };

        // 이동 입력(Move) 액션에 canceled (입력 종료) 이벤트 구독
        playerInputActions.Player.Move.canceled += ctx =>
        {
            currentMoveInput = Vector2.zero; // 입력이 없으면 0으로 설정
            // 디버그: 입력 값 확인 (필요시 주석 해제)
            // Debug.Log($"Input Canceled: X={currentMoveInput.x}, Y={currentMoveInput.y}");
        };
    }

    /// <summary>
    /// MonoBehaviour의 OnEnable 메서드입니다.
    /// 컴포넌트가 활성화될 때 Input System 액션 맵을 활성화합니다.
    /// </summary>
    void OnEnable()
    {
        playerInputActions.Enable();
    }

    /// <summary>
    /// MonoBehaviour의 OnDisable 메서드입니다.
    /// 컴포넌트가 비활성화될 때 Input System 액션 맵을 비활성화합니다.
    /// </summary>
    void OnDisable()
    {
        playerInputActions.Disable();
    }

    #endregion
    public void Move(AbilityContext abilityContext)
    {
        // 컨텍스트에서 받은 방향과 현재 속도, Time.deltaTime을 곱하여 오브젝트를 이동시킵니다.
        Vector3 movement = MoveSpeed * Time.deltaTime * (abilityContext.InputDirection ?? Vector3.zero);
        transform.position += movement;
    }
    public void GetPlayerMovementInput(AbilityContext abilityContext)
    {
        // 현재 입력 벡터(Vector2)를 3D 벡터(Vector3)로 변환하고 정규화하여 방향만 남깁니다.
        // Y축은 Z축으로 매핑하지 않고, 2D 게임이라면 X, Y만 사용하도록 합니다.
        // 만약 3D 게임이고 위/아래 이동도 필요하다면 Z축을 조정해야 합니다.
        Vector3 moveDirection = (Vector3)currentMoveInput.normalized;

        // AbilityContext의 inputDirection 필드를 업데이트합니다.
        abilityContext.InputDirection = moveDirection;
    }
}
public class AbilityContext
{
    public Pawn SourcePawn { get; set; }
    public Pawn TargetPawn { get; set; }
    public Vector3? InputDirection { get; set; }
    public float? DamageAmount { get; set; }
}
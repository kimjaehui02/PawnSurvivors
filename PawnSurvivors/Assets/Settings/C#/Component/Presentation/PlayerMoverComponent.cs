using UnityEngine;
using Game.Core;
using UnityEngine.InputSystem;
using Game.Core.Base;
using Game.Core.Contexts;
using Game.Core.Enums; // Input System 관련 네임스페이스

/// <summary>
/// 플레이어 입력에 따라 이동 방향을 결정하고 AbilityContext에 전달하는 컴포넌트입니다.
/// </summary>
public class PlayerMoverComponent : PawnBase
{
    #region Fields

    private PlayerInputActions playerInputActions; // Input System 액션 맵 인스턴스
    private Vector2 currentMoveInput;             // 현재 입력된 이동 벡터 값

    #endregion

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

    #region Ability Registration

    /// <summary>
    /// PawnAction의 RegisterAbilities를 오버라이드하여
    /// 이 컴포넌트의 능력을 Pawn의 델리게이트 시스템에 등록합니다.
    /// </summary>
    public override void RegisterAbilities()
    {
        // Acts.OnUpdate 델리게이트에 GetPlayerMovementInput 메서드를 연결합니다.
        // 이로써 Pawn의 Update 루프마다 플레이어 입력이 AbilityContext에 반영됩니다.
        AddAction(Acts.OnUpdate, GetPlayerMovementInput);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 플레이어의 현재 이동 입력을 AbilityContext에 반영합니다.
    /// Acts.OnUpdate 델리게이트에 연결되어 매 프레임 호출됩니다.
    /// </summary>
    /// <param name="abilityContext">이동 방향 정보를 담을 컨텍스트.</param>
    public void GetPlayerMovementInput(AbilityContext abilityContext)
    {
        // 현재 입력 벡터(Vector2)를 3D 벡터(Vector3)로 변환하고 정규화하여 방향만 남깁니다.
        // Y축은 Z축으로 매핑하지 않고, 2D 게임이라면 X, Y만 사용하도록 합니다.
        // 만약 3D 게임이고 위/아래 이동도 필요하다면 Z축을 조정해야 합니다.
        Vector3 moveDirection = (Vector3)currentMoveInput.normalized;

        // AbilityContext의 inputDirection 필드를 업데이트합니다.
        abilityContext.InputDirection = moveDirection;
    }

    #endregion
}
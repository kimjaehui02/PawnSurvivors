using UnityEngine;
using Game.Core; // PawnAction과의 연동을 위해 Game.Core 네임스페이스 추가

/// <summary>
/// Pawn의 시각적 표현 (스프라이트, 애니메이션)을 담당하는 컴포넌트입니다.
/// SpriteRenderer와 Animator를 제어하여 그래픽 업데이트를 처리합니다.
/// </summary>
public class GraphicComponent : PawnAction // PawnAction을 상속합니다.
{
    #region Fields & Initialization

    [SerializeField] private SpriteRenderer spriteRenderer; // 인스펙터에서 연결
    [SerializeField] private Animator animator;              // 인스펙터에서 연결

    /// <summary>
    /// MonoBehaviour의 Awake 메서드입니다.
    /// 연결된 SpriteRenderer와 Animator가 없을 경우 자동으로 찾습니다.
    /// </summary>
    void Awake()
    {
        // Null 체크 또는 GetComponent<T>()로 자동 찾기 (선택)
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();

        // 중요한 그래픽 컴포넌트가 없으면 경고 로그 (선택 사항)
        if (spriteRenderer == null) Debug.LogWarning($"GraphicComponent: {gameObject.name}에 SpriteRenderer가 연결되지 않았습니다.", this);
        if (animator == null) Debug.LogWarning($"GraphicComponent: {gameObject.name}에 Animator가 연결되지 않았습니다.", this);
    }

    /// <summary>
    /// 그래픽 컴포넌트를 초기화합니다. (PawnSpawner 등에서 호출될 수 있음)
    /// </summary>
    /// <param name="initialSprite">초기 설정할 스프라이트입니다.</param>
    /// <param name="initialAnimation">초기 재생할 애니메이션 클립의 이름입니다.</param>
    public void InitializeGraphics(Sprite initialSprite, string initialAnimation = "")
    {
        SetSprite(initialSprite);
        if (!string.IsNullOrEmpty(initialAnimation))
        {
            PlayAnimation(initialAnimation);
        }
        Debug.Log($"{gameObject.name}의 그래픽이 초기화되었습니다.");
    }

    #endregion

    #region Ability Registration

    /// <summary>
    /// PawnAction의 RegisterAbilities를 오버라이드하여
    /// 이 컴포넌트의 능력을 Pawn의 델리게이트 시스템에 등록합니다.
    /// 그래픽 업데이트는 주로 다른 컴포넌트(예: MoveableComponent, PlayerMoverComponent)에서
    /// AbilityContext를 통해 요청될 수 있습니다.
    /// </summary>
    public override void RegisterAbilities()
    {
        // TODO: 만약 특정 Acts가 발생했을 때 그래픽 업데이트 로직이 필요하다면 이곳에 추가합니다.
        // 예를 들어:
        // AddAction(Acts.OnMove, (context) => HandleMovementGraphics(context));
        // AddAction(Acts.OnDamaged, (context) => PlayHitAnimation());

        // 현재는 RegisterAbilities에서 직접적으로 특정 Acts에 연결하지 않고 있습니다.
        // throw new System.NotImplementedException(); // 이 줄이 더 이상 필요 없으면 제거합니다.
    }

    // 예시: Acts.OnMove에 연결될 수 있는 메서드
    /*
    private void HandleMovementGraphics(AbilityContext context)
    {
        // 이동 방향에 따라 애니메이터 파라미터 설정 및 스프라이트 뒤집기 등을 처리합니다.
        if (animator != null)
        {
            animator.SetFloat("MoveX", context.inputDirection.x);
            animator.SetFloat("MoveY", context.inputDirection.y);
            animator.SetBool("IsMoving", context.inputDirection.magnitude > 0);
        }
        if (spriteRenderer != null)
        {
            // 이동 방향에 따라 스프라이트 뒤집기 (예: 오른쪽으로 가면 FlipX = false, 왼쪽으로 가면 FlipX = true)
            if (context.inputDirection.x < 0) FlipX(true);
            else if (context.inputDirection.x > 0) FlipX(false);
        }
    }
    */

    #endregion

    #region Public Methods (Graphic Control)

    /// <summary>
    /// SpriteRenderer의 스프라이트를 변경합니다.
    /// </summary>
    /// <param name="newSprite">새로 설정할 스프라이트입니다.</param>
    public void SetSprite(Sprite newSprite)
    {
        if (spriteRenderer != null) spriteRenderer.sprite = newSprite;
    }

    /// <summary>
    /// SpriteRenderer의 색상을 변경합니다.
    /// </summary>
    /// <param name="color">새로 설정할 색상입니다.</param>
    public void SetColor(Color color)
    {
        if (spriteRenderer != null) spriteRenderer.color = color;
    }

    /// <summary>
    /// SpriteRenderer를 X축으로 뒤집을지 설정합니다.
    /// </summary>
    /// <param name="flip">뒤집을지 여부입니다.</param>
    public void FlipX(bool flip)
    {
        if (spriteRenderer != null) spriteRenderer.flipX = flip;
    }

    /// <summary>
    /// 애니메이터에서 특정 애니메이션 클립을 재생합니다.
    /// </summary>
    /// <param name="animationClipName">재생할 애니메이션 클립의 이름입니다.</param>
    public void PlayAnimation(string animationClipName)
    {
        if (animator != null) animator.Play(animationClipName);
    }

    /// <summary>
    /// 애니메이션 재생 속도를 설정합니다.
    /// </summary>
    /// <param name="speed">설정할 재생 속도입니다.</param>
    public void SetAnimationSpeed(float speed)
    {
        if (animator != null) animator.speed = speed;
    }

    /// <summary>
    /// 애니메이터의 Float 파라미터를 설정합니다.
    /// </summary>
    /// <param name="parameterName">파라미터 이름입니다.</param>
    /// <param name="value">설정할 Float 값입니다.</param>
    public void SetAnimatorParameter(string parameterName, float value)
    {
        if (animator != null) animator.SetFloat(parameterName, value);
    }

    /// <summary>
    /// 애니메이터의 Bool 파라미터를 설정합니다.
    /// </summary>
    /// <param name="parameterName">파라미터 이름입니다.</param>
    /// <param name="value">설정할 Bool 값입니다.</param>
    public void SetAnimatorParameter(string parameterName, bool value)
    {
        if (animator != null) animator.SetBool(parameterName, value);
    }

    /// <summary>
    /// 애니메이터의 Integer 파라미터를 설정합니다.
    /// </summary>
    /// <param name="parameterName">파라미터 이름입니다.</param>
    /// <param name="value">설정할 Integer 값입니다.</param>
    public void SetAnimatorParameter(string parameterName, int value)
    {
        if (animator != null) animator.SetInteger(parameterName, value);
    }

    #endregion
}
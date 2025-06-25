using UnityEngine;

// 2. GraphicComponent (IGraphic 구현)
public class GraphicComponent : MonoBehaviour, IGraphic
{
    [SerializeField] private SpriteRenderer spriteRenderer; // 인스펙터에서 연결
    [SerializeField] private Animator animator;             // 인스펙터에서 연결

    void Awake()
    {
        // Null 체크 또는 GetComponent<T>()로 자동 찾기 (선택)
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void SetSprite(Sprite newSprite)
    {
        if (spriteRenderer != null) spriteRenderer.sprite = newSprite;
    }

    public void SetColor(Color color)
    {
        if (spriteRenderer != null) spriteRenderer.color = color;
    }

    public void FlipX(bool flip)
    {
        if (spriteRenderer != null) spriteRenderer.flipX = flip;
    }

    public void PlayAnimation(string animationClipName)
    {
        if (animator != null) animator.Play(animationClipName);
    }

    public void SetAnimationSpeed(float speed)
    {
        if (animator != null) animator.speed = speed;
    }

    public void SetAnimatorParameter(string parameterName, float value)
    {
        if (animator != null) animator.SetFloat(parameterName, value);
    }

    public void SetAnimatorParameter(string parameterName, bool value)
    {
        if (animator != null) animator.SetBool(parameterName, value);
    }

    public void SetAnimatorParameter(string parameterName, int value)
    {
        if (animator != null) animator.SetInteger(parameterName, value);
    }

    // 초기화 메서드 (PawnSpawner에서 호출)
    public void InitializeGraphics(Sprite initialSprite, string initialAnimation = "")
    {
        SetSprite(initialSprite);
        if (!string.IsNullOrEmpty(initialAnimation))
        {
            PlayAnimation(initialAnimation);
        }
        Debug.Log($"{gameObject.name}의 그래픽이 초기화되었습니다.");
    }
}
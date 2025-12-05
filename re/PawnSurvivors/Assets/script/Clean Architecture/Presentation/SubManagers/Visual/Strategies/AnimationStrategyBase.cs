using UnityEngine;

/// <summary>
/// 모든 폰 애니메이션 전략에 대한 추상 MonoBehaviour 기본 클래스입니다.
/// 각 구체적인 전략은 폰에 연결된 구성 요소여야 합니다.
/// </summary>
public abstract class AnimationStrategyBase : MonoBehaviour
{
    protected PawnManager _pawnManager;
    protected GameObject _visualsObject;

    public virtual void Init(PawnManager pawnManager, GameObject visualsObject)
    {
        _pawnManager = pawnManager;
        _visualsObject = visualsObject;
    }

    protected virtual void Awake()
    {
        if (_pawnManager == null)
        {
            _pawnManager = GetComponent<PawnManager>();
        }
        if (_pawnManager == null)
        {
            Debug.LogError("An AnimationStrategy must be on a GameObject with a PawnManager.", this);
        }
    }

    /// <summary>
    /// 애니메이션 로직을 실행합니다.
    /// </summary>
    public abstract void Animate();

    /// <summary>
    /// 레시피 설정에 따라 전략의 초기 활성화 상태를 설정합니다.
    /// </summary>
    /// <param name="enabledState">전략을 기본적으로 활성화해야 하는지 여부입니다.</param>
    public void SetInitialEnabledState(bool enabledState)
    {
        this.enabled = enabledState;
    }

    /// <summary>
    /// 게임 델타타임을 가져옵니다. (일시정지 시 0)
    /// </summary>
    protected float GetGameDeltaTime()
    {
        if (GameManager.Instance?.LifecycleManager != null)
        {
            return GameManager.Instance.LifecycleManager.GameDeltaTime;
        }
        return Time.deltaTime; // 폴백
    }
}


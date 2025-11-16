using UnityEngine;

public abstract class PawnSubManager : MonoBehaviour
{
    public PawnManager _pawnManager;

    private void Awake()
    {
        _pawnManager = GetComponent<PawnManager>();
    }

    private void Start()
    {
        if (_pawnManager != null)
        {
            _pawnManager.RegisterSubManager(this);
        }
    }

    private void OnDestroy()
    {
        if (_pawnManager != null)
        {
            _pawnManager.UnregisterSubManager(this);
        }
    }

    public abstract void SubUpdate();

    public abstract void SubStart();

    /// <summary>
    /// 게임 델타타임을 가져옵니다. (일시정지 시 0)
    /// LifecycleManager가 없으면 Time.deltaTime을 반환합니다.
    /// </summary>
    protected float GetGameDeltaTime()
    {
        if (GameManager.Instance?.LifecycleManager != null)
        {
            return GameManager.Instance.LifecycleManager.GameDeltaTime;
        }
        return Time.deltaTime; // 폴백
    }

    /// <summary>
    /// 게임 시간을 가져옵니다. (일시정지 중에는 멈춤)
    /// LifecycleManager가 없으면 Time.time을 반환합니다.
    /// </summary>
    protected float GetGameTime()
    {
        if (GameManager.Instance?.LifecycleManager != null)
        {
            return GameManager.Instance.LifecycleManager.GameTime;
        }
        return Time.time; // 폴백
    }
}

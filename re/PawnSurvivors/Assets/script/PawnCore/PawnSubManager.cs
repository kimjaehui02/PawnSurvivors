using UnityEngine;

public abstract class PawnSubManager : MonoBehaviour
{
    private PawnManager _pawnManager;

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
}

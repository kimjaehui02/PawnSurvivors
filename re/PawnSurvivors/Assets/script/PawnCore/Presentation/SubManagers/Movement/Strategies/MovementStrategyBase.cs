using UnityEngine;

/// <summary>
/// An abstract MonoBehaviour base class for all pawn movement strategies.
/// Each concrete strategy should be a component attached to the pawn.
/// </summary>
public abstract class MovementStrategyBase : MonoBehaviour
{
    protected PawnManager _pawnManager;

    public virtual void Init(PawnManager pawnManager)
    {
        _pawnManager = pawnManager;
    }

    protected virtual void Awake()
    {
        if (_pawnManager == null)
        {
            _pawnManager = GetComponent<PawnManager>();
        }
        if (_pawnManager == null)
        {
            Debug.LogError("A MovementStrategy must be on a GameObject with a PawnManager.", this);
        }
    }

    /// <summary>
    /// Executes the movement logic for the Pawn.
    /// </summary>
    public abstract void Move();

    /// <summary>
    /// Sets the initial enabled state of the strategy based on the recipe setup.
    /// </summary>
    /// <param name="enabledState">Whether the strategy should be enabled by default.</param>
    public void SetInitialEnabledState(bool enabledState)
    {
        this.enabled = enabledState;
    }
}

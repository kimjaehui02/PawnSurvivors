using UnityEngine;

/// <summary>
/// An abstract MonoBehaviour base class for all pawn movement strategies.
/// Each concrete strategy should be a component attached to the pawn.
/// </summary>
public abstract class MovementStrategyBase : MonoBehaviour
{
    protected PawnManager _pawnManager;

    protected virtual void Awake()
    {
        _pawnManager = GetComponent<PawnManager>();
        if (_pawnManager == null)
        {
            Debug.LogError("A MovementStrategy must be on a GameObject with a PawnManager.", this);
        }
    }

    /// <summary>
    /// Executes the movement logic for the Pawn.
    /// </summary>
    public abstract void Move();
}

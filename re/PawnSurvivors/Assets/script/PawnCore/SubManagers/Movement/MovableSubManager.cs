using UnityEngine;
using PawnSurvivors.PawnCore.Movement; // Added for MovementStrategyType

public class MovableSubManager : PawnSubManager
{
    // The currently active movement strategy.
    // This will be set via the Inspector or dynamically through events.
    [SerializeReference] // Allows serializing interfaces/abstract classes in Unity Inspector
    private IMovementStrategy _currentStrategy;

    // Use an enum to select the initial movement strategy in the Inspector.
    public MovementStrategyType initialStrategyType = MovementStrategyType.None;

    public override void SubStart()
    {
        // If no strategy is explicitly set, instantiate based on the enum selection.
        if (_currentStrategy == null)
        {
            switch (initialStrategyType)
            {
                case MovementStrategyType.Keyboard:
                    _currentStrategy = new KeyboardMovementStrategy();
                    break;
                case MovementStrategyType.Directional:
                    _currentStrategy = new DirectionalMovementStrategy();
                    break;
                case MovementStrategyType.Target:
                    _currentStrategy = new TargetMovementStrategy();
                    break;
                case MovementStrategyType.None:
                default:
                    Debug.LogWarning("MovableSubManager: No specific movement strategy selected or assigned. Pawn will not move.", this);
                    // Optionally, assign a 'NoMovementStrategy' here if you have one.
                    break;
            }
        }

        if (_currentStrategy == null)
        {
            Debug.LogWarning("MovableSubManager: No movement strategy assigned after SubStart. Pawn will not move.", this);
        }
    }

    public override void SubUpdate()
    {
        // Delegate the movement logic to the current strategy.
        if (_currentStrategy != null)
        {
            // Pass the PawnManager (_pawnManager) and deltaTime to the strategy.
            // The strategy will then access the Transform and other necessary components from pawnManager.
            _currentStrategy.Move(_pawnManager, Time.deltaTime);
        }
    }

    /// <summary>
    /// Sets the active movement strategy.
    /// This method could be called directly or in response to an event.
    /// </summary>
    /// <param name="newStrategy">The new movement strategy to use.</param>
    public void SetStrategy(IMovementStrategy newStrategy)
    {
        if (newStrategy == null)
        {
            Debug.LogWarning("MovableSubManager: Attempted to set a null movement strategy.", this);
            return;
        }
        _currentStrategy = newStrategy;
        Debug.Log($"MovableSubManager: Movement strategy changed to {_currentStrategy.GetType().Name}");
    }
}

using System;
using System.Linq;
using UnityEngine;

public class MovableSubManager : PawnSubManager
{
    private MovementStrategyBase _currentStrategy;

    public override void SubStart()
    {
        var allStrategies = GetComponents<MovementStrategyBase>();

        // Initialize all strategies with a reference to the PawnManager
        foreach (var strategy in allStrategies)
        {
            strategy.Init(_pawnManager);
        }

        // Find the strategy that is initially enabled (set by JSON recipe)
        _currentStrategy = allStrategies.FirstOrDefault(s => s.enabled);

        if (_currentStrategy == null && allStrategies.Length > 0)
        {
            // If none are enabled, default to the first one and enable it.
            _currentStrategy = allStrategies[0];
            _currentStrategy.enabled = true;
            Debug.LogWarning($"MovableSubManager: No movement strategy was enabled by default by the recipe. Defaulting to and enabling ''{_currentStrategy.GetType().Name}'.", this);
        }
        
        // Ensure only the current strategy is active. This is crucial if multiple were enabled by mistake.
        foreach (var strategy in allStrategies)
        {
            if (strategy != _currentStrategy)
            {
                strategy.enabled = false;
            }
        }

        // Subscribe to the event to handle runtime strategy changes
        _pawnManager.Subscribe<ChangeMovementStrategyEvent>(HandleChangeStrategyEvent);
    }

    private void OnDestroy()
    {
        if (_pawnManager != null)
        {
            _pawnManager.Unsubscribe<ChangeMovementStrategyEvent>(HandleChangeStrategyEvent);
        }
    }

    public override void SubUpdate()
    {
        if (_currentStrategy != null)
        {
            _currentStrategy.Move();
        }
    }

    private void HandleChangeStrategyEvent(ChangeMovementStrategyEvent evt)
    {
        if (evt?.StrategyType == null) return;
        if (_currentStrategy != null && _currentStrategy.GetType() == evt.StrategyType) return; // Already the active strategy

        // Get the current list of components at the moment the event is handled.
        // This makes the system robust to components being added/removed at runtime.
        var allStrategies = GetComponents<MovementStrategyBase>();
        var nextStrategy = allStrategies.FirstOrDefault(s => s.GetType() == evt.StrategyType);

        if (nextStrategy != null)
        {
            // Disable the current strategy
            if (_currentStrategy != null)
            {
                _currentStrategy.enabled = false;
            }

            // Enable the new strategy
            nextStrategy.enabled = true;
            _currentStrategy = nextStrategy;
            Debug.Log($"MovableSubManager: Movement strategy changed to ''{_currentStrategy.GetType().Name}'.");
        }
        else
        {
            Debug.LogWarning($"MovableSubManager: A request was made to switch to strategy ''{evt.StrategyType.Name}', but no such component is attached to this GameObject.", this);
        }
    }
}

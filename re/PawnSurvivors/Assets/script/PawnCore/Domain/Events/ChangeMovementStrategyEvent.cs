using System;
using UnityEngine;

public class ChangeMovementStrategyEvent
{
    public Type StrategyType { get; }

    public ChangeMovementStrategyEvent(Type strategyType)
    {
        if (!typeof(MovementStrategyBase).IsAssignableFrom(strategyType))
        {
            Debug.LogError($"Type {strategyType.Name} is not a valid movement strategy.");
            return;
        }
        StrategyType = strategyType;
    }
}

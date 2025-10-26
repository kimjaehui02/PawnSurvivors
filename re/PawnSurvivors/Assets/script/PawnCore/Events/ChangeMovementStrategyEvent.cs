using UnityEngine;

public class ChangeMovementStrategyEvent
{
    public IMovementStrategy NewStrategy { get; }

    public ChangeMovementStrategyEvent(IMovementStrategy newStrategy)
    {
        NewStrategy = newStrategy;
    }
}

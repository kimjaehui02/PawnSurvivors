using System;
using System.Collections.Generic;

public class ActionCollection
{
    private readonly List<EnumActions> actions = new();

    public IReadOnlyList<EnumActions> ActionsList => actions.AsReadOnly();

    public void Add(EnumActions action)
    {
        if (!actions.Contains(action))
            actions.Add(action);
    }

    public bool Has(EnumActions action) => actions.Contains(action);

    public void Update(int index, EnumActions newAction)
    {
        if (index < 0 || index >= actions.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        actions[index] = newAction;
    }

    public bool Remove(EnumActions action) => actions.Remove(action);

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= actions.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        actions.RemoveAt(index);
    }
}

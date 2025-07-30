using Game.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManagerBase2 : MonoBehaviour
{
    private readonly EnumDelegateMap<GameEventType, GameEventContext> _delegateMap = new();

    public IReadOnlyDictionary<GameEventType, Action<GameEventContext>> GetActions => _delegateMap.GetActions;

    public void AddAction(GameEventType gameEventType, Action<GameEventContext> action) => _delegateMap.AddAction(gameEventType, action);
    public void RemoveAction(GameEventType gameEventType, Action<GameEventContext> action = null) => _delegateMap.RemoveAction(gameEventType, action);

    public void RequestAction(GameEventType gameEventType, GameEventContext context) => _delegateMap.RequestAction(gameEventType, context);

    public void RequestActions(List<GameEventType> gameEventType, GameEventContext context) => _delegateMap.RequestActions(gameEventType, context);

    public virtual void RegisterAbilities() => _delegateMap.RegisterAbilities();


}

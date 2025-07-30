using Game.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SubComponentBase<TEnum, TContext> : MonoBehaviour where TEnum : Enum
{
    public BaseConfig baseConfig; // PawnAction이 사용하는 설정을 담는 BaseConfig
    public void SetBaseConfig(BaseConfig config) => baseConfig = config;


    private readonly EnumDelegateMap<TEnum, TContext> _delegateMap = new();

    //private Dictionary<TEnum, Action<TContext>> MyDelegates => _delegateMap._myDelegatesMap;

    public IReadOnlyDictionary<TEnum, Action<TContext>> GetActions => _delegateMap.GetActions;

    public void AddAction(TEnum act, Action<TContext> action) => _delegateMap.AddAction(act, action);
    public void RemoveAction(TEnum act, Action<TContext> action = null) => _delegateMap.RemoveAction(act, action);

    public void RequestAction(TEnum act, TContext context) => _delegateMap.RequestAction(act, context);

    public void RequestActions(List<TEnum> TEnum, TContext context) => _delegateMap.RequestActions(TEnum, context);

    public virtual void RegisterAbilities() => _delegateMap.RegisterAbilities();
}

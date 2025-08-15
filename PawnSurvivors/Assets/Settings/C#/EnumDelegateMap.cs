using Game.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnumDelegateMap<TEnum, TContext> where TEnum : Enum
{
    #region Fields & Properties
    private readonly Dictionary<TEnum, Action<TContext>> _myDelegatesMap = new();

    public IReadOnlyDictionary<TEnum, Action<TContext>> GetActions => _myDelegatesMap;
    #endregion

    #region Public Methods

    public void AddAction(TEnum act, Action<TContext> action)
    {
        if (action == null)
        {
            Debug.LogWarning($"AddAction: {act}에 null 액션이 전달되었습니다. 추가하지 않습니다.");
            return;
        }

        if (_myDelegatesMap.ContainsKey(act))
        {
            _myDelegatesMap[act] += action;
        }
        else
        {
            _myDelegatesMap.Add(act, action);
        }
    }

    public void RemoveAction(TEnum act, Action<TContext> action = null)
    {
        if (!_myDelegatesMap.ContainsKey(act))
        {
            Debug.LogWarning($"RemoveAction: {_myDelegatesMap}에 {act}가 존재하지 않습니다.");
            return;
        }

        if (action != null)
        {
            _myDelegatesMap[act] -= action;

            if (_myDelegatesMap[act] == null)
            {
                _myDelegatesMap.Remove(act);
            }
        }
        else
        {
            _myDelegatesMap.Remove(act);
        }
    }

    public void RequestAction(TEnum act, TContext context)
    {
        if (!_myDelegatesMap.TryGetValue(act, out Action<TContext> actionDelegate))
        {
            Debug.LogWarning($"RequestAction: {act}에 등록된 액션이 없습니다.");
            return;
        }

        actionDelegate?.Invoke(context);
    }

    public void RequestActions(List<TEnum> acts, TContext context)
    {
        if (acts == null || acts.Count == 0)
        {
            Debug.LogWarning("RequestActions: 전달된 Enum 리스트가 null이거나 비어있습니다.");
            return;
        }

        foreach (var act in acts)
        {
            RequestAction(act, context);
        }
    }

    #endregion

    #region Abstract / Virtual

    public virtual void RegisterAbilities()
    {
        // 자식 클래스에서 구현
    }

    #endregion
}

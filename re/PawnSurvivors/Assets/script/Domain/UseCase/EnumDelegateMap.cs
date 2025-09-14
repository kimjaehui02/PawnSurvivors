using System;
using System.Collections.Generic;
using UnityEngine;

public class EnumDelegateMap<TEnum, TContext> where TEnum : Enum
{
    private readonly Dictionary<TEnum, Action<TContext>> _actions = new();

    public void Add(TEnum key, Action<TContext> action)
    {
        if (action == null) return;
        if (_actions.ContainsKey(key)) _actions[key] += action;
        else _actions.Add(key, action);
    }

    public void Remove(TEnum key, Action<TContext> action = null)
    {
        if (!_actions.ContainsKey(key)) return;

        if (action != null)
        {
            _actions[key] -= action;
            if (_actions[key] == null) _actions.Remove(key);
        }
        else
        {
            _actions.Remove(key);
        }
    }

    public void Invoke(TEnum key, TContext context)
    {
        if (_actions.TryGetValue(key, out var action))
            action?.Invoke(context);
    }

    public void Invoke(IEnumerable<TEnum> keys, TContext context)
    {
        foreach (var key in keys) Invoke(key, context);
    }

    public void Clear() => _actions.Clear();

    // 🔹 모든 키-액션 쌍 반환 (B -> C 병합용)
    public Dictionary<TEnum, List<Action<TContext>>> GetAll()
    {
        var dict = new Dictionary<TEnum, List<Action<TContext>>>();
        foreach (var kv in _actions)
        {
            var list = new List<Action<TContext>>();
            foreach (Delegate d in kv.Value.GetInvocationList())
            {
                list.Add((Action<TContext>)d);
            }
            dict[kv.Key] = list;
        }
        return dict;
    }
}

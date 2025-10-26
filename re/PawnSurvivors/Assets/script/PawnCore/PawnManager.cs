using System;
using System.Collections.Generic;
using System.Linq; // Added for LINQ (ToList())
using UnityEngine;

public class PawnManager : MonoBehaviour
{
    // 다양한 이벤트 타입에 대한 이벤트 핸들러를 저장하는 딕셔너리입니다.
    // Key: 이벤트의 타입 (예: typeof(AttackInputEvent))
    // Value: 해당 이벤트 타입에 구독된 델리게이트(Action<TEvent>) 목록입니다.
    private Dictionary<Type, List<Delegate>> _eventHandlers = new();

    // Using a list for dynamic addition and removal of sub-managers.
    public List<PawnSubManager> pawnSubManagers = new();

    public static readonly List<PawnManager> AllPawnManagers = new();

    private void OnEnable()
    {
        if (!AllPawnManagers.Contains(this))
        {
            AllPawnManagers.Add(this);
        }
    }

    private void OnDisable()
    {
        if (AllPawnManagers.Contains(this))
        {
            AllPawnManagers.Remove(this);
        }
        // 메모리 누수를 방지하기 위해 비활성화될 때 모든 구독을 해제합니다.
        _eventHandlers.Clear();
    }

    public void RegisterSubManager(PawnSubManager subManager)
    {
        if (!pawnSubManagers.Contains(subManager))
        {
            pawnSubManagers.Add(subManager);
            // Enqueue the SubStart action instead of calling it directly.
            GameManager.Instance.LifecycleManager.EnqueueAction(subManager.SubStart);
        }
    }

    public void UnregisterSubManager(PawnSubManager subManager)
    {
        if (pawnSubManagers.Contains(subManager))
        {
            pawnSubManagers.Remove(subManager);
        }
    }

    /// <summary>
    /// 특정 이벤트 타입에 핸들러를 구독합니다.
    /// </summary>
    /// <typeparam name="TEvent">구독할 이벤트의 타입입니다.</typeparam>
    /// <param name="handler">이벤트가 발행될 때 호출될 액션입니다.</param>
    public void Subscribe<TEvent>(Action<TEvent> handler)
    {
        Type eventType = typeof(TEvent);
        if (!_eventHandlers.ContainsKey(eventType))
        {
            _eventHandlers[eventType] = new List<Delegate>();
        }
        _eventHandlers[eventType].Add(handler);
    }

    /// <summary>
    /// 특정 이벤트 타입에서 핸들러 구독을 해지합니다.
    /// </summary>
    /// <typeparam name="TEvent">구독을 해지할 이벤트의 타입입니다.</typeparam>
    /// <param name="handler">이전에 구독했던 액션입니다.</param>
    public void Unsubscribe<TEvent>(Action<TEvent> handler)
    {
        Type eventType = typeof(TEvent);
        if (_eventHandlers.ContainsKey(eventType))
        {   
            _eventHandlers[eventType].Remove(handler);
            if (_eventHandlers[eventType].Count == 0)
            {
                _eventHandlers.Remove(eventType);
            }
        }
    }

    /// <summary>
    /// 이벤트를 발행하여 해당 이벤트 타입에 구독된 모든 핸들러를 호출합니다.
    /// </summary>
    /// <typeparam name="TEvent">발행할 이벤트의 타입입니다.</typeparam>
    /// <param name="eventData">핸들러에 전달할 이벤트 데이터입니다.</param>
    public void Publish<TEvent>(TEvent eventData)
    {
        Type eventType = typeof(TEvent);
        if (_eventHandlers.ContainsKey(eventType))
        {
            // 반복 중에 핸들러가 구독을 해지할 수 있도록 복사본을 순회합니다.
            foreach (var handler in _eventHandlers[eventType].ToList()) 
            {
                (handler as Action<TEvent>)?.Invoke(eventData);
            }
        }
    }

    public void ManagedUpdate()
    {
        // Iterate backwards to allow for safe removal during the loop if needed in the future.
        for (int i = pawnSubManagers.Count - 1; i >= 0; i--)
        {
            pawnSubManagers[i].SubUpdate();
        }
    }
}


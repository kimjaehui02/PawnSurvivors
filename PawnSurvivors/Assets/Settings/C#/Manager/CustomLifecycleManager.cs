using Game.Core;
using Game.Core.Base;
using Game.Core.Contexts;
using Game.Core.Enums;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomLifecycleManager : ManagerBase
{
    #region Fields
    private readonly Queue<Action> _startQueue = new();
    private readonly Dictionary<UpdateActionTypes, Action> _updateActionMap = new();
    private readonly List<UpdateActionTypes> updatecycle = new() { UpdateActionTypes.Update };
    #endregion

    #region ManagerBase
    public override void RegisterAbilities()
    {
        Debug.Log("CustomLifecycleManager: Update 이벤트 등록");
        AddAction(GameEventType.Update, CustomLifecycle);
    }
    #endregion

    #region Start Queue
    /// <summary> _startQueue에 담긴 액션을 순차 실행 </summary>
    private void ProcessStartQueue()
    {
        if (_startQueue.Count == 0) return;

        Debug.Log($"_startQueue 처리 시작. 큐 크기: {_startQueue.Count}");
        while (_startQueue.Count > 0)
            _startQueue.Dequeue()?.Invoke();

        Debug.Log("_startQueue 처리 완료.");
    }

    /// <summary> _startQueue에 액션 추가 </summary>
    public void EnqueueStartAction(Action action)
    {
        if (action != null)
        {
            _startQueue.Enqueue(action);
            Debug.Log($"액션 추가됨. 현재 큐 크기: {_startQueue.Count}");
        }
    }
    #endregion

    #region Update Actions
    /// <summary> 지정된 타입에 액션 등록 </summary>
    public void AddUpdate(UpdateActionTypes type, Action action)
    {
        if (_updateActionMap.ContainsKey(type))
            _updateActionMap[type] += action;
        else
            _updateActionMap[type] = action;
    }

    /// <summary> 지정된 타입에서 액션 제거 </summary>
    public void RemoveUpdate(UpdateActionTypes type, Action action = null)
    {
        if (!_updateActionMap.ContainsKey(type)) return;

        if (action != null)
        {
            _updateActionMap[type] -= action;
            if (_updateActionMap[type] == null)
                _updateActionMap.Remove(type);
        }
        else
        {
            _updateActionMap.Remove(type);
        }
    }

    /// <summary> 지정된 타입의 액션 실행 </summary>
    public void RequestUpdate(UpdateActionTypes type)
    {
        if (_updateActionMap.TryGetValue(type, out Action actionDelegate))
            actionDelegate?.Invoke();
    }

    /// <summary> 타입 리스트의 액션 일괄 실행 </summary>
    public void RequestUpdates(List<UpdateActionTypes> types)
    {
        if (types == null || types.Count == 0) return;

        foreach (var item in types)
            RequestUpdate(item);
    }

    /// <summary> updatecycle에 등록된 액션 실행 </summary>
    private void Process_updateActionMap()
    {
        RequestUpdates(updatecycle);
    }
    #endregion

    #region Lifecycle
    /// <summary> Update 이벤트에서 호출됨 </summary>
    private void CustomLifecycle(GameEventContext gameEventContext)
    {
        if (gameEventContext.stopUpdate) return;

        ProcessStartQueue();
        Process_updateActionMap();
    }
    #endregion
}

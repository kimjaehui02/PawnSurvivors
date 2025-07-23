using Game.Core;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomLifecycleManager : MonoBehaviour
{
    #region
    #endregion

    #region Start

    private readonly Queue<Action> _startQueue = new();
    /// <summary>
    /// _startQueue에 담긴 모든 액션들을 순차적으로 실행하고 큐에서 제거합니다.
    /// </summary>
    private void ProcessStartQueue()
    {
        if(_startQueue.Count == 0)
        {
            //Debug.Log("_startQueue가 비어있습니다. 처리할 액션이 없습니다.");
            return; // 큐가 비어있으면 아무것도 하지 않습니다.
        }
        Debug.Log($"_startQueue 처리 시작. 현재 큐 크기: {_startQueue.Count}");

        // 큐가 비어있지 않은 동안 반복합니다.
        while (_startQueue.Count > 0)
        {
            // 1. Dequeue(): 큐의 맨 앞(가장 먼저 들어온) 요소를 꺼내고 큐에서 제거합니다.
            Action currentAction = _startQueue.Dequeue();

            // 2. 실행: 꺼낸 Action 델리게이트를 호출합니다. (null 체크는 안전을 위해)
            currentAction?.Invoke();

            //Debug.Log($"액션 실행 완료. 남은 큐 크기: {_startQueue.Count}");
        }

        Debug.Log("_startQueue 처리 완료. 큐가 비었습니다.");
    }

    /// <summary>
    /// 외부에서 _startQueue에 액션을 추가할 때 사용하는 메서드.
    /// </summary>
    /// <param name="action">큐에 추가할 매개변수 없는 메서드.</param>
    public void EnqueueStartAction(Action action)
    {
        if (action != null)
        {
            _startQueue.Enqueue(action);
            Debug.Log($"새로운 액션이 _startQueue에 추가됨. 현재 큐 크기: {_startQueue.Count}");
        }
    }

    #endregion 

    #region Update
    private readonly Dictionary<UpdateActionTypes, Action> _updateActionMap = new();

    #region 딕셔너리 관리 함수들


    /// <summary>
    /// 지정된 Acts에 대한 Action 델리게이트를 맵에 추가하거나 기존 델리게이트에 연결합니다.
    /// 하나의 Acts에 여러 함수를 연결할 수 있도록 멀티캐스트 델리게이트를 지원합니다.
    /// </summary>
    public void AddUpdate(UpdateActionTypes type, Action action)
    {
        if (_updateActionMap.ContainsKey(type))
        {
            _updateActionMap[type] += action;
        }
        else
        {
            _updateActionMap.Add(type, action);
        }
    }

    /// <summary>
    /// 지정된 Acts에서 특정 Action 델리게이트를 제거하거나, 해당 Acts에 연결된 모든 델리게이트를 제거합니다.
    /// </summary>
    public void RemoveUpdate(UpdateActionTypes type, Action action = null)
    {
        if (!_updateActionMap.ContainsKey(type))
        {
            return; // 맵에 해당 Acts가 없으면 아무것도 하지 않습니다.
        }

        if (action != null)
        {
            _updateActionMap[type] -= action; // 특정 액션 제거
            // 해당 Acts에 더 이상 연결된 델리게이트가 없으면, 맵에서 Acts를 완전히 제거합니다.
            if (_updateActionMap[type] == null)
            {
                _updateActionMap.Remove(type);
            }
        }
        else
        {
            _updateActionMap.Remove(type); // 해당 Acts와 연결된 모든 델리게이트 제거
        }
    }

    /// <summary>
    /// 지정된 Acts에 등록된 모든 델리게이트 함수들을 주어진 AbilityContext와 함께 실행합니다.
    /// </summary>
    public void RequestUpdate(UpdateActionTypes type)
    {
        // 맵에서 해당 Acts에 연결된 델리게이트를 안전하게 가져옵니다.
        if (_updateActionMap.TryGetValue(type, out Action actionDelegate))
        {
            actionDelegate?.Invoke(); // 델리게이트가 null이 아니면 호출
        }
        // else { Debug.LogWarning($"[{name}] RequestAction: Acts.{act}에 등록된 델리게이트가 없습니다."); }
        count = actionDelegate.GetInvocationList().Length; // 현재 연결된 델리게이트의 개수를 count에 저장합니다.

    }

    /// <summary>
    /// 지정된 Acts에 등록된 모든 델리게이트 함수들을 주어진 AbilityContext와 함께 실행합니다.
    /// </summary>
    public void RequestUpdates(List<UpdateActionTypes> types)
    {
        // 입력 리스트의 유효성 검사를 추가하면 더욱 견고해집니다.
        if (types == null || types.Count == 0)
        {
            // Debug.LogWarning("RequestActions: 실행할 Acts 리스트가 비어있거나 null입니다.");
            return;
        }

        foreach (var item in types) // 'item' 대신 'act' 또는 'currentAct'로 변수명을 명확히 하면 더 좋습니다.
        {
            // 단일 Acts를 처리하는 기존 RequestAction 메서드를 재사용합니다.
            RequestUpdate(item);
        }
        // 주석 처리된 else 문은 RequestAction 메서드 내부에 이미 있으므로 여기에 필요 없습니다.
        // 이는 각 개별 Acts에 대한 경고를 RequestAction에서 이미 처리하기 때문입니다.
    }

    #endregion

    // 기존 코드:
    // const List<UpdateActionType> updatecycle = { UpdateActionType.Update };

    // 수정된 코드:
    private readonly List<UpdateActionTypes> updatecycle = new()
    { 
        UpdateActionTypes.Update, 
    };

    public int count = 0;
    public bool _stopUpdate = false;
    private void Process_updateActionMap()
    {
        if(_stopUpdate)
        {
            return;
        }

        RequestUpdates(updatecycle);
        //count = _updateActionMap.Count;
    }

    #endregion



    private void CustomLifecycle()
    {
        // 업데이트 문 전에 예약된 스타트문 전부 돌림
        ProcessStartQueue();
        // 업데이트문을 실행함
        Process_updateActionMap();




    }

    private void Update()
    {
        CustomLifecycle();
    }


}

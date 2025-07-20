using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomLifecycleManager : MonoBehaviour
{
    #region Start

    private Queue<Action> OnStartQueue = new Queue<Action>();
    /// <summary>
    /// OnStartQueue에 담긴 모든 액션들을 순차적으로 실행하고 큐에서 제거합니다.
    /// </summary>
    private void ProcessOnStartQueue()
    {
        //Debug.Log($"OnStartQueue 처리 시작. 현재 큐 크기: {OnStartQueue.Count}");

        // 큐가 비어있지 않은 동안 반복합니다.
        while (OnStartQueue.Count > 0)
        {
            // 1. Dequeue(): 큐의 맨 앞(가장 먼저 들어온) 요소를 꺼내고 큐에서 제거합니다.
            Action currentAction = OnStartQueue.Dequeue();

            // 2. 실행: 꺼낸 Action 델리게이트를 호출합니다. (null 체크는 안전을 위해)
            currentAction?.Invoke();

            //Debug.Log($"액션 실행 완료. 남은 큐 크기: {OnStartQueue.Count}");
        }

        //Debug.Log("OnStartQueue 처리 완료. 큐가 비었습니다.");
    }

    /// <summary>
    /// 외부에서 OnStartQueue에 액션을 추가할 때 사용하는 메서드.
    /// </summary>
    /// <param name="action">큐에 추가할 매개변수 없는 메서드.</param>
    public void EnqueueForOnStart(Action action)
    {
        if (action != null)
        {
            OnStartQueue.Enqueue(action);
            Debug.Log($"새로운 액션이 OnStartQueue에 추가됨. 현재 큐 크기: {OnStartQueue.Count}");
        }
    }

    #endregion 

    #region Update
    public List<Action> OnUpdateActions = new List<Action>();
    private void ProcessOnUpdateActions()
    {

        for (int i = 0; i < OnUpdateActions.Count; i++)
        {
            OnUpdateActions[i]?.Invoke();
            // 만약 여기서 제거 로직이 있다면 i-- 로 인덱스 조정 필요
        }
    }

    #endregion



    private void CustomLifecycle()
    {
        // 업데이트 문 전에 예약된 스타트문 전부 돌림
        ProcessOnStartQueue();
        // 업데이트문을 실행함
        ProcessOnUpdateActions();




    }

    private void Update()
    {
        CustomLifecycle();
    }


}

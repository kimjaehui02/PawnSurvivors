using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 게임 내 개체(Pawn)의 핵심 허브 역할을 하는 중앙 관리자 클래스입니다.
/// Pawn의 생명주기 동안 하위 시스템(SubManager)들을 조율하고,
/// 제네릭 이벤트 버스 시스템을 통해 컴포넌트 간의 유연한 통신을 담당합니다.
/// </summary>
public class PawnManager : MonoBehaviour
{
    #region 필드 (Fields)

    /// <summary>
    /// 다양한 이벤트 타입에 대한 핸들러를 저장하는 딕셔너리입니다.
    /// Key: 이벤트 타입 (예: typeof(AttackInputEvent))
    /// Value: 해당 이벤트에 구독된 델리게이트 목록
    /// </summary>
    private Dictionary<Type, List<Delegate>> _eventHandlers = new();

    /// <summary>
    /// 이 Pawn에 등록된 모든 하위 관리자(SubManager)의 목록입니다.
    /// </summary>
    public List<PawnSubManager> pawnSubManagers = new();

    /// <summary>
    /// 현재 활성화된 모든 PawnManager 인스턴스를 추적하는 정적 목록입니다.
    /// </summary>
    public static readonly List<PawnManager> AllPawnManagers = new();

    #endregion

    #region Unity 생명주기 (Lifecycle)

    private void OnEnable()
    {
        // 활성화 시 자기 자신을 전체 목록에 추가합니다.
        if (!AllPawnManagers.Contains(this))
        {
            AllPawnManagers.Add(this);
        }
    }

    private void OnDisable()
    {
        // 비활성화 시 전체 목록에서 자기 자신을 제거합니다.
        if (AllPawnManagers.Contains(this))
        {
            AllPawnManagers.Remove(this);
        }
        
        // 메모리 누수를 방지하기 위해 모든 이벤트 구독을 해제합니다.
        _eventHandlers.Clear();
    }

    #endregion

    #region 하위 관리자 (Sub-Manager) 관리

    /// <summary>
    /// 새로운 하위 관리자를 이 Pawn에 등록합니다.
    /// </summary>
    /// <param name="subManager">등록할 하위 관리자 인스턴스입니다.</param>
    public void RegisterSubManager(PawnSubManager subManager)
    {
        if (!pawnSubManagers.Contains(subManager))
        {
            pawnSubManagers.Add(subManager);
            // SubManager의 초기화 로직(SubStart)을 GameManager의 생명주기 관리자에게 위임하여 실행 순서를 보장합니다.
            GameManager.Instance.LifecycleManager.EnqueueAction(subManager.SubStart);
        }
    }

    /// <summary>
    /// 이 Pawn에서 하위 관리자를 등록 해제합니다.
    /// </summary>
    /// <param name="subManager">등록 해제할 하위 관리자 인스턴스입니다.</param>
    public void UnregisterSubManager(PawnSubManager subManager)
    {
        if (pawnSubManagers.Contains(subManager))
        {
            pawnSubManagers.Remove(subManager);
        }
    }

    /// <summary>
    /// 등록된 모든 하위 관리자들의 Update 로직을 실행합니다.
    /// GameManager에 의해 관리되는 커스텀 Update 루프입니다.
    /// </summary>
    public void ManagedUpdate()
    {
        // 향후 반복 중 안전한 제거를 허용하기 위해 역순으로 반복합니다.
        for (int i = pawnSubManagers.Count - 1; i >= 0; i--)
        {
            pawnSubManagers[i].SubUpdate();
        }
    }

    #endregion

    #region 제네릭 이벤트 버스 (Generic Event Bus)

    /// <summary>
    /// 특정 이벤트 타입에 핸들러(콜백 함수)를 구독합니다.
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
    /// 이벤트를 발행하여 해당 타입에 구독된 모든 핸들러를 호출합니다.
    /// </summary>
    /// <typeparam name="TEvent">발행할 이벤트의 타입입니다.</typeparam>
    /// <param name="eventData">핸들러에 전달할 이벤트 데이터입니다.</param>
    public void Publish<TEvent>(TEvent eventData)
    {
        Type eventType = typeof(TEvent);
        if (_eventHandlers.ContainsKey(eventType))
        {
            // 반복 중에 핸들러가 구독을 해지하는 경우를 대비하여, 핸들러 목록의 복사본을 만들어 순회합니다.
            foreach (var handler in _eventHandlers[eventType].ToList()) 
            {
                (handler as Action<TEvent>)?.Invoke(eventData);
            }
        }
    }

    #endregion
}
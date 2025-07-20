using UnityEngine;
using Game.Core;
using System.Collections.Generic;
using System;

/// <summary>
/// Pawn에 부착되어 특정 능력을 제공하는 기반 컴포넌트입니다.
/// Acts에 연결된 Action 델리게이트 딕셔너리를 관리하는 기능을 구현합니다.
/// </summary>
public abstract class PawnAction : MonoBehaviour
{
    #region Fields & Properties
    // 새롭게 추가할 우선순위 필드
    // 숫자가 낮을수록(예: 0, 1, 2...) 높은 우선순위를 가지도록 설계하는 것이 일반적입니다.
    [Tooltip("이 PawnAction의 등록 우선순위입니다. 숫자가 낮을수록 먼저 처리됩니다.")]
    public Game.Core.PawnActionPriority PawnActionPriority = 0; // 기본값 0

    // 이 PawnAction 컴포넌트에 등록된 Acts-Action 델리게이트 맵입니다.
    // Pawn이 이 딕셔너리의 내용을 통합하여 관리합니다.
    public readonly Dictionary<Acts, Action<AbilityContext>> _myDelegates = new();

    /// <summary>
    /// 내부 델리게이트 맵(_myDelegates)에 대한 읽기 전용 뷰를 제공합니다.
    /// 외부에서는 이 맵의 내용을 읽을 수만 있고 변경할 수는 없습니다.
    /// </summary>
    public IReadOnlyDictionary<Acts, Action<AbilityContext>> GetActions => _myDelegates;

    #endregion

    #region Public Methods

    /// <summary>
    /// 지정된 Acts에 대한 Action 델리게이트를 맵에 추가하거나 기존 델리게이트에 연결합니다.
    /// 하나의 Acts에 여러 함수를 연결할 수 있도록 멀티캐스트 델리게이트를 지원합니다.
    /// </summary>
    /// <param name="act">등록할 Acts Enum 값.</param>
    /// <param name="action">해당 Acts에 연결할 Action 델리게이트.</param>
    public void AddAction(Acts act, Action<AbilityContext> action)
    {
        if (_myDelegates.ContainsKey(act))
        {
            _myDelegates[act] += action;
        }
        else
        {
            _myDelegates.Add(act, action);
        }
    }

    /// <summary>
    /// 지정된 Acts에서 특정 Action 델리게이트를 제거하거나, 해당 Acts에 연결된 모든 델리게이트를 제거합니다.
    /// </summary>
    /// <param name="act">제거할 대상 Acts Enum 값.</param>
    /// <param name="action">제거할 특정 Action 델리게이트. null이면 해당 Acts의 모든 델리게이트를 제거합니다.</param>
    public void RemoveAction(Acts act, Action<AbilityContext> action = null)
    {
        if (!_myDelegates.ContainsKey(act))
        {
            return; // 맵에 해당 Acts가 없으면 아무것도 하지 않습니다.
        }

        if (action != null)
        {
            _myDelegates[act] -= action; // 특정 액션 제거
            // 해당 Acts에 더 이상 연결된 델리게이트가 없으면, 맵에서 Acts를 완전히 제거합니다.
            if (_myDelegates[act] == null)
            {
                _myDelegates.Remove(act);
            }
        }
        else
        {
            _myDelegates.Remove(act); // 해당 Acts와 연결된 모든 델리게이트 제거
        }
    }

    /// <summary>
    /// 지정된 Acts에 등록된 모든 델리게이트 함수들을 주어진 AbilityContext와 함께 실행합니다.
    /// </summary>
    /// <param name="act">실행할 Acts Enum 값.</param>
    /// <param name="context">행동에 필요한 정보를 담은 컨텍스트.</param>
    public void RequestAction(Acts act, AbilityContext context)
    {
        // 맵에서 해당 Acts에 연결된 델리게이트를 안전하게 가져옵니다.
        if (_myDelegates.TryGetValue(act, out Action<AbilityContext> actionDelegate))
        {
            actionDelegate?.Invoke(context); // 델리게이트가 null이 아니면 호출
        }
        // else { Debug.LogWarning($"[{name}] RequestAction: Acts.{act}에 등록된 델리게이트가 없습니다."); }
    }

    #endregion

    #region Abstract Methods

    /// <summary>
    /// 이 PawnAction을 상속받는 자식 클래스들이 반드시 구현하여
    /// 자신의 고유한 능력 델리게이트들을 AddAction 메서드를 통해 등록해야 합니다.
    /// 이 메서드는 PawnAwake 또는 RegisterAbilities 등 초기화 시점에 호출됩니다.
    /// </summary>
    public abstract void RegisterAbilities();

    #endregion
}
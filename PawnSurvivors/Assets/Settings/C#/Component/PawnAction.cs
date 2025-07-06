using UnityEngine;
using Game.Core;
using System.Collections.Generic;
using System;

/// <summary>
/// Pawn에 부착되어 특정 능력을 제공하는 기반 컴포넌트입니다.
/// Acts에 연결된 Action 델리게이트 딕셔너리를 관리하는 기능을 구현합니다.
/// </summary>
public abstract class PawnAction : MonoBehaviour//, IActionMapManager // IActionMapManager 인터페이스 구현
{
    // 실제 Acts-Action 델리게이트 딕셔너리 인스턴스입니다.
    // PawnAbility가 직접 관리하며, 한 번 초기화되면 변경되지 않습니다 (readonly).
    private readonly Dictionary<Acts, Action<AbilityContext>> _myDelegates = new();

    /// <summary>
    /// IActionMapManager 인터페이스의 GetActions 프로퍼티를 구현합니다.
    /// 이 프로퍼티는 내부 딕셔너리(_myDelegates)에 대한 읽기 전용 뷰를 제공합니다.
    /// </summary>
    public IReadOnlyDictionary<Acts, Action<AbilityContext>> GetActions => _myDelegates;

    /// <summary>
    /// IActionMapManager 인터페이스의 AddAction 메서드를 구현합니다.
    /// 지정된 Acts에 대한 Action 델리게이트를 딕셔너리에 추가하거나 연결합니다.
    /// </summary>
    /// <param name="act">등록할 Acts Enum 값.</param>
    /// <param name="action">해당 Acts에 연결할 Action 델리게이트.</param>
    public void AddAction(Acts act, Action<AbilityContext> action)
    {
        if (_myDelegates.ContainsKey(act))
        {
            // 이미 같은 Acts에 등록된 델리게이트가 있다면, 새로운 액션을 기존 델리게이트에 '추가'합니다.
            // 이렇게 하면 하나의 Acts에 여러 함수를 연결할 수 있습니다 (멀티캐스트 델리게이트).
            _myDelegates[act] += action;
        }
        else
        {
            // 해당 Acts가 딕셔너리에 없다면 새로 추가합니다.
            _myDelegates.Add(act, action);
        }
    }

    /// <summary>
    /// IActionMapManager 인터페이스의 RemoveAction 메서드를 구현합니다.
    /// 지정된 Acts에 대한 특정 Action 델리게이트를 제거하거나, 해당 Acts에 연결된 모든 델리게이트를 제거합니다.
    /// </summary>
    /// <param name="act">제거할 대상 Acts Enum 값.</param>
    /// <param name="action">제거할 특정 Action 델리게이트. null이면 해당 Acts의 모든 델리게이트를 제거합니다.</param>
    public void RemoveAction(Acts act, Action<AbilityContext> action = null)
    {
        // 해당 Acts가 딕셔너리에 없으면 아무것도 하지 않습니다.
        if (!_myDelegates.ContainsKey(act))
        {
            return;
        }

        if (action != null)
        {
            // 특정 액션만 제거합니다.
            _myDelegates[act] -= action;
            // 만약 해당 Acts에 더 이상 연결된 델리게이트가 없다면, 딕셔너리에서 Acts를 완전히 제거합니다.
            if (_myDelegates[act] == null)
            {
                _myDelegates.Remove(act);
            }
        }
        else
        {
            // 해당 Acts와 연결된 모든 델리게이트를 제거합니다.
            _myDelegates.Remove(act);
        }
    }

    /// <summary>
    /// 이 PawnAbility를 상속받는 자식 클래스들이 반드시 구현하여
    /// 자신의 고유한 능력 델리게이트들을 등록해야 하는 추상 메서드입니다.
    /// 이 메서드 안에서 AddAction을 호출하여 델리게이트를 채웁니다.
    /// </summary>
    public abstract void RegisterAbilities();
}
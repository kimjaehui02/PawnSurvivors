

using Game.Core;
using System;
using System.Collections.Generic;

public class EnumDelegateMap<TEnum, TContext> where TEnum : Enum
{
    #region Fields & Properties
    //[field: SerializeField] // 유니티 인스펙터에서 설정 가능하도록
    //public PawnActionPriority PawnActionPriority { get; private set; } = PawnActionPriority.Normal; // 👈 이 필드 추가

    // 이 PawnAction 컴포넌트에 등록된 TEnum-Action 델리게이트 맵입니다.
    // Pawn이 이 딕셔너리의 내용을 통합하여 관리합니다.
    private readonly Dictionary<TEnum, Action<TContext>> _myDelegatesMap = new();

    /// <summary>
    /// 내부 델리게이트 맵(_myDelegatesMap)에 대한 읽기 전용 뷰를 제공합니다.
    /// 외부에서는 이 맵의 내용을 읽을 수만 있고 변경할 수는 없습니다.
    /// </summary>
    public IReadOnlyDictionary<TEnum, Action<TContext>> GetActions => _myDelegatesMap;

    #endregion

    #region Public Methods

    /// <summary>
    /// 지정된 TEnum에 대한 Action 델리게이트를 맵에 추가하거나 기존 델리게이트에 연결합니다.
    /// 하나의 TEnum에 여러 함수를 연결할 수 있도록 멀티캐스트 델리게이트를 지원합니다.
    /// </summary>
    /// <param name="act">등록할 TEnum Enum 값.</param>
    /// <param name="action">해당 TEnum에 연결할 Action 델리게이트.</param>
    public void AddAction(TEnum act, Action<TContext> action)
    {
        if (_myDelegatesMap.ContainsKey(act))
        {
            _myDelegatesMap[act] += action;
        }
        else
        {
            _myDelegatesMap.Add(act, action);
        }
    }

    /// <summary>
    /// 지정된 TEnum에서 특정 Action 델리게이트를 제거하거나, 해당 TEnum에 연결된 모든 델리게이트를 제거합니다.
    /// </summary>
    /// <param name="act">제거할 대상 TEnum Enum 값.</param>
    /// <param name="action">제거할 특정 Action 델리게이트. null이면 해당 TEnum의 모든 델리게이트를 제거합니다.</param>
    public void RemoveAction(TEnum act, Action<TContext> action = null)
    {
        if (!_myDelegatesMap.ContainsKey(act))
        {
            return; // 맵에 해당 TEnum가 없으면 아무것도 하지 않습니다.
        }

        if (action != null)
        {
            _myDelegatesMap[act] -= action; // 특정 액션 제거
            // 해당 TEnum에 더 이상 연결된 델리게이트가 없으면, 맵에서 TEnum를 완전히 제거합니다.
            if (_myDelegatesMap[act] == null)
            {
                _myDelegatesMap.Remove(act);
            }
        }
        else
        {
            _myDelegatesMap.Remove(act); // 해당 TEnum와 연결된 모든 델리게이트 제거
        }
    }

    /// <summary>
    /// 지정된 TEnum에 등록된 모든 델리게이트 함수들을 주어진 TContext와 함께 실행합니다.
    /// </summary>
    /// <param name="act">실행할 TEnum Enum 값.</param>
    /// <param name="context">행동에 필요한 정보를 담은 컨텍스트.</param>
    public void RequestAction(TEnum act, TContext context)
    {
        // 맵에서 해당 TEnum에 연결된 델리게이트를 안전하게 가져옵니다.
        if (_myDelegatesMap.TryGetValue(act, out Action<TContext> actionDelegate))
        {
            actionDelegate?.Invoke(context); // 델리게이트가 null이 아니면 호출
        }

    }

    /// <summary>
    /// 지정된 TEnum에 등록된 모든 델리게이트 함수들을 주어진 TContext와 함께 실행합니다.
    /// </summary>
    /// <param name="TEnum">실행할 TEnum Enum 값들의 리스트.</param>
    /// <param name="context">행동에 필요한 정보를 담은 컨텍스트.</param>
    public void RequestActions(List<TEnum> TEnum, TContext context)
    {
        // 입력 리스트의 유효성 검사를 추가하면 더욱 견고해집니다.
        if (TEnum == null || TEnum.Count == 0)
        {

            return;
        }

        foreach (var item in TEnum) // 'item' 대신 'act' 또는 'currentAct'로 변수명을 명확히 하면 더 좋습니다.
        {
            // 단일 TEnum를 처리하는 기존 RequestAction 메서드를 재사용합니다.
            RequestAction(item, context);
        }
        // 주석 처리된 else 문은 RequestAction 메서드 내부에 이미 있으므로 여기에 필요 없습니다.
        // 이는 각 개별 TEnum에 대한 경고를 RequestAction에서 이미 처리하기 때문입니다.
    }

    #endregion

    #region Abstract Methods

    /// <summary>
    /// 이 PawnAction을 상속받는 자식 클래스들이 반드시 구현하여
    /// 자신의 고유한 능력 델리게이트들을 AddAction 메서드를 통해 등록해야 합니다.
    /// 이 메서드는 PawnAwake 또는 RegisterAbilities 등 초기화 시점에 호출됩니다.
    /// </summary>
    public virtual void RegisterAbilities() 
    {

    }

    #endregion
}

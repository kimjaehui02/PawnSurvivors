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
    public BaseConfig baseConfig; // PawnAction이 사용하는 설정을 담는 BaseConfig
    public void SetBaseConfig(BaseConfig config) => baseConfig = config;


    private readonly EnumDelegateMap<Acts, AbilityContext> _delegateMap = new();

    //private Dictionary<Acts, Action<AbilityContext>> MyDelegates => _delegateMap._myDelegatesMap;

    public IReadOnlyDictionary<Acts, Action<AbilityContext>> GetActions => _delegateMap.GetActions;

    public void AddAction(Acts act, Action<AbilityContext> action) => _delegateMap.AddAction(act, action);
    public void RemoveAction(Acts act, Action<AbilityContext> action = null) => _delegateMap.RemoveAction(act, action);

    public void RequestAction(Acts act, AbilityContext context) => _delegateMap.RequestAction(act, context);

    public void RequestActions(List<Acts> acts, AbilityContext context) => _delegateMap.RequestActions(acts, context);

    public virtual void RegisterAbilities() => _delegateMap.RegisterAbilities();
}

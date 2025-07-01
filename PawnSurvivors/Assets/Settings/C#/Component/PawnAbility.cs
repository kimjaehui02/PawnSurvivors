using UnityEngine;
using Game.Core;
using System.Collections.Generic;
using System;

public class PawnAbility : MonoBehaviour
{
    // 델리게이트 딕셔너리를 외부에 노출 (PawnManager 등이 접근하여 연결)
    private readonly Dictionary<Acts, Action> _actionDelegates = new Dictionary<Acts, Action>();

    public Dictionary<Acts, Action> GetActionDelegates()
    {
        return _actionDelegates;
    }

    // 자식 클래스들이 반드시 구현하여 자신의 능력을 등록하도록 추상 메서드로 정의
    public virtual void RegisterAbilities() { }
}

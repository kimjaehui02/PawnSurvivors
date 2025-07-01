using Game.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
// CommonEnums.cs 또는 GameActs.cs (새로운 파일)

public class Pawn : MonoBehaviour, IPawn
{
    // 실제 Id 속성의 구현
    public string Id { get; private set; } // private set을 사용하여 내부에서만 Id를 변경할 수 있도록 함

    // 여기서 폰의 자식들은 컴포넌트를 아는게 아니라 컴포넌트들의 이벤트를 알고싶음
    // 

    //public enum Acts // 델리게이트의 키로 사용될 enum
    //{
    //    OnMove,
    //    OnDamaged
    //    // 여기에 필요한 모든 행동들을 추가할 수 있습니다.
    //}

    // 델리게이트 딕셔너리를 외부에 노출 (PawnManager 등이 접근하여 연결)
    private readonly Dictionary<Acts, Action> _actionDelegates = new Dictionary<Acts, Action>();

    public Dictionary<Acts, Action> GetActionDelegates()
    {
        return _actionDelegates;
    }


    // 생성처리를 합니다
    public virtual void PawnAwake()
    {

    }

    public virtual void PawnStart()
    {

    }

    // 지속처리를 합니다
    public virtual void PawnUpdate()
    {

    }

    // 소멸처리를 합니다
    public virtual void PawnDisable()
    {

    }

    public virtual void PawnDespawn()
    {

    }
}

using Game.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
// CommonEnums.cs 또는 GameActs.cs (새로운 파일)

public class Pawn : PawnAction//, IActionMapManager//, IPawn
{
    // 실제 Id 속성의 구현
    public string Id { get; private set; } // private set을 사용하여 내부에서만 Id를 변경할 수 있도록 함

    public AbilityContext AbilityContext { get; private set; }


    public override void RegisterAbilities()
    {
        throw new NotImplementedException();
    }


    // 델리게이트 딕셔너리를 외부에 노출 (PawnManager 등이 접근하여 연결)

    #region 라이프사이클

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


    #endregion

}

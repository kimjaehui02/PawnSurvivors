using UnityEngine;

public class Pawn : MonoBehaviour, IPawn
{
    // 실제 Id 속성의 구현
    public string Id { get; private set; } // private set을 사용하여 내부에서만 Id를 변경할 수 있도록 함

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

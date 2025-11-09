using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackInputSubManager : PawnSubManager
{
    public override void SubStart()
    {
        // 이 하위 관리자에는 특정 초기화가 필요하지 않습니다.
    }

    public override void SubUpdate()
    {
        // 마우스 왼쪽 버튼을 클릭하면 공격 입력 이벤트를 게시합니다.
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            _pawnManager.Publish(new AttackInputEvent(this.gameObject));
        }
    }
}

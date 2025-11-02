using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackInputSubManager : PawnSubManager
{
    public override void SubStart()
    {
        // No specific initialization needed for this sub-manager.
    }

    public override void SubUpdate()
    {
        // On left mouse button click, publish an attack input event.
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            _pawnManager.Publish(new AttackInputEvent(this.gameObject));
        }
    }
}

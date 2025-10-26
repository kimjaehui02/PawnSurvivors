using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackInputSubManager : PawnSubManager
{
    private IAttackable _attackHandler;

    public override void SubStart()
    {
        // Find the attack implementation on the same GameObject.
        _attackHandler = GetComponent<IAttackable>();
        if (_attackHandler == null)
        {
            Debug.LogWarning("PlayerAttackInputSubManager requires a component that implements IAttackable on the same GameObject.", this);
            // Disable self if no attack handler is found.
            this.enabled = false;
        }
    }

    public override void SubUpdate()
    {
        // Trigger attack on left mouse button click.
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            _attackHandler.Attack();
        }
    }
}

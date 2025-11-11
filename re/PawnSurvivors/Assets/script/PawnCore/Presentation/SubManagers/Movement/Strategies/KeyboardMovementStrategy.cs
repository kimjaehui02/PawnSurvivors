using UnityEngine;
using UnityEngine.InputSystem;
using PawnCore.Domain;

public class KeyboardMovementStrategy : MovementStrategyBase
{
    private PawnData _pawnData;

    public override void Init(PawnManager pawnManager)
    {
        base.Init(pawnManager);
        _pawnData = pawnManager.PawnData;
        
        // MovableData가 없으면 생성
        if (_pawnData.movableData == null)
        {
            _pawnData.movableData = new PawnCore.Domain.MovableData();
        }
    }

    public override void Move()
    {
        if (_pawnManager == null) return;
        if (_pawnData?.movableData == null) return;

        Vector2 inputDirection = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) inputDirection.y += 1;
            if (Keyboard.current.sKey.isPressed) inputDirection.y -= 1;
            if (Keyboard.current.aKey.isPressed) inputDirection.x -= 1;
            if (Keyboard.current.dKey.isPressed) inputDirection.x += 1;
        }

        MovementUsecases.MoveWithInput(_pawnManager.transform, inputDirection, _pawnData);
    }
}

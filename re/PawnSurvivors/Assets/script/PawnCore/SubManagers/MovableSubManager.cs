using UnityEngine;

public class MovableSubManager : PawnSubManager
{
    public float moveSpeed = 5f;

    public override void SubStart()
    {
        // Initialization logic for movable, if any
    }

    public override void SubUpdate()
    {
        MovementUsecases.Move(gameObject, moveSpeed);
    }
}

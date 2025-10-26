using UnityEngine;

public interface IMovementStrategy
{
    /// <summary>
    /// Executes the movement logic for the Pawn.
    /// </summary>
    /// <param name="pawnManager">The PawnManager instance controlling the Pawn.</param>
    /// <param name="deltaTime">The time elapsed since the last frame.</param>
    void Move(PawnManager pawnManager, float deltaTime);
}

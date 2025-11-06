// using UnityEngine;

// [CreateAssetMenu(fileName = "TargetStrategySetup", menuName = "Pawn/Recipe/Setup/Movement/Target")]
// public class TargetStrategySetup : MovementStrategySetup
// {
//     public float speed = 5f;
//     // Note: The actual target Transform is set on the MovableSubManager at runtime, not here.

//     public override void AddAndConfigure(GameObject pawnObject)
//     {
//         var strategy = pawnObject.AddComponent<TargetMovementStrategy>();
//         strategy.speed = speed;
//         strategy.enabled = isEnabledByDefault;
//     }
// }
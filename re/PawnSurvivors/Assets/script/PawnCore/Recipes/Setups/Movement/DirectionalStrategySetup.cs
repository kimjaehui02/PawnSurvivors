// using UnityEngine;

// [CreateAssetMenu(fileName = "DirectionalStrategySetup", menuName = "Pawn/Recipe/Setup/Movement/Directional")]
// public class DirectionalStrategySetup : MovementStrategySetup
// {
//     public float speed = 20f;
//     public float lifetime = 5f;
//     public Vector3 moveDirection;

//     public override void AddAndConfigure(GameObject pawnObject)
//     {
//         var strategy = pawnObject.AddComponent<DirectionalMovementStrategy>();
//         strategy.speed = speed;
//         strategy.lifetime = lifetime;
//         strategy.moveDirection = moveDirection;
//         strategy.enabled = isEnabledByDefault;
//     }
// }
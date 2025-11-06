// using UnityEngine;

// [CreateAssetMenu(fileName = "HomingStrategySetup", menuName = "Pawn/Recipe/Setup/Movement/Homing")]
// public class HomingStrategySetup : MovementStrategySetup
// {
//     public float speed = 5f;
//     public string targetTag = "Enemy";
//     public float detectionRange = 10f;
//     public float retargetFrequency = 0.5f;

//     public override void AddAndConfigure(GameObject pawnObject)
//     {
//         var strategy = pawnObject.AddComponent<HomingMovementStrategy>();
//         strategy.speed = speed;
//         strategy.targetTag = targetTag;
//         strategy.detectionRange = detectionRange;
//         strategy.retargetFrequency = retargetFrequency;
//         strategy.enabled = isEnabledByDefault;
//     }
// }
// using UnityEngine;

// /// <summary>
// /// An abstract ScriptableObject that defines the contract for adding and configuring a MovementStrategy.
// /// </summary>
// public abstract class MovementStrategySetup : ScriptableObject
// {
//     [Tooltip("If true, this strategy component will be enabled by default. Only one strategy should be enabled.")]
//     [SerializeField] public bool isEnabledByDefault = false;

//     /// <summary>
//     /// Adds and configures the specific MovementStrategy on the given GameObject.
//     /// </summary>
//     /// <param name="pawnObject">The GameObject to which the strategy will be added.</param>
//     public abstract void AddAndConfigure(GameObject pawnObject);
// }
using UnityEngine;

public class CreationManager : MonoBehaviour
{
    /// <summary>
    /// Instantiates a Pawn prefab and ensures it's integrated into the lifecycle system.
    /// </summary>
    /// <param name="prefab">The pawn prefab to instantiate.</param>
    /// <param name="position">The position to spawn the pawn at.</param>
    /// <param name="rotation">The rotation of the spawned pawn.</param>
    /// <returns>The created GameObject.</returns>
    public GameObject CreatePawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogError("CreationManager: Prefab to instantiate is null.");
            return null;
        }

        return Instantiate(prefab, position, rotation);
    }
}

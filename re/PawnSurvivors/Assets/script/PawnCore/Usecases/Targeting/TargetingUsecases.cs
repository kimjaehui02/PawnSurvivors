using UnityEngine;
using System.Linq;

public static class TargetingUsecases
{
    /// <summary>
    /// Finds the nearest GameObject with a specific tag within a given range.
    /// </summary>
    /// <param name="currentPosition">The position from which to search.</param>
    /// <param name="tag">The tag of the target GameObjects (e.g., "Enemy").</param>
    /// <param name="range">The maximum distance to search for targets.</param>
    /// <returns>The Transform of the nearest target, or null if none found.</returns>
    public static Transform FindNearestTargetWithTag(Vector3 currentPosition, string tag, float range)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);

        if (targets == null || targets.Length == 0)
        {
            return null;
        }

        Transform nearestTarget = null;
        float minDistanceSqr = range * range; // Use squared distance for performance

        foreach (GameObject target in targets)
        {
            float distanceSqr = (target.transform.position - currentPosition).sqrMagnitude;
            if (distanceSqr < minDistanceSqr)
            {
                minDistanceSqr = distanceSqr;
                nearestTarget = target.transform;
            }
        }

        return nearestTarget;
    }
}

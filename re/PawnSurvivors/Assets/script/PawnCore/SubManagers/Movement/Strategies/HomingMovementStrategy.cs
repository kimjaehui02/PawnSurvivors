using UnityEngine;
using System.Linq; // For potential future use, though not strictly needed for current FindNearestTargetWithTag

public class HomingMovementStrategy : MovementStrategyBase
{
    [Header("Homing Settings")]
    public float speed = 5f;
    public string targetTag = "Enemy"; // Tag of the target to home towards
    public float detectionRange = 10f; // How far to look for targets
    public float retargetFrequency = 0.5f; // How often to re-acquire target (seconds)

    private Transform _currentTarget;
    private float _nextRetargetTime;

    public override void Move()
    {
        if (_pawnManager == null) return;

        // Re-acquire target if needed
        if (Time.time >= _nextRetargetTime)
        {
            _nextRetargetTime = Time.time + retargetFrequency;
            _currentTarget = TargetingUsecases.FindNearestTargetWithTag(_pawnManager.transform.position, targetTag, detectionRange);
        }

        // If a target is found, move towards it
        if (_currentTarget != null)
        {
            Vector3 direction = (_currentTarget.position - _pawnManager.transform.position);
            MovementUsecases.MoveInDirection(_pawnManager.transform, direction, speed);
        }
        else
        {
            // Optional: What to do if no target is found (e.g., continue last direction, stop, patrol)
            // For now, it just stops if no target. 
        }
    }
}

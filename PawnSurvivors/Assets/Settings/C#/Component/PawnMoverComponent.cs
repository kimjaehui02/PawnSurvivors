using UnityEngine;

public class PawnMoverComponent : MonoBehaviour, IPawnMover
{

    // 배회 기능은 없으므로, Vector3.zero 반환
    public Vector3 GetWanderDirection()
    {
        return Vector3.zero; // 이 AI는 배회하지 않습니다.
    }

    // 플레이어 추적 기능 구현 (GetFaceDirection 역할)
    public Vector3 GetFaceDirection(Vector3 targetPosition)
    {

        // 플레이어 (혹은 주어진 targetPosition)를 향하는 방향 계산
        Vector3 directionToTarget = targetPosition - transform.position;
        if (directionToTarget.magnitude > 0)
        {
            return directionToTarget.normalized;
        }
        return Vector3.zero;

    }
}

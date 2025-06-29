using UnityEngine;

/// <summary>
/// 자체적인 AI 규칙과 로직에 따라 이동 방향을 결정하고 MoveableComponent에 전달하는 책임을 가집니다.
/// Update 외의 주기적인 로직(코루틴 등)을 포함할 수 있습니다.
/// </summary>
public interface IPawnMover
{
    /// <summary>
    /// AI의 현재 상태와 규칙에 기반한 '배회(Wander)' 이동 방향 벡터를 반환합니다.
    /// 이 메서드는 AI 로직의 업데이트 주기(Update, 코루틴 등)에 맞춰 호출될 수 있습니다.
    /// </summary>
    /// <returns>AI 로직에 따른 배회 이동 방향 벡터</returns>
    Vector3 GetWanderDirection(); // 명확성을 위해 GetWander -> GetWanderDirection으로 변경 제안

    /// <summary>
    /// AI의 현재 위치에서 특정 목표 좌표를 향하는 '향하다(Face/Seek)' 이동 방향 벡터를 반환합니다.
    /// </summary>
    /// <param name="targetPosition">향할 목표 월드 좌표</param>
    /// <returns>목표를 향하는 정규화된 이동 방향 벡터</returns>
    Vector3 GetFaceDirection(Vector3 targetPosition); // 명확성을 위해 Getface -> GetFaceDirection으로 변경 제안
}
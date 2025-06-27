using UnityEngine;

/// <summary>
/// 플레이어의 입력을 받아 이동 방향을 결정하고 MoveableComponent에 전달하는 책임을 가집니다.
/// </summary>
public interface IPlayerMover
{
    /// <summary>
    /// 플레이어의 현재 입력에 기반한 이동 방향 벡터를 반환합니다.
    /// 이 메서드는 보통 Update 루프에서 호출되어야 합니다.
    /// </summary>
    /// <returns>플레이어의 입력에 따른 이동 방향 벡터</returns>
    Vector3 GetPlayerMovementInput();
}
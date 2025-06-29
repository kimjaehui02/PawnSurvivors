using UnityEngine;

/// <summary>
/// 이동할 수 있는 모든 게임 오브젝트가 구현해야 하는 인터페이스입니다.
/// </summary>
public interface IMoveable
{
    /// <summary>
    /// 현재 이동 속도를 가져옵니다. (읽기 전용)
    /// </summary>
    float MoveSpeed { get; }

    public delegate void MoveableDelegate(Vector3 direction);

    /// <summary>
    /// 지정된 방향으로 이동을 수행하는 메서드입니다.
    /// </summary>
    /// <param name="direction">이동할 방향 벡터</param>
    void Move(Vector3 direction);
}
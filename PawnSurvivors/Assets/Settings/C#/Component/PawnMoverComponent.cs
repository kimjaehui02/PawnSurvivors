using UnityEngine;
using Game.Core; // PawnAction과의 연동을 위해 Game.Core 네임스페이스 추가

/// <summary>
/// 특정 대상의 위치를 기반으로 이동 방향을 제공하는 컴포넌트입니다.
/// 주로 AI 몬스터의 추적 로직에 사용될 수 있습니다.
/// </summary>
public class PawnMoverComponent : PawnAction // PawnAction을 상속합니다.
{
    #region Ability Registration

    /// <summary>
    /// PawnAction의 RegisterAbilities를 오버라이드하여
    /// 이 컴포넌트의 능력을 Pawn의 델리게이트 시스템에 등록합니다.
    /// 현재 이 컴포넌트의 메서드(GetFaceDirection 등)는 다른 AI 로직 컴포넌트에서
    /// 직접 호출되어 AbilityContext를 업데이트하는 방식으로 사용될 수 있습니다.
    /// </summary>
    public override void RegisterAbilities()
    {
        // TODO: 만약 특정 Acts에 의해 이동 방향 계산이 자동으로 이뤄져야 한다면,
        // 예를 들어 Acts.OnAITick 시점에 GetFaceDirection을 호출하도록 등록할 수 있습니다.
        // AddAction(Acts.OnAITick, (context) => { context.inputDirection = GetFaceDirection(context.targetPosition); });
        // 위 예시를 사용하려면 AbilityContext에 'targetPosition' 필드가 필요합니다.

        // 현재는 RegisterAbilities에서 아무것도 등록하지 않고 있습니다.
        // throw new System.NotImplementedException(); // 이 줄이 더 이상 필요 없으면 제거하세요.
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 배회(Wander) 기능을 제공하지 않으므로 항상 Vector3.zero를 반환합니다.
    /// 이 AI는 독립적인 배회 행동을 수행하지 않습니다.
    /// </summary>
    /// <returns>항상 Vector3.zero (이동 없음)</returns>
    public Vector3 GetWanderDirection()
    {
        return Vector3.zero; // 이 컴포넌트는 배회 기능을 구현하지 않습니다.
    }

    /// <summary>
    /// 지정된 목표 위치를 향하는 방향 벡터를 계산하여 반환합니다.
    /// 주로 추적(Face Direction) 로직에 사용됩니다.
    /// </summary>
    /// <param name="targetPosition">추적할 목표 대상의 월드 위치입니다.</param>
    /// <returns>목표를 향하는 정규화된 방향 벡터. 목표가 현재 위치와 같으면 Vector3.zero.</returns>
    public void GetFaceDirection(AbilityContext abilityContext)
    {
        // 현재 위치에서 목표 위치를 향하는 벡터를 계산합니다.
        Vector3 directionToTarget = abilityContext.TargetPawn.transform.position - transform.position;

        // 거리가 0보다 크면 (즉, 목표가 현재 위치와 다르면) 방향을 정규화하여 반환합니다.
        if (directionToTarget.magnitude > 0)
        {
            abilityContext.InputDirection = directionToTarget.normalized;
            return;
        }

        // 목표가 현재 위치와 같거나 매우 가까워 방향을 계산할 수 없으면 이동하지 않습니다.
        abilityContext.InputDirection = Vector3.zero;
        return;
    }

    #endregion
}
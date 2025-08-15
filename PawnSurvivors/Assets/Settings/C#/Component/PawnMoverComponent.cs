using UnityEngine;
using Game.Core;
using Game.Core.Base;
using Game.Core.Enums;
using Game.Core.Contexts; // PawnAction과의 연동을 위해 Game.Core 네임스페이스 추가

/// <summary>
/// 특정 대상의 위치를 기반으로 이동 방향을 제공하는 컴포넌트입니다.
/// 주로 AI 몬스터의 추적 로직에 사용될 수 있습니다.
/// </summary>
public class PawnMoverComponent : PawnBase // PawnAction을 상속합니다.
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

        AddAction(Acts.OnUpdate, GetFaceDirection);
    }

    #endregion

    #region Public Methods


    // 이건 아직 고민중인 로직입니다
    // 아직은 추적만할거에요
    //public Vector3 GetWanderDirection()
    //{
    //    return Vector3.zero; // 이 컴포넌트는 배회 기능을 구현하지 않습니다.
    //}

    /// <summary>
    /// 지정된 목표 위치를 향하는 방향 벡터를 계산하여 반환합니다.
    /// 주로 추적(Face Direction) 로직에 사용됩니다.
    /// </summary>
    /// <param name="targetPosition">추적할 목표 대상의 월드 위치입니다.</param>
    /// <returns>목표를 향하는 정규화된 방향 벡터. 목표가 현재 위치와 같으면 Vector3.zero.</returns>
    public void GetFaceDirection(AbilityContext abilityContext)
    {
        // 1. abilityContext 자체가 null인지 확인
        if (abilityContext == null)
        {
            Debug.LogWarning("GetFaceDirection: abilityContext가 null입니다. 이동 방향을 계산할 수 없습니다.");
            // null일 경우 기본값으로 Vector3.zero를 설정하거나 아무것도 하지 않을 수 있습니다.
            // 여기서는 그냥 메서드를 종료하고 InputDirection에 접근하지 않도록 합니다.
            return;
        }

        // 2. abilityContext.TargetPawn이 null인지 확인
        // (TargetPawn이 없다는 것은 이동할 대상이 없다는 의미)
        if (abilityContext.TargetPawn == null)
        {
             Debug.LogWarning("GetFaceDirection: abilityContext.TargetPawn이 null입니다. 이동 방향을 계산할 수 없습니다.");
            // 대상이 없으므로 이동 방향은 0으로 설정
            abilityContext.InputDirection = Vector3.zero;
            return;
        }

        // 3. TargetPawn.transform이 null일 수도 있음을 고려 (매우 드물지만, 오브젝트가 파괴되었을 때 등)
        if (abilityContext.TargetPawn.transform == null)
        {
            Debug.LogWarning($"GetFaceDirection: TargetPawn '{abilityContext.TargetPawn.name}'의 transform이 null입니다. 이동 방향을 계산할 수 없습니다.");
            abilityContext.InputDirection = Vector3.zero;
            return;
        }

        // 현재 위치에서 목표 위치를 향하는 벡터를 계산합니다.
        Vector3 directionToTarget = abilityContext.TargetPawn.transform.position - transform.position;

        // 거리가 0보다 크면 (즉, 목표가 현재 위치와 다르면) 방향을 정규화하여 반환합니다.
        if (directionToTarget.magnitude > 0)
        {
            abilityContext.InputDirection = directionToTarget.normalized;
            //Debug.Log("성공");

            return;
        }

        // 목표가 현재 위치와 같거나 매우 가까워 방향을 계산할 수 없으면 이동하지 않습니다.
        abilityContext.InputDirection = Vector3.zero;
        //Debug.Log("마지막");

        return;
    }

    #endregion
}
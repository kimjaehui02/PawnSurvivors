using UnityEngine;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Events;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Utilities;

namespace PawnSurvivors.Domain.Combat
{
    /// <summary>
    /// 즉발 공격 방식입니다. 투사체 없이 즉시 타겟에게 데미지를 입힙니다.
    /// </summary>
    public class InstantAttackMethod : IAttackMethod
    {
        public bool Execute(
            PawnData pawnData,
            Transform attackPoint,
            string targetTag,
            float damage,
            float attackRange,
            GameObject attacker,
            PawnManager owner)
        {
            // 가장 가까운 타겟 찾기
            Transform closestTarget = TargetingUsecases.FindClosestTargetByTag(attackPoint.position, targetTag, attackRange);
            
            if (closestTarget == null)
            {
                return false; // 타겟 없음
            }

            // 데미지를 받을 수 있는 Pawn인지 확인
            if (PawnHelper.TryGetDamageablePawn(closestTarget.gameObject, out var targetPawnManager))
            {
                // 즉시 DamageEvent 발행
                targetPawnManager.Publish(new DamageEvent(targetPawnManager, damage, attacker));
                
                return true; // 공격 성공
            }

            return false; // 공격 실패
        }
    }
}


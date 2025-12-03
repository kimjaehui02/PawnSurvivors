using UnityEngine;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Events;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Utilities;

namespace PawnSurvivors.Domain.Combat
{
    /// <summary>
    /// 범위 공격 방식입니다. 
    /// 공격자 주변의 모든 적에게 즉시 데미지를 입힙니다.
    /// 투사체나 장판 없이 범위 내 모든 대상을 타격합니다.
    /// </summary>
    public class AreaAttackMethod : IAttackMethod
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
            if (attackRange <= 0)
            {
                Debug.LogWarning("[AreaAttackMethod] attackRange must be greater than 0 for area attacks.");
                return false;
            }

            // 범위 내 모든 타겟 찾기
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
            
            if (hits.Length == 0)
            {
                return false; // 타겟 없음
            }

            int hitCount = 0;

            foreach (var hit in hits)
            {
                // 같은 팀은 무시 (아군 공격 방지)
                if (TagHelper.IsSameTeam(hit.gameObject, attacker))
                {
                    continue;
                }

                // 타겟 태그 확인
                if (!hit.CompareTag(targetTag))
                {
                    continue;
                }

                // 데미지를 받을 수 있는 Pawn인지 확인
                if (PawnHelper.TryGetDamageablePawn(hit.gameObject, out var targetPawnManager))
                {
                    // 즉시 DamageEvent 발행
                    targetPawnManager.Publish(new DamageEvent(targetPawnManager, damage, attacker));
                    hitCount++;
                }
            }

            return hitCount > 0; // 1명 이상 타격 시 성공
        }
    }
}


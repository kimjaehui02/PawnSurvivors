using UnityEngine;
using PawnSurvivors.Domain;

namespace PawnSurvivors.Domain.Combat
{
    /// <summary>
    /// 공격 방식(Method)을 정의하는 인터페이스입니다.
    /// Projectile(투사체), Instant(즉발), Area(장판) 등의 공격 방식을 구현합니다.
    /// </summary>
    public interface IAttackMethod
    {
        /// <summary>
        /// 단일 공격을 실행합니다.
        /// </summary>
        /// <param name="pawnData">공격자의 PawnData</param>
        /// <param name="attackPoint">공격 시작 위치</param>
        /// <param name="targetTag">타겟 태그</param>
        /// <param name="damage">데미지</param>
        /// <param name="attackRange">공격 범위 (0이면 무한)</param>
        /// <param name="attacker">공격자 GameObject</param>
        /// <param name="owner">공격자 PawnManager</param>
        /// <returns>공격 성공 여부</returns>
        bool Execute(
            PawnData pawnData,
            Transform attackPoint,
            string targetTag,
            float damage,
            float attackRange,
            GameObject attacker,
            PawnManager owner
        );
    }
}


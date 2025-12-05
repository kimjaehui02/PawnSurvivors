using UnityEngine;

namespace PawnSurvivors.Domain.Events
{
    /// <summary>
    /// Pawn이 실제로 데미지를 받은 후 발행되는 이벤트입니다.
    /// 피격 시 추가 효과(무적, 넉백, 이펙트 등)를 구현할 때 이 이벤트를 구독하세요.
    /// </summary>
    public class PawnDamagedEvent
    {
        /// <summary>
        /// 데미지를 받은 Pawn의 PawnManager입니다.
        /// </summary>
        public PawnManager Target { get; }

        /// <summary>
        /// 실제로 적용된 데미지 양입니다.
        /// </summary>
        public float DamageApplied { get; }

        /// <summary>
        /// 데미지를 가한 주체 (없을 수 있음)입니다.
        /// </summary>
        public GameObject Attacker { get; }

        /// <summary>
        /// 피격 후 남은 체력입니다.
        /// </summary>
        public float RemainingHealth { get; }

        /// <summary>
        /// 이번 피격으로 사망했는지 여부입니다.
        /// </summary>
        public bool IsFatal { get; }

        public PawnDamagedEvent(PawnManager target, float damageApplied, GameObject attacker, float remainingHealth, bool isFatal)
        {
            Target = target;
            DamageApplied = damageApplied;
            Attacker = attacker;
            RemainingHealth = remainingHealth;
            IsFatal = isFatal;
        }
    }
}


using UnityEngine;

namespace PawnCore.Domain.Events
{
    /// <summary>
    /// Pawn이 데미지를 받았을 때 발행되는 이벤트입니다.
    /// </summary>
    public class DamageEvent
    {
        /// <summary>
        /// 데미지를 받은 Pawn의 PawnManager입니다.
        /// </summary>
        public PawnManager Target { get; }

        /// <summary>
        /// 받은 데미지 양입니다.
        /// </summary>
        public float Amount { get; }

        /// <summary>
        /// 데미지를 가한 주체 (없을 수 있음)입니다.
        /// </summary>
        public GameObject Attacker { get; }

        public DamageEvent(PawnManager target, float amount, GameObject attacker = null)
        {
            Target = target;
            Amount = amount;
            Attacker = attacker;
        }
    }
}


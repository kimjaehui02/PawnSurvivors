using UnityEngine;

namespace PawnCore.Domain.Events
{
    /// <summary>
    /// 충돌로 인한 데미지를 처리해야 할 때 발행되는 이벤트입니다.
    /// </summary>
    public class CollisionDamageEvent
    {
        /// <summary>
        /// 충돌한 자기 자신의 GameObject입니다.
        /// </summary>
        public GameObject Self { get; }

        /// <summary>
        /// 충돌한 상대방의 Collider2D입니다.
        /// </summary>
        public Collider2D Other { get; }

        /// <summary>
        /// 가할 데미지 양입니다.
        /// </summary>
        public float Damage { get; }

        public CollisionDamageEvent(GameObject self, Collider2D other, float damage)
        {
            Self = self;
            Other = other;
            Damage = damage;
        }
    }
}


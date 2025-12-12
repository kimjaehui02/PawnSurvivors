using UnityEngine;

namespace PawnSurvivors.Domain.Events
{
    /// <summary>
    /// Pawn이 사망했을 때 발행되는 이벤트입니다.
    /// </summary>
    public class PawnDeathEvent
    {
        /// <summary>
        /// 사망한 Pawn의 PawnManager입니다.
        /// </summary>
        public PawnManager DeadPawn { get; }

        /// <summary>
        /// 사망 원인을 제공한 주체 (없을 수 있음)입니다.
        /// </summary>
        public GameObject Killer { get; }

        /// <summary>
        /// 이벤트가 취소되었는지 여부입니다.
        /// DeathEffectSubManager 등에서 파괴를 지연시킬 때 사용합니다.
        /// </summary>
        public bool IsCancelled { get; private set; } = false;

        public PawnDeathEvent(PawnManager deadPawn, GameObject killer = null)
        {
            DeadPawn = deadPawn;
            Killer = killer;
        }

        /// <summary>
        /// 이벤트를 취소합니다. 취소되면 기본 파괴 처리가 스킵됩니다.
        /// </summary>
        public void Cancel()
        {
            IsCancelled = true;
        }
    }
}


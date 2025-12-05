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

        public PawnDeathEvent(PawnManager deadPawn, GameObject killer = null)
        {
            DeadPawn = deadPawn;
            Killer = killer;
        }
    }
}


using UnityEngine;

namespace PawnSurvivors.Domain.Events
{
    /// <summary>
    /// 공격 입력이 감지되었을 때 발생하는 이벤트를 나타냅니다.
    /// 이 이벤트는 PawnManager의 제네릭 이벤트 버스를 통해 발행됩니다.
    /// </summary>
    public class AttackInputEvent
    {
        /// <summary>
        /// 공격을 시작한 게임 오브젝트입니다.
        /// </summary>
        public GameObject Attacker { get; }

        /// <summary>
        /// AttackInputEvent 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="attacker">공격을 시작한 게임 오브젝트입니다.</param>
        public AttackInputEvent(GameObject attacker)
        {
            Attacker = attacker;
        }
    }
}

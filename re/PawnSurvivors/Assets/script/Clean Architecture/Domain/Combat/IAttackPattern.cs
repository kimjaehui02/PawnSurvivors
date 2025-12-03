using UnityEngine;
using PawnSurvivors.Domain;

namespace PawnSurvivors.Domain.Combat
{
    /// <summary>
    /// 공격 패턴(Pattern)을 정의하는 인터페이스입니다.
    /// Single(단발), Burst(연타), MultiShot(동시 여러발) 등의 공격 패턴을 구현합니다.
    /// </summary>
    public interface IAttackPattern
    {
        /// <summary>
        /// 패턴이 완료되었는지 여부
        /// </summary>
        bool IsComplete { get; }
        
        /// <summary>
        /// 패턴을 시작합니다.
        /// </summary>
        void Start(float startTime);
        
        /// <summary>
        /// 패턴을 리셋합니다.
        /// </summary>
        void Reset();
        
        /// <summary>
        /// 매 프레임 호출되어 패턴을 진행합니다.
        /// </summary>
        /// <param name="gameTime">현재 게임 시간</param>
        /// <param name="attackMethod">사용할 공격 방식</param>
        /// <param name="pawnData">공격자의 PawnData</param>
        /// <param name="attackPoint">공격 시작 위치</param>
        /// <param name="targetTag">타겟 태그</param>
        /// <param name="damage">데미지</param>
        /// <param name="attackRange">공격 범위</param>
        /// <param name="attacker">공격자 GameObject</param>
        /// <param name="owner">공격자 PawnManager</param>
        void Update(
            float gameTime,
            IAttackMethod attackMethod,
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


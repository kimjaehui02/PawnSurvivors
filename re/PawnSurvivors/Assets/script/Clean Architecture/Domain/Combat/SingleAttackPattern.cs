using UnityEngine;
using PawnSurvivors.Domain;

namespace PawnSurvivors.Domain.Combat
{
    /// <summary>
    /// 단일 공격 패턴입니다. 1회만 공격합니다.
    /// </summary>
    public class SingleAttackPattern : IAttackPattern
    {
        private bool _isComplete = false;

        public bool IsComplete => _isComplete;

        public void Start(float startTime)
        {
            _isComplete = false;
        }

        public void Reset()
        {
            _isComplete = false;
        }

        public void Update(
            float gameTime,
            IAttackMethod attackMethod,
            PawnData pawnData,
            Transform attackPoint,
            string targetTag,
            float damage,
            float attackRange,
            GameObject attacker,
            PawnManager owner)
        {
            if (_isComplete) return; // 이미 완료됨

            // 단일 공격 실행
            attackMethod.Execute(pawnData, attackPoint, targetTag, damage, attackRange, attacker, owner);
            
            _isComplete = true; // 즉시 완료
        }
    }
}


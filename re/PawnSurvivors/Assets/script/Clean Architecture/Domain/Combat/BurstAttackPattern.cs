using UnityEngine;
using PawnSurvivors.Domain;

namespace PawnSurvivors.Domain.Combat
{
    /// <summary>
    /// 연타 공격 패턴입니다. 짧은 간격으로 여러 번 공격합니다.
    /// 시간 기반 순차 실행으로 구현되어 GameTime과 일관성을 유지합니다.
    /// </summary>
    public class BurstAttackPattern : IAttackPattern
    {
        /// <summary>
        /// 연타 횟수
        /// </summary>
        public int burstCount = 2;
        
        /// <summary>
        /// 연타 간격 (초)
        /// </summary>
        public float burstDelay = 0.1f;
        
        // Private fields
        private int _currentBurstIndex = 0;
        private float _nextBurstTime = 0f;
        private bool _isComplete = false;

        public BurstAttackPattern(int burstCount = 2, float burstDelay = 0.1f)
        {
            this.burstCount = burstCount;
            this.burstDelay = burstDelay;
        }

        public bool IsComplete => _isComplete;

        public void Start(float startTime)
        {
            _currentBurstIndex = 0;
            _nextBurstTime = startTime; // 즉시 첫 공격
            _isComplete = false;
        }

        public void Reset()
        {
            _currentBurstIndex = 0;
            _nextBurstTime = 0f;
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

            // 시간 확인 후 공격 (기존 패턴과 동일)
            if (gameTime >= _nextBurstTime)
            {
                // 공격 실행
                attackMethod.Execute(pawnData, attackPoint, targetTag, damage, attackRange, attacker, owner);
                
                _currentBurstIndex++;
                
                if (_currentBurstIndex >= burstCount)
                {
                    _isComplete = true; // 연타 완료
                }
                else
                {
                    // 다음 공격 시간 예약 (기존 패턴과 동일)
                    _nextBurstTime = gameTime + burstDelay;
                }
            }
        }
    }
}


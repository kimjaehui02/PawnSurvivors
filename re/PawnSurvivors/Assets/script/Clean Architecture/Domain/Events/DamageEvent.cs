using UnityEngine;

namespace PawnSurvivors.Domain.Events
{
    /// <summary>
    /// 이벤트 핸들러의 실행 우선순위를 정의합니다.
    /// 숫자가 작을수록 먼저 실행됩니다.
    /// </summary>
    public enum EventPriority
    {
        /// <summary>
        /// 가장 먼저 실행 - 이벤트를 차단하거나 수정하는 로직
        /// 예: 무적 시스템, 쉴드 시스템
        /// </summary>
        Highest = 0,

        /// <summary>
        /// 높은 우선순위 - 주요 전처리 로직
        /// 예: 버프/디버프 계산, 데미지 증감
        /// </summary>
        High = 100,

        /// <summary>
        /// 일반 우선순위 - 기본 로직 (기본값)
        /// 예: 체력 감소, 기본 데미지 처리
        /// </summary>
        Normal = 200,

        /// <summary>
        /// 낮은 우선순위 - 후처리 로직
        /// 예: 피격 이펙트, 사운드
        /// </summary>
        Low = 300,

        /// <summary>
        /// 가장 나중에 실행 - 결과 처리
        /// 예: 로그 기록, 통계 업데이트
        /// </summary>
        Lowest = 400
    }

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

        /// <summary>
        /// 이벤트가 취소되었는지 여부입니다.
        /// 무적 등의 이유로 데미지를 무시해야 할 때 true로 설정하세요.
        /// </summary>
        public bool IsCancelled { get; set; }

        public DamageEvent(PawnManager target, float amount, GameObject attacker = null)
        {
            Target = target;
            Amount = amount;
            Attacker = attacker;
            IsCancelled = false;
        }
    }
}


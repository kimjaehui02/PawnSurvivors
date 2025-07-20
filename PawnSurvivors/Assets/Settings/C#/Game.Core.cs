using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    #region Enums

    /// <summary>
    /// 게임 내에서 발생하는 핵심 행동들을 정의하는 Enum입니다.
    /// Pawn의 델리게이트 시스템에서 키로 사용됩니다.
    /// </summary>
    public enum Acts
    {
        // 폰의 기본 행동 및 생명 주기 관련 액트
        OnMove,         // 이동 요청
        OnAttack,       // 공격 요청 (어떤 방식이든 상관 없이 공격 행위 자체)
        OnDamaged,      // 피해를 받았을 때
        OnDeath,        // 사망했을 때
        OnHeal,         // 체력을 회복했을 때

        // 유니티 생명 주기 메서드에 연결될 액트 (전역 업데이트 요청용)
        OnUpdate,       // MonoBehaviour.Update() 시점에 해당 델리게이트 등록 함수들 호출
        OnFixedUpdate,  // MonoBehaviour.FixedUpdate() 시점 (주로 물리 연산)
        OnLateUpdate,   // MonoBehaviour.LateUpdate() 시점 (주로 카메라, UI 업데이트)

        // 특정 입력 처리가 완료되었음을 알리는 액트 (InputHandler 등에서 요청)
        OnInputProcessed,

        // 필요한 다른 핵심 행동들을 여기에 추가합니다.
        // OnSkillUse,    // 스킬 사용
        // OnInteract,    // 상호작용
    }

    /// <summary>
    /// 피해의 속성 또는 타입을 정의하는 Enum입니다.
    /// (예: 물리, 화염, 냉기, 독 등)
    /// 이 Enum은 피해를 계산하고 적용하는 데 사용됩니다.
    /// </summary>
    public enum DamageType
    {
        None,           // 정의되지 않은 피해 타입 또는 기본값
        Physical,       // 물리 피해: 일반적인 타격, 베기, 찌르기 등
        Fire,           // 화염 피해: 불 속성 공격
        Ice,            // 냉기 피해: 얼음 속성 공격
        Poison,         // 독 피해: 독 속성 공격
        Lightning,      // 번개 피해: 전기 속성 공격
        Holy,           // 신성 피해: 성스러운 힘을 이용한 공격
        Dark,           // 암흑 피해: 어둠의 힘을 이용한 공격
        // 필요한 피해 속성을 여기에 추가합니다.
    }

    /// <summary>
    /// 공격의 구체적인 방식을 정의하는 Enum입니다.
    /// (예: 주먹질, 불 뿜기, 투사체 발사 등)
    /// 이 Enum은 DamageDealerComponent에서 특정 공격 방식에 따른 피해량 및 타입을 조회할 때 사용됩니다.
    /// </summary>
    public enum AttackMethod
    {
        None,           // 정의되지 않은 공격 방식 또는 기본값
        Punch,          // 주먹질 (근접)
        Bite,           // 물기 (근접)
        FireBreath,     // 불 뿜기 (원거리, 광역)
        SpitPoison,     // 독침 뱉기 (투사체)
        SwordSwing,     // 칼 휘두르기 (근접)
        Charge,         // 돌진 공격
        Explosion,      // 폭발 공격 (광역)
        Projectile,     // 일반 투사체 발사
        // 필요한 공격 방식을 여기에 추가합니다.
    }

    public enum Components
    {
        DamageDealerComponent,
        DamageableComponent,
        GraphicComponent,
        MoveableComponent,
        PawnMoverComponent,
        PlayerMoverComponent
    }

    #endregion



    #region AbilityContext Class

    /// <summary>
    /// 델리게이트에 필요한 모든 맥락 정보를 담는 클래스입니다.
    /// 이 클래스의 인스턴스는 각 행위(Act) 발생 시 관련 데이터를 전달하는 데 사용됩니다.
    /// </summary>
    public class AbilityContext
    {
        // --- 비단발성 (Persistent) 정보 ---
        public Vector3 inputDirection;
        public Pawn targetPawn; // 오타 수정: tartgetPawn -> targetPawn

        // --- 단발성 (Transient) 정보 ---
        // #region 으로 묶어두는 것은 코드 가독성 측면에서 좋습니다.
        #region TransientInfo
        public float damageAmount;
        // 여기에 hitPoint, hitNormal, isCriticalHit, sourceObject 등
        // 단발성 액트와 관련된 다른 필드들을 추가할 수 있습니다.
        #endregion

        /// <summary>
        /// 단발성(Transient) 정보를 초기화하는 메서드입니다.
        /// 액트 처리 후 또는 다음 액트 시작 전에 호출됩니다.
        /// </summary>
        public void ResetTransientInfo()
        {
            damageAmount = 0;
            // 여기에 다른 모든 단발성 필드들을 초기화하는 로직을 추가합니다.
            // 예: hitPoint = Vector3.zero; hitNormal = Vector3.zero; isCriticalHit = false;
            // sourceObject = null;
        }

        // 모든 필드를 초기화하는 Reset() 메서드는 비단발성 정보가 있다면 이제 신중하게 사용해야 합니다.
        // public void Reset() { ... }
    }

    #endregion

    #region 아이템스탯

    #endregion

    #region Interfaces

    /// <summary>
    /// Acts와 연결된 Action 델리게이트 딕셔너리를 관리하는 기능을 제공하는 인터페이스입니다.
    /// PawnAction 등의 컴포넌트들이 이 인터페이스를 구현하여 자신의 능력을 시스템에 등록하고 해제합니다.
    /// </summary>
    public interface IActionMapManager
    {
        /// <summary>
        /// 이 관리자가 소유한 Acts-Action 델리게이트 딕셔너리에 대한 읽기 전용 뷰를 가져옵니다.
        /// 외부에서 딕셔너리 내용을 직접 변경(추가/제거)할 수 없도록 IReadOnlyDictionary를 사용합니다.
        /// </summary>
        IReadOnlyDictionary<Acts, Action<AbilityContext>> GetActions { get; }

        /// <summary>
        /// 지정된 Acts에 대한 Action 델리게이트를 추가하거나, 이미 존재하는 경우 기존 델리게이트에 연결합니다.
        /// </summary>
        /// <param name="act">등록할 Acts Enum 값.</param>
        /// <param name="action">해당 Acts에 연결할 Action 델리게이트.</param>
        void AddAction(Acts act, Action<AbilityContext> action);

        /// <summary>
        /// 지정된 Acts에 대한 특정 Action 델리게이트를 제거하거나,
        /// action 매개변수가 null인 경우 해당 Acts에 연결된 모든 델리게이트를 제거합니다.
        /// </summary>
        /// <param name="act">제거할 대상 Acts Enum 값.</param>
        /// <param name="action">제거할 특정 Action 델리게이트. null이면 해당 Acts의 모든 델리게이트를 제거합니다.</param>
        void RemoveAction(Acts act, Action<AbilityContext> action = null);

        // 참고: InvokeAction 메서드는 델리게이트 맵 '관리' 기능이라기보다는 '사용' 기능에 가깝습니다.
        // 딕셔너리를 직접 노출하는 GetActions를 통해 외부에서 Invoke를 할 수 있으므로,
        // 이 인터페이스에 반드시 포함될 필요는 없습니다. 하지만 필요하다면 추가할 수 있습니다.
        // void InvokeAction(Acts act, AbilityContext context);
    }

    #endregion
}
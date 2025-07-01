// GameCore.cs 파일
using UnityEngine;

namespace Game.Core // 또는 당신의 게임 이름.Core
{
    public enum Acts // 델리게이트 키로 사용될 핵심 행동 정의
    {
        OnMove,
        OnDamaged,
        OnDeath,
        OnAttack,
        OnHeal,
        // ...
    }

    /// <summary>
    /// 델리게이트에 필요한 모든 맥락 정보를 담는 클래스.
    /// </summary>
    public class AbilityContext
    {
        // --- 공통 정보 ---
        public Pawn SourcePawn { get; set; }
        public GameObject TargetObject { get; set; }

        // --- Acts별 데이터 필드 ---
        public float DamageAmount { get; set; }
        public Vector2 MoveDirection { get; set; }
        public bool IsCriticalHit { get; set; }
        public Vector3 KnockbackForce { get; set; }
        // ... 그 외 필요한 필드들

        // 생성자 오버로드 등
    }

    // 여기에 나중에 다른 공통 Enum이나 인터페이스 등을 추가할 수도 있습니다.
    // public interface IHealth { float CurrentHealth { get; } }
}
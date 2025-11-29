using System.Collections.Generic;
using UnityEngine;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Events;
using PawnSurvivors.Domain.Usecases;

/// <summary>
/// 충돌 시 데미지를 가하는 SubManager입니다.
/// 충돌이 발생하면 DamageEvent를 발행합니다.
/// 쿨다운 계산은 CombatUsecases에 위임합니다.
/// </summary>
public class CollisionDamageSubManager : PawnSubManager
{
    private PawnData _pawnData;
    
    /// <summary>
    /// 충돌 시 자신을 파괴할지 여부입니다.
    /// 발사체는 true, 근접 공격 유닛은 false로 설정하세요.
    /// </summary>
    public bool destroyOnHit = true;
    
    /// <summary>
    /// 연속 충돌 시 데미지를 주는 간격 (초)
    /// destroyOnHit이 false일 때만 사용됩니다.
    /// </summary>
    public float damageCooldown = 0.5f;
    
    /// <summary>
    /// 마지막으로 데미지를 준 대상과 시간을 기록
    /// </summary>
    private Dictionary<PawnManager, float> _lastDamageTimes = new Dictionary<PawnManager, float>();
    
    /// <summary>
    /// 이미 충돌했는지 여부 (destroyOnHit이 true일 때 중복 충돌 방지)
    /// </summary>
    private bool _hasCollided = false;

    public override void SubStart()
    {
        _pawnData = _pawnManager.PawnData;
        
        // CombatData 가져오기 또는 생성
        _pawnData.GetOrCreateCombatData();
        
        // CombatData에서 destroyOnHit 설정 가져오기
        destroyOnHit = _pawnData.combatData.destroyOnHit;
    }

    public override void SubUpdate()
    {
        // 특정 업데이트 로직이 필요하지 않음
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ProcessCollision(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // destroyOnHit이 false인 경우(근접 공격 유닛)만 연속 충돌 처리
        if (!destroyOnHit)
        {
            ProcessCollision(other);
        }
    }

    private void ProcessCollision(Collider2D other)
    {
        // 이미 충돌 처리했으면 무시 (destroyOnHit이 true인 경우)
        if (destroyOnHit && _hasCollided)
        {
            return;
        }

        // 같은 태그는 무시 (아군끼리 공격 방지)
        if (other.CompareTag(gameObject.tag))
        {
            return;
        }

        // 총알끼리 충돌 무시 (Bullet과 EnemyBullet)
        bool isSelfBullet = gameObject.CompareTag("Bullet") || gameObject.CompareTag("EnemyBullet");
        bool isOtherBullet = other.CompareTag("Bullet") || other.CompareTag("EnemyBullet");
        if (isSelfBullet && isOtherBullet)
        {
            return;
        }

        // CombatData가 없으면 무시
        if (_pawnData?.combatData == null) return;

        // 상대방이 PawnManager를 가지고 있는지 확인
        if (other.TryGetComponent<PawnManager>(out var targetPawnManager))
        {
            // DamageableSubManager가 있는 대상만 공격 (데미지를 받을 수 있는 대상만)
            // 코인처럼 DamageableSubManager가 없는 아이템은 자동으로 무시됨
            if (!targetPawnManager.TryGetComponent<DamageableSubManager>(out _))
            {
                return;
            }

            // ✅ 쿨다운 체크 로직을 UseCases에 위임 (destroyOnHit이 false인 경우만)
            if (!destroyOnHit)
            {
                float currentTime = GetGameTime();
                if (!CombatUsecases.CanDealDamageWithCooldown(targetPawnManager, currentTime, damageCooldown, _lastDamageTimes))
                {
                    return; // 쿨다운 중
                }
            }

            // ✅ 투사체의 경우 이미 발사자에서 계산된 데미지가 PawnData.combatData.damage에 설정되어 있음
            // 플레이어 Pawn의 경우에만 PawnStatCalculator를 사용하여 아이템 효과를 반영
            float effectiveDamage = _pawnData.combatData.damage; // 기본값
            if (GameManager.Instance?.PawnStatCalculator != null)
            {
                // 투사체가 아닌 경우 (플레이어 Pawn 직접 공격 등)에만 재계산
                // 투사체는 이미 발사자에서 계산된 데미지가 설정되어 있음
                if (_pawnManager.Owner == null)
                {
                    // 소유자가 없는 경우 (직접 공격)에만 재계산
                    effectiveDamage = GameManager.Instance.PawnStatCalculator.GetEffectiveDamage(_pawnData);
                }
                // 투사체의 경우 이미 계산된 데미지를 그대로 사용
            }
            
            // 상대방에게 DamageEvent 발행
            targetPawnManager.Publish(new DamageEvent(targetPawnManager, effectiveDamage, gameObject));
            
            // 임시 디버그 로그 (테스트용 - 문제 해결 후 제거)
            if (Time.frameCount % 60 == 0) // 1초마다 한 번씩만 로그
            {
                Debug.Log($"[CollisionDamage] {gameObject.name} → {other.name}: base={_pawnData.combatData.damage}, effective={effectiveDamage}, owner={_pawnManager.Owner?.name}");
            }

            // destroyOnHit이 true일 경우에만 자신 파괴 (발사체의 경우)
            if (destroyOnHit)
            {
                _hasCollided = true; // 충돌 플래그 설정
                _pawnManager.DestroyPawn();
            }
        }
    }

    private void OnDisable()
    {
        // 메모리 누수 방지
        _lastDamageTimes.Clear();
    }
}

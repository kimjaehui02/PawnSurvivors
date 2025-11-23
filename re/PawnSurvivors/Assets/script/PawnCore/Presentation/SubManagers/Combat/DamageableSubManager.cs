using UnityEngine;
using PawnCore.Domain;
using PawnCore.Domain.Events;

/// <summary>
/// Pawn이 데미지를 받을 수 있도록 하는 SubManager입니다.
/// DamageEvent를 구독하여 데미지를 처리하고, 체력이 0이 되면 PawnDeathEvent를 발행합니다.
/// 비즈니스 로직은 CombatUsecases에 위임합니다.
/// </summary>
public class DamageableSubManager : PawnSubManager
{
    private PawnData _pawnData;

    public override void SubStart()
    {
        _pawnData = _pawnManager.PawnData;
        
        // HealthData 가져오기 또는 생성
        var healthData = _pawnData.GetOrCreateHealthData();
        healthData.currentHealth = healthData.maxHealth;

        // DamageEvent 구독 (일반 우선순위)
        // 무적 시스템(Highest)이 먼저 실행된 후 데미지를 처리합니다.
        _pawnManager.Subscribe<DamageEvent>(HandleDamageEvent, EventPriority.Normal);
    }

    private void OnDisable()
    {
        if (_pawnManager != null)
        {
            _pawnManager.Unsubscribe<DamageEvent>(HandleDamageEvent);
        }
    }

    public override void SubUpdate()
    {
        // 체력은 기본적으로 프레임별 업데이트가 필요하지 않습니다.
    }

    /// <summary>
    /// DamageEvent를 처리합니다.
    /// </summary>
    private void HandleDamageEvent(DamageEvent evt)
    {
        // 이 Pawn을 대상으로 한 데미지인지 확인
        if (evt.Target != _pawnManager) return;

        // 이벤트가 취소되었으면 무시 (무적 등)
        if (evt.IsCancelled)
        {
            Debug.Log($"{_pawnManager.name} damage was cancelled (invincibility, etc.)");
            return;
        }

        // HealthData가 없으면 무시
        if (_pawnData?.healthData == null) return;

            // ✅ 비즈니스 로직은 UseCases에 위임
            var (actualDamage, newHealth, isFatal) = CombatUsecases.ApplyDamage(_pawnData.healthData, evt.Amount);

            // Debug.Log($"{_pawnManager.name} took {actualDamage} damage. Current health: {newHealth}");

        // ✅ 공격자의 경험치(experienceData.currentProgress) 직접 업데이트
        // 적에게 데미지를 입힌 플레이어의 PawnData에 데미지 기록
        // 주의: ExperienceData는 LevelUpSubManager가 있을 때만 생성되므로, 여기서는 존재할 때만 업데이트
        if (evt.Attacker != null && _pawnManager.CompareTag("Enemy"))
        {
            PawnManager attackerPawnManager = evt.Attacker.GetComponent<PawnManager>();
            if (attackerPawnManager != null)
            {
                // 탄환의 경우 Owner를 확인, 직접 공격의 경우 Attacker 자체를 확인
                PawnManager actualOwner = attackerPawnManager.Owner ?? attackerPawnManager;
                
                // 플레이어가 적에게 데미지를 입힌 경우
                // ExperienceData가 있는 경우에만 업데이트 (레벨업 시스템이 있는 Pawn만)
                if (IsPlayerPawn(actualOwner) && 
                    actualOwner.PawnData != null && 
                    actualOwner.PawnData.experienceData != null)
                {
                    // 데미지만큼 진행도 증가
                    actualOwner.PawnData.experienceData.currentProgress += actualDamage;
                }
            }
        }

        // 피격 이벤트 발행 (무적, 넉백, 이펙트 등 추가 효과를 위해)
        var damagedEvent = new PawnDamagedEvent(
            _pawnManager, 
            actualDamage, 
            evt.Attacker, 
            newHealth,
            isFatal
        );
        Debug.Log($"[DamageableSubManager] {_pawnManager.name} - PawnDamagedEvent 발행: Attacker={evt.Attacker?.name}, Damage={actualDamage}, Target={_pawnManager.name}");
        _pawnManager.Publish(damagedEvent);

        // 통계 기록은 UseCase를 통해 처리 (DamageTrackingUseCase가 PawnDamagedEvent를 구독하여 처리)
        // 여기서는 직접 기록하지 않음

        // 체력이 0 이하가 되면 사망 이벤트 발행
        if (isFatal)
        {
            Debug.Log($"{_pawnManager.name} has run out of health and will be destroyed.");
            
            // SessionData에 처치 수 기록 (레벨업 시스템용) - UseCase를 통해 처리
            if (GameManager.Instance?.KillTrackingUseCase != null && evt.Attacker != null)
            {
                // ✅ Owner 기반으로 플레이어 공격 확인 (Tag/Layer 하드코딩 제거)
                PawnManager attackerPawnManager = evt.Attacker.GetComponent<PawnManager>();
                if (attackerPawnManager != null)
                {
                    // 탄환의 경우 Owner를 확인, 직접 공격의 경우 Attacker 자체를 확인
                    PawnManager actualOwner = attackerPawnManager.Owner ?? attackerPawnManager;
                    
                    // 플레이어가 적을 처치한 경우
                    if (IsPlayerPawn(actualOwner) && _pawnManager.CompareTag("Enemy"))
                    {
                        // UseCase를 통해 처치 수 기록
                        if (GameManager.Instance?.KillTrackingUseCase != null)
                        {
                            GameManager.Instance.KillTrackingUseCase.RecordKill();
                        }
                    }
                }
            }
            
            _pawnManager.Publish(new PawnDeathEvent(_pawnManager, evt.Attacker));
        }
    }
    
    /// <summary>
    /// 해당 PawnManager가 플레이어인지 확인합니다.
    /// PlayerController의 playerPawns에 포함되어 있으면 플레이어입니다.
    /// </summary>
    private bool IsPlayerPawn(PawnManager pawnManager)
    {
        if (pawnManager == null || GameManager.Instance?.PlayerController == null) return false;
        
        // PlayerController의 playerPawns에 포함되어 있는지 확인
        return GameManager.Instance.PlayerController.playerPawns.Contains(pawnManager.gameObject);
    }

    /// <summary>
    /// 외부에서 직접 데미지를 주기 위한 메서드 (하위 호환성)
    /// 내부적으로 DamageEvent를 발행합니다.
    /// </summary>
    public void TakeDamage(float amount, GameObject attacker = null)
    {
        _pawnManager.Publish(new DamageEvent(_pawnManager, amount, attacker));
    }
}

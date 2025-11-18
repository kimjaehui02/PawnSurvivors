using UnityEngine;
using PawnCore.Domain.Events;
using System.Collections.Generic;

/// <summary>
/// 누적 데미지를 기준으로 레벨업하는 전략입니다.
/// 모든 PawnManager의 PawnDamagedEvent를 구독하여 플레이어가 입힌 데미지를 추적합니다.
/// </summary>
public class DamageDealtLevelUpStrategy : LevelUpStrategyBase
{
    /// <summary>레벨업에 필요한 누적 데미지</summary>
    public float requiredDamage = 500f;

    /// <summary>이 전략이 초기화된 이후 입힌 누적 데미지</summary>
    private float _damageDealt = 0f;
    
    /// <summary>구독한 PawnManager 목록 (구독 해지용)</summary>
    private List<PawnManager> _subscribedPawns = new List<PawnManager>();
    
    /// <summary>새로운 PawnManager 확인 주기 (초)</summary>
    private float _checkInterval = 1f;
    private float _lastCheckTime = 0f;

    public override void Init(PawnManager pawnManager)
    {
        base.Init(pawnManager);

        // 모든 PawnManager의 PawnDamagedEvent 구독 (플레이어가 입힌 데미지 추적)
        SubscribeToAllPawns();

        _damageDealt = 0f;

        Debug.Log($"[DamageDealtLevelUpStrategy] {pawnManager.name} 초기화 - 목표: {requiredDamage} 데미지", this);
    }

    private void OnEnable()
    {
        // 활성화 시 모든 PawnManager에 구독
        SubscribeToAllPawns();
    }

    private void OnDisable()
    {
        // 구독 해지
        UnsubscribeFromAllPawns();
    }
    
    private void Update()
    {
        // 주기적으로 새로운 PawnManager 확인 (Enemy가 나중에 생성될 수 있음)
        if (GameManager.Instance?.LifecycleManager != null)
        {
            float currentTime = GameManager.Instance.LifecycleManager.GameTime;
            if (currentTime - _lastCheckTime >= _checkInterval)
            {
                _lastCheckTime = currentTime;
                SubscribeToAllPawns(); // 새로운 PawnManager가 있으면 구독
            }
        }
    }

    /// <summary>
    /// 모든 PawnManager의 PawnDamagedEvent를 구독합니다.
    /// </summary>
    private void SubscribeToAllPawns()
    {
        int subscribedCount = 0;
        foreach (var pawn in PawnManager.AllPawnManagers)
        {
            if (pawn != null && !_subscribedPawns.Contains(pawn))
            {
                pawn.Subscribe<PawnDamagedEvent>(HandlePawnDamaged);
                _subscribedPawns.Add(pawn);
                subscribedCount++;
            }
        }
        if (subscribedCount > 0)
        {
            Debug.Log($"[DamageDealtLevelUpStrategy] {_pawnManager.name} - {subscribedCount}개의 PawnManager에 구독 완료 (총 {_subscribedPawns.Count}개 구독 중)");
        }
    }

    /// <summary>
    /// 모든 구독을 해지합니다.
    /// </summary>
    private void UnsubscribeFromAllPawns()
    {
        foreach (var pawn in _subscribedPawns)
        {
            if (pawn != null)
            {
                pawn.Unsubscribe<PawnDamagedEvent>(HandlePawnDamaged);
            }
        }
        _subscribedPawns.Clear();
    }

    /// <summary>
    /// PawnDamagedEvent를 처리하여 플레이어가 입힌 데미지를 추적합니다.
    /// </summary>
    private void HandlePawnDamaged(PawnDamagedEvent evt)
    {
        Debug.Log($"[DamageDealtLevelUpStrategy] {_pawnManager.name} - HandlePawnDamaged 호출됨: Attacker={evt.Attacker?.name}, Target={evt.Target?.name}, Damage={evt.DamageApplied}");
        
        // 이 전략이 붙은 Pawn이 입힌 데미지인지 확인
        if (evt.Attacker == null)
        {
            Debug.Log($"[DamageDealtLevelUpStrategy] {_pawnManager.name} - Attacker가 null입니다.");
            return;
        }
        if (evt.Target == null)
        {
            Debug.Log($"[DamageDealtLevelUpStrategy] {_pawnManager.name} - Target이 null입니다.");
            return;
        }
        
        // 피해를 받은 대상이 적(Enemy)인지 확인 (Tag 기반 - 적 식별용)
        if (!evt.Target.CompareTag("Enemy"))
        {
            Debug.Log($"[DamageDealtLevelUpStrategy] {_pawnManager.name} - Target이 Enemy가 아닙니다. Tag: {evt.Target.tag}");
            return;
        }
        
        // Attacker의 PawnManager 찾기
        PawnManager attackerPawnManager = evt.Attacker.GetComponent<PawnManager>();
        if (attackerPawnManager == null)
        {
            Debug.Log($"[DamageDealtLevelUpStrategy] {_pawnManager.name} - Attacker에 PawnManager가 없습니다.");
            return;
        }
        
        // ✅ Owner 기반으로 발사자 확인 (하드코딩 제거)
        // 탄환의 경우 Owner를 확인, 직접 공격의 경우 Attacker 자체를 확인
        PawnManager actualOwner = attackerPawnManager.Owner ?? attackerPawnManager;
        
        Debug.Log($"[DamageDealtLevelUpStrategy] {_pawnManager.name} - Attacker: {attackerPawnManager.name}, Owner: {attackerPawnManager.Owner?.name ?? "null"}, ActualOwner: {actualOwner.name}, MyPawn: {_pawnManager.name}");
        
        // 이 전략이 붙은 Pawn이 발사한 탄환이거나 직접 공격한 경우
        // Unity의 MonoBehaviour는 참조 비교가 안정적이지 않을 수 있으므로 GetInstanceID 사용
        if (actualOwner != null && _pawnManager != null && actualOwner.GetInstanceID() == _pawnManager.GetInstanceID())
        {
            _damageDealt += evt.DamageApplied;
            Debug.Log($"[DamageDealtLevelUpStrategy] ✅ {_pawnManager.name} 데미지 기록: {evt.DamageApplied} (누적: {_damageDealt:F0}/{requiredDamage:F0})");
        }
        else
        {
            Debug.Log($"[DamageDealtLevelUpStrategy] ❌ {_pawnManager.name} - Owner 불일치: actualOwner={actualOwner?.name ?? "null"}, myPawn={_pawnManager?.name ?? "null"}");
        }
    }

    public override bool CheckCondition()
    {
        return _damageDealt >= requiredDamage;
    }

    public override float GetProgress()
    {
        return Mathf.Clamp01(_damageDealt / requiredDamage);
    }

    public override string GetProgressText()
    {
        float currentProgress = Mathf.Min(_damageDealt, requiredDamage);
        return $"{currentProgress:F0}/{requiredDamage:F0} 데미지";
    }
}


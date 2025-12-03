using UnityEngine;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Events;
using PawnSurvivors.Domain.Usecases;

/// <summary>
/// 즉발 공격(Instant Attack)을 처리하는 SubManager입니다.
/// 투사체를 발사하지 않고 즉시 타겟에게 데미지를 입힙니다.
/// 비즈니스 로직은 CombatUsecases.HandleInstantAttack에 위임합니다.
/// </summary>
public class InstantAttackSubManager : PawnSubManager
{
    private PawnData _pawnData;
    
    /// <summary>
    /// 공격 시작 지점 (null이면 자신의 위치 사용)
    /// </summary>
    public Transform attackPoint;
    
    /// <summary>
    /// 공격 범위 (0이면 무제한)
    /// </summary>
    public float attackRange = 0f;
    
    /// <summary>
    /// 다음 공격 가능 시간
    /// </summary>
    private float _nextFireTime = 0f;

    public override void SubStart()
    {
        _pawnData = _pawnManager.PawnData;

        // CombatData 가져오기 또는 생성
        _pawnData.GetOrCreateCombatData();

        if (attackPoint == null)
        {
            // attackPoint가 지정되지 않았으면 자신의 Transform 사용
            attackPoint = this.transform;
        }
        
        // AttackInputEvent 구독 (선택적 - 수동 공격용)
        // _pawnManager.Subscribe<AttackInputEvent>(HandleAttackInput);
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        // if (_pawnManager != null)
        // {
        //     _pawnManager.Unsubscribe<AttackInputEvent>(HandleAttackInput);
        // }
    }

    public override void SubUpdate()
    {
        // CombatData가 없으면 무시
        if (_pawnData?.combatData == null) return;

        // 타겟 태그 결정: 설정되어 있으면 사용, 없으면 자동 결정
        string targetTag;
        if (!string.IsNullOrEmpty(_pawnData.combatData.targetTag))
        {
            // 설정에서 지정된 타겟 태그 사용
            targetTag = _pawnData.combatData.targetTag;
        }
        else
        {
            // 자동 결정: 자신의 태그에 따라 타겟 결정
            // Player 태그면 Enemy를 타겟, Enemy 태그면 Player를 타겟
            targetTag = _pawnManager.gameObject.CompareTag("Player") ? "Enemy" : "Player";
        }
        
        // 자동 공격 수행
        PerformInstantAttack(targetTag);
    }

    /// <summary>
    /// 수동 공격 입력 처리 (선택적)
    /// </summary>
    private void HandleAttackInput(AttackInputEvent evt)
    {
        if (evt.Attacker == this.gameObject)
        {
            string targetTag = _pawnManager.gameObject.CompareTag("Player") ? "Enemy" : "Player";
            PerformInstantAttack(targetTag);
        }
    }

    /// <summary>
    /// 즉발 공격을 수행합니다.
    /// </summary>
    private void PerformInstantAttack(string targetTag)
    {
        if (_pawnData?.combatData == null) return;

        float currentGameTime = GetGameTime(); // 게임 시간 사용 (정지 시 멈춤)
        
        // ✅ PawnStatCalculator를 사용하여 실제 스탯 계산
        PawnStatCalculator statCalculator = GameManager.Instance?.PawnStatCalculator;
        if (statCalculator == null) return;
        
        float effectiveFireRate = statCalculator.GetEffectiveFireRate(_pawnData);
        float effectiveDamage = statCalculator.GetEffectiveDamage(_pawnData);
        
        // ✅ 비즈니스 로직은 CombatUsecases에 위임
        bool attackSuccess = CombatUsecases.HandleInstantAttack(
            ref _nextFireTime, 
            effectiveFireRate, 
            attackPoint.position, 
            attackRange, 
            targetTag, 
            effectiveDamage, 
            currentGameTime, 
            _pawnManager.gameObject
        );
        
        // 공격 성공 시 추가 처리 (이펙트 등)
        if (attackSuccess)
        {
            // TODO: 나중에 이펙트 추가 시 여기서 처리
            // Debug.Log($"[InstantAttack] {_pawnManager.name} 즉발 공격 성공! 데미지: {effectiveDamage}");
        }
    }
}


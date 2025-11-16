using UnityEngine;
using PawnCore.Domain;
using PawnCore.Domain.Events;

/// <summary>
/// 피격 시 일시적인 무적 효과를 제공하는 SubManager입니다.
/// PawnDamagedEvent를 구독하여 피격 후 무적 시간을 활성화합니다.
/// 
/// 사용 예시: 플레이어 레시피에 추가하여 피격 후 1초간 무적 적용
/// </summary>
public class InvincibilitySubManager : PawnSubManager
{
    [Header("무적 설정")]
    [Tooltip("피격 후 무적 지속 시간 (초)")]
    public float invincibilityDuration = 1f;
    
    [Tooltip("무적 중 깜빡임 효과 속도")]
    public float blinkSpeed = 5f;
    
    private float _invincibilityTimer = 0f;
    private bool _isInvincible = false;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    /// <summary>
    /// 현재 무적 상태인지 여부입니다.
    /// </summary>
    public bool IsInvincible => _isInvincible;

    public override void SubStart()
    {
        // PawnDamagedEvent 구독 (일반 우선순위)
        _pawnManager.Subscribe<PawnDamagedEvent>(HandlePawnDamaged, EventPriority.Normal);
        
        // DamageEvent 구독 - 최고 우선순위로 데미지 차단
        // DamageableSubManager보다 먼저 실행되어 IsCancelled를 설정합니다.
        _pawnManager.Subscribe<DamageEvent>(HandleDamageEvent, EventPriority.Highest);
    }
    
    /// <summary>
    /// SpriteRenderer를 찾습니다. SubStart보다 늦게 호출되어야 VisualSubManager가 생성한 후입니다.
    /// </summary>
    private void FindSpriteRenderer()
    {
        if (_spriteRenderer != null) return; // 이미 찾았으면 리턴
        
        // 1. "Visuals" 자식에서 찾기 (VisualSubManager가 생성)
        Transform visualsChild = transform.Find("Visuals");
        if (visualsChild != null)
        {
            _spriteRenderer = visualsChild.GetComponent<SpriteRenderer>();
            Debug.Log($"[InvincibilitySubManager] {name}: Visuals 자식에서 SpriteRenderer 발견!");
        }
        
        // 2. 직접 찾기
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (_spriteRenderer != null)
            {
                Debug.Log($"[InvincibilitySubManager] {name}: GetComponentInChildren으로 SpriteRenderer 발견!");
            }
        }
        
        // 3. 찾았으면 원래 색상 저장
        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
            Debug.Log($"[InvincibilitySubManager] {name}: SpriteRenderer 색상 = {_originalColor}");
        }
        else
        {
            Debug.LogError($"[InvincibilitySubManager] {name}: SpriteRenderer를 찾을 수 없습니다! 하이어라키 구조 확인 필요!");
            
            // 디버깅: 모든 자식 출력
            Debug.Log($"[InvincibilitySubManager] 자식 오브젝트 목록:");
            foreach (Transform child in transform)
            {
                Debug.Log($"  - {child.name}");
            }
        }
    }

    private void OnDisable()
    {
        if (_pawnManager != null)
        {
            _pawnManager.Unsubscribe<PawnDamagedEvent>(HandlePawnDamaged);
            _pawnManager.Unsubscribe<DamageEvent>(HandleDamageEvent);
        }
    }

    public override void SubUpdate()
    {
        if (!_isInvincible) return;

        // SpriteRenderer를 아직 못 찾았으면 다시 찾기
        if (_spriteRenderer == null)
        {
            FindSpriteRenderer();
        }

        // 무적 타이머 감소
        _invincibilityTimer -= GetGameDeltaTime();

        // 깜빡임 효과
        if (_spriteRenderer != null)
        {
            float alpha = Mathf.PingPong(GetGameTime() * blinkSpeed, 1f);
            Color blinkColor = _originalColor;
            blinkColor.a = Mathf.Lerp(0.3f, 1f, alpha);
            _spriteRenderer.color = blinkColor;
        }

        // 무적 시간 종료
        if (_invincibilityTimer <= 0f)
        {
            EndInvincibility();
        }
    }

    /// <summary>
    /// 피격 이벤트 처리 - 무적 활성화
    /// </summary>
    private void HandlePawnDamaged(PawnDamagedEvent evt)
    {
        // 이 Pawn을 대상으로 한 피격인지 확인
        if (evt.Target != _pawnManager) return;

        // 사망 시 무적 비활성화 (이미 죽었으므로 의미 없음)
        if (evt.IsFatal)
        {
            EndInvincibility();
            return;
        }

        // 무적 활성화
        StartInvincibility();
    }

    /// <summary>
    /// 데미지 이벤트 처리 - 무적 중에는 데미지 무시
    /// EventPriority.Highest로 구독하여 DamageableSubManager보다 먼저 실행됩니다.
    /// </summary>
    private void HandleDamageEvent(DamageEvent evt)
    {
        // 이 Pawn을 대상으로 한 데미지인지 확인
        if (evt.Target != _pawnManager) return;

        // 무적 중이면 이벤트 취소
        if (_isInvincible)
        {
            evt.IsCancelled = true;
            Debug.Log($"{_pawnManager.name} is invincible! Damage ignored.");
        }
    }

    /// <summary>
    /// 무적 상태 시작
    /// </summary>
    private void StartInvincibility()
    {
        _isInvincible = true;
        _invincibilityTimer = invincibilityDuration;
        
        // SpriteRenderer 찾기 시도
        FindSpriteRenderer();
        
        Debug.Log($"[InvincibilitySubManager] {_pawnManager.name} is now invincible for {invincibilityDuration} seconds!");
    }

    /// <summary>
    /// 무적 상태 종료
    /// </summary>
    private void EndInvincibility()
    {
        _isInvincible = false;
        _invincibilityTimer = 0f;

        // 원래 색상으로 복구
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _originalColor;
        }

        Debug.Log($"{_pawnManager.name} invincibility ended.");
    }

    /// <summary>
    /// 외부에서 강제로 무적 상태를 활성화할 수 있습니다.
    /// </summary>
    public void ForceInvincibility(float duration)
    {
        invincibilityDuration = duration;
        StartInvincibility();
    }
}


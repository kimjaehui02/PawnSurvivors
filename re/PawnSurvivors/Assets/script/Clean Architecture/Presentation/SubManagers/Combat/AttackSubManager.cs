using UnityEngine;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Combat;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Utilities;

/// <summary>
/// 통합 공격 SubManager입니다.
/// AttackMethod(공격 방식)와 AttackPattern(공격 패턴)을 조합하여 다양한 공격을 수행합니다.
/// 
/// 예시:
/// - Method: Instant, Pattern: Burst → 즉발 2연타
/// - Method: Projectile, Pattern: Single → 투사체 단발
/// - Method: Projectile, Pattern: Burst → 투사체 2연발
/// </summary>
public class AttackSubManager : PawnSubManager
{
    [Header("Attack Configuration")]
    [Tooltip("공격 방식: Projectile(투사체), Instant(즉발)")]
    public AttackMethodType attackMethodType = AttackMethodType.Projectile;
    
    [Tooltip("공격 패턴: Single(단발), Burst(연타)")]
    public AttackPatternType attackPatternType = AttackPatternType.Single;
    
    [Header("Attack Settings")]
    [Tooltip("공격 시작 지점 (null이면 자신의 Transform 사용)")]
    public Transform attackPoint;
    
    [Tooltip("공격 범위 (0이면 무한)")]
    public float attackRange = 0f;
    
    [Header("Burst Pattern Settings")]
    [Tooltip("연타 횟수 (Burst 패턴일 때만 사용)")]
    public int burstCount = 2;
    
    [Tooltip("연타 간격 (초, Burst 패턴일 때만 사용)")]
    public float burstDelay = 0.1f;
    
    [Header("Range Visualization")]
    [Tooltip("공격 범위를 시각적으로 표시 (Area 공격만)")]
    public bool showRangeIndicator = false;
    
    [Tooltip("범위 표시 색상")]
    public Color rangeColor = new Color(1f, 1f, 1f, 0.3f);
    
    // Private fields
    private PawnData _pawnData;
    private float _nextFireTime = 0f;
    private IAttackMethod _attackMethod;
    private IAttackPattern _attackPattern;
    private GameObject _rangeIndicator;

    public override void SubStart()
    {
        _pawnData = _pawnManager.PawnData;

        // CombatData 가져오기 또는 생성
        _pawnData.GetOrCreateCombatData();

        if (attackPoint == null)
        {
            attackPoint = this.transform;
        }
        
        // AttackMethod 인스턴스 생성
        _attackMethod = CreateAttackMethod(attackMethodType);
        
        // AttackPattern 인스턴스 생성
        _attackPattern = CreateAttackPattern(attackPatternType);
        
        // 초기 상태 설정
        _nextFireTime = 0f;
        
        // Area 공격이고 범위 표시 옵션이 켜져있으면
        if (attackMethodType == AttackMethodType.Area && showRangeIndicator && attackRange > 0)
        {
            CreateRangeIndicator();
        }
    }
    
    /// <summary>
    /// 공격 범위를 시각적으로 표시하는 원을 생성합니다.
    /// </summary>
    private void CreateRangeIndicator()
    {
        // 범위 표시용 자식 GameObject 생성
        _rangeIndicator = new GameObject("RangeIndicator");
        _rangeIndicator.transform.SetParent(transform);
        _rangeIndicator.transform.localPosition = Vector3.zero;
        
        // SpriteRenderer 추가
        SpriteRenderer spriteRenderer = _rangeIndicator.AddComponent<SpriteRenderer>();
        
        // 원형 스프라이트 생성 (Unity 기본 스프라이트 사용)
        Sprite circleSprite = Resources.Load<Sprite>("Sprites/Circle");
        if (circleSprite == null)
        {
            // 기본 Circle 스프라이트가 없으면 임시로 생성
            Texture2D texture = new Texture2D(256, 256);
            Color[] pixels = new Color[256 * 256];
            Vector2 center = new Vector2(128, 128);
            
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = distance < 120 ? 1f : 0f;
                    pixels[y * 256 + x] = new Color(1, 1, 1, alpha);
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            circleSprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f), 100f);
        }
        
        spriteRenderer.sprite = circleSprite;
        spriteRenderer.color = rangeColor;
        
        // 범위에 맞게 스케일 조정 (픽셀 퍼 유닛 고려)
        // Unity 기본 Circle 스프라이트는 256x256 픽셀, PPU=100
        // 따라서 실제 크기는 2.56 유닛
        // attackRange에 맞추려면: scale = (attackRange * 2) / 2.56
        float targetDiameter = attackRange * 2f; // 지름
        float baseSize = 2.56f; // 기본 스프라이트 크기
        float scale = targetDiameter / baseSize;
        _rangeIndicator.transform.localScale = new Vector3(scale, scale, 1f);
        
        // 렌더 순서 조정 (타일맵보다 앞에, 캐릭터보다 뒤에)
        spriteRenderer.sortingOrder = -1;
    }
    
    private void OnDestroy()
    {
        // 범위 표시 정리
        if (_rangeIndicator != null)
        {
            Destroy(_rangeIndicator);
        }
    }
    
    /// <summary>
    /// 외부에서 공격 상태를 리셋할 수 있는 public 메서드입니다.
    /// 다음 스테이지 시작 시 GameManager에서 호출됩니다.
    /// </summary>
    public void ResetForNewStage()
    {
        _nextFireTime = 0f; // 즉시 공격 가능
        
        if (_attackPattern != null)
        {
            _attackPattern.Reset(); // Pattern 초기화
        }
        
        Debug.Log($"[AttackSubManager] {gameObject.name} 공격 상태 초기화 - Method: {attackMethodType}, Pattern: {attackPatternType}");
    }

    public override void SubUpdate()
    {
        // CombatData가 없으면 무시
        if (_pawnData?.combatData == null)
        {
            Debug.LogWarning($"[AttackSubManager] {gameObject.name} CombatData is null!");
            return;
        }
        
        // AttackMethod/Pattern 체크
        if (_attackMethod == null || _attackPattern == null)
        {
            Debug.LogWarning($"[AttackSubManager] {gameObject.name} Method={_attackMethod != null}, Pattern={_attackPattern != null}");
            return;
        }

        float currentGameTime = GetGameTime();
        
        // 패턴이 완료되었으면 다음 공격 사이클 시작 체크
        if (_attackPattern != null && _attackPattern.IsComplete)
        {
            // 공격 속도 체크 (기존 패턴과 동일)
            if (currentGameTime >= _nextFireTime)
            {
                // 새로운 공격 사이클 시작
                StartNewAttackCycle(currentGameTime);
            }
        }
        else if (_attackPattern != null && !_attackPattern.IsComplete)
        {
            // 패턴 진행 중 - Update 호출
            UpdateAttackPattern(currentGameTime);
        }
        else
        {
            // 첫 공격 시작
            StartNewAttackCycle(currentGameTime);
        }
    }

    /// <summary>
    /// 새로운 공격 사이클을 시작합니다.
    /// </summary>
    private void StartNewAttackCycle(float currentGameTime)
    {
        if (_attackMethod == null || _attackPattern == null) return;

        // ✅ PawnStatCalculator를 사용하여 실제 스탯 계산
        PawnStatCalculator statCalculator = GameManager.Instance?.PawnStatCalculator;
        if (statCalculator == null) return;
        
        float effectiveFireRate = statCalculator.GetEffectiveFireRate(_pawnData);
        
        // 패턴 시작
        _attackPattern.Start(currentGameTime);
        
        // 다음 공격 사이클 시간 갱신 (기존 패턴과 동일)
        _nextFireTime = currentGameTime + 1f / effectiveFireRate;
    }

    /// <summary>
    /// 공격 패턴을 진행합니다.
    /// </summary>
    private void UpdateAttackPattern(float currentGameTime)
    {
        if (_attackMethod == null || _attackPattern == null) return;

        // ✅ PawnStatCalculator를 사용하여 실제 스탯 계산
        PawnStatCalculator statCalculator = GameManager.Instance?.PawnStatCalculator;
        if (statCalculator == null) return;
        
        float effectiveDamage = statCalculator.GetEffectiveDamage(_pawnData);
        string targetTag = GetTargetTag();
        
        // 패턴 업데이트 (기존 패턴과 동일: SubUpdate에서 시간 체크)
        _attackPattern.Update(
            currentGameTime,
            _attackMethod,
            _pawnData,
            attackPoint,
            targetTag,
            effectiveDamage,
            attackRange,
            _pawnManager.gameObject,
            _pawnManager
        );
    }

    /// <summary>
    /// 타겟 태그를 결정합니다.
    /// </summary>
    private string GetTargetTag()
    {
        if (!string.IsNullOrEmpty(_pawnData.combatData.targetTag))
        {
            return _pawnData.combatData.targetTag;
        }
        
        // 자동 결정
        return TagHelper.GetOppositeTag(_pawnManager.gameObject);
    }

    /// <summary>
    /// AttackMethodType에 따라 IAttackMethod 인스턴스를 생성합니다.
    /// </summary>
    private IAttackMethod CreateAttackMethod(AttackMethodType type)
    {
        switch (type)
        {
            case AttackMethodType.Projectile:
                return new ProjectileAttackMethod();
            case AttackMethodType.Instant:
                return new InstantAttackMethod();
            case AttackMethodType.Area:
                return new AreaAttackMethod();
            default:
                Debug.LogWarning($"[AttackSubManager] Unknown AttackMethodType: {type}. Using ProjectileAttackMethod.");
                return new ProjectileAttackMethod();
        }
    }

    /// <summary>
    /// AttackPatternType에 따라 IAttackPattern 인스턴스를 생성합니다.
    /// </summary>
    private IAttackPattern CreateAttackPattern(AttackPatternType type)
    {
        switch (type)
        {
            case AttackPatternType.Single:
                return new SingleAttackPattern();
            case AttackPatternType.Burst:
                return new BurstAttackPattern(burstCount, burstDelay);
            default:
                Debug.LogWarning($"[AttackSubManager] Unknown AttackPatternType: {type}. Using SingleAttackPattern.");
                return new SingleAttackPattern();
        }
    }
}

/// <summary>
/// 공격 방식 타입
/// </summary>
public enum AttackMethodType
{
    Projectile, // 투사체
    Instant,    // 즉발
    Area,       // 범위 (주변 모든 적 타격)
}

/// <summary>
/// 공격 패턴 타입
/// </summary>
public enum AttackPatternType
{
    Single,  // 단발
    Burst,   // 연타
}


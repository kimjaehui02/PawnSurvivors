using UnityEngine;

/// <summary>
/// 이동 중일 때 콩콩 뛰는 바운스 애니메이션 전략입니다.
/// Visuals 자식만 Y축으로 움직여서 실제 충돌/물리에는 영향을 주지 않습니다.
/// </summary>
public class BounceAnimationStrategy : AnimationStrategyBase
{
    [Header("바운스 설정")]
    [Tooltip("바운스 높이")]
    public float bounceHeight = 0.1f;
    
    [Tooltip("바운스 속도 (높을수록 빠름)")]
    public float bounceSpeed = 10f;
    
    [Tooltip("이동 시작으로 간주할 최소 속도")]
    public float movementThreshold = 0.1f;

    private Vector3 _lastPosition;
    private float _bounceTimer = 0f;

    public override void Init(PawnManager pawnManager, GameObject visualsObject)
    {
        base.Init(pawnManager, visualsObject);
        _lastPosition = _pawnManager.transform.position;
    }

    public override void Animate()
    {
        if (_visualsObject == null) return;

        bool isMoving = false;
        
        // 현재 위치와 이전 프레임 위치 차이로 이동 체크
        Vector3 currentPosition = _pawnManager.transform.position;
        float positionDelta = Vector3.Distance(currentPosition, _lastPosition);
        float deltaTime = GetGameDeltaTime();
        
        // deltaTime이 0이면 (정지 중) 속도 계산 건너뛰기
        float speed = 0f;
        if (deltaTime > 0f)
        {
            speed = positionDelta / deltaTime; // 속도 계산 (단위: units/sec)
        }
        
        isMoving = speed > movementThreshold;
        
        // 다음 프레임을 위해 위치 저장
        _lastPosition = currentPosition;
        
        if (isMoving)
        {
            // 이동 중: 바운스 타이머 증가
            _bounceTimer += GetGameDeltaTime() * bounceSpeed;
            
            // Sine Wave로 상하 움직임 (0 ~ bounceHeight)
            float yOffset = Mathf.Abs(Mathf.Sin(_bounceTimer)) * bounceHeight;
            _visualsObject.transform.localPosition = new Vector3(0f, yOffset, 0f);
        }
        else
        {
            // 정지 중: 원위치로 부드럽게 복귀
            Vector3 currentPos = _visualsObject.transform.localPosition;
            if (currentPos.y > 0.01f)
            {
                // Lerp로 부드럽게 내려오기
                float newY = Mathf.Lerp(currentPos.y, 0f, GetGameDeltaTime() * bounceSpeed);
                _visualsObject.transform.localPosition = new Vector3(0f, newY, 0f);
            }
            else
            {
                // 거의 0에 가까우면 정확히 0으로
                _visualsObject.transform.localPosition = Vector3.zero;
                _bounceTimer = 0f; // 타이머 초기화
            }
        }
    }
}


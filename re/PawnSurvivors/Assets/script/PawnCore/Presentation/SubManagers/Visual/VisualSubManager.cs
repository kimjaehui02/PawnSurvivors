using UnityEngine;
using PawnCore.Domain;

namespace PawnCore.Presentation.SubManagers.Visual
{
    public class VisualSubManager : PawnSubManager
    {
        private PawnData _pawnData;
        private SpriteRenderer _spriteRenderer; // SpriteRenderer 참조 추가
        private GameObject _shadowObject;
        private SpriteRenderer _shadowRenderer;
        private GameObject _visualsObject; // Visuals 게임오브젝트 참조
        private Rigidbody2D _rigidbody2D; // 이동 체크용 (옵션)
        private Vector3 _lastPosition; // 이전 프레임 위치
        
        [Header("바운스 애니메이션 설정")]
        [Tooltip("바운스 효과 활성화")]
        public bool enableBounce = true;
        
        [Tooltip("바운스 높이")]
        public float bounceHeight = 0.1f;
        
        [Tooltip("바운스 속도 (높을수록 빠름)")]
        public float bounceSpeed = 10f;
        
        [Tooltip("이동 시작으로 간주할 최소 속도")]
        public float movementThreshold = 0.1f;
        
        private float _bounceTimer = 0f;

        public override void SubStart()
        {
            _pawnData = _pawnManager.PawnData;

            // VisualData 가져오기 또는 생성
            _pawnData.GetOrCreateVisualData();

            // 그림자 프리셋 로드 및 생성
            if (!string.IsNullOrEmpty(_pawnData.visualData.shadowPresetName))
            {
                LoadAndCreateShadow();
            }

            // "Visuals" 라는 이름의 자식 게임오브젝트 생성
            _visualsObject = new GameObject("Visuals");
            _visualsObject.transform.SetParent(_pawnManager.transform);
            _visualsObject.transform.localPosition = Vector3.zero; // 위치 초기화

            // 자식 오브젝트에 SpriteRenderer 추가 또는 가져오기
            _spriteRenderer = _visualsObject.AddComponent<SpriteRenderer>();
            
            // Rigidbody2D는 SubUpdate에서 찾기 (PhysicsSubManager가 나중에 추가할 수 있음)
            
            // 이전 프레임 위치 초기화
            _lastPosition = _pawnManager.transform.position;

            Sprite visualSprite = null;
            
            // 지정된 스프라이트 로드 시도
            if (!string.IsNullOrEmpty(_pawnData.visualData.visualSpriteName))
            {
                // 인덱스 기반 로딩 (visualSpriteIndex >= 0)
                if (_pawnData.visualData.visualSpriteIndex >= 0)
                {
                    Sprite[] allSprites = Resources.LoadAll<Sprite>(_pawnData.visualData.visualSpriteName);
                    
                    if (allSprites != null && allSprites.Length > _pawnData.visualData.visualSpriteIndex)
                    {
                        visualSprite = allSprites[_pawnData.visualData.visualSpriteIndex];
                        // Debug.Log($"VisualSubManager: '{_pawnData.visualData.visualSpriteName}'의 인덱스 {_pawnData.visualData.visualSpriteIndex} 스프라이트 로드 완료! (총 {allSprites.Length}개 슬라이스)", this);
                    }
                    else
                    {
                        Debug.LogWarning($"VisualSubManager: '{_pawnData.visualData.visualSpriteName}'의 인덱스 {_pawnData.visualData.visualSpriteIndex}를 찾을 수 없습니다. (총 {allSprites?.Length ?? 0}개 슬라이스). 기본 원 스프라이트를 사용합니다.", this);
                    }
                }
                // 이름 기반 로딩 (visualSpriteIndex == -1)
                else
                {
                    visualSprite = Resources.Load<Sprite>(_pawnData.visualData.visualSpriteName);
                    
                    if (visualSprite == null)
                    {
                        // 슬라이스된 스프라이트를 찾을 수 없는 경우, 부모 이미지에서 첫 번째 스프라이트 시도
                        string basePath = _pawnData.visualData.visualSpriteName;
                        
                        // "Temporary/mini_0" → "Temporary/mini"로 변환
                        if (basePath.Contains("_"))
                        {
                            int underscoreIndex = basePath.LastIndexOf('_');
                            basePath = basePath.Substring(0, underscoreIndex);
                        }
                        
                        // 모든 슬라이스 로드 시도
                        Sprite[] allSprites = Resources.LoadAll<Sprite>(basePath);
                        
                        if (allSprites != null && allSprites.Length > 0)
                        {
                            visualSprite = allSprites[0]; // 첫 번째 슬라이스 사용
                            // Debug.Log($"VisualSubManager: '{_pawnData.visualData.visualSpriteName}'를 찾을 수 없어 '{basePath}'의 첫 번째 스프라이트를 사용합니다. (총 {allSprites.Length}개 슬라이스)", this);
                        }
                        else
                        {
                            Debug.LogWarning($"VisualSubManager: '{_pawnData.visualData.visualSpriteName}' 및 '{basePath}' 스프라이트를 찾을 수 없습니다. 기본 원 스프라이트를 사용합니다.", this);
                        }
                    }
                }
            }

            // 찾을 수 없거나 지정되지 않은 경우 기본 원 스프라이트로 대체
            if (visualSprite == null)
            {
                visualSprite = Resources.Load<Sprite>("Temporary/Circle");
                
                if (visualSprite == null)
                {
                    Debug.LogError("VisualSubManager: 'Resources/Temporary/Circle'에서 기본 원 스프라이트를 찾을 수 없습니다. 시각적 개체가 인스턴스화되지 않습니다.", this);
                    return; // 렌더링할 스프라이트 없음
                }
                
                // Debug.Log($"{_pawnManager.name}: 기본 원 스프라이트를 사용합니다.");
            }

            _spriteRenderer.sprite = visualSprite;
            _spriteRenderer.color = _pawnData.visualData.visualColor; // 색상 적용
            
            // 시각적 스케일 적용
            _visualsObject.transform.localScale = _pawnData.visualData.visualScale;
        }

        /// <summary>
        /// 그림자 프리셋을 로드하고 그림자를 생성합니다.
        /// </summary>
        private void LoadAndCreateShadow()
        {
            // 프리셋 로드
            _pawnData.visualData.shadowPreset = GameManager.Instance.CreationManager.GetShadowPreset(_pawnData.visualData.shadowPresetName);
            
            if (_pawnData.visualData.shadowPreset == null)
            {
                Debug.LogWarning($"VisualSubManager: ShadowPreset '{_pawnData.visualData.shadowPresetName}'를 찾을 수 없습니다.");
                return;
            }
            
            CreateShadow();
        }

        /// <summary>
        /// 그림자를 생성합니다.
        /// </summary>
        private void CreateShadow()
        {
            ShadowPresetData preset = _pawnData.visualData.shadowPreset;
            if (preset == null) return;

            // "Shadow" 자식 GameObject 생성
            _shadowObject = new GameObject("Shadow");
            _shadowObject.transform.SetParent(_pawnManager.transform);
            _shadowObject.transform.localPosition = preset.shadowOffset;

            // SpriteRenderer 추가
            _shadowRenderer = _shadowObject.AddComponent<SpriteRenderer>();

            // 타원형 그림자용 스프라이트 (기본 Circle 사용)
            Sprite shadowSprite = Resources.Load<Sprite>("Temporary/Circle");
            if (shadowSprite == null)
            {
                Debug.LogWarning("VisualSubManager: 그림자용 Circle 스프라이트를 찾을 수 없습니다.");
                return;
            }

            _shadowRenderer.sprite = shadowSprite;
            _shadowRenderer.color = preset.shadowColor;
            _shadowRenderer.sortingOrder = -10; // 모든 것 아래에 렌더링
            
            // 타원형으로 스케일 조정 (x: 가로, y: 세로)
            _shadowObject.transform.localScale = new Vector3(
                preset.shadowScale.x,
                preset.shadowScale.y,
                1f
            );
        }

        public override void SubUpdate()
        {
            // 바운스 애니메이션
            if (enableBounce && _visualsObject != null)
            {
                UpdateBounceAnimation();
            }
        }
        
        /// <summary>
        /// 이동 중일 때 콩콩 뛰는 바운스 애니메이션을 업데이트합니다.
        /// Visuals 자식만 Y축으로 움직여서 실제 충돌/물리에는 영향을 주지 않습니다.
        /// </summary>
        private void UpdateBounceAnimation()
        {
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
            
            // 디버그 로그 (1초에 한 번)
            // if (Time.frameCount % 60 == 0)
            // {
            //     // Debug.Log($"[Bounce] {_pawnManager.name}: speed={speed:F2}, positionDelta={positionDelta:F3}, isMoving={isMoving}, enableBounce={enableBounce}");
            // }
            
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
}
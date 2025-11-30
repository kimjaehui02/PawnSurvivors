using System.Linq;
using UnityEngine;
using PawnSurvivors.Domain;

namespace PawnSurvivors.Presentation.SubManagers.Visual
{
    /// <summary>
    /// Pawn의 시각적 표현을 관리하는 SubManager입니다.
    /// 여러 애니메이션 전략을 관리합니다.
    /// </summary>
    public class VisualSubManager : PawnSubManager
    {
        private PawnData _pawnData;
        private SpriteRenderer _spriteRenderer; // SpriteRenderer 참조 추가
        private GameObject _shadowObject;
        private SpriteRenderer _shadowRenderer;
        private GameObject _visualsObject; // Visuals 게임오브젝트 참조
        private AnimationStrategyBase _currentStrategy;

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
                visualSprite = Resources.Load<Sprite>("Sprites/ex/Circle");
                
                if (visualSprite == null)
                {
                    Debug.LogError("VisualSubManager: 'Resources/Sprites/ex/Circle'에서 기본 원 스프라이트를 찾을 수 없습니다. 시각적 개체가 인스턴스화되지 않습니다.", this);
                    return; // 렌더링할 스프라이트 없음
                }
                
                // Debug.Log($"{_pawnManager.name}: 기본 원 스프라이트를 사용합니다.");
            }

            _spriteRenderer.sprite = visualSprite;
            _spriteRenderer.color = _pawnData.visualData.visualColor; // 색상 적용
            
            // 시각적 스케일 적용
            _visualsObject.transform.localScale = _pawnData.visualData.visualScale;

            // 애니메이션 전략 초기화
            InitializeAnimationStrategies();
        }

        /// <summary>
        /// 애니메이션 전략들을 초기화합니다.
        /// </summary>
        private void InitializeAnimationStrategies()
        {
            var allStrategies = GetComponents<AnimationStrategyBase>();

            // PawnManager와 VisualsObject에 대한 참조로 모든 전략 초기화
            foreach (var strategy in allStrategies)
            {
                strategy.Init(_pawnManager, _visualsObject);
            }

            // 초기에 활성화된 전략 찾기 (JSON 레시피로 설정)
            _currentStrategy = allStrategies.FirstOrDefault(s => s.enabled);

            if (_currentStrategy == null && allStrategies.Length > 0)
            {
                // 활성화된 것이 없으면 첫 번째 것으로 기본 설정하고 활성화합니다.
                _currentStrategy = allStrategies[0];
                _currentStrategy.enabled = true;
            }

            // 현재 전략만 활성 상태인지 확인합니다. 실수로 여러 개가 활성화된 경우 중요합니다.
            foreach (var strategy in allStrategies)
            {
                if (strategy != _currentStrategy)
                {
                    strategy.enabled = false;
                }
            }
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
            Sprite shadowSprite = Resources.Load<Sprite>("Sprites/ex/Circle");
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
            // 현재 활성화된 애니메이션 전략 실행
            if (_currentStrategy != null && _currentStrategy.enabled)
            {
                _currentStrategy.Animate();
            }
        }
    }
}
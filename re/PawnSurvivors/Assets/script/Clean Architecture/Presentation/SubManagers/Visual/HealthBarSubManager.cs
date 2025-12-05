using UnityEngine;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Events;

namespace PawnSurvivors.Presentation.SubManagers.Visual
{
    /// <summary>
    /// Pawn 위에 체력바를 표시하는 SubManager입니다.
    /// SpriteRenderer를 사용하여 간단하고 가볍게 구현됩니다.
    /// </summary>
    public class HealthBarSubManager : PawnSubManager
    {
        [Header("Health Bar Settings")]
        [Tooltip("체력바 위치 오프셋 (Pawn 위 상대 위치)")]
        public Vector3 barOffset = new Vector3(0f, 1.5f, 0f);
        
        [Tooltip("체력바 너비")]
        public float barWidth = 2f;
        
        [Tooltip("체력바 높이")]
        public float barHeight = 0.3f;
        
        [Tooltip("배경 색상")]
        public Color backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        
        [Tooltip("체력 색상")]
        public Color healthColor = new Color(0.2f, 0.8f, 0.2f, 1f);
        
        [Tooltip("낮은 체력 색상 (30% 이하)")]
        public Color lowHealthColor = new Color(0.9f, 0.2f, 0.2f, 1f);
        
        // Private fields
        private GameObject _barBackground;
        private GameObject _barForeground;
        private SpriteRenderer _backgroundRenderer;
        private SpriteRenderer _foregroundRenderer;
        private PawnData _pawnData;

        public override void SubStart()
        {
            _pawnData = _pawnManager.PawnData;
            
            if (_pawnData?.healthData == null)
            {
                Debug.LogWarning($"[HealthBarSubManager] {gameObject.name} HealthData가 없어 체력바를 생성할 수 없습니다.");
                return;
            }
            
            CreateHealthBar();
            
            // PawnDamagedEvent 구독 (체력 변화 감지)
            _pawnManager.Subscribe<PawnDamagedEvent>(OnPawnDamaged);
        }

        private void OnDisable()
        {
            if (_pawnManager != null)
            {
                _pawnManager.Unsubscribe<PawnDamagedEvent>(OnPawnDamaged);
            }
        }

        public override void SubUpdate()
        {
            // 체력바는 이벤트 기반으로 업데이트되므로 SubUpdate 불필요
        }

        /// <summary>
        /// 체력바를 생성합니다.
        /// </summary>
        private void CreateHealthBar()
        {
            // 배경 생성
            _barBackground = new GameObject("HealthBarBackground");
            _barBackground.transform.SetParent(transform);
            _barBackground.transform.localPosition = barOffset;
            _barBackground.transform.localRotation = Quaternion.identity;
            _barBackground.transform.localScale = new Vector3(barWidth, barHeight, 1f);
            
            _backgroundRenderer = _barBackground.AddComponent<SpriteRenderer>();
            _backgroundRenderer.sprite = CreateSquareSprite();
            _backgroundRenderer.color = backgroundColor;
            _backgroundRenderer.sortingOrder = 10; // 캐릭터보다 앞
            
            // 체력 바 (foreground)
            _barForeground = new GameObject("HealthBarForeground");
            _barForeground.transform.SetParent(_barBackground.transform);
            _barForeground.transform.localPosition = Vector3.zero;
            _barForeground.transform.localRotation = Quaternion.identity;
            _barForeground.transform.localScale = Vector3.one; // 배경과 같은 크기
            
            _foregroundRenderer = _barForeground.AddComponent<SpriteRenderer>();
            _foregroundRenderer.sprite = CreateSquareSprite();
            _foregroundRenderer.color = healthColor;
            _foregroundRenderer.sortingOrder = 11; // 배경보다 앞
            
            UpdateHealthBar();
        }

        /// <summary>
        /// 체력바를 업데이트합니다.
        /// </summary>
        private void UpdateHealthBar()
        {
            if (_pawnData?.healthData == null || _foregroundRenderer == null) return;
            
            float healthPercent = _pawnData.healthData.currentHealth / _pawnData.healthData.maxHealth;
            healthPercent = Mathf.Clamp01(healthPercent);
            
            // X 스케일만 조정 (왼쪽에서 오른쪽으로 채워짐)
            Vector3 scale = _barForeground.transform.localScale;
            scale.x = healthPercent;
            _barForeground.transform.localScale = scale;
            
            // 위치 조정 (왼쪽 정렬)
            Vector3 pos = _barForeground.transform.localPosition;
            pos.x = -(1f - healthPercent) * 0.5f;
            _barForeground.transform.localPosition = pos;
            
            // 낮은 체력 색상 변경
            if (healthPercent <= 0.3f)
            {
                _foregroundRenderer.color = lowHealthColor;
            }
            else
            {
                _foregroundRenderer.color = healthColor;
            }
        }

        /// <summary>
        /// Pawn이 데미지를 받았을 때 호출됩니다.
        /// </summary>
        private void OnPawnDamaged(PawnDamagedEvent evt)
        {
            if (evt.Target == _pawnManager)
            {
                UpdateHealthBar();
            }
        }

        /// <summary>
        /// 1x1 사각형 스프라이트를 생성합니다.
        /// </summary>
        private Sprite CreateSquareSprite()
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }
    }
}


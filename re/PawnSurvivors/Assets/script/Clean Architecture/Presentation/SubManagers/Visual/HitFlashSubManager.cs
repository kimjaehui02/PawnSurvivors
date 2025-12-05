using UnityEngine;
using PawnSurvivors.Domain.Events;

namespace PawnSurvivors.Presentation.SubManagers.Visual
{
    /// <summary>
    /// 피격 시 스프라이트를 흰색으로 번쩍이게 하는 SubManager입니다.
    /// Sprites/FlashEffect 셰이더의 _FlashAmount 프로퍼티를 사용합니다.
    /// </summary>
    public class HitFlashSubManager : PawnSubManager
    {
        [Header("피격 효과 설정")]
        [Tooltip("피격 시 흰색으로 변하는 지속 시간 (초)")]
        public float flashDuration = 0.1f;
        
        [Tooltip("피격 시 번쩍이는 색상")]
        public Color flashColor = Color.white;
        
        private SpriteRenderer _spriteRenderer;
        private Material _material;
        private Shader _flashShader;
        private Shader _originalShader;
        private float _flashTimer = 0f;
        private bool _isFlashing = false;
        
        private static readonly int FlashAmountProperty = Shader.PropertyToID("_FlashAmount");
        private static readonly int FlashColorProperty = Shader.PropertyToID("_FlashColor");

        public override void SubStart()
        {
            // PawnDamagedEvent 구독 (낮은 우선순위 - 데미지 처리 후 효과)
            _pawnManager.Subscribe<PawnDamagedEvent>(HandlePawnDamaged, EventPriority.Low);
        }

        private void OnDisable()
        {
            if (_pawnManager != null)
            {
                _pawnManager.Unsubscribe<PawnDamagedEvent>(HandlePawnDamaged);
            }
            
            // Shader 복구
            if (_material != null && _originalShader != null)
            {
                _material.shader = _originalShader;
            }
        }

        public override void SubUpdate()
        {
            if (!_isFlashing) return;

            _flashTimer -= GetGameDeltaTime();

            if (_flashTimer <= 0f)
            {
                EndFlash();
            }
        }

        /// <summary>
        /// SpriteRenderer를 찾고 Flash Shader를 설정합니다.
        /// </summary>
        private void SetupFlashShader()
        {
            if (_material != null) return; // 이미 설정됨

            // 1. SpriteRenderer 찾기
            Transform visualsChild = transform.Find("Visuals");
            if (visualsChild != null)
            {
                _spriteRenderer = visualsChild.GetComponent<SpriteRenderer>();
            }

            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (_spriteRenderer == null)
            {
                Debug.LogError($"[HitFlashSubManager] {name}: SpriteRenderer를 찾을 수 없습니다!");
                return;
            }

            // 2. Flash Shader 로드
            _flashShader = Shader.Find("Sprites/FlashEffect");
            if (_flashShader == null)
            {
                Debug.LogError($"[HitFlashSubManager] {name}: Sprites/FlashEffect 셰이더를 찾을 수 없습니다!");
                return;
            }

            // 3. Material 가져오기 (인스턴스 생성)
            _material = _spriteRenderer.material; // 자동으로 복사본 생성
            _originalShader = _material.shader;

            // 4. Flash Shader로 변경
            _material.shader = _flashShader;
            _material.SetFloat(FlashAmountProperty, 0f);
            _material.SetColor(FlashColorProperty, flashColor);

            // Debug.Log($"[HitFlashSubManager] {name}: Flash Shader 설정 완료!");
        }

        /// <summary>
        /// 피격 이벤트 처리 - 흰색 번쩍임
        /// </summary>
        private void HandlePawnDamaged(PawnDamagedEvent evt)
        {
            // 이 Pawn을 대상으로 한 피격인지 확인
            if (evt.Target != _pawnManager) return;

            // 사망 시 효과 없음
            if (evt.IsFatal) return;

            // 흰색 번쩍임 시작
            StartFlash();
        }

        /// <summary>
        /// 흰색 번쩍임 시작
        /// </summary>
        private void StartFlash()
        {
            // Flash Shader 설정
            SetupFlashShader();

            if (_material == null)
            {
                Debug.LogWarning($"[HitFlashSubManager] {name}: Material을 설정할 수 없습니다.");
                return;
            }

            _isFlashing = true;
            _flashTimer = flashDuration;

            // FlashAmount를 1로 설정 → 완전히 하얗게!
            _material.SetFloat(FlashAmountProperty, 1f);
        }

        /// <summary>
        /// 흰색 번쩍임 종료 - FlashAmount를 0으로
        /// </summary>
        private void EndFlash()
        {
            _isFlashing = false;
            _flashTimer = 0f;

            if (_material != null)
            {
                _material.SetFloat(FlashAmountProperty, 0f);
            }
        }

        /// <summary>
        /// 외부에서 강제로 번쩍임을 활성화할 수 있습니다.
        /// </summary>
        public void ForceFlash(float duration)
        {
            flashDuration = duration;
            StartFlash();
        }
    }
}


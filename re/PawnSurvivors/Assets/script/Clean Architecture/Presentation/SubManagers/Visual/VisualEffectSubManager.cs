using UnityEngine;
using PawnSurvivors.Domain.Events;

namespace PawnSurvivors.Presentation.SubManagers.Visual
{
    /// <summary>
    /// 피격 시 깜빡임 등 시각적 효과를 담당하는 SubManager입니다.
    /// VisualSubManager가 생성한 SpriteRenderer에 효과를 적용합니다.
    /// </summary>
    public class VisualEffectSubManager : PawnSubManager
    {
        [Header("깜빡임 효과 설정")]
        [Tooltip("깜빡임 효과 속도")]
        public float blinkSpeed = 5f;
        
        [Tooltip("깜빡임 시 최소 투명도 (0-1)")]
        public float minAlpha = 0.3f;
        
        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private bool _isBlinking = false;
        private float _blinkEndTime = 0f;

        public override void SubStart()
        {
            // SpriteRenderer 찾기 - Visuals 자식 오브젝트에서
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            if (_spriteRenderer != null)
            {
                _originalColor = _spriteRenderer.color;
            }
            else
            {
                Debug.LogWarning($"[VisualEffectSubManager] {name}: SpriteRenderer를 찾을 수 없습니다.");
            }
        }

        public override void SubUpdate()
        {
            if (!_isBlinking || _spriteRenderer == null) return;

            float currentTime = GetGameTime();
            
            // 깜빡임 시간 체크
            if (currentTime >= _blinkEndTime)
            {
                StopBlinking();
                return;
            }

            // 깜빡임 효과
            float alpha = Mathf.PingPong(currentTime * blinkSpeed, 1f);
            Color blinkColor = _originalColor;
            blinkColor.a = Mathf.Lerp(minAlpha, 1f, alpha);
            _spriteRenderer.color = blinkColor;
        }

        /// <summary>
        /// 깜빡임 효과를 시작합니다.
        /// </summary>
        public void StartBlinking(float duration)
        {
            if (_spriteRenderer == null)
            {
                Debug.LogWarning($"[VisualEffectSubManager] {name}: SpriteRenderer가 없어 깜빡임 효과를 시작할 수 없습니다.");
                return;
            }

            _isBlinking = true;
            _blinkEndTime = GetGameTime() + duration;
        }

        /// <summary>
        /// 깜빡임 효과를 중지합니다.
        /// </summary>
        public void StopBlinking()
        {
            _isBlinking = false;

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _originalColor;
            }
        }

        /// <summary>
        /// 색상을 임시로 변경합니다.
        /// </summary>
        public void SetTemporaryColor(Color color, float duration)
        {
            if (_spriteRenderer == null) return;

            _spriteRenderer.color = color;
            Invoke(nameof(ResetColor), duration);
        }

        private void ResetColor()
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _originalColor;
            }
        }

        /// <summary>
        /// 원래 색상을 업데이트합니다. (무적 깜빡임 종료 후 등)
        /// </summary>
        public void RefreshOriginalColor()
        {
            if (_spriteRenderer != null)
            {
                _originalColor = _spriteRenderer.color;
            }
        }
    }
}


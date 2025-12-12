using UnityEngine;
using PawnSurvivors.Domain.Events;

namespace PawnSurvivors.Presentation.SubManagers.Visual
{
    /// <summary>
    /// Pawn 사망 시 시각적 효과를 재생하는 SubManager입니다.
    /// 기본적으로 스프라이트 페이드아웃 + 스케일 효과를 제공하며,
    /// 파티클 프리팹이 설정되면 파티클을 재생합니다.
    /// LifecycleManager와 호환되도록 SubUpdate에서 효과를 처리합니다.
    /// </summary>
    public class DeathEffectSubManager : PawnSubManager
    {
        [Header("기본 효과 설정")]
        [Tooltip("페이드아웃 사용 여부")]
        public bool useFadeOut = true;

        [Tooltip("페이드아웃 지속 시간")]
        public float fadeOutDuration = 0.3f;

        [Tooltip("사망 시 스케일 변화 (1.0 = 변화 없음, 1.5 = 1.5배로 커짐)")]
        public float scaleMultiplier = 1.2f;

        [Tooltip("사망 시 색상 변화 (빨간색 번쩍임 등)")]
        public Color deathTintColor = Color.white;

        [Header("파티클 설정 (선택)")]
        [Tooltip("파티클 프리팹 경로 (Resources 폴더 기준, 비어있으면 기본 효과만 사용)")]
        public string particlePrefabPath = "";

        [Tooltip("파티클 지속 시간")]
        public float particleDuration = 1f;

        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private Vector3 _originalScale;

        // 사망 효과 상태
        private bool _isDying = false;
        private float _deathElapsed = 0f;
        private Color _deathStartColor;
        private Vector3 _deathTargetScale;

        public override void SubStart()
        {
            // SpriteRenderer 찾기
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (_spriteRenderer != null)
            {
                _originalColor = _spriteRenderer.color;
                _originalScale = _spriteRenderer.transform.localScale;
            }

            // PawnDeathEvent 구독 (High 우선순위로 PawnManager보다 먼저 실행)
            _pawnManager.Subscribe<PawnDeathEvent>(HandlePawnDeath, EventPriority.High);
        }

        private void OnDisable()
        {
            if (_pawnManager != null)
            {
                _pawnManager.Unsubscribe<PawnDeathEvent>(HandlePawnDeath);
            }
        }

        public override void SubUpdate()
        {
            // 사망 효과 진행 중
            if (_isDying && _spriteRenderer != null)
            {
                _deathElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(_deathElapsed / fadeOutDuration);

                // 부드러운 이징 (EaseOutQuad)
                float easedT = 1f - Mathf.Pow(1f - t, 2f);

                // 알파 페이드
                Color currentColor = _spriteRenderer.color;
                currentColor.a = Mathf.Lerp(_deathStartColor.a, 0f, easedT);
                _spriteRenderer.color = currentColor;

                // 스케일 변화
                if (scaleMultiplier != 1f)
                {
                    _spriteRenderer.transform.localScale = Vector3.Lerp(_originalScale, _deathTargetScale, easedT);
                }

                // 효과 완료 시 파괴
                if (t >= 1f)
                {
                    _pawnManager.DestroyPawn();
                }
            }
        }

        /// <summary>
        /// Pawn 사망 이벤트를 처리합니다.
        /// </summary>
        private void HandlePawnDeath(PawnDeathEvent evt)
        {
            // 자신의 사망인지 확인
            if (evt.DeadPawn != _pawnManager) return;

            // 이미 사망 처리 중이면 무시
            if (_isDying) return;

            // 플레이어 Pawn은 DeathEffect 없이 비활성화됨 (PawnManager에서 처리)
            bool isPlayerPawn = GameManager.Instance?.PlayerController != null &&
                                GameManager.Instance.PlayerController.playerPawns.Contains(_pawnManager.gameObject);
            if (isPlayerPawn) return;

            // 파티클 재생 (설정된 경우)
            if (!string.IsNullOrEmpty(particlePrefabPath))
            {
                SpawnParticle();
            }

            // 페이드아웃 효과 시작
            if (useFadeOut && _spriteRenderer != null)
            {
                _isDying = true;
                _deathElapsed = 0f;
                _deathTargetScale = _originalScale * scaleMultiplier;

                // 시작 시 색상 틴트 적용
                if (deathTintColor != Color.white)
                {
                    _spriteRenderer.color = new Color(
                        deathTintColor.r,
                        deathTintColor.g,
                        deathTintColor.b,
                        _originalColor.a
                    );
                }
                _deathStartColor = _spriteRenderer.color;

                // PawnDeathEvent를 취소하여 PawnManager가 즉시 파괴하지 않도록 함
                evt.Cancel();
            }
        }

        /// <summary>
        /// 파티클을 생성합니다.
        /// </summary>
        private void SpawnParticle()
        {
            // FloatingEffectManager를 통해 파티클 생성
            var stageScreen = FindFirstObjectByType<PawnSurvivors.UI.StageScreen>();
            if (stageScreen != null)
            {
                var floatingEffectManager = stageScreen.GetComponent<PawnSurvivors.Managers.FloatingEffectManager>();
                if (floatingEffectManager != null)
                {
                    floatingEffectManager.ShowParticleEffect(
                        transform.position,
                        particlePrefabPath,
                        particleDuration
                    );
                }
            }
        }
    }
}

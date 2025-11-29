using UnityEngine;
using System.Collections;
using TMPro;
using PawnCore.Domain;
using PawnSurvivors.Managers;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 범용 FloatingEffect 컴포넌트입니다.
    /// 텍스트, 파티클 효과를 지원합니다.
    /// </summary>
    public class FloatingEffect : MonoBehaviour
    {
        private FloatingEffectPool _pool;
        private Coroutine _animationCoroutine;

        private TextMeshProUGUI _textMeshPro;
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _textMeshPro = GetComponent<TextMeshProUGUI>();
            _particleSystem = GetComponent<ParticleSystem>();
        }

        public void SetPool(FloatingEffectPool pool)
        {
            _pool = pool;
        }

        /// <summary>
        /// 효과를 표시하고 애니메이션을 시작합니다.
        /// </summary>
        public void Show(FloatingEffectData data)
        {
            if (data == null)
            {
                LogManager.LogError(LogCategory.UI, "FloatingEffectData가 null입니다.");
                return;
            }

            if (gameObject == null)
            {
                LogManager.LogError(LogCategory.UI, "gameObject가 null입니다.");
                return;
            }

            // 기존 애니메이션 중지
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            // GameObject 먼저 활성화 (컴포넌트 접근 전에)
            gameObject.SetActive(true);

            // 모든 시각적 컴포넌트 비활성화
            if (_textMeshPro != null) _textMeshPro.gameObject.SetActive(false);
            if (_particleSystem != null) _particleSystem.gameObject.SetActive(false);

            // 타입별 설정
            SetupEffectByType(data);

            // 위치 설정 (랜덤 오프셋 적용)
            Vector3 randomOffset = new Vector3(
                Random.Range(-data.RandomOffsetRange.x, data.RandomOffsetRange.x),
                Random.Range(-data.RandomOffsetRange.y, data.RandomOffsetRange.y),
                0f
            );
            transform.position = data.WorldPosition + randomOffset;

            // 폰트와 별개의 표시 크기 조정
            transform.localScale = Vector3.one * data.DisplayScale;
            transform.rotation = Quaternion.identity;

            // 애니메이션 시작
            _animationCoroutine = StartCoroutine(AnimateEffect(data));
        }

        private void SetupEffectByType(FloatingEffectData data)
        {
            switch (data.EffectType)
            {
                case FloatingEffectType.Text:
                    SetupTextEffect(data);
                    break;
                case FloatingEffectType.Particle:
                    SetupParticleEffect(data);
                    break;
            }
        }

        private void SetupTextEffect(FloatingEffectData data)
        {
            // RectTransform이 없으면 추가 (World Space Canvas용)
            if (GetComponent<RectTransform>() == null)
            {
                gameObject.AddComponent<RectTransform>();
            }

            if (_textMeshPro == null)
            {
                _textMeshPro = gameObject.AddComponent<TextMeshProUGUI>();
            }

            if (_textMeshPro == null)
            {
                LogManager.LogError(LogCategory.UI, "TextMeshProUGUI 컴포넌트를 생성할 수 없습니다.");
                return;
            }

            _textMeshPro.gameObject.SetActive(true);

            _textMeshPro.text = data.Text ?? "";
            _textMeshPro.color = data.TextColor;
            _textMeshPro.fontSize = data.FontSize;
            _textMeshPro.alignment = TextAlignmentOptions.Center;
            
            if (data.FontAsset != null)
            {
                _textMeshPro.font = data.FontAsset;
            }
            else
            {
                // 기본 폰트 로드 시도
                TMP_FontAsset defaultFont = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
                if (defaultFont != null)
                {
                    _textMeshPro.font = defaultFont;
                }
            }
        }

        private void SetupParticleEffect(FloatingEffectData data)
        {
            if (_particleSystem == null)
            {
                _particleSystem = gameObject.AddComponent<ParticleSystem>();
            }
            _particleSystem.gameObject.SetActive(true);
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = _particleSystem.main;
            main.duration = data.Duration;
            main.startLifetime = data.Duration;
            main.startSpeed = data.MoveDistanceRange.y / data.Duration;
            main.startColor = data.TextColor;

            _particleSystem.Play();
        }

        private IEnumerator AnimateEffect(FloatingEffectData data)
        {
            float elapsed = 0f;
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + Vector3.up * Random.Range(data.MoveDistanceRange.x, data.MoveDistanceRange.y);

            while (elapsed < data.Duration)
            {
                elapsed += GetGameDeltaTime();
                float t = elapsed / data.Duration;

                // 위치 애니메이션
                transform.position = Vector3.Lerp(startPos, endPos, data.MoveCurve.Evaluate(t));

                // 투명도 애니메이션 (텍스트만)
                if (data.EffectType == FloatingEffectType.Text && _textMeshPro != null && _textMeshPro.gameObject.activeSelf)
                {
                    Color currentColor = _textMeshPro.color;
                    currentColor.a = data.AlphaCurve.Evaluate(t);
                    _textMeshPro.color = currentColor;
                }

                yield return null;
            }

            // 애니메이션 종료 후 처리
            ReturnToPool();
        }

        private float GetGameDeltaTime()
        {
            if (GameManager.Instance?.LifecycleManager != null)
            {
                return GameManager.Instance.LifecycleManager.GameDeltaTime;
            }
            return Time.deltaTime;
        }

        private void ReturnToPool()
        {
            if (_pool != null)
            {
                if (_particleSystem != null)
                {
                    _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
                _pool.ReturnToPool(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}

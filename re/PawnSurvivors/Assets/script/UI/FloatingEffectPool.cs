using System.Collections.Generic;
using UnityEngine;
using PawnCore.Domain;
using TMPro;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 범용 FloatingEffect 풀링 시스템입니다.
    /// </summary>
    public class FloatingEffectPool : MonoBehaviour
    {
        [Header("풀 설정")]
        [SerializeField] private int initialPoolSize = 20;
        [SerializeField] private int expandSize = 10;

        private Dictionary<FloatingEffectType, Queue<FloatingEffect>> _availableEffects = new Dictionary<FloatingEffectType, Queue<FloatingEffect>>();
        private Dictionary<FloatingEffectType, List<FloatingEffect>> _allEffects = new Dictionary<FloatingEffectType, List<FloatingEffect>>();

        private Transform _poolParent;

        public void Initialize(Transform parent)
        {
            _poolParent = parent;
            
            foreach (FloatingEffectType type in System.Enum.GetValues(typeof(FloatingEffectType)))
            {
                _availableEffects[type] = new Queue<FloatingEffect>();
                _allEffects[type] = new List<FloatingEffect>();
            }

            // 초기 풀 생성 (파티클 제외)
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateNewEffect(FloatingEffectType.Text);
            }
        }

        public FloatingEffect GetFromPool(FloatingEffectType effectType, FloatingEffectData data = null)
        {
            // 파티클 타입은 항상 새로 생성
            if (effectType == FloatingEffectType.Particle)
            {
                return CreateNewEffect(effectType);
            }

            // 사용 가능한 효과가 없으면 확장
            if (_availableEffects[effectType].Count == 0)
            {
                ExpandPool(effectType);
            }

            if (_availableEffects[effectType].Count > 0)
            {
                FloatingEffect effect = _availableEffects[effectType].Dequeue();
                return effect;
            }

            Debug.LogWarning($"[FloatingEffectPool] {effectType} 타입의 효과를 풀에서 가져올 수 없습니다.");
            return null;
        }

        public void ReturnToPool(FloatingEffect effect)
        {
            if (effect == null) return;

            FloatingEffectType effectType = GetEffectType(effect);
            
            effect.gameObject.SetActive(false);
            effect.transform.SetParent(_poolParent);
            
            // 파티클은 풀에 추가하지 않고 파괴
            if (effectType == FloatingEffectType.Particle)
            {
                Destroy(effect.gameObject);
                return;
            }

            if (!_availableEffects[effectType].Contains(effect))
            {
                _availableEffects[effectType].Enqueue(effect);
            }
        }

        private FloatingEffectType GetEffectType(FloatingEffect effect)
        {
            if (effect.GetComponent<TMPro.TextMeshProUGUI>() != null)
                return FloatingEffectType.Text;
            if (effect.GetComponent<ParticleSystem>() != null)
                return FloatingEffectType.Particle;
            
            return FloatingEffectType.Text; // 기본값
        }

        private FloatingEffect CreateNewEffect(FloatingEffectType effectType)
        {
            GameObject effectObj = new GameObject($"FloatingEffect_{effectType}");
            effectObj.transform.SetParent(_poolParent);
            effectObj.SetActive(false);
            
            // 파티클이 아닌 경우 RectTransform 추가
            if (effectType != FloatingEffectType.Particle)
            {
                effectObj.AddComponent<RectTransform>();
            }
            
            FloatingEffect floatingEffect = effectObj.AddComponent<FloatingEffect>();
            floatingEffect.SetPool(this);
            _allEffects[effectType].Add(floatingEffect);
            
            // 텍스트 타입 설정
            if (effectType == FloatingEffectType.Text)
            {
                TMPro.TextMeshProUGUI textMeshPro = effectObj.AddComponent<TMPro.TextMeshProUGUI>();
                textMeshPro.alignment = TMPro.TextAlignmentOptions.Center;
                textMeshPro.fontSize = 24f;
                textMeshPro.color = Color.white;
            }
            else if (effectType == FloatingEffectType.Particle)
            {
                ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.startLifetime = 1f;
                main.startSpeed = 2f;
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            
            // 파티클은 풀에 넣지 않음
            if (effectType != FloatingEffectType.Particle)
            {
                _availableEffects[effectType].Enqueue(floatingEffect);
            }
            
            return floatingEffect;
        }

        private void ExpandPool(FloatingEffectType effectType)
        {
            for (int i = 0; i < expandSize; i++)
            {
                CreateNewEffect(effectType);
            }
        }
    }
}

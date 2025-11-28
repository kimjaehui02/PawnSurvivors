using UnityEngine;
using System;
using TMPro;

namespace PawnCore.Domain
{
    public enum FloatingEffectType
    {
        Text,
        Particle
    }

    [Serializable]
    public class FloatingEffectData
    {
        public FloatingEffectType EffectType = FloatingEffectType.Text;

        [Header("공통 설정")]
        public Vector3 WorldPosition;
        public float Duration = 0.5f;
        public Vector2 MoveDistanceRange = new Vector2(0.5f, 1.5f);
        public Vector2 RandomOffsetRange = new Vector2(0.2f, 0.2f);
        public AnimationCurve AlphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        public AnimationCurve MoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("텍스트 설정")]
        public string Text;
        public Color TextColor = Color.white;
        public float FontSize = 24f; // 정상 크기
        public float DisplayScale = 1f; // 폰트와 별개의 표시 크기 조정
        public TMP_FontAsset FontAsset;

        [Header("파티클 설정")]
        public string ParticleSystemPrefabPath; // Resources 경로

        // 팩토리 메서드
        public static FloatingEffectData CreateDamageText(Vector3 position, float damage, Color color, float fontSize = 24f, float displayScale = 1f, float moveDistance = 1f, float duration = 0.5f)
        {
            return new FloatingEffectData
            {
                EffectType = FloatingEffectType.Text,
                WorldPosition = position,
                Text = $"{damage:F0}", // "-" 기호 제거
                TextColor = color,
                FontSize = fontSize,
                DisplayScale = displayScale,
                MoveDistanceRange = new Vector2(moveDistance, moveDistance),
                Duration = duration,
                AlphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0),
                MoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                RandomOffsetRange = new Vector2(0.2f, 0.2f)
            };
        }

        public static FloatingEffectData CreateParticleEffect(Vector3 position, string prefabPath, float duration = 1f)
        {
            return new FloatingEffectData
            {
                EffectType = FloatingEffectType.Particle,
                WorldPosition = position,
                ParticleSystemPrefabPath = prefabPath,
                Duration = duration
            };
        }
    }
}

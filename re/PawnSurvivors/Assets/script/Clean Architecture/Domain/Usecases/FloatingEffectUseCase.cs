using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Events;
using UnityEngine;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// FloatingEffect 표시와 관련된 비즈니스 로직을 담당하는 UseCase입니다.
    /// </summary>
    public class FloatingEffectUseCase
    {
        /// <summary>
        /// PawnDamagedEvent를 기반으로 데미지 텍스트 표시 여부를 결정하고 데이터를 생성합니다.
        /// </summary>
        /// <param name="evt">PawnDamagedEvent</param>
        /// <param name="textColor">텍스트 색상</param>
        /// <param name="fontSize">폰트 크기</param>
        /// <param name="displayScale">표시 크기 스케일</param>
        /// <param name="moveDistance">이동 거리</param>
        /// <param name="duration">지속 시간</param>
        /// <returns>표시할 FloatingEffectData, 표시하지 않으면 null</returns>
        public FloatingEffectData CreateDamageTextFromEvent(
            PawnDamagedEvent evt,
            Color textColor,
            float fontSize = 24f,
            float displayScale = 1f,
            float moveDistance = 1f,
            float duration = 0.5f)
        {
            // 데미지가 0 이하면 표시하지 않음
            if (evt == null || evt.DamageApplied <= 0)
            {
                return null;
            }

            // 위치 결정
            Vector3 position = evt.Target != null && evt.Target.transform != null
                ? evt.Target.transform.position
                : Vector3.zero;

            // 데미지 텍스트 생성
            return FloatingEffectData.CreateDamageText(
                position,
                evt.DamageApplied,
                textColor,
                fontSize,
                displayScale,
                moveDistance,
                duration
            );
        }

        /// <summary>
        /// 파티클 효과 데이터를 생성합니다.
        /// </summary>
        /// <param name="position">위치</param>
        /// <param name="prefabPath">프리팹 경로</param>
        /// <param name="duration">지속 시간</param>
        /// <returns>FloatingEffectData</returns>
        public FloatingEffectData CreateParticleEffectData(
            Vector3 position,
            string prefabPath,
            float duration = 1f)
        {
            if (string.IsNullOrEmpty(prefabPath))
            {
                return null;
            }

            return FloatingEffectData.CreateParticleEffect(position, prefabPath, duration);
        }
    }
}


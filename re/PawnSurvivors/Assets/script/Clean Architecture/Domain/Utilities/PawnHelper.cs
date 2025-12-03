using UnityEngine;

namespace PawnSurvivors.Utilities
{
    /// <summary>
    /// Pawn 관련 유틸리티 함수 모음입니다.
    /// Unity 의존적이지만 재사용이 필요한 로직을 제공합니다.
    /// </summary>
    public static class PawnHelper
    {
        /// <summary>
        /// GameObject에서 PawnManager를 가져옵니다.
        /// </summary>
        /// <param name="obj">대상 GameObject</param>
        /// <param name="pawn">출력 PawnManager</param>
        /// <returns>PawnManager가 있으면 true</returns>
        public static bool TryGetPawn(GameObject obj, out PawnManager pawn)
        {
            return obj.TryGetComponent<PawnManager>(out pawn);
        }

        /// <summary>
        /// PawnManager가 데미지를 받을 수 있는지 확인합니다.
        /// DamageableSubManager의 존재 여부로 판단합니다.
        /// </summary>
        /// <param name="pawn">대상 PawnManager</param>
        /// <returns>데미지를 받을 수 있으면 true</returns>
        public static bool CanTakeDamage(PawnManager pawn)
        {
            if (pawn == null) return false;
            return pawn.TryGetComponent<DamageableSubManager>(out _);
        }

        /// <summary>
        /// GameObject가 데미지를 받을 수 있는 Pawn인지 확인합니다.
        /// PawnManager와 DamageableSubManager를 모두 확인합니다.
        /// </summary>
        /// <param name="obj">대상 GameObject</param>
        /// <param name="pawn">출력 PawnManager</param>
        /// <returns>데미지를 받을 수 있는 Pawn이면 true</returns>
        public static bool TryGetDamageablePawn(GameObject obj, out PawnManager pawn)
        {
            if (TryGetPawn(obj, out pawn))
            {
                return CanTakeDamage(pawn);
            }
            pawn = null;
            return false;
        }
    }
}


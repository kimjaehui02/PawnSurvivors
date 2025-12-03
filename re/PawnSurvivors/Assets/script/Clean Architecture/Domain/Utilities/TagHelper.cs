using UnityEngine;

namespace PawnSurvivors.Utilities
{
    /// <summary>
    /// 태그 관련 유틸리티 함수 모음입니다.
    /// Unity 태그 시스템의 재사용 가능한 로직을 제공합니다.
    /// </summary>
    public static class TagHelper
    {
        /// <summary>
        /// 두 GameObject가 같은 팀인지 확인합니다.
        /// 같은 태그를 가지면 같은 팀으로 간주합니다.
        /// </summary>
        /// <param name="a">첫 번째 GameObject</param>
        /// <param name="b">두 번째 GameObject</param>
        /// <returns>같은 태그를 가지면 true</returns>
        public static bool IsSameTeam(GameObject a, GameObject b)
        {
            return a.CompareTag(b.tag);
        }

        /// <summary>
        /// 반대편 태그를 가져옵니다.
        /// Player → Enemy, Enemy → Player
        /// </summary>
        /// <param name="obj">대상 GameObject</param>
        /// <returns>반대편 태그 문자열</returns>
        public static string GetOppositeTag(GameObject obj)
        {
            return obj.CompareTag("Player") ? "Enemy" : "Player";
        }

        /// <summary>
        /// 반대편 태그를 가져옵니다. (태그 문자열 입력 버전)
        /// Player → Enemy, Enemy → Player
        /// </summary>
        /// <param name="tag">태그 문자열</param>
        /// <returns>반대편 태그 문자열</returns>
        public static string GetOppositeTag(string tag)
        {
            return tag == "Player" ? "Enemy" : "Player";
        }
    }
}


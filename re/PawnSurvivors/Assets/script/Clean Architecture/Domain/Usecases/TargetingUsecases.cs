using UnityEngine;
using System.Linq;

namespace PawnSurvivors.Domain.Usecases
{
    public static class TargetingUsecases
    {
        /// <summary>
        /// 주어진 위치에서 가장 가까운 특정 태그를 가진 GameObject를 찾습니다.
        /// </summary>
        /// <param name="origin">탐색을 시작할 위치.</param>
        /// <param name="tag">찾을 대상의 태그.</param>
        /// <param name="range">탐색 범위. 0 이하일 경우 무한대.</param>
        /// <returns>가장 가까운 대상의 Transform. 찾지 못하면 null을 반환합니다.</returns>
        public static Transform FindClosestTargetByTag(Vector3 origin, string tag, float range)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            if (targets.Length == 0) return null;

            Transform closestTarget = null;
            float minDistance = Mathf.Infinity;

            foreach (GameObject target in targets)
            {
                float distance = Vector3.Distance(origin, target.transform.position);
                if (distance < minDistance && (range <= 0 || distance <= range))
                {
                    minDistance = distance;
                    closestTarget = target.transform;
                }
            }
            return closestTarget;
            
        }
    }
}

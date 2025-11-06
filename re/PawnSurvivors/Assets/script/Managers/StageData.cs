using System;
using UnityEngine;

namespace PawnSurvivors.Managers
{
    [Serializable]
    public class StageData
    {
        public string stageName = "Default Stage";
        public float spawnInterval = 3f;
        public float spawnRadius = 10f;
        public float spawnDistanceFromCamera = 2f; // 카메라 뷰포트 바깥쪽으로 얼마나 떨어져서 스폰할지
        // 추가적인 스테이지 관련 설정들을 여기에 포함할 수 있습니다.
        // 예: stageDuration, bossSpawnTime, enemyTypes, difficultyScaling 등
    }
}

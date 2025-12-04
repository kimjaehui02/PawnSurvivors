using System;
using System.Collections.Generic;
using UnityEngine;

namespace PawnSurvivors.Managers
{
    [Serializable]
    public class StageData
    {
        public string stageName = "Default Stage";
        public float spawnRadius = 10f;
        public float spawnDistanceFromCamera = 2f; // 카메라 뷰포트 바깥쪽으로 얼마나 떨어져서 스폰할지
        public float stageDuration = 300f; // 스테이지 지속 시간 (초). -1이면 무한
        public string bgmName = "Audio/BGM"; // 배경 음악 (Resources 경로)
        
        // 웨이브 기반 적 소환 시스템
        public List<EnemyWave> enemyWaves = new List<EnemyWave>();
        
        // 하위 호환성을 위한 레거시 필드 (deprecated)
        [Obsolete("Use enemyWaves instead")]
        public float spawnInterval = 3f;
    }

    [Serializable]
    public class EnemyWave
    {
        public float startTime = 0f; // 웨이브 시작 시간 (초)
        public float endTime = -1f; // 웨이브 종료 시간 (초). -1이면 스테이지 종료까지
        public float spawnInterval = 2.5f; // 적 소환 간격 (초)
        public bool spawnOnce = false; // true면 한 번만 소환 (보스 등)
        public List<EnemyType> enemyTypes = new List<EnemyType>(); // 소환할 적 종류들
    }

    [Serializable]
    public class EnemyType
    {
        public string recipeName = "Enemy"; // Recipes 폴더의 레시피 파일 이름 (확장자 제외)
        public int weight = 100; // 가중치 (높을수록 더 자주 소환됨)
    }
}

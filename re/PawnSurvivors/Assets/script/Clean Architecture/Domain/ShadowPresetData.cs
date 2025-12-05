using System;
using UnityEngine;

namespace PawnSurvivors.Domain
{
    /// <summary>
    /// 그림자 프리셋 데이터입니다.
    /// StreamingAssets/ShadowPresets/에 JSON으로 저장됩니다.
    /// </summary>
    [Serializable]
    public class ShadowPresetData
    {
        [Tooltip("프리셋 이름 (파일명과 동일해야 함)")]
        public string presetName;
        
        [Tooltip("그림자 색상")]
        public Color shadowColor = new Color(0, 0, 0, 0.5f);
        
        [Tooltip("그림자 크기 (x: 가로, y: 세로) - 타원형")]
        public Vector2 shadowScale = new Vector2(1f, 0.5f);
        
        [Tooltip("그림자 위치 오프셋")]
        public Vector3 shadowOffset = new Vector3(0, -0.5f, 0);
    }
}


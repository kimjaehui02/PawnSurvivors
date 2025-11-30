using System;
using System.Collections.Generic;
using UnityEngine;

namespace PawnSurvivors.Managers
{
    /// <summary>
    /// 스테이지 리스트 데이터입니다.
    /// 여러 스테이지를 순차적으로 진행하기 위한 순서를 정의합니다.
    /// </summary>
    [Serializable]
    public class StageListData
    {
        /// <summary>
        /// 캠페인 이름
        /// </summary>
        public string campaignName = "Main Campaign";
        
        /// <summary>
        /// 스테이지 순서 (순서대로 진행됨)
        /// </summary>
        public List<string> stages = new List<string>();
    }
}


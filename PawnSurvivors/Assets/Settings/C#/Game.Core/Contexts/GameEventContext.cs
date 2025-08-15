using System.Collections.Generic;
using UnityEngine;
using Game.Core.Models;

namespace Game.Core.Contexts
{
    public class GameEventContext
    {
        public bool stopUpdate = false;
        public PawnData PawnData { get; set; }
        public int PawnDataIndex { get; set; }
        public List<PawnData> PawnDatas { get; set; }

        public void LogCurrentState()
        {
            Debug.Log("--- GameEventContext 상태 ---");
            Debug.Log($"stopUpdate: {stopUpdate}");
            Debug.Log($"PawnData: {(PawnData != null ? "데이터 있음" : "null")}");
            if (PawnData != null)
            {
                Debug.Log($"PawnData.PawnConfig.Name: {PawnData.PawnConfig?.Name ?? "null"}");
                Debug.Log($"PawnData.ComponentConfigs.Count: {PawnData.ComponentConfigs?.Count ?? 0}");
            }
            Debug.Log("-----------------------------");
        }
    }
}

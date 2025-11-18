using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace PawnSurvivors.Managers
{
    public class StageLoader
    {
        private Dictionary<string, StageData> _stages = new Dictionary<string, StageData>();

        public void LoadStages(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Debug.LogError($"Stage directory not found: {directoryPath}");
                return;
            }

            var info = new DirectoryInfo(directoryPath);
            var fileInfo = info.GetFiles("*.json");

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            foreach (var file in fileInfo)
            {
                string json = File.ReadAllText(file.FullName);
                StageData data = JsonConvert.DeserializeObject<StageData>(json, settings);
                _stages[data.stageName] = data;
            }
        }

        public StageData GetStage(string stageName)
        {
            _stages.TryGetValue(stageName, out StageData stage);
            return stage;
        }

        /// <summary>
        /// 로드된 모든 스테이지 이름을 가져옵니다.
        /// </summary>
        /// <returns>스테이지 이름 리스트</returns>
        public List<string> GetAllStageNames()
        {
            return new List<string>(_stages.Keys);
        }
    }
}









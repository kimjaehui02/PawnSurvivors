using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using PawnSurvivors.Managers;

namespace PawnSurvivors.Data.DataSources
{
    /// <summary>
    /// 스테이지 JSON 파일들을 읽어서 StageData 모델로 반환하는 데이터 소스입니다.
    /// Data 계층의 데이터 소스 역할을 담당합니다.
    /// </summary>
    public class StageDataSource
    {
        private Dictionary<string, StageData> _stages = new Dictionary<string, StageData>();

        /// <summary>
        /// 지정된 디렉토리에서 모든 스테이지 JSON 파일을 로드합니다.
        /// </summary>
        /// <param name="directoryPath">스테이지 디렉토리 경로</param>
        public void LoadStages(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                LogManager.LogError(LogCategory.Stage, $"Stage directory not found: {directoryPath}");
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
                try
                {
                    string json = File.ReadAllText(file.FullName);
                    StageData data = JsonConvert.DeserializeObject<StageData>(json, settings);
                    if (data != null && !string.IsNullOrEmpty(data.stageName))
                    {
                        _stages[data.stageName] = data;
                    }
                }
                catch (System.Exception ex)
                {
                    LogManager.LogError(LogCategory.Stage, $"{file.Name} 로드 실패: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 스테이지 이름으로 스테이지 데이터를 가져옵니다.
        /// </summary>
        /// <param name="stageName">스테이지 이름</param>
        /// <returns>StageData 또는 null</returns>
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


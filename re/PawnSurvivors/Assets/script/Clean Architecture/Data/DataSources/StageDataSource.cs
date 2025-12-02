using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using PawnSurvivors.Managers;

namespace PawnSurvivors.Data.DataSources
{
    /// <summary>
    /// 스테이지 JSON 파일들을 읽어서 StageData 모델로 반환하는 데이터 소스입니다.
    /// Data 계층의 데이터 소스 역할을 담당합니다.
    /// WebGL 호환을 위해 Resources 폴더를 사용합니다.
    /// </summary>
    public class StageDataSource
    {
        private Dictionary<string, StageData> _stages = new Dictionary<string, StageData>();

        /// <summary>
        /// Resources 폴더에서 모든 스테이지 JSON 파일을 로드합니다.
        /// </summary>
        /// <param name="resourcePath">Resources 폴더 기준 경로 (예: "StreamingAssets/Stages")</param>
        public void LoadStages(string resourcePath)
        {
            // Resources.LoadAll로 모든 TextAsset 로드
            TextAsset[] jsonAssets = Resources.LoadAll<TextAsset>(resourcePath);

            if (jsonAssets == null || jsonAssets.Length == 0)
            {
                LogManager.LogError(LogCategory.Stage, $"Stage directory not found: {resourcePath}");
                return;
            }

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            foreach (var jsonAsset in jsonAssets)
            {
                try
                {
                    string json = jsonAsset.text;
                    StageData data = JsonConvert.DeserializeObject<StageData>(json, settings);
                    if (data != null && !string.IsNullOrEmpty(data.stageName))
                    {
                        _stages[data.stageName] = data;
                    }
                }
                catch (System.Exception ex)
                {
                    LogManager.LogError(LogCategory.Stage, $"{jsonAsset.name} 로드 실패: {ex.Message}");
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
        /// 로드된 모든 스테이지 이름 목록을 반환합니다.
        /// </summary>
        public IEnumerable<string> GetAllStageNames()
        {
            return _stages.Keys;
        }
    }
}

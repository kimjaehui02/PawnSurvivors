using UnityEngine;
using Newtonsoft.Json;
using PawnSurvivors.Managers;

namespace PawnSurvivors.Data.DataSources
{
    /// <summary>
    /// 스테이지 리스트 JSON 파일을 읽어서 StageListData 모델로 반환하는 데이터 소스입니다.
    /// WebGL 호환을 위해 Resources 폴더를 사용합니다.
    /// </summary>
    public class StageListDataSource
    {
        private StageListData _stageList;

        /// <summary>
        /// Resources 폴더에서 스테이지 리스트 JSON 파일을 로드합니다.
        /// </summary>
        /// <param name="resourcePath">Resources 폴더 기준 경로 (예: "StreamingAssets/Stages/StageList", 확장자 제외)</param>
        public void LoadStageList(string resourcePath)
        {
            // Resources.Load로 TextAsset 로드
            TextAsset jsonAsset = Resources.Load<TextAsset>(resourcePath);

            if (jsonAsset == null)
            {
                LogManager.LogError(LogCategory.Stage, $"StageList.json 파일을 찾을 수 없습니다: {resourcePath}");
                return;
            }

            try
            {
                string json = jsonAsset.text;
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                };
                _stageList = JsonConvert.DeserializeObject<StageListData>(json, settings);
                
                if (_stageList != null && _stageList.stages != null && _stageList.stages.Count > 0)
                {
                    LogManager.LogInfo(LogCategory.Stage, $"StageList 로드 완료: {_stageList.campaignName}, 스테이지 수: {_stageList.stages.Count}");
                }
                else
                {
                    LogManager.LogError(LogCategory.Stage, "StageList 데이터가 비어있거나 잘못되었습니다.");
                }
            }
            catch (System.Exception ex)
            {
                LogManager.LogError(LogCategory.Stage, $"StageList.json 로드 실패: {ex.Message}");
            }
        }

        /// <summary>
        /// 로드된 스테이지 리스트를 가져옵니다.
        /// </summary>
        /// <returns>StageListData 또는 null</returns>
        public StageListData GetStageList()
        {
            return _stageList;
        }

        /// <summary>
        /// 현재 스테이지의 다음 스테이지 이름을 가져옵니다.
        /// </summary>
        /// <param name="currentStageName">현재 스테이지 이름</param>
        /// <returns>다음 스테이지 이름, 없으면 null</returns>
        public string GetNextStageName(string currentStageName)
        {
            if (_stageList == null || _stageList.stages == null || _stageList.stages.Count == 0)
            {
                return null;
            }

            int currentIndex = _stageList.stages.IndexOf(currentStageName);
            if (currentIndex < 0)
            {
                // 현재 스테이지가 리스트에 없으면 첫 번째 스테이지 반환
                LogManager.LogWarning(LogCategory.Stage, $"현재 스테이지 '{currentStageName}'가 StageList에 없습니다. 첫 번째 스테이지를 반환합니다.");
                return _stageList.stages[0];
            }

            int nextIndex = currentIndex + 1;
            if (nextIndex >= _stageList.stages.Count)
            {
                // 마지막 스테이지면 null 반환 (게임 종료 또는 반복)
                return null;
            }

            return _stageList.stages[nextIndex];
        }
    }
}

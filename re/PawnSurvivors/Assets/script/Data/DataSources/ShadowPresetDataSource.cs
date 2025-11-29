using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PawnCore.Domain;
using PawnSurvivors.Managers;

namespace PawnSurvivors.Data.DataSources
{
    /// <summary>
    /// ShadowPreset JSON 파일들을 읽어서 ShadowPresetData 모델로 반환하는 데이터 소스입니다.
    /// Data 계층의 데이터 소스 역할을 담당합니다.
    /// </summary>
    public class ShadowPresetDataSource
    {
        private Dictionary<string, ShadowPresetData> _presets = new Dictionary<string, ShadowPresetData>();

        /// <summary>
        /// 지정된 디렉토리에서 모든 ShadowPreset JSON 파일을 로드합니다.
        /// </summary>
        /// <param name="presetsPath">ShadowPresets 폴더 경로</param>
        public void LoadPresets(string presetsPath)
        {
            if (!Directory.Exists(presetsPath))
            {
                LogManager.LogWarning(LogCategory.System, $"ShadowPresetDataSource: 경로를 찾을 수 없습니다: {presetsPath}");
                return;
            }

            string[] jsonFiles = Directory.GetFiles(presetsPath, "*.json");

            foreach (string filePath in jsonFiles)
            {
                try
                {
                    string jsonContent = File.ReadAllText(filePath);
                    ShadowPresetData preset = JsonUtility.FromJson<ShadowPresetData>(jsonContent);

                    if (preset != null && !string.IsNullOrEmpty(preset.presetName))
                    {
                        _presets[preset.presetName] = preset;
                    }
                    else
                    {
                        LogManager.LogWarning(LogCategory.System, $"ShadowPresetDataSource: {Path.GetFileName(filePath)} - presetName이 없습니다.");
                    }
                }
                catch (System.Exception ex)
                {
                    LogManager.LogError(LogCategory.System, $"ShadowPresetDataSource: {Path.GetFileName(filePath)} 로드 실패: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 이름으로 프리셋을 가져옵니다.
        /// </summary>
        /// <param name="presetName">프리셋 이름</param>
        /// <returns>ShadowPresetData 또는 null</returns>
        public ShadowPresetData GetPreset(string presetName)
        {
            if (string.IsNullOrEmpty(presetName))
            {
                return null;
            }

            if (_presets.TryGetValue(presetName, out ShadowPresetData preset))
            {
                return preset;
            }

            LogManager.LogWarning(LogCategory.System, $"ShadowPresetDataSource: '{presetName}' 프리셋을 찾을 수 없습니다.");
            return null;
        }

        /// <summary>
        /// 로드된 모든 프리셋의 이름 목록을 반환합니다.
        /// </summary>
        public IEnumerable<string> GetAllPresetNames()
        {
            return _presets.Keys;
        }
    }
}


using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PawnCore.Domain;

namespace PawnCore.Recipes
{
    /// <summary>
    /// ShadowPreset JSON 파일들을 로드하고 관리합니다.
    /// RecipeLoader와 유사한 구조입니다.
    /// </summary>
    public class ShadowPresetLoader
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
                Debug.LogWarning($"ShadowPresetLoader: 경로를 찾을 수 없습니다: {presetsPath}");
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
                        // Debug.Log($"ShadowPresetLoader: '{preset.presetName}' 프리셋 로드 완료 from {Path.GetFileName(filePath)}");
                    }
                    else
                    {
                        Debug.LogWarning($"ShadowPresetLoader: {Path.GetFileName(filePath)} - presetName이 없습니다.");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"ShadowPresetLoader: {Path.GetFileName(filePath)} 로드 실패: {ex.Message}");
                }
            }

            // Debug.Log($"ShadowPresetLoader: 총 {_presets.Count}개의 프리셋 로드 완료.");
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

            Debug.LogWarning($"ShadowPresetLoader: '{presetName}' 프리셋을 찾을 수 없습니다.");
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


#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using PawnSurvivors.UI;

namespace PawnSurvivors.Editor
{
    /// <summary>
    /// Unity 에디터 메뉴에서 AudioSettingsScreen 프리팹을 자동으로 생성합니다.
    /// Tools > Create Audio Settings Screen Prefab 메뉴 사용
    /// </summary>
    public static class CreateOptionsScreenPrefab
    {
        [MenuItem("Tools/Create Audio Settings Screen Prefab")]
        public static void CreateAudioSettingsPrefab()
        {
            // GameObject 생성
            GameObject audioSettingsObj = new GameObject("AudioSettingsScreen");
            audioSettingsObj.AddComponent<AudioSettingsScreen>();
            
            // Resources/Prefabs/UI 폴더 확인 및 생성
            string folderPath = "Assets/Resources/Prefabs/UI";
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Prefabs");
            }
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets/Resources/Prefabs", "UI");
            }
            
            // 프리팹으로 저장
            string prefabPath = "Assets/Resources/Prefabs/UI/AudioSettingsScreen.prefab";
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(audioSettingsObj, prefabPath);
            
            // 씬의 임시 GameObject 제거
            Object.DestroyImmediate(audioSettingsObj);
            
            // 프리팹 선택 (Project 창에서 하이라이트)
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
            
            Debug.Log($"[Editor] AudioSettingsScreen 프리팹 생성 완료: {prefabPath}");
        }
    }
}
#endif


#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using PawnSurvivors.UI;

namespace PawnSurvivors.Editor
{
    /// <summary>
    /// 게임 씬을 자동으로 생성하고 필요한 오브젝트를 배치하는 에디터 툴입니다.
    /// </summary>
    public static class CreateGameScenes
    {
        private const string SCENES_FOLDER_PATH = "Assets/Scenes";
        private const string UI_PREFAB_FOLDER_PATH = "Assets/Resources/Prefabs/UI";
        
        #region Individual Scene Creation
        
        [MenuItem("Tools/Create Scenes/TitleScene")]
        public static void CreateTitleScene()
        {
            CreateSceneWithUI("TitleScene", "TitleScreen");
        }
        
        [MenuItem("Tools/Create Scenes/CharacterSelectScene")]
        public static void CreateCharacterSelectScene()
        {
            CreateSceneWithUI("CharacterSelectScene", "CharacterSelectScreen");
        }
        
        [MenuItem("Tools/Create Scenes/StageSelectScene")]
        public static void CreateStageSelectScene()
        {
            CreateSceneWithUI("StageSelectScene", "CampaignSelectScreen");
        }
        
        [MenuItem("Tools/Create Scenes/StageScene")]
        public static void CreateStageScene()
        {
            CreateSceneWithUI("StageScene", "StageScreen");
        }
        
        [MenuItem("Tools/Create Scenes/ShopScene")]
        public static void CreateShopScene()
        {
            CreateSceneWithUI("ShopScene", "BrotatoShopScreen");
        }
        
        [MenuItem("Tools/Create Scenes/GameOverScene")]
        public static void CreateGameOverScene()
        {
            CreateSceneWithUI("GameOverScene", "GameOverScreen");
        }
        
        #endregion
        
        #region Batch Creation
        
        [MenuItem("Tools/Create Scenes/Create All Scenes")]
        public static void CreateAllScenes()
        {
            try
            {
                CreateTitleScene();
                CreateCharacterSelectScene();
                CreateStageSelectScene();
                CreateStageScene();
                CreateShopScene();
                CreateGameOverScene();
                
                Debug.Log("[Editor] 모든 씬 생성 완료!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Editor] 씬 생성 중 오류 발생: {e.Message}");
                EditorUtility.DisplayDialog("오류", $"씬 생성 중 오류가 발생했습니다:\n{e.Message}", "확인");
            }
        }
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// UI Screen 프리팹을 포함한 씬을 생성합니다.
        /// </summary>
        private static void CreateSceneWithUI(string sceneName, string screenPrefabName)
        {
            try
            {
                // 폴더 확인 및 생성
                EnsureFolderExists(SCENES_FOLDER_PATH);
                
                // 씬 경로
                string scenePath = $"{SCENES_FOLDER_PATH}/{sceneName}.unity";
                
                // 기존 씬이 있으면 확인
                Scene existingScene = EditorSceneManager.GetSceneByPath(scenePath);
                if (existingScene.IsValid())
                {
                    if (!EditorUtility.DisplayDialog(
                        "씬 덮어쓰기",
                        $"'{sceneName}' 씬이 이미 존재합니다. 덮어쓰시겠습니까?",
                        "덮어쓰기",
                        "취소"))
                    {
                        return;
                    }
                }
                
                // 새 씬 생성 (기본 게임 오브젝트 포함: 카메라, Light 등)
                Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                newScene.name = sceneName;
                
                // 카메라 설정 (2D 게임용)
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    mainCamera.orthographic = true;
                    mainCamera.orthographicSize = 5f;
                    mainCamera.transform.position = new Vector3(0, 0, -10);
                }
                
                // EventSystem 생성 (UI용)
                if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
                {
                    GameObject eventSystemObj = new GameObject("EventSystem");
                    eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                }
                
                // UI Screen 프리팹 인스턴스화
                string prefabPath = $"{UI_PREFAB_FOLDER_PATH}/{screenPrefabName}.prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                
                if (prefab != null)
                {
                    GameObject screenInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    if (screenInstance != null)
                    {
                        screenInstance.name = screenPrefabName;
                        Debug.Log($"[Editor] {screenPrefabName} 프리팹 인스턴스화 완료");
                    }
                    else
                    {
                        Debug.LogWarning($"[Editor] {screenPrefabName} 프리팹 인스턴스화 실패");
                    }
                }
                else
                {
                    Debug.LogWarning($"[Editor] {screenPrefabName} 프리팹을 찾을 수 없습니다: {prefabPath}\n먼저 프리팹을 생성해주세요.");
                    
                    // 프리팹이 없으면 빈 GameObject라도 생성 (나중에 수동으로 추가 가능)
                    GameObject screenObj = new GameObject(screenPrefabName);
                    Debug.Log($"[Editor] {screenPrefabName} GameObject 생성 (프리팹 없음)");
                }
                
                // 씬 저장
                bool saveSuccess = EditorSceneManager.SaveScene(newScene, scenePath);
                if (saveSuccess)
                {
                    Debug.Log($"[Editor] {sceneName} 씬 생성 완료: {scenePath}");
                    
                    // 씬 선택 (Project 창에서 하이라이트)
                    Object sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                    if (sceneAsset != null)
                    {
                        Selection.activeObject = sceneAsset;
                        EditorGUIUtility.PingObject(sceneAsset);
                    }
                }
                else
                {
                    Debug.LogError($"[Editor] {sceneName} 씬 저장 실패");
                    EditorUtility.DisplayDialog("오류", $"{sceneName} 씬 저장에 실패했습니다.", "확인");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Editor] {sceneName} 씬 생성 중 오류: {e.Message}\n{e.StackTrace}");
                EditorUtility.DisplayDialog("오류", $"{sceneName} 씬 생성 중 오류가 발생했습니다:\n{e.Message}", "확인");
            }
        }
        
        /// <summary>
        /// 폴더가 존재하지 않으면 생성합니다.
        /// </summary>
        private static void EnsureFolderExists(string folderPath)
        {
            string[] folders = folderPath.Split('/');
            string currentPath = folders[0]; // "Assets"
            
            for (int i = 1; i < folders.Length; i++)
            {
                string nextPath = $"{currentPath}/{folders[i]}";
                if (!AssetDatabase.IsValidFolder(nextPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }
                currentPath = nextPath;
            }
        }
        
        #endregion
    }
}
#endif


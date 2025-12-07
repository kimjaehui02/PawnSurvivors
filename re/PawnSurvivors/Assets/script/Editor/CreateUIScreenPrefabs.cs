#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using PawnSurvivors.UI;

namespace PawnSurvivors.Editor
{
    /// <summary>
    /// 모든 UI Screen 프리팹을 자동으로 생성하는 에디터 툴입니다.
    /// Tools > Create UI Screen Prefabs 메뉴에서 사용할 수 있습니다.
    /// </summary>
    public static class CreateUIScreenPrefabs
    {
        private const string PREFAB_FOLDER_PATH = "Assets/Resources/Prefabs/UI";
        
        #region Scene UI Screens (씬에 배치할 주요 화면들)
        
        [MenuItem("Tools/Create UI Prefabs/TitleScreen")]
        public static void CreateTitleScreenPrefab()
        {
            CreateScreenPrefab<TitleScreen>("TitleScreen");
        }
        
        [MenuItem("Tools/Create UI Prefabs/CharacterSelectScreen")]
        public static void CreateCharacterSelectScreenPrefab()
        {
            CreateScreenPrefab<CharacterSelectScreen>("CharacterSelectScreen");
        }
        
        [MenuItem("Tools/Create UI Prefabs/CampaignSelectScreen")]
        public static void CreateCampaignSelectScreenPrefab()
        {
            CreateScreenPrefab<CampaignSelectScreen>("CampaignSelectScreen");
        }
        
        [MenuItem("Tools/Create UI Prefabs/StageScreen")]
        public static void CreateStageScreenPrefab()
        {
            CreateScreenPrefab<StageScreen>("StageScreen");
        }
        
        [MenuItem("Tools/Create UI Prefabs/BrotatoShopScreen")]
        public static void CreateBrotatoShopScreenPrefab()
        {
            CreateScreenPrefab<BrotatoShopScreen>("BrotatoShopScreen");
        }
        
        [MenuItem("Tools/Create UI Prefabs/GameOverScreen")]
        public static void CreateGameOverScreenPrefab()
        {
            CreateScreenPrefab<GameOverScreen>("GameOverScreen");
        }
        
        #endregion
        
        #region Overlay UI Screens (오버레이 화면들)
        
        [MenuItem("Tools/Create UI Prefabs/PauseMenuScreen")]
        public static void CreatePauseMenuScreenPrefab()
        {
            CreateScreenPrefab<PauseMenuScreen>("PauseMenuScreen");
        }
        
        [MenuItem("Tools/Create UI Prefabs/OptionsScreen")]
        public static void CreateOptionsScreenPrefab()
        {
            CreateScreenPrefab<OptionsScreen>("OptionsScreen");
        }
        
        [MenuItem("Tools/Create UI Prefabs/AudioSettingsScreen")]
        public static void CreateAudioSettingsScreenPrefab()
        {
            CreateScreenPrefab<AudioSettingsScreen>("AudioSettingsScreen");
        }
        
        #endregion
        
        #region Legacy/Other Screens (기타 화면들)
        
        [MenuItem("Tools/Create UI Prefabs/ShopScreen")]
        public static void CreateShopScreenPrefab()
        {
            CreateScreenPrefab<ShopScreen>("ShopScreen");
        }
        
        [MenuItem("Tools/Create UI Prefabs/StageClearScreen")]
        public static void CreateStageClearScreenPrefab()
        {
            CreateScreenPrefab<StageClearScreen>("StageClearScreen");
        }
        
        [MenuItem("Tools/Create UI Prefabs/MainMenuScreen")]
        public static void CreateMainMenuScreenPrefab()
        {
            CreateScreenPrefab<MainMenuScreen>("MainMenuScreen");
        }
        
        #endregion
        
        #region Batch Creation (일괄 생성)
        
        [MenuItem("Tools/Create UI Prefabs/Create All Scene Screens")]
        public static void CreateAllSceneScreens()
        {
            CreateTitleScreenPrefab();
            CreateCharacterSelectScreenPrefab();
            CreateCampaignSelectScreenPrefab();
            CreateStageScreenPrefab();
            CreateBrotatoShopScreenPrefab();
            CreateGameOverScreenPrefab();
            
            Debug.Log("[Editor] 모든 씬 UI Screen 프리팹 생성 완료!");
        }
        
        [MenuItem("Tools/Create UI Prefabs/Create All Overlay Screens")]
        public static void CreateAllOverlayScreens()
        {
            CreatePauseMenuScreenPrefab();
            CreateOptionsScreenPrefab();
            CreateAudioSettingsScreenPrefab();
            
            Debug.Log("[Editor] 모든 오버레이 UI Screen 프리팹 생성 완료!");
        }
        
        [MenuItem("Tools/Create UI Prefabs/Create All Screens")]
        public static void CreateAllScreens()
        {
            CreateAllSceneScreens();
            CreateAllOverlayScreens();
            
            Debug.Log("[Editor] 모든 UI Screen 프리팹 생성 완료!");
        }
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// 특정 타입의 UI Screen 프리팹을 생성합니다.
        /// </summary>
        private static void CreateScreenPrefab<T>(string screenName) where T : MonoBehaviour
        {
            // 폴더 확인 및 생성
            EnsureFolderExists(PREFAB_FOLDER_PATH);
            
            // GameObject 생성
            GameObject screenObj = new GameObject(screenName);
            T component = screenObj.AddComponent<T>();
            
            // 에디터에서 Awake() 강제 호출하여 UI 생성
            // Unity 에디터에서는 AddComponent 후 Awake()가 자동 호출되지 않으므로 수동 호출
            component.SendMessage("Awake", SendMessageOptions.DontRequireReceiver);
            component.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
            
            // 프리팹 경로
            string prefabPath = $"{PREFAB_FOLDER_PATH}/{screenName}.prefab";
            
            // 기존 프리팹이 있으면 덮어쓰기 확인
            GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (existingPrefab != null)
            {
                if (!EditorUtility.DisplayDialog(
                    "프리팹 덮어쓰기",
                    $"'{screenName}' 프리팹이 이미 존재합니다. 덮어쓰시겠습니까?",
                    "덮어쓰기",
                    "취소"))
                {
                    Object.DestroyImmediate(screenObj);
                    return;
                }
            }
            
            // 프리팹으로 저장
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(screenObj, prefabPath);
            
            // 씬의 임시 GameObject 제거
            Object.DestroyImmediate(screenObj);
            
            // 프리팹 선택 (Project 창에서 하이라이트)
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
            
            Debug.Log($"[Editor] {screenName} 프리팹 생성 완료: {prefabPath}");
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


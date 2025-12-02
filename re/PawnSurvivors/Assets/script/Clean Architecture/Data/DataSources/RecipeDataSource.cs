using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using PawnSurvivors.Managers;
using PawnSurvivors.Data.Recipes;

namespace PawnSurvivors.Data.DataSources
{
    /// <summary>
    /// Pawn 레시피 JSON 파일들을 읽어서 PawnRecipeData 모델로 반환하는 데이터 소스입니다.
    /// Data 계층의 데이터 소스 역할을 담당합니다.
    /// WebGL 호환을 위해 Resources 폴더를 사용합니다.
    /// </summary>
    public class RecipeDataSource
    {
        private Dictionary<string, PawnRecipeData> _recipes = new Dictionary<string, PawnRecipeData>();

        /// <summary>
        /// Resources 폴더에서 모든 Pawn 레시피 JSON 파일을 로드합니다.
        /// Players/, Enemies/, Projectiles/ 폴더만 읽습니다.
        /// </summary>
        /// <param name="resourcePath">Resources 폴더 기준 경로 (예: "StreamingAssets/Recipes")</param>
        public void LoadRecipes(string resourcePath)
        {
            // Pawn 레시피가 있는 폴더만 읽기 (Items 폴더 제외)
            string[] recipeFolders = { "Players", "Enemies", "Projectiles" };
            int totalLoaded = 0;

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            foreach (var folderName in recipeFolders)
            {
                string folderPath = $"{resourcePath}/{folderName}";
                
                // Resources.LoadAll로 해당 폴더의 모든 TextAsset 로드
                TextAsset[] jsonAssets = Resources.LoadAll<TextAsset>(folderPath);
                
                if (jsonAssets == null || jsonAssets.Length == 0)
                {
                    LogManager.LogWarning(LogCategory.Recipe, $"레시피 폴더에서 파일을 찾을 수 없습니다: {folderPath}");
                    continue;
                }

                foreach (var jsonAsset in jsonAssets)
                {
                    try
                    {
                        string json = jsonAsset.text;
                        PawnRecipeData data = JsonConvert.DeserializeObject<PawnRecipeData>(json, settings);
                        
                        // pawnName이 null이거나 비어있으면 스킵
                        if (data == null || string.IsNullOrEmpty(data.pawnName))
                        {
                            LogManager.LogWarning(LogCategory.Recipe, $"{jsonAsset.name} - pawnName이 없거나 유효하지 않습니다. 스킵합니다.");
                            continue;
                        }
                        
                        _recipes[data.pawnName] = data;
                        LogManager.LogInfo(LogCategory.Recipe, $"레시피 로드: {data.pawnName} (from {jsonAsset.name})");
                        totalLoaded++;
                    }
                    catch (System.Exception ex)
                    {
                        LogManager.LogError(LogCategory.Recipe, $"{jsonAsset.name} 로드 실패: {ex.Message}");
                    }
                }
            }
            
            LogManager.LogInfo(LogCategory.Recipe, $"총 {totalLoaded}개의 레시피 로드 완료");
        }

        /// <summary>
        /// 레시피 이름으로 레시피 데이터를 가져옵니다.
        /// </summary>
        /// <param name="recipeName">레시피 이름</param>
        /// <returns>PawnRecipeData 또는 null</returns>
        public PawnRecipeData GetRecipe(string recipeName)
        {
            _recipes.TryGetValue(recipeName, out PawnRecipeData recipe);
            return recipe;
        }

        /// <summary>
        /// 모든 Player 레시피 이름 목록을 가져옵니다.
        /// </summary>
        public List<string> GetAllPlayerRecipeNames()
        {
            var playerRecipes = new List<string>();
            
            foreach (var kvp in _recipes)
            {
                if (kvp.Key.StartsWith("Player"))
                {
                    playerRecipes.Add(kvp.Key);
                }
            }
            
            return playerRecipes;
        }
    }
}

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using PawnSurvivors.Managers;

namespace PawnCore.Recipes.Json
{
    public class RecipeLoader
    {
        private Dictionary<string, PawnRecipeData> recipes = new Dictionary<string, PawnRecipeData>();

        public void LoadRecipes(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                LogManager.LogError(LogCategory.Recipe, $"Recipe directory not found: {directoryPath}");
                return;
            }

            var info = new DirectoryInfo(directoryPath);
            // 하위 폴더까지 재귀적으로 검색 (Players/, Enemies/, Projectiles/ 등)
            var fileInfo = info.GetFiles("*.json", SearchOption.AllDirectories);

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            foreach (var file in fileInfo)
            {
                try
                {
                    string json = File.ReadAllText(file.FullName);
                    PawnRecipeData data = JsonConvert.DeserializeObject<PawnRecipeData>(json, settings);
                    
                    // pawnName이 null이거나 비어있으면 스킵 (ItemData 등 다른 형식의 JSON일 수 있음)
                    if (data == null || string.IsNullOrEmpty(data.pawnName))
                    {
                        LogManager.LogWarning(LogCategory.Recipe, $"{file.Name} - pawnName이 없거나 유효하지 않습니다. 스킵합니다.");
                        continue;
                    }
                    
                    recipes[data.pawnName] = data;
                    LogManager.LogInfo(LogCategory.Recipe, $"레시피 로드: {data.pawnName} (from {file.Name})");
                }
                catch (System.Exception ex)
                {
                    LogManager.LogError(LogCategory.Recipe, $"{file.Name} 로드 실패: {ex.Message}");
                }
            }
            
            LogManager.LogInfo(LogCategory.Recipe, $"총 {recipes.Count}개의 레시피 로드 완료");
        }

        public PawnRecipeData GetRecipe(string recipeName)
        {
            recipes.TryGetValue(recipeName, out PawnRecipeData recipe);
            return recipe;
        }
    }
}
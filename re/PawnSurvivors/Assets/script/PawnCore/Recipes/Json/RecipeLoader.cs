using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace PawnCore.Recipes.Json
{
    public class RecipeLoader
    {
        private Dictionary<string, PawnRecipeData> recipes = new Dictionary<string, PawnRecipeData>();

        public void LoadRecipes(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Debug.LogError($"Recipe directory not found: {directoryPath}");
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
                string json = File.ReadAllText(file.FullName);
                PawnRecipeData data = JsonConvert.DeserializeObject<PawnRecipeData>(json, settings);
                recipes[data.pawnName] = data;
                Debug.Log($"[RecipeLoader] 레시피 로드: {data.pawnName} (from {file.Name})");
            }
            
            Debug.Log($"[RecipeLoader] 총 {recipes.Count}개의 레시피 로드 완료");
        }

        public PawnRecipeData GetRecipe(string recipeName)
        {
            recipes.TryGetValue(recipeName, out PawnRecipeData recipe);
            return recipe;
        }
    }
}
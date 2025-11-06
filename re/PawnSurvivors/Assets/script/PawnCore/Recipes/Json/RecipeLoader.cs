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
            var fileInfo = info.GetFiles("*.json");

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            foreach (var file in fileInfo)
            {
                string json = File.ReadAllText(file.FullName);
                PawnRecipeData data = JsonConvert.DeserializeObject<PawnRecipeData>(json, settings);
                recipes[data.pawnName] = data;
            }
        }

        public PawnRecipeData GetRecipe(string recipeName)
        {
            recipes.TryGetValue(recipeName, out PawnRecipeData recipe);
            return recipe;
        }
    }
}
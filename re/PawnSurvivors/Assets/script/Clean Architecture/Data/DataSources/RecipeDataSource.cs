using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using PawnSurvivors.Managers;
using PawnSurvivors.Data.Recipes;

namespace PawnSurvivors.Data.DataSources
{
    /// <summary>
    /// Pawn 레시피 JSON 파일들을 읽어서 PawnRecipeData 모델로 반환하는 데이터 소스입니다.
    /// Data 계층의 데이터 소스 역할을 담당합니다.
    /// </summary>
    public class RecipeDataSource
    {
        private Dictionary<string, PawnRecipeData> _recipes = new Dictionary<string, PawnRecipeData>();

        /// <summary>
        /// 지정된 디렉토리에서 모든 Pawn 레시피 JSON 파일을 로드합니다.
        /// Players/, Enemies/, Projectiles/ 폴더만 읽습니다.
        /// </summary>
        /// <param name="directoryPath">레시피 디렉토리 경로</param>
        public void LoadRecipes(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                LogManager.LogError(LogCategory.Recipe, $"Recipe directory not found: {directoryPath}");
                return;
            }

            // Pawn 레시피가 있는 폴더만 읽기 (Items 폴더 제외)
            string[] recipeFolders = { "Players", "Enemies", "Projectiles" };
            var allFiles = new List<FileInfo>();

            foreach (var folderName in recipeFolders)
            {
                string folderPath = Path.Combine(directoryPath, folderName);
                if (Directory.Exists(folderPath))
                {
                    var folderInfo = new DirectoryInfo(folderPath);
                    var files = folderInfo.GetFiles("*.json", SearchOption.TopDirectoryOnly);
                    allFiles.AddRange(files);
                }
                else
                {
                    LogManager.LogWarning(LogCategory.Recipe, $"레시피 폴더를 찾을 수 없습니다: {folderPath}");
                }
            }

            if (allFiles.Count == 0)
            {
                LogManager.LogWarning(LogCategory.Recipe, "로드할 레시피 파일이 없습니다.");
                return;
            }

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            foreach (var file in allFiles)
            {
                try
                {
                    string json = File.ReadAllText(file.FullName);
                    PawnRecipeData data = JsonConvert.DeserializeObject<PawnRecipeData>(json, settings);
                    
                    // pawnName이 null이거나 비어있으면 스킵
                    if (data == null || string.IsNullOrEmpty(data.pawnName))
                    {
                        LogManager.LogWarning(LogCategory.Recipe, $"{file.Name} - pawnName이 없거나 유효하지 않습니다. 스킵합니다.");
                        continue;
                    }
                    
                    _recipes[data.pawnName] = data;
                    LogManager.LogInfo(LogCategory.Recipe, $"레시피 로드: {data.pawnName} (from {file.Name})");
                }
                catch (System.Exception ex)
                {
                    LogManager.LogError(LogCategory.Recipe, $"{file.Name} 로드 실패: {ex.Message}");
                }
            }
            
            LogManager.LogInfo(LogCategory.Recipe, $"총 {_recipes.Count}개의 레시피 로드 완료");
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
    }
}


using System.Collections.Generic;
using System.Linq;
using PawnSurvivors.Data.DataSources;
using PawnSurvivors.Data.Recipes;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.Data.Repositories
{
    /// <summary>
    /// Pawn 레시피 데이터 접근을 위한 Repository 구현체입니다.
    /// Data 계층에 위치하며, RecipeDataSource를 통해 데이터에 접근합니다.
    /// </summary>
    public class RecipeRepository : IRecipeRepository
    {
        private readonly RecipeDataSource _recipeDataSource;

        public RecipeRepository(RecipeDataSource recipeDataSource)
        {
            _recipeDataSource = recipeDataSource;
        }

        /// <summary>
        /// 레시피 이름으로 레시피 데이터를 가져옵니다. (하위 호환성)
        /// </summary>
        public PawnRecipeData GetRecipe(string recipeName)
        {
            return _recipeDataSource.GetRecipe(recipeName);
        }

        /// <summary>
        /// 플레이어 캐릭터 enum으로 레시피 데이터를 가져옵니다. (enum 기반)
        /// </summary>
        public PawnRecipeData GetRecipe(PlayerCharacter character)
        {
            string recipeName = character.ToString();
            return _recipeDataSource.GetRecipe(recipeName);
        }

        /// <summary>
        /// 모든 Player 레시피 이름 목록을 가져옵니다. (하위 호환성)
        /// </summary>
        public List<string> GetAllPlayerRecipeNames()
        {
            return _recipeDataSource.GetAllPlayerRecipeNames();
        }

        /// <summary>
        /// 모든 플레이어 캐릭터 enum 목록을 가져옵니다. (enum 기반)
        /// </summary>
        public List<PlayerCharacter> GetAllPlayerCharacters()
        {
            var names = _recipeDataSource.GetAllPlayerRecipeNames();
            var characters = new List<PlayerCharacter>();
            
            foreach (var name in names)
            {
                if (System.Enum.TryParse<PlayerCharacter>(name, true, out PlayerCharacter character))
                {
                    characters.Add(character);
                }
            }
            
            return characters;
        }
    }
}


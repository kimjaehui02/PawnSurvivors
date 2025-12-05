using System.Collections.Generic;
using PawnSurvivors.Data.Recipes;

namespace PawnSurvivors.Domain.Repositories
{
    /// <summary>
    /// Pawn 레시피 데이터 접근을 위한 Repository 인터페이스입니다.
    /// Domain 계층에 위치하며, Data 계층의 구현체에 의존하지 않습니다.
    /// </summary>
    public interface IRecipeRepository
    {
        /// <summary>
        /// 레시피 이름으로 레시피 데이터를 가져옵니다.
        /// </summary>
        PawnRecipeData GetRecipe(string recipeName);

        /// <summary>
        /// 모든 Player 레시피 이름 목록을 가져옵니다.
        /// </summary>
        List<string> GetAllPlayerRecipeNames();
    }
}


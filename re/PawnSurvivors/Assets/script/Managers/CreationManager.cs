using UnityEngine;
using PawnCore.Recipes.Json;
using PawnCore.Recipes;
using System.IO;
using System.Linq;
using PawnCore.Domain;

public class CreationManager : MonoBehaviour
{
    private RecipeLoader recipeLoader;
    private ShadowPresetLoader shadowPresetLoader;

    private void Awake()
    {
        recipeLoader = new RecipeLoader();
        string recipesPath = Path.Combine(Application.streamingAssetsPath, "Recipes");
        recipeLoader.LoadRecipes(recipesPath);
        
        // ShadowPreset 로드
        shadowPresetLoader = new ShadowPresetLoader();
        string shadowPresetsPath = Path.Combine(Application.streamingAssetsPath, "ShadowPresets");
        shadowPresetLoader.LoadPresets(shadowPresetsPath);
    }
    
    /// <summary>
    /// ShadowPreset을 이름으로 가져옵니다.
    /// </summary>
    public ShadowPresetData GetShadowPreset(string presetName)
    {
        return shadowPresetLoader?.GetPreset(presetName);
    }

    public PawnRecipeData GetRecipe(string recipeName)
    {
        return recipeLoader.GetRecipe(recipeName);
    }

    public GameObject CreatePawn(string recipeName, Vector3 position, Quaternion rotation)
    {
        PawnRecipeData recipeData = recipeLoader.GetRecipe(recipeName);
        if (recipeData == null)
        {
            Debug.LogError($"CreationManager: PawnRecipe with name '{recipeName}' not found.");
            return null;
        }
        return CreatePawn(recipeData, position, rotation);
    }

    public GameObject CreatePawn(PawnRecipeData recipeData, Vector3 position, Quaternion rotation, Vector3? direction = null)
    {
        if (recipeData == null)
        {
            Debug.LogError("CreationManager: PawnRecipeData is null.");
            return null;
        }

        // 1. 기본 GameObject 생성
        GameObject pawnObject = new GameObject(recipeData.pawnName);
        pawnObject.transform.position = position;
        pawnObject.transform.rotation = rotation;

        // 3. 필수 PawnManager를 추가하고 PawnData를 초기화합니다.
        PawnManager pawnManager = pawnObject.AddComponent<PawnManager>();
        pawnManager.PawnData = recipeData.ToPawnData();

        // 방향이 제공되면 PawnData의 directionalMovement.moveDirection을 재정의합니다.
        if (direction.HasValue && pawnManager.PawnData.movableData.directionalMovement != null)
        {
            pawnManager.PawnData.movableData.directionalMovement.moveDirection = direction.Value;
        }

        // 4. 레시피에서 모든 하위 관리자를 추가하고 구성합니다.
        if (recipeData.subManagerSetups != null)
        {
            foreach (var setup in recipeData.subManagerSetups)
            {
                if (setup != null)
                {
                    MonoBehaviour subManagerComponent = setup.AddSubManagerComponent(pawnObject);
                    if (subManagerComponent is PawnSubManager pawnSubManager)
                    {
                        pawnManager.RegisterSubManager(pawnSubManager);
                    }
                }
            }
        }

        // PawnData가 완전히 설정된 후 등록된 모든 SubManager를 초기화합니다.
        pawnManager.InitializeSubManagers();

        // Debug.Log($"JSON 레시피에서 '{recipeData.pawnName}' 폰을 성공적으로 생성했습니다.");
        return pawnObject;
    }

    /// <summary>
    /// 모든 Pawn을 파괴합니다. (메인 메뉴로 돌아갈 때 사용)
    /// </summary>
    public void DestroyAllPawns()
    {
        // PawnManager.AllPawnManagers의 복사본을 만들어 순회
        // (파괴 중에 리스트가 수정되므로)
        var allPawns = PawnManager.AllPawnManagers.ToArray();
        
        foreach (var pawnManager in allPawns)
        {
            if (pawnManager != null && pawnManager.gameObject != null)
            {
                Destroy(pawnManager.gameObject);
            }
        }
        
        Debug.Log($"Destroyed {allPawns.Length} pawns.");
    }
}
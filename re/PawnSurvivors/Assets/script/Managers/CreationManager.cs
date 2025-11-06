using UnityEngine;
using PawnCore.Recipes.Json;
using System.IO;
using PawnCore.Domain;

public class CreationManager : MonoBehaviour
{
    private RecipeLoader recipeLoader;

    private void Awake()
    {
        recipeLoader = new RecipeLoader();
        string recipesPath = Path.Combine(Application.streamingAssetsPath, "Recipes");
        recipeLoader.LoadRecipes(recipesPath);
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

        // 1. Create the base GameObject
        GameObject pawnObject = new GameObject(recipeData.pawnName);
        pawnObject.transform.position = position;
        pawnObject.transform.rotation = rotation;

        // 3. Add the mandatory PawnManager and initialize PawnData
        PawnManager pawnManager = pawnObject.AddComponent<PawnManager>();
        pawnManager.PawnData = recipeData.ToPawnData();

        // If a direction is provided, override the PawnData's directionalMovement.moveDirection
        if (direction.HasValue && pawnManager.PawnData.movableData.directionalMovement != null)
        {
            pawnManager.PawnData.movableData.directionalMovement.moveDirection = direction.Value;
        }

        // 4. Add and configure all sub-managers from the recipe
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

        // Initialize all registered SubManagers after PawnData is fully set up.
        pawnManager.InitializeSubManagers();

        // Debug.Log($"Successfully created pawn '{recipeData.pawnName}' from JSON recipe.");
        return pawnObject;
    }
}
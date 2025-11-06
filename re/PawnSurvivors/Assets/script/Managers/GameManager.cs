using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public LifecycleManager LifecycleManager { get; private set; }
    public CreationManager CreationManager { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Get components on the same GameObject
        LifecycleManager = GetComponent<LifecycleManager>();
        CreationManager = GetComponent<CreationManager>();

        if (LifecycleManager == null)
        {
            Debug.LogError("GameManager: LifecycleManager component not found on the same GameObject.");
        }
        if (CreationManager == null)
        {
            Debug.LogError("GameManager: CreationManager component not found on the same GameObject.");
        }
    }

    private void Start()
    {
        // Create the player pawn using the "Player" recipe
        PawnCore.Recipes.Json.PawnRecipeData playerRecipe = CreationManager.GetRecipe("Player");
        if (playerRecipe != null)
        {
            CreationManager.CreatePawn(playerRecipe, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Player recipe not found! Cannot create player pawn.");
        }

        // Create an enemy pawn for testing collision
        PawnCore.Recipes.Json.PawnRecipeData enemyRecipe = CreationManager.GetRecipe("Enemy");
        if (enemyRecipe != null)
        {
            CreationManager.CreatePawn(enemyRecipe, new Vector3(2, 2, 0), Quaternion.identity); // Spawn enemy at a different position
        }
        else
        {
            Debug.LogError("Enemy recipe not found! Cannot create enemy pawn.");
        }
    }
}

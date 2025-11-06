using UnityEngine;
using System.Collections.Generic;

// This class is now obsolete. Pawn configuration is handled by JSON files and PawnCore.Recipes.Json.PawnRecipeData.
// This file is kept as a placeholder to avoid breaking existing ScriptableObject assets that might reference it.
// You can delete this file and its associated assets once all recipes have been migrated to JSON.
[CreateAssetMenu(fileName = "NewPawnRecipe", menuName = "Pawn/Pawn Recipe")]
public class PawnRecipe : ScriptableObject
{
    [Header("General")]
    public string pawnName = "New Pawn";
    public GameObject visualPrefab; // A prefab with just the visuals (Sprite, Animator, etc.)
}
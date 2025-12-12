using UnityEngine;
using PawnSurvivors.Data.Recipes;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using PawnSurvivors.Domain;
using PawnSurvivors.Managers;
using PawnSurvivors.Data.DataSources;

public class CreationManager : MonoBehaviour
{
    private RecipeDataSource _recipeDataSource;
    private ShadowPresetDataSource _shadowPresetDataSource;

    private void Awake()
    {
        _recipeDataSource = new RecipeDataSource();
        // Resources 폴더 경로 사용 (WebGL 호환)
        string recipesPath = "StreamingAssets/Recipes";
        _recipeDataSource.LoadRecipes(recipesPath);
        
        // ShadowPreset 로드
        _shadowPresetDataSource = new ShadowPresetDataSource();
        string shadowPresetsPath = "StreamingAssets/ShadowPresets";
        _shadowPresetDataSource.LoadPresets(shadowPresetsPath);
    }
    
    /// <summary>
    /// ShadowPreset을 이름으로 가져옵니다.
    /// </summary>
    public ShadowPresetData GetShadowPreset(string presetName)
    {
        return _shadowPresetDataSource?.GetPreset(presetName);
    }

    public PawnRecipeData GetRecipe(string recipeName)
    {
        return _recipeDataSource.GetRecipe(recipeName);
    }

    public GameObject CreatePawn(string recipeName, Vector3 position, Quaternion rotation)
    {
        PawnRecipeData recipeData = _recipeDataSource.GetRecipe(recipeName);
        if (recipeData == null)
        {
            LogManager.LogError(LogCategory.System, $"PawnRecipe with name '{recipeName}' not found.");
            return null;
        }
        return CreatePawn(recipeData, position, rotation);
    }

    public GameObject CreatePawn(PawnRecipeData recipeData, Vector3 position, Quaternion rotation, Vector3? direction = null, PawnManager owner = null, float overrideDamage = 0f, float overrideSpeed = 0f, bool isSupportCharacter = false)
    {
        if (recipeData == null)
        {
            LogManager.LogError(LogCategory.System, "PawnRecipeData is null.");
            return null;
        }

        // 1. 기본 GameObject 생성
        GameObject pawnObject = new GameObject(recipeData.pawnName);
        pawnObject.transform.position = position;
        pawnObject.transform.rotation = rotation;

        // 3. 필수 PawnManager를 추가하고 PawnData를 초기화합니다.
        PawnManager pawnManager = pawnObject.AddComponent<PawnManager>();
        pawnManager.PawnData = recipeData.ToPawnData();
        
        // 레시피 이름 설정 (UI 표시용)
        pawnManager.PawnData.recipeName = recipeData.pawnName;
        
        // 소유자 설정 (탄환의 경우 발사자)
        pawnManager.Owner = owner;
        
        // 발사자의 데미지가 제공되면 투사체의 데미지를 덮어쓰기
        if (overrideDamage > 0f && pawnManager.PawnData.combatData != null)
        {
            pawnManager.PawnData.combatData.damage = overrideDamage;
        }

        // 발사자의 속도가 제공되면 투사체의 속도를 덮어쓰기
        if (overrideSpeed > 0f && pawnManager.PawnData.movableData.directionalMovement != null)
        {
            pawnManager.PawnData.movableData.directionalMovement.speed = overrideSpeed;
        }

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
                    // ✅ 서포트 캐릭터는 불필요한 SubManager 스킵
                    // - DamageableSubManager: 데미지 안 받음
                    // - HealthBarSubManager: 체력바 불필요
                    // - InvincibilitySubManager: 데미지 안 받으니 무적도 불필요
                    // 주의: PhysicsSubManager는 태그 설정이 필요하므로 스킵하지 않음
                    if (isSupportCharacter)
                    {
                        if (setup is DamageableSubManagerSetupData ||
                            setup is HealthBarSubManagerSetupData ||
                            setup is InvincibilitySubManagerSetupData)
                        {
                            continue; // 스킵
                        }

                        // PhysicsSubManager는 Collider만 비활성화 (태그는 필요)
                        if (setup is PhysicsSubManagerSetupData physicsSetup)
                        {
                            physicsSetup.colliderType = ColliderType.None;
                        }
                    }

                    MonoBehaviour subManagerComponent = setup.AddSubManagerComponent(pawnObject);
                    if (subManagerComponent is PawnSubManager pawnSubManager)
                    {
                        pawnManager.RegisterSubManager(pawnSubManager);
                    }
                }
            }
        }
        
        // 5. upgradeSettings가 있으면 자동으로 CharacterUpgradeSubManager 추가
        if (recipeData.upgradeSettings != null)
        {
            // 이미 CharacterUpgradeSubManager가 있는지 확인
            if (pawnObject.GetComponent<CharacterUpgradeSubManager>() == null)
            {
                var upgradeSubManager = pawnObject.AddComponent<CharacterUpgradeSubManager>();
                if (upgradeSubManager is PawnSubManager pawnSubManager)
                {
                    pawnManager.RegisterSubManager(pawnSubManager);
                }
            }
        }

        // PawnData가 완전히 설정된 후 등록된 모든 SubManager를 초기화합니다.
        pawnManager.InitializeSubManagers();
        
        // 소유자의 playerIndex를 투사체에 전달 (아이템 효과 계산용)
        // InitializeSubManagers() 이후에 설정하여 SubManager가 playerIndex를 덮어쓰지 않도록 함
        if (owner != null && owner.PawnData != null)
        {
            if (owner.PawnData.playerIndex >= 0)
            {
                pawnManager.PawnData.playerIndex = owner.PawnData.playerIndex;
                // Debug.Log($"[CreationManager] 투사체 {pawnObject.name}의 playerIndex를 {owner.PawnData.playerIndex}로 설정 (Owner: {owner.name})");
            }
        }

        // FloatingEffectManager에 새로 생성된 Pawn 구독 (StageScreen에서 찾기)
        var stageScreen = FindFirstObjectByType<PawnSurvivors.UI.StageScreen>();
        if (stageScreen != null)
        {
            var floatingEffectManager = stageScreen.GetComponent<FloatingEffectManager>();
            if (floatingEffectManager != null)
            {
                floatingEffectManager.SubscribeToPawnManager(pawnManager);
            }
        }

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
        
        LogManager.LogInfo(LogCategory.System, $"Destroyed {allPawns.Length} pawns.");
    }

    /// <summary>
    /// 모든 Player 레시피 이름 목록을 가져옵니다.
    /// </summary>
    public List<string> GetAllPlayerRecipeNames()
    {
        return _recipeDataSource.GetAllPlayerRecipeNames();
    }
}
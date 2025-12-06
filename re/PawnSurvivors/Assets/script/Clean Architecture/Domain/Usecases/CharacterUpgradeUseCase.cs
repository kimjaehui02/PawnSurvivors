using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Repositories;
using PawnSurvivors.Data.Recipes;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// 캐릭터 강화를 담당하는 UseCase입니다.
    /// 같은 캐릭터를 여러 번 구매할 때 기존 Pawn을 강화합니다.
    /// JSON 레시피에서 강화 설정을 로드하여 사용합니다.
    /// </summary>
    public class CharacterUpgradeUseCase
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly ISessionDataRepository _sessionRepository;
        
        private const string PURCHASE_COUNT_MAP = "characterPurchases";
        
        public CharacterUpgradeUseCase(
            IRecipeRepository recipeRepository,
            ISessionDataRepository sessionRepository)
        {
            _recipeRepository = recipeRepository;
            _sessionRepository = sessionRepository;
        }

        /// <summary>
        /// 캐릭터 구매 개수를 증가시킵니다. (첫 구매 포함)
        /// </summary>
        public void IncrementPurchaseCount(PlayerCharacter characterType)
        {
            string characterKey = characterType.ToString();
            int currentCount = _sessionRepository.GetCounter(PURCHASE_COUNT_MAP, characterKey, 0);
            _sessionRepository.SetCounter(PURCHASE_COUNT_MAP, characterKey, currentCount + 1);
        }
        
        /// <summary>
        /// 캐릭터를 구매했을 때 호출됩니다. 구매 개수를 증가시키고 필요시 강화를 적용합니다.
        /// </summary>
        /// <param name="characterType">구매한 캐릭터 타입</param>
        /// <param name="allPawnData">모든 Pawn의 PawnData 리스트</param>
        /// <returns>강화가 적용되었으면 true, 새로 생성해야 하면 false</returns>
        public bool OnCharacterPurchased(PlayerCharacter characterType, List<PawnData> allPawnData)
        {
            if (allPawnData == null || allPawnData.Count == 0)
            {
                return false;
            }

            // 같은 캐릭터 타입의 첫 번째 Pawn 찾기
            var existingPawn = allPawnData.FirstOrDefault(p => 
                p != null && 
                p.characterType.HasValue && 
                p.characterType.Value == characterType);

            if (existingPawn == null)
            {
                // 기존 Pawn이 없으면 새로 생성해야 함
                return false;
            }

            // 구매 개수 증가 전 현재 값 저장
            string characterKey = characterType.ToString();
            int currentCount = _sessionRepository.GetCounter(PURCHASE_COUNT_MAP, characterKey, 0);
            IncrementPurchaseCount(characterType);
            int newCount = currentCount + 1;

            // 강화 설정 가져오기
            var recipe = _recipeRepository.GetRecipe(characterType);
            if (recipe == null || recipe.upgradeSettings == null)
            {
                // 강화 설정이 없으면 강화 안 함
                return true;
            }

            // 현재 강화 단계 계산
            int currentStage = GetCurrentUpgradeStage(newCount, recipe.upgradeSettings);
            int previousStage = GetCurrentUpgradeStage(currentCount, recipe.upgradeSettings);

            // 강화 단계가 올라갔으면 보상 적용
            if (currentStage > previousStage)
            {
                // CharacterUpgradeSubManager를 찾아서 스탯 적용 위임
                var pawnManager = FindPawnManagerByPawnData(existingPawn);
                if (pawnManager != null)
                {
                    var upgradeSubManager = pawnManager.GetComponent<CharacterUpgradeSubManager>();
                    if (upgradeSubManager != null)
                    {
                        upgradeSubManager.ApplyUpgradeStage(currentStage, recipe.upgradeSettings);
                        existingPawn.upgradeLevel = currentStage;
                    }
                    else
                    {
                        // SubManager가 없으면 경고 (JSON에 설정이 없을 수 있음)
                        Debug.LogWarning($"[CharacterUpgradeUseCase] {characterType}의 CharacterUpgradeSubManager를 찾을 수 없습니다. 강화가 적용되지 않습니다.");
                    }
                }
                else
                {
                    Debug.LogWarning($"[CharacterUpgradeUseCase] {characterType}의 PawnManager를 찾을 수 없습니다. 강화가 적용되지 않습니다.");
                }
            }

            return true;
        }
        
        /// <summary>
        /// PawnData로부터 해당하는 PawnManager를 찾습니다.
        /// </summary>
        private PawnManager FindPawnManagerByPawnData(PawnData pawnData)
        {
            if (pawnData == null) return null;
            
            // PawnManager.AllPawnManagers에서 해당 PawnData를 가진 PawnManager 찾기
            foreach (var pawnManager in PawnManager.AllPawnManagers)
            {
                if (pawnManager != null && pawnManager.PawnData == pawnData)
                {
                    return pawnManager;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 현재 구매 개수에 따른 강화 단계를 계산합니다.
        /// </summary>
        private int GetCurrentUpgradeStage(int purchaseCount, CharacterUpgradeSettings settings)
        {
            if (settings == null || settings.requiredPurchaseCounts == null)
            {
                return 0;
            }

            int stage = 0;
            for (int i = 0; i < settings.requiredPurchaseCounts.Length; i++)
            {
                if (purchaseCount >= settings.requiredPurchaseCounts[i])
                {
                    stage = i + 1;
                }
                else
                {
                    break;
                }
            }

            return stage;
        }

        /// <summary>
        /// 특정 캐릭터 타입의 현재 구매 개수를 가져옵니다.
        /// </summary>
        public int GetPurchaseCount(PlayerCharacter characterType)
        {
            string characterKey = characterType.ToString();
            return _sessionRepository.GetCounter(PURCHASE_COUNT_MAP, characterKey, 0);
        }
        
        /// <summary>
        /// 특정 캐릭터 타입의 현재 강화 단계를 가져옵니다.
        /// </summary>
        public int GetUpgradeStage(PlayerCharacter characterType)
        {
            var recipe = _recipeRepository.GetRecipe(characterType);
            if (recipe == null || recipe.upgradeSettings == null)
            {
                return 0;
            }

            int purchaseCount = GetPurchaseCount(characterType);
            return GetCurrentUpgradeStage(purchaseCount, recipe.upgradeSettings);
        }

        /// <summary>
        /// 특정 캐릭터 타입이 이미 존재하는지 확인합니다.
        /// </summary>
        /// <param name="characterType">캐릭터 타입</param>
        /// <param name="allPawnData">모든 Pawn의 PawnData 리스트</param>
        /// <returns>존재 여부</returns>
        public bool HasCharacter(PlayerCharacter characterType, List<PawnData> allPawnData)
        {
            if (allPawnData == null || allPawnData.Count == 0)
            {
                return false;
            }

            return allPawnData.Any(p => 
                p != null && 
                p.characterType.HasValue && 
                p.characterType.Value == characterType);
        }

        /// <summary>
        /// 고유한 캐릭터 종류 개수를 반환합니다.
        /// </summary>
        /// <param name="allPawnData">모든 Pawn의 PawnData 리스트</param>
        /// <returns>고유한 캐릭터 종류 개수</returns>
        public int GetUniqueCharacterCount(List<PawnData> allPawnData)
        {
            if (allPawnData == null || allPawnData.Count == 0)
            {
                return 0;
            }

            var uniqueCharacterTypes = new HashSet<PlayerCharacter>();
            foreach (var pawnData in allPawnData)
            {
                if (pawnData != null && pawnData.characterType.HasValue)
                {
                    uniqueCharacterTypes.Add(pawnData.characterType.Value);
                }
            }

            return uniqueCharacterTypes.Count;
        }

        /// <summary>
        /// 새 캐릭터를 구매할 수 있는지 확인합니다. (최대 종류 제한 체크 포함)
        /// </summary>
        /// <param name="characterType">구매하려는 캐릭터 타입</param>
        /// <param name="allPawnData">모든 Pawn의 PawnData 리스트</param>
        /// <returns>구매 가능 여부</returns>
        public bool CanPurchaseNewCharacter(PlayerCharacter characterType, List<PawnData> allPawnData)
        {
            // 이미 같은 캐릭터가 있으면 중복 구매 가능 (강화 목적)
            if (HasCharacter(characterType, allPawnData))
            {
                return true;
            }

            // 새 캐릭터인 경우 최대 종류 제한 체크
            int currentCount = GetUniqueCharacterCount(allPawnData);
            return currentCount < GameConstants.MAX_CHARACTER_TYPES;
        }
    }
}


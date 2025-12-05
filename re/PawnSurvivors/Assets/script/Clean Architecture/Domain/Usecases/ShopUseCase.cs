using System.Collections.Generic;
using System.Linq;
using PawnSurvivors.Data;
using PawnSurvivors.Data.Recipes;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// 상점 관리를 담당하는 UseCase입니다.
    /// 상점 아이템 + 캐릭터 가져오기, 리롤 등의 로직을 처리합니다.
    /// </summary>
    public class ShopUseCase
    {
        private readonly ItemPoolUseCase _itemPoolUseCase;
        private readonly IItemRepository _itemRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly CurrencyUseCase _currencyUseCase;

        public ShopUseCase(
            ItemPoolUseCase itemPoolUseCase,
            IItemRepository itemRepository,
            IRecipeRepository recipeRepository,
            CurrencyUseCase currencyUseCase)
        {
            _itemPoolUseCase = itemPoolUseCase;
            _itemRepository = itemRepository;
            _recipeRepository = recipeRepository;
            _currencyUseCase = currencyUseCase;
        }

        /// <summary>
        /// 상점에 표시할 아이템을 랜덤하게 가져옵니다.
        /// </summary>
        /// <param name="count">가져올 아이템 개수</param>
        /// <param name="excludeOwned">보유한 아이템 제외 여부</param>
        /// <param name="excludeItemIds">제외할 아이템 ID 목록 (잠긴 슬롯의 아이템 등)</param>
        /// <returns>선택된 아이템 목록</returns>
        public List<ItemData> GetShopItems(int count, bool excludeOwned = false, List<string> excludeItemIds = null)
        {
            return _itemPoolUseCase.GetRandomShopItems(count, excludeOwned, _itemRepository, excludeItemIds);
        }

        /// <summary>
        /// 상점을 리롤합니다. (골드를 소비하고 새 아이템을 가져옵니다)
        /// </summary>
        /// <param name="cost">리롤 비용</param>
        /// <param name="itemCount">가져올 아이템 개수</param>
        /// <param name="excludeOwned">보유한 아이템 제외 여부</param>
        /// <param name="excludeItemIds">제외할 아이템 ID 목록 (잠긴 슬롯의 아이템 등)</param>
        /// <returns>리롤 성공 여부 및 새 아이템 목록 (성공 시 아이템 목록, 실패 시 null)</returns>
        public List<ItemData> RerollShop(int cost, int itemCount, bool excludeOwned = false, List<string> excludeItemIds = null)
        {
            // 골드 확인 및 소비
            if (!_currencyUseCase.SpendGold(cost))
            {
                return null; // 골드 부족
            }

            // 새 아이템 가져오기
            return GetShopItems(itemCount, excludeOwned, excludeItemIds);
        }

        /// <summary>
        /// 리롤 비용을 지불할 수 있는지 확인합니다.
        /// </summary>
        /// <param name="cost">리롤 비용</param>
        /// <returns>리롤 가능 여부</returns>
        public bool CanReroll(int cost)
        {
            return _currencyUseCase.HasEnoughGold(cost);
        }

        // ==================== 아이템 + 캐릭터 랜덤 선택 ====================

        /// <summary>
        /// 상점에 표시할 아이템과 캐릭터를 랜덤하게 선택합니다.
        /// </summary>
        /// <param name="count">선택할 총 개수</param>
        /// <param name="excludeOwnedItems">보유한 아이템 제외 여부</param>
        /// <param name="excludeItemIds">제외할 아이템 ID 목록</param>
        /// <param name="excludeCharacterNames">제외할 캐릭터 이름 목록</param>
        /// <returns>ShopSlotData 목록 (아이템 또는 캐릭터)</returns>
        public List<ShopSlotData> GetRandomShopSlots(
            int count,
            bool excludeOwnedItems = false,
            List<string> excludeItemIds = null,
            List<string> excludeCharacterNames = null)
        {
            var availableSlots = new List<ShopSlotData>();

            // 1. 모든 아이템 가져오기
            List<ItemData> allItems = _itemPoolUseCase.GetRandomShopItems(9999, excludeOwnedItems, _itemRepository, excludeItemIds);
            
            // 아이템을 ShopSlotData로 변환
            foreach (var item in allItems)
            {
                availableSlots.Add(new ShopSlotData
                {
                    slotType = ShopSlotType.Item,
                    itemData = item,
                    characterRecipeName = null
                });
            }

            // 2. 모든 플레이어 캐릭터 가져오기
            if (_recipeRepository != null)
            {
                List<string> allCharacters = _recipeRepository.GetAllPlayerRecipeNames();
                
                // 특정 캐릭터 이름 제외
                if (excludeCharacterNames != null && excludeCharacterNames.Count > 0)
                {
                    var excludeSet = new HashSet<string>(excludeCharacterNames);
                    allCharacters = allCharacters.Where(name => !excludeSet.Contains(name)).ToList();
                }

                // 캐릭터를 ShopSlotData로 변환
                foreach (var characterName in allCharacters)
                {
                    var recipe = _recipeRepository.GetRecipe(characterName);
                    if (recipe != null)
                    {
                        availableSlots.Add(new ShopSlotData
                        {
                            slotType = ShopSlotType.Character,
                            itemData = null,
                            characterRecipeName = characterName,
                            characterRecipe = recipe
                        });
                    }
                }
            }

            // 3. 랜덤 선택
            var selectedSlots = new List<ShopSlotData>();
            int selectCount = System.Math.Min(count, availableSlots.Count);

            for (int i = 0; i < selectCount; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableSlots.Count);
                selectedSlots.Add(availableSlots[randomIndex]);
                availableSlots.RemoveAt(randomIndex); // 중복 방지
            }

            return selectedSlots;
        }

        /// <summary>
        /// 캐릭터만 랜덤하게 선택합니다.
        /// </summary>
        public List<string> GetRandomCharacters(
            int count,
            List<string> excludeCharacterNames = null)
        {
            if (_recipeRepository == null) return new List<string>();

            List<string> allCharacters = _recipeRepository.GetAllPlayerRecipeNames();

            if (excludeCharacterNames != null && excludeCharacterNames.Count > 0)
            {
                var excludeSet = new HashSet<string>(excludeCharacterNames);
                allCharacters = allCharacters.Where(name => !excludeSet.Contains(name)).ToList();
            }

            var selectedCharacters = new List<string>();
            var availableCharacters = new List<string>(allCharacters);

            int selectCount = System.Math.Min(count, availableCharacters.Count);
            for (int i = 0; i < selectCount; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableCharacters.Count);
                selectedCharacters.Add(availableCharacters[randomIndex]);
                availableCharacters.RemoveAt(randomIndex);
            }

            return selectedCharacters;
        }
    }

    /// <summary>
    /// 상점 슬롯의 타입 (아이템 또는 캐릭터)
    /// </summary>
    public enum ShopSlotType
    {
        Item,
        Character
    }

    /// <summary>
    /// 상점 슬롯 데이터 (아이템 또는 캐릭터)
    /// </summary>
    public class ShopSlotData
    {
        public ShopSlotType slotType;
        public ItemData itemData; // slotType이 Item일 때 사용
        public string characterRecipeName; // slotType이 Character일 때 사용
        public PawnRecipeData characterRecipe; // 캐릭터 레시피 전체 데이터
    }
}


using System.Collections.Generic;
using System.Linq;
using PawnSurvivors.Data;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// 아이템 풀 관리를 담당하는 UseCase입니다.
    /// 상점에 표시할 아이템을 랜덤 선택하는 로직을 처리합니다.
    /// </summary>
    public class ItemPoolUseCase
    {
        private readonly IItemPoolRepository _itemPoolRepository;

        public ItemPoolUseCase(IItemPoolRepository itemPoolRepository)
        {
            _itemPoolRepository = itemPoolRepository;
        }

        /// <summary>
        /// 상점에 표시할 아이템을 랜덤하게 선택합니다.
        /// </summary>
        /// <param name="count">선택할 아이템 개수</param>
        /// <param name="excludeOwned">보유한 아이템 제외 여부</param>
        /// <param name="itemRepository">보유 아이템 확인용 (null이면 제외하지 않음)</param>
        /// <returns>선택된 아이템 목록</returns>
        public List<ItemData> GetRandomShopItems(
            int count,
            bool excludeOwned = false,
            PawnSurvivors.Domain.Repositories.IItemRepository itemRepository = null)
        {
            // 모든 아이템 가져오기
            List<ItemData> allItems = _itemPoolRepository.GetAllItems();

            if (allItems.Count == 0)
            {
                return new List<ItemData>();
            }

            // 보유한 아이템 제외
            if (excludeOwned && itemRepository != null)
            {
                allItems = allItems.Where(item => !itemRepository.HasItem(item.itemId)).ToList();
            }

            // 랜덤 선택
            var selectedItems = new List<ItemData>();
            var availableItems = new List<ItemData>(allItems);

            int selectCount = System.Math.Min(count, availableItems.Count);
            for (int i = 0; i < selectCount; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableItems.Count);
                selectedItems.Add(availableItems[randomIndex]);
                availableItems.RemoveAt(randomIndex); // 중복 방지
            }

            return selectedItems;
        }

        /// <summary>
        /// 특정 타입의 아이템을 랜덤하게 선택합니다.
        /// </summary>
        /// <param name="itemType">아이템 타입</param>
        /// <param name="count">선택할 아이템 개수</param>
        /// <param name="excludeOwned">보유한 아이템 제외 여부</param>
        /// <param name="itemRepository">보유 아이템 확인용</param>
        /// <returns>선택된 아이템 목록</returns>
        public List<ItemData> GetRandomShopItemsByType(
            ItemType itemType,
            int count,
            bool excludeOwned = false,
            PawnSurvivors.Domain.Repositories.IItemRepository itemRepository = null)
        {
            // 특정 타입의 아이템만 가져오기
            List<ItemData> itemsByType = _itemPoolRepository.GetItemsByType(itemType);

            if (itemsByType.Count == 0)
            {
                return new List<ItemData>();
            }

            // 보유한 아이템 제외
            if (excludeOwned && itemRepository != null)
            {
                itemsByType = itemsByType.Where(item => !itemRepository.HasItem(item.itemId)).ToList();
            }

            // 랜덤 선택
            var selectedItems = new List<ItemData>();
            var availableItems = new List<ItemData>(itemsByType);

            int selectCount = System.Math.Min(count, availableItems.Count);
            for (int i = 0; i < selectCount; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableItems.Count);
                selectedItems.Add(availableItems[randomIndex]);
                availableItems.RemoveAt(randomIndex);
            }

            return selectedItems;
        }

        /// <summary>
        /// 아이템 ID로 아이템 데이터를 가져옵니다.
        /// </summary>
        public ItemData GetItem(string itemId)
        {
            return _itemPoolRepository.GetItem(itemId);
        }

        /// <summary>
        /// 모든 아이템 목록을 가져옵니다.
        /// </summary>
        public List<ItemData> GetAllItems()
        {
            return _itemPoolRepository.GetAllItems();
        }
    }
}


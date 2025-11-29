using System.Collections.Generic;
using PawnSurvivors.Data;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Repositories;
using PawnSurvivors.Data.Loaders;

namespace PawnSurvivors.Data.Repositories
{
    /// <summary>
    /// ItemPoolLoader를 IItemPoolRepository로 감싸는 어댑터입니다.
    /// 실제 게임에서 사용하는 Repository 구현체입니다.
    /// </summary>
    public class ItemPoolRepository : IItemPoolRepository
    {
        private readonly ItemPoolLoader _itemPoolLoader;

        public ItemPoolRepository(ItemPoolLoader itemPoolLoader)
        {
            _itemPoolLoader = itemPoolLoader;
        }

        public ItemData GetItem(string itemId)
        {
            return _itemPoolLoader.GetItem(itemId);
        }

        public List<ItemData> GetAllItems()
        {
            return _itemPoolLoader.GetAllItems();
        }

        public List<ItemData> GetItemsByType(ItemType itemType)
        {
            return _itemPoolLoader.GetItemsByType(itemType);
        }
    }
}


using System.Collections.Generic;
using PawnSurvivors.Data;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Repositories;
using PawnSurvivors.Data.DataSources;

namespace PawnSurvivors.Data.Repositories
{
    /// <summary>
    /// IItemPoolRepository의 구현체입니다.
    /// ItemPoolDataSource로부터 데이터를 받아 Domain 계층에 제공합니다.
    /// </summary>
    public class ItemPoolRepository : IItemPoolRepository
    {
        private readonly ItemPoolDataSource _dataSource;

        public ItemPoolRepository(ItemPoolDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public ItemData GetItem(string itemId)
        {
            return _dataSource.GetItem(itemId);
        }

        public List<ItemData> GetAllItems()
        {
            return _dataSource.GetAllItems();
        }

        public List<ItemData> GetItemsByType(ItemType itemType)
        {
            return _dataSource.GetItemsByType(itemType);
        }
    }
}


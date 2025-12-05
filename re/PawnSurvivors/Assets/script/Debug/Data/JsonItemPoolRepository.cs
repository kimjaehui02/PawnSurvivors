using System.Collections.Generic;
using PawnSurvivors.Data;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.Debugging.Data
{
    /// <summary>
    /// IItemPoolRepository의 구현체입니다.
    /// JsonItemPoolDataSource로부터 데이터를 받아 Domain 계층에 제공합니다.
    /// 디버깅/테스트용으로 사용되며, 나중에 실제 구현 시 참고용입니다.
    /// </summary>
    public class JsonItemPoolRepository : IItemPoolRepository
    {
        private readonly JsonItemPoolDataSource _dataSource;

        public JsonItemPoolRepository(JsonItemPoolDataSource dataSource)
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


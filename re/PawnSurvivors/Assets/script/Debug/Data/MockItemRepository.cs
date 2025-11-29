using System.Collections.Generic;
using PawnSurvivors.Data;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.Debugging.Data
{
    /// <summary>
    /// 디버깅용 Mock ItemRepository 구현체입니다.
    /// 실제 아이템 저장/조회 기능은 제공하지 않고, HasItem 체크만 수행합니다.
    /// </summary>
    public class MockItemRepository : IItemRepository
    {
        private readonly HashSet<string> _ownedItems = new HashSet<string>();

        public MockItemRepository()
        {
            // 테스트용으로 일부 아이템을 보유한 상태로 시작
            _ownedItems.Add("MockGlobalDamagePlus1");
        }

        public void SaveItem(ItemData itemData)
        {
            if (itemData != null && !string.IsNullOrEmpty(itemData.itemId))
            {
                _ownedItems.Add(itemData.itemId);
            }
        }

        public bool HasItem(string itemId)
        {
            return _ownedItems.Contains(itemId);
        }

        public ItemData GetItemData(string itemId)
        {
            // Mock이므로 null 반환 (실제 데이터는 ItemPoolRepository에서 가져옴)
            return null;
        }

        public List<ItemData> GetGlobalItems()
        {
            return new List<ItemData>();
        }

        public List<ItemData> GetEquippedItems(int playerIndex)
        {
            return new List<ItemData>();
        }

        public void EquipItemToPawn(string itemId, int playerIndex)
        {
            // Mock 구현
        }

        public void UnequipItemFromPawn(string itemId, int playerIndex)
        {
            // Mock 구현
        }

        public bool IsItemEquippedToPawn(string itemId, int playerIndex)
        {
            return false;
        }

        public bool RemoveItemStack(string itemId, int count = 1)
        {
            if (_ownedItems.Contains(itemId))
            {
                _ownedItems.Remove(itemId);
                return true;
            }
            return false;
        }

        public void ClearAllItems()
        {
            _ownedItems.Clear();
        }

        public void AddItemStack(string itemId, int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                _ownedItems.Add(itemId);
            }
        }

        public int GetItemStackCount(string itemId)
        {
            return _ownedItems.Contains(itemId) ? 1 : 0;
        }
    }
}


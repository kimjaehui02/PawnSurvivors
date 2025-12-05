using System.Collections.Generic;
using PawnSurvivors.Data;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.Debugging.Data
{
    /// <summary>
    /// IItemPoolRepository의 Mock 구현체입니다.
    /// Domain 계층 테스트/디버깅용으로 사용됩니다.
    /// 실제 게임 로직과는 완전히 별개입니다.
    /// </summary>
    public class MockItemPoolRepository : IItemPoolRepository
    {
        private Dictionary<string, ItemData> _mockItems = new Dictionary<string, ItemData>();

        public MockItemPoolRepository()
        {
            // 테스트용 더미 아이템 생성
            InitializeMockItems();
        }

        private void InitializeMockItems()
        {
            // 전역 아이템: 공격력 +1
            var globalDamageItem = new ItemData
            {
                itemId = "MockGlobalDamagePlus1",
                itemName = "[테스트] 전역 공격력 +1",
                itemType = ItemType.Global,
                description = "테스트용 전역 아이템",
                cost = 50
            };
            globalDamageItem.SetStatModifier(StatKey.Damage, 1f);
            _mockItems[globalDamageItem.itemId] = globalDamageItem;

            // 장착 아이템: 공격력 2배
            var equippedDamageItem = new ItemData
            {
                itemId = "MockEquippedDamageX2",
                itemName = "[테스트] 공격력 2배",
                itemType = ItemType.Equipped,
                description = "테스트용 장착 아이템",
                cost = 100
            };
            equippedDamageItem.SetStatMultiplier(StatKey.Damage, 2f);
            _mockItems[equippedDamageItem.itemId] = equippedDamageItem;

            // 전역 아이템: 체력 +10
            var globalHealthItem = new ItemData
            {
                itemId = "MockGlobalHealthPlus10",
                itemName = "[테스트] 전역 체력 +10",
                itemType = ItemType.Global,
                description = "테스트용 전역 아이템",
                cost = 75
            };
            globalHealthItem.SetStatModifier(StatKey.MaxHealth, 10f);
            _mockItems[globalHealthItem.itemId] = globalHealthItem;
        }

        public ItemData GetItem(string itemId)
        {
            _mockItems.TryGetValue(itemId, out ItemData item);
            return item;
        }

        public List<ItemData> GetAllItems()
        {
            return new List<ItemData>(_mockItems.Values);
        }

        public List<ItemData> GetItemsByType(ItemType itemType)
        {
            var result = new List<ItemData>();
            foreach (var item in _mockItems.Values)
            {
                if (item.itemType == itemType)
                {
                    result.Add(item);
                }
            }
            return result;
        }
    }
}


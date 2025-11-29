using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PawnSurvivors.Data;
using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.Data.Repositories
{
    /// <summary>
    /// 아이템 데이터 접근을 위한 Repository 구현체입니다.
    /// Data 계층에 위치하며, GameSessionData를 직접 접근합니다.
    /// </summary>
    public class ItemRepository : IItemRepository
    {
        private readonly GameSessionData _sessionData;

        public ItemRepository(GameSessionData sessionData)
        {
            _sessionData = sessionData;
        }

        /// <summary>
        /// 아이템을 깊은 복사하여 저장합니다.
        /// 기존에 같은 아이템이 있으면 스택만 증가시킵니다.
        /// </summary>
        public void SaveItem(ItemData itemData)
        {
            if (itemData == null || string.IsNullOrEmpty(itemData.itemId))
            {
                return;
            }

            // 기존 아이템이 없으면 새로 저장
            if (!_sessionData.itemSessionData.ownedItems.ContainsKey(itemData.itemId))
            {
                // 깊은 복사 (Dictionary는 JsonUtility로 직렬화되지 않으므로 수동 복사)
                ItemData copiedData = new ItemData
                {
                    itemId = itemData.itemId,
                    itemName = itemData.itemName,
                    itemType = itemData.itemType,
                    description = itemData.description,
                    cost = itemData.cost,
                    iconPath = itemData.iconPath,
                    itemFunctionType = itemData.itemFunctionType
                };
                
                // Dictionary 수동 복사
                copiedData.statModifiers = new Dictionary<int, float>(itemData.statModifiers);
                copiedData.statMultipliers = new Dictionary<int, float>(itemData.statMultipliers);
                copiedData.upgradeModifiers = new Dictionary<int, int>(itemData.upgradeModifiers);
                copiedData.functionParameters = new Dictionary<string, float>(itemData.functionParameters);

                _sessionData.itemSessionData.ownedItems[itemData.itemId] = copiedData;
                _sessionData.itemSessionData.itemStacks[itemData.itemId] = 1;
            }
            else
            {
                // 기존 아이템이 있으면 스택만 증가
                AddItemStack(itemData.itemId, 1);
            }
        }

        /// <summary>
        /// 특정 Pawn에 아이템을 장착합니다.
        /// </summary>
        public void EquipItemToPawn(string itemId, int playerIndex)
        {
            if (!_sessionData.itemSessionData.ownedItems.ContainsKey(itemId))
            {
                return; // 아이템을 보유하지 않음
            }

            var itemData = _sessionData.itemSessionData.ownedItems[itemId];
            if (itemData.itemType != PawnSurvivors.Domain.ItemType.Equipped)
            {
                return; // 장착 가능한 아이템이 아님
            }

            // equippedItemIds에 추가 (List로 변경하여 같은 아이템 중복 장착 가능)
            if (!_sessionData.itemSessionData.equippedItemIds.ContainsKey(playerIndex))
            {
                _sessionData.itemSessionData.equippedItemIds[playerIndex] = new List<string>();
            }

            _sessionData.itemSessionData.equippedItemIds[playerIndex].Add(itemId);
        }

        /// <summary>
        /// 특정 Pawn에서 아이템을 해제합니다.
        /// </summary>
        public void UnequipItemFromPawn(string itemId, int playerIndex)
        {
            if (!_sessionData.itemSessionData.equippedItemIds.ContainsKey(playerIndex))
            {
                return;
            }

            _sessionData.itemSessionData.equippedItemIds[playerIndex].Remove(itemId);
        }

        /// <summary>
        /// 전역 아이템 목록을 가져옵니다.
        /// globalItemIds를 제거하고 ownedItems에서 직접 필터링합니다.
        /// </summary>
        public List<ItemData> GetGlobalItems()
        {
            return _sessionData.itemSessionData.ownedItems.Values
                .Where(item => item.itemType == PawnSurvivors.Domain.ItemType.Global)
                .ToList();
        }

        /// <summary>
        /// 특정 Pawn에 장착된 아이템 목록을 가져옵니다.
        /// </summary>
        public List<ItemData> GetEquippedItems(int playerIndex)
        {
            if (!_sessionData.itemSessionData.equippedItemIds.ContainsKey(playerIndex))
            {
                return new List<ItemData>();
            }

            // List에서 중복을 제거하지 않고 그대로 반환 (같은 아이템 여러 번 장착 가능)
            return _sessionData.itemSessionData.equippedItemIds[playerIndex]
                .Where(itemId => _sessionData.itemSessionData.ownedItems.ContainsKey(itemId))
                .Select(itemId => _sessionData.itemSessionData.ownedItems[itemId])
                .ToList();
        }

        /// <summary>
        /// 아이템 데이터를 가져옵니다.
        /// </summary>
        public ItemData GetItemData(string itemId)
        {
            return _sessionData.itemSessionData.ownedItems.ContainsKey(itemId) 
                ? _sessionData.itemSessionData.ownedItems[itemId] 
                : null;
        }

        /// <summary>
        /// 아이템을 보유하고 있는지 확인합니다.
        /// </summary>
        public bool HasItem(string itemId)
        {
            return _sessionData.itemSessionData.ownedItems.ContainsKey(itemId);
        }

        /// <summary>
        /// 특정 Pawn에 아이템이 장착되어 있는지 확인합니다.
        /// </summary>
        public bool IsItemEquippedToPawn(string itemId, int playerIndex)
        {
            if (!_sessionData.itemSessionData.equippedItemIds.ContainsKey(playerIndex))
            {
                return false;
            }

            return _sessionData.itemSessionData.equippedItemIds[playerIndex].Contains(itemId);
        }

        /// <summary>
        /// 모든 아이템을 제거합니다.
        /// </summary>
        public void ClearAllItems()
        {
            _sessionData.itemSessionData.ownedItems.Clear();
            _sessionData.itemSessionData.equippedItemIds.Clear();
            _sessionData.itemSessionData.itemStacks.Clear();
        }

        /// <summary>
        /// 아이템 스택을 추가합니다. (같은 아이템을 여러 번 구매할 때)
        /// </summary>
        public void AddItemStack(string itemId, int amount = 1)
        {
            if (string.IsNullOrEmpty(itemId) || amount <= 0)
            {
                return;
            }

            if (!_sessionData.itemSessionData.itemStacks.ContainsKey(itemId))
            {
                _sessionData.itemSessionData.itemStacks[itemId] = 0;
            }

            _sessionData.itemSessionData.itemStacks[itemId] += amount;
        }

        /// <summary>
        /// 아이템 스택 개수를 가져옵니다.
        /// </summary>
        public int GetItemStackCount(string itemId)
        {
            return _sessionData.itemSessionData.itemStacks.ContainsKey(itemId) 
                ? _sessionData.itemSessionData.itemStacks[itemId] 
                : 0;
        }

        /// <summary>
        /// 아이템 스택을 제거합니다.
        /// </summary>
        public bool RemoveItemStack(string itemId, int amount = 1)
        {
            if (string.IsNullOrEmpty(itemId) || amount <= 0)
            {
                return false;
            }

            if (!_sessionData.itemSessionData.itemStacks.ContainsKey(itemId))
            {
                return false;
            }

            int currentStack = _sessionData.itemSessionData.itemStacks[itemId];
            if (currentStack <= amount)
            {
                // 스택이 모두 제거되면 아이템도 제거
                _sessionData.itemSessionData.itemStacks.Remove(itemId);
                _sessionData.itemSessionData.ownedItems.Remove(itemId);
                
                // 모든 Pawn에서 장착 해제 (List에서 모든 항목 제거)
                foreach (var equippedList in _sessionData.itemSessionData.equippedItemIds.Values)
                {
                    equippedList.RemoveAll(id => id == itemId);
                }
                
                return true;
            }
            else
            {
                _sessionData.itemSessionData.itemStacks[itemId] -= amount;
                return true;
            }
        }
    }
}


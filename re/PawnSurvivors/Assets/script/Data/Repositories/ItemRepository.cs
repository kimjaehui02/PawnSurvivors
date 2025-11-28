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
            if (!_sessionData.ownedItems.ContainsKey(itemData.itemId))
            {
                // 깊은 복사 (JsonUtility 사용)
                string json = JsonUtility.ToJson(itemData);
                ItemData copiedData = JsonUtility.FromJson<ItemData>(json);

                _sessionData.ownedItems[itemData.itemId] = copiedData;
                _sessionData.itemStacks[itemData.itemId] = 1;

                // 전역 아이템인 경우 globalItemIds에 추가
                if (itemData.itemType == PawnSurvivors.Domain.ItemType.Global)
                {
                    _sessionData.globalItemIds.Add(itemData.itemId);
                }
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
            if (!_sessionData.ownedItems.ContainsKey(itemId))
            {
                return; // 아이템을 보유하지 않음
            }

            var itemData = _sessionData.ownedItems[itemId];
            if (itemData.itemType != PawnSurvivors.Domain.ItemType.Equipped)
            {
                return; // 장착 가능한 아이템이 아님
            }

            // equippedItemIds에 추가
            if (!_sessionData.equippedItemIds.ContainsKey(playerIndex))
            {
                _sessionData.equippedItemIds[playerIndex] = new HashSet<string>();
            }

            _sessionData.equippedItemIds[playerIndex].Add(itemId);
        }

        /// <summary>
        /// 특정 Pawn에서 아이템을 해제합니다.
        /// </summary>
        public void UnequipItemFromPawn(string itemId, int playerIndex)
        {
            if (!_sessionData.equippedItemIds.ContainsKey(playerIndex))
            {
                return;
            }

            _sessionData.equippedItemIds[playerIndex].Remove(itemId);
        }

        /// <summary>
        /// 전역 아이템 목록을 가져옵니다.
        /// </summary>
        public List<ItemData> GetGlobalItems()
        {
            return _sessionData.globalItemIds
                .Where(itemId => _sessionData.ownedItems.ContainsKey(itemId))
                .Select(itemId => _sessionData.ownedItems[itemId])
                .ToList();
        }

        /// <summary>
        /// 특정 Pawn에 장착된 아이템 목록을 가져옵니다.
        /// </summary>
        public List<ItemData> GetEquippedItems(int playerIndex)
        {
            if (!_sessionData.equippedItemIds.ContainsKey(playerIndex))
            {
                return new List<ItemData>();
            }

            return _sessionData.equippedItemIds[playerIndex]
                .Where(itemId => _sessionData.ownedItems.ContainsKey(itemId))
                .Select(itemId => _sessionData.ownedItems[itemId])
                .ToList();
        }

        /// <summary>
        /// 아이템 데이터를 가져옵니다.
        /// </summary>
        public ItemData GetItemData(string itemId)
        {
            return _sessionData.ownedItems.ContainsKey(itemId) 
                ? _sessionData.ownedItems[itemId] 
                : null;
        }

        /// <summary>
        /// 아이템을 보유하고 있는지 확인합니다.
        /// </summary>
        public bool HasItem(string itemId)
        {
            return _sessionData.ownedItems.ContainsKey(itemId);
        }

        /// <summary>
        /// 특정 Pawn에 아이템이 장착되어 있는지 확인합니다.
        /// </summary>
        public bool IsItemEquippedToPawn(string itemId, int playerIndex)
        {
            if (!_sessionData.equippedItemIds.ContainsKey(playerIndex))
            {
                return false;
            }

            return _sessionData.equippedItemIds[playerIndex].Contains(itemId);
        }

        /// <summary>
        /// 모든 아이템을 제거합니다.
        /// </summary>
        public void ClearAllItems()
        {
            _sessionData.ownedItems.Clear();
            _sessionData.globalItemIds.Clear();
            _sessionData.equippedItemIds.Clear();
            _sessionData.itemStacks.Clear();
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

            if (!_sessionData.itemStacks.ContainsKey(itemId))
            {
                _sessionData.itemStacks[itemId] = 0;
            }

            _sessionData.itemStacks[itemId] += amount;
        }

        /// <summary>
        /// 아이템 스택 개수를 가져옵니다.
        /// </summary>
        public int GetItemStackCount(string itemId)
        {
            return _sessionData.itemStacks.ContainsKey(itemId) 
                ? _sessionData.itemStacks[itemId] 
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

            if (!_sessionData.itemStacks.ContainsKey(itemId))
            {
                return false;
            }

            int currentStack = _sessionData.itemStacks[itemId];
            if (currentStack <= amount)
            {
                // 스택이 모두 제거되면 아이템도 제거
                _sessionData.itemStacks.Remove(itemId);
                _sessionData.ownedItems.Remove(itemId);
                _sessionData.globalItemIds.Remove(itemId);
                
                // 모든 Pawn에서 장착 해제
                foreach (var equippedSet in _sessionData.equippedItemIds.Values)
                {
                    equippedSet.Remove(itemId);
                }
                
                return true;
            }
            else
            {
                _sessionData.itemStacks[itemId] -= amount;
                return true;
            }
        }
    }
}


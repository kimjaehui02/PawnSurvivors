using System.Collections.Generic;
using PawnSurvivors.Data;

namespace PawnSurvivors.Domain.Repositories
{
    /// <summary>
    /// 아이템 데이터 접근을 위한 Repository 인터페이스입니다.
    /// Domain 계층에 위치하며, Data 계층의 구현체에 의존하지 않습니다.
    /// </summary>
    public interface IItemRepository
    {
        /// <summary>
        /// 아이템을 저장합니다.
        /// </summary>
        void SaveItem(ItemData itemData);
        
        /// <summary>
        /// 특정 Pawn에 아이템을 장착합니다.
        /// </summary>
        void EquipItemToPawn(string itemId, int playerIndex);
        
        /// <summary>
        /// 특정 Pawn에서 아이템을 해제합니다.
        /// </summary>
        void UnequipItemFromPawn(string itemId, int playerIndex);
        
        /// <summary>
        /// 전역 아이템 목록을 가져옵니다.
        /// </summary>
        List<ItemData> GetGlobalItems();
        
        /// <summary>
        /// 특정 Pawn에 장착된 아이템 목록을 가져옵니다.
        /// </summary>
        List<ItemData> GetEquippedItems(int playerIndex);
        
        /// <summary>
        /// 아이템 데이터를 가져옵니다.
        /// </summary>
        ItemData GetItemData(string itemId);
        
        /// <summary>
        /// 아이템을 보유하고 있는지 확인합니다.
        /// </summary>
        bool HasItem(string itemId);
        
        /// <summary>
        /// 특정 Pawn에 아이템이 장착되어 있는지 확인합니다.
        /// </summary>
        bool IsItemEquippedToPawn(string itemId, int playerIndex);
        
        /// <summary>
        /// 모든 아이템을 제거합니다.
        /// </summary>
        void ClearAllItems();
        
        /// <summary>
        /// 아이템 스택을 추가합니다. (같은 아이템을 여러 번 구매할 때)
        /// </summary>
        void AddItemStack(string itemId, int amount = 1);
        
        /// <summary>
        /// 아이템 스택 개수를 가져옵니다.
        /// </summary>
        int GetItemStackCount(string itemId);
        
        /// <summary>
        /// 아이템 스택을 제거합니다.
        /// </summary>
        bool RemoveItemStack(string itemId, int amount = 1);
    }
}


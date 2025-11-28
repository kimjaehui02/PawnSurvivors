using System.Collections.Generic;
using PawnSurvivors.Data;
using PawnSurvivors.Domain;

namespace PawnSurvivors.Domain.Repositories
{
    /// <summary>
    /// 아이템 풀 데이터 접근을 위한 Repository 인터페이스입니다.
    /// Domain 계층에 위치하며, Data 계층의 구현체에 의존하지 않습니다.
    /// </summary>
    public interface IItemPoolRepository
    {
        /// <summary>
        /// 아이템 ID로 아이템 데이터를 가져옵니다.
        /// </summary>
        ItemData GetItem(string itemId);

        /// <summary>
        /// 모든 아이템 목록을 가져옵니다.
        /// </summary>
        List<ItemData> GetAllItems();

        /// <summary>
        /// 특정 타입의 아이템 목록을 가져옵니다.
        /// </summary>
        List<ItemData> GetItemsByType(ItemType itemType);
    }
}


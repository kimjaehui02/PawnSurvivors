using System;
using System.Collections.Generic;
using PawnSurvivors.Data;

namespace PawnSurvivors.Data
{
    /// <summary>
    /// 게임 세션의 아이템 데이터를 관리하는 클래스입니다.
    /// PawnPersistentData와 유사한 구조로 아이템 관련 데이터를 구조화합니다.
    /// </summary>
    [Serializable]
    public class ItemSessionData
    {
        /// <summary>
        /// 보유한 아이템 데이터입니다.
        /// 키: itemId
        /// 값: ItemData 전체
        /// </summary>
        public Dictionary<string, ItemData> ownedItems = new Dictionary<string, ItemData>();
        
        /// <summary>
        /// 아이템 스택 개수입니다.
        /// 키: itemId
        /// 값: 해당 아이템의 보유 개수 (스택)
        /// </summary>
        public Dictionary<string, int> itemStacks = new Dictionary<string, int>();
        
        /// <summary>
        /// 장착 아이템 목록입니다. (개별 Pawn에 장착)
        /// 첫 번째 키: playerIndex
        /// 값: 해당 Pawn에 장착된 아이템 ID 목록 (List로 변경하여 같은 아이템 중복 장착 가능)
        /// </summary>
        public Dictionary<int, List<string>> equippedItemIds = new Dictionary<int, List<string>>();
    }
}


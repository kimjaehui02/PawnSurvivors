using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using PawnSurvivors.Data;
using PawnSurvivors.Managers;

namespace PawnSurvivors.Data.Loaders
{
    /// <summary>
    /// 아이템 풀 JSON 파일들을 로드하고 관리합니다.
    /// Data 계층의 데이터 소스 역할을 합니다.
    /// </summary>
    public class ItemPoolLoader
    {
        private Dictionary<string, ItemData> _itemPool = new Dictionary<string, ItemData>();

        /// <summary>
        /// 지정된 디렉토리에서 모든 아이템 JSON 파일을 로드합니다.
        /// </summary>
        /// <param name="itemsPath">Items 폴더 경로</param>
        public void LoadItems(string itemsPath)
        {
            if (!Directory.Exists(itemsPath))
            {
                LogManager.LogWarning(LogCategory.Recipe, $"경로를 찾을 수 없습니다: {itemsPath}");
                return;
            }

            var info = new DirectoryInfo(itemsPath);
            var fileInfo = info.GetFiles("*.json", SearchOption.AllDirectories);

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            foreach (var file in fileInfo)
            {
                try
                {
                    string json = File.ReadAllText(file.FullName);
                    ItemData itemData = JsonConvert.DeserializeObject<ItemData>(json, settings);

                    // itemId가 있는 경우에만 아이템으로 인식 (Pawn 레시피 등 다른 JSON 파일은 건너뛰기)
                    if (itemData != null && !string.IsNullOrEmpty(itemData.itemId))
                    {
                        _itemPool[itemData.itemId] = itemData;
                        LogManager.LogInfo(LogCategory.Recipe, $"아이템 로드: {itemData.itemId} ({itemData.itemName}) from {file.Name}");
                    }
                    // itemId가 없으면 아이템이 아닌 파일이므로 조용히 건너뛰기 (경고 제거)
                }
                catch (System.Exception)
                {
                    // 역직렬화 실패는 아이템이 아닌 파일일 가능성이 높으므로 조용히 건너뛰기
                }
            }

            LogManager.LogInfo(LogCategory.Recipe, $"총 {_itemPool.Count}개의 아이템 로드 완료");
        }

        /// <summary>
        /// 아이템 ID로 아이템 데이터를 가져옵니다.
        /// </summary>
        public ItemData GetItem(string itemId)
        {
            _itemPool.TryGetValue(itemId, out ItemData item);
            return item;
        }

        /// <summary>
        /// 모든 아이템 목록을 가져옵니다.
        /// </summary>
        public List<ItemData> GetAllItems()
        {
            return new List<ItemData>(_itemPool.Values);
        }

        /// <summary>
        /// 특정 타입의 아이템 목록을 가져옵니다.
        /// </summary>
        public List<ItemData> GetItemsByType(PawnSurvivors.Domain.ItemType itemType)
        {
            var result = new List<ItemData>();
            foreach (var item in _itemPool.Values)
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


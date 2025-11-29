using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using PawnSurvivors.Data;

namespace PawnCore.Recipes.Json
{
    /// <summary>
    /// 아이템 풀 JSON 파일들을 로드하고 관리합니다.
    /// RecipeLoader와 유사한 구조입니다.
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
                Debug.LogWarning($"[ItemPoolLoader] 경로를 찾을 수 없습니다: {itemsPath}");
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

                    if (itemData != null && !string.IsNullOrEmpty(itemData.itemId))
                    {
                        _itemPool[itemData.itemId] = itemData;
                        Debug.Log($"[ItemPoolLoader] 아이템 로드: {itemData.itemId} ({itemData.itemName}) from {file.Name}");
                    }
                    else
                    {
                        Debug.LogWarning($"[ItemPoolLoader] {file.Name} - itemId가 없습니다.");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[ItemPoolLoader] {file.Name} 로드 실패: {ex.Message}");
                }
            }

            Debug.Log($"[ItemPoolLoader] 총 {_itemPool.Count}개의 아이템 로드 완료");
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



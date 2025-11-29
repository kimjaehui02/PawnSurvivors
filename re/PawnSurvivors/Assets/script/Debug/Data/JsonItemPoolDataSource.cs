using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using PawnSurvivors.Data;
using PawnSurvivors.Managers;

namespace PawnSurvivors.Debugging.Data
{
    /// <summary>
    /// JSON 파일을 읽어서 ItemData 모델로 반환하는 데이터 소스입니다.
    /// 디버깅/테스트용으로 사용되며, 나중에 실제 구현 시 참고용입니다.
    /// 
    /// 실제 구현 시에는 Data 계층에 위치하게 됩니다.
    /// </summary>
    public class JsonItemPoolDataSource
    {
        private Dictionary<string, ItemData> _loadedItems = new Dictionary<string, ItemData>();

        /// <summary>
        /// 디버깅용 JSON 파일을 자동으로 로드합니다.
        /// StreamingAssets/Debug/Items 경로에서 로드합니다.
        /// </summary>
        public void LoadDebugItems()
        {
            string debugItemsPath = Path.Combine(Application.streamingAssetsPath, "Debug", "Items");
            LoadItems(debugItemsPath);
        }

        /// <summary>
        /// 지정된 디렉토리에서 모든 아이템 JSON 파일을 로드합니다.
        /// </summary>
        /// <param name="itemsPath">Items 폴더 경로</param>
        public void LoadItems(string itemsPath)
        {
            if (!Directory.Exists(itemsPath))
            {
                LogManager.LogWarning(LogCategory.Debug, $"경로를 찾을 수 없습니다: {itemsPath}");
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
                    
                    // 배열 형태인지 확인
                    if (json.TrimStart().StartsWith("["))
                    {
                        // 배열 형태: 여러 아이템이 하나의 파일에 있음
                        List<ItemData> itemList = JsonConvert.DeserializeObject<List<ItemData>>(json, settings);
                        if (itemList != null)
                        {
                            foreach (var itemData in itemList)
                            {
                                if (itemData != null && !string.IsNullOrEmpty(itemData.itemId))
                                {
                                    _loadedItems[itemData.itemId] = itemData;
                                    LogManager.LogDebug(LogCategory.Debug, $"아이템 로드: {itemData.itemId} ({itemData.itemName}) from {file.Name}");
                                }
                            }
                        }
                    }
                    else
                    {
                        // 단일 객체 형태: 하나의 아이템만 있음
                        ItemData itemData = JsonConvert.DeserializeObject<ItemData>(json, settings);
                        if (itemData != null && !string.IsNullOrEmpty(itemData.itemId))
                        {
                            _loadedItems[itemData.itemId] = itemData;
                            LogManager.LogDebug(LogCategory.Debug, $"아이템 로드: {itemData.itemId} ({itemData.itemName}) from {file.Name}");
                        }
                        else
                        {
                            LogManager.LogWarning(LogCategory.Debug, $"{file.Name} - itemId가 없습니다.");
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    LogManager.LogError(LogCategory.Debug, $"{file.Name} 로드 실패: {ex.Message}");
                }
            }

            LogManager.LogInfo(LogCategory.Debug, $"총 {_loadedItems.Count}개의 아이템 로드 완료");
        }

        /// <summary>
        /// 아이템 ID로 아이템 데이터를 가져옵니다.
        /// </summary>
        /// <param name="itemId">아이템 ID</param>
        /// <returns>ItemData 모델, 없으면 null</returns>
        public ItemData GetItem(string itemId)
        {
            _loadedItems.TryGetValue(itemId, out ItemData item);
            return item;
        }

        /// <summary>
        /// 모든 아이템 목록을 가져옵니다.
        /// </summary>
        /// <returns>ItemData 모델 리스트</returns>
        public List<ItemData> GetAllItems()
        {
            return new List<ItemData>(_loadedItems.Values);
        }

        /// <summary>
        /// 특정 타입의 아이템 목록을 가져옵니다.
        /// </summary>
        /// <param name="itemType">아이템 타입</param>
        /// <returns>ItemData 모델 리스트</returns>
        public List<ItemData> GetItemsByType(PawnSurvivors.Domain.ItemType itemType)
        {
            var result = new List<ItemData>();
            foreach (var item in _loadedItems.Values)
            {
                if (item.itemType == itemType)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 로드된 아이템을 모두 제거합니다.
        /// </summary>
        public void Clear()
        {
            _loadedItems.Clear();
        }
    }
}


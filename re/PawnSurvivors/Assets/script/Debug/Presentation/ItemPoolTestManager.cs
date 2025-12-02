using UnityEngine;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Domain.Repositories;
using PawnSurvivors.Debugging.Data;
using PawnSurvivors.Managers;
using System.IO;

namespace PawnSurvivors.Debugging.Presentation
{
    /// <summary>
    /// ItemPoolUseCase를 독립적으로 테스트/디버깅하기 위한 MonoBehaviour입니다.
    /// 실제 게임 로직(GameManager, StageManager 등)과 완전히 별개로 동작합니다.
    /// 
    /// 사용법:
    /// 1. 빈 GameObject에 이 스크립트를 추가
    /// 2. Play 모드에서 실행
    /// 3. Inspector에서 "Run Tests" 버튼 클릭 또는 자동 실행
    /// </summary>
    public class ItemPoolTestManager : MonoBehaviour
    {
        [Header("테스트 설정")]
        [SerializeField] private bool runTestsOnStart = true;
        [SerializeField] private int randomItemCount = 3;
        [SerializeField] private bool excludeOwnedItems = false;

        [Header("JSON 데이터 소스 설정")]
        [SerializeField] private bool useJsonDataSource = false;
        [SerializeField] private string jsonItemsPath = "";

        private ItemPoolUseCase _itemPoolUseCase;
        private IItemPoolRepository _itemPoolRepository;
        private IItemRepository _itemRepository; // Mock ItemRepository

        private void Start()
        {
            InitializeDomainLayer();
            
            if (runTestsOnStart)
            {
                RunAllTests();
            }
        }

        /// <summary>
        /// Domain 계층만 독립적으로 초기화합니다.
        /// </summary>
        private void InitializeDomainLayer()
        {
            // Mock ItemRepository 생성 (디버깅용)
            _itemRepository = new MockItemRepository();
            
            if (useJsonDataSource)
            {
                // JSON 데이터 소스 사용
                var jsonDataSource = new JsonItemPoolDataSource();
                
                // 경로가 지정되어 있으면 해당 경로 사용, 없으면 디버깅 경로 자동 사용
                if (!string.IsNullOrEmpty(jsonItemsPath))
                {
                    jsonDataSource.LoadItems(jsonItemsPath);
                    LogManager.LogInfo(LogCategory.Debug, $"JSON 데이터 소스로 초기화 완료 (경로: {jsonItemsPath})");
                }
                else
                {
                    jsonDataSource.LoadDebugItems();
                    LogManager.LogInfo(LogCategory.Debug, "디버깅 JSON 데이터 소스로 초기화 완료");
                }
                
                _itemPoolRepository = new JsonItemPoolRepository(jsonDataSource);
            }
            else
            {
                // Mock Repository 사용
                _itemPoolRepository = new MockItemPoolRepository();
                LogManager.LogInfo(LogCategory.Debug, "Mock 데이터로 초기화 완료");
            }
            
            // UseCase 생성 (Domain 계층만)
            _itemPoolUseCase = new ItemPoolUseCase(_itemPoolRepository, _itemRepository);
            
            LogManager.LogInfo(LogCategory.Debug, "Domain 계층 초기화 완료");
        }

        /// <summary>
        /// 모든 테스트를 실행합니다.
        /// </summary>
        [ContextMenu("Run All Tests")]
        public void RunAllTests()
        {
            if (_itemPoolUseCase == null)
            {
                LogManager.LogError(LogCategory.Debug, "UseCase가 초기화되지 않았습니다.");
                return;
            }

            LogManager.LogInfo(LogCategory.Debug, "=== ItemPoolUseCase 테스트 시작 ===");
            
            TestGetAllItems();
            TestGetItem();
            TestGetItemsByType();
            TestGetRandomShopItems();
            TestGetRandomShopItemsByType();
            
            LogManager.LogInfo(LogCategory.Debug, "=== ItemPoolUseCase 테스트 완료 ===");
        }

        /// <summary>
        /// 모든 아이템 가져오기 테스트
        /// </summary>
        [ContextMenu("Test: GetAllItems")]
        private void TestGetAllItems()
        {
            var allItems = _itemPoolUseCase.GetAllItems();
            LogManager.LogDebug(LogCategory.Debug, $"GetAllItems: {allItems.Count}개의 아이템");
            foreach (var item in allItems)
            {
                LogManager.LogDebug(LogCategory.Debug, $"  - {item.itemId}: {item.itemName} ({item.itemType})");
            }
        }

        /// <summary>
        /// 특정 아이템 가져오기 테스트
        /// </summary>
        [ContextMenu("Test: GetItem")]
        private void TestGetItem()
        {
            // Mock 데이터의 첫 번째 아이템 ID 사용
            string testItemId = useJsonDataSource ? "DebugGlobalItem1" : "MockGlobalDamagePlus1";
            var item = _itemPoolUseCase.GetItem(testItemId);
            if (item != null)
            {
                LogManager.LogDebug(LogCategory.Debug, $"GetItem: {item.itemId} - {item.itemName}");
                LogManager.LogDebug(LogCategory.Debug, $"  - 타입: {item.itemType}");
                LogManager.LogDebug(LogCategory.Debug, $"  - 가격: {item.cost}");
                LogManager.LogDebug(LogCategory.Debug, $"  - 설명: {item.description}");
            }
            else
            {
                LogManager.LogWarning(LogCategory.Debug, $"GetItem: 아이템 '{testItemId}'을 찾을 수 없습니다.");
            }
        }

        /// <summary>
        /// 타입별 아이템 가져오기 테스트
        /// </summary>
        [ContextMenu("Test: GetItemsByType")]
        private void TestGetItemsByType()
        {
            var allItems = _itemPoolUseCase.GetAllItems();
            var globalCount = 0;
            var equippedCount = 0;
            
            foreach (var item in allItems)
            {
                if (item.itemType == PawnSurvivors.Domain.ItemType.Global) globalCount++;
                if (item.itemType == PawnSurvivors.Domain.ItemType.Equipped) equippedCount++;
            }
            
            LogManager.LogDebug(LogCategory.Debug, $"GetItemsByType:");
            LogManager.LogDebug(LogCategory.Debug, $"  - Global: {globalCount}개");
            LogManager.LogDebug(LogCategory.Debug, $"  - Equipped: {equippedCount}개");
        }

        /// <summary>
        /// 랜덤 상점 아이템 선택 테스트
        /// </summary>
        [ContextMenu("Test: GetRandomShopItems")]
        private void TestGetRandomShopItems()
        {
            var randomItems = _itemPoolUseCase.GetRandomShopItems(
                randomItemCount,
                excludeOwnedItems,
                null // Mock ItemRepository는 사용하지 않음
            );
            
            LogManager.LogDebug(LogCategory.Debug, $"GetRandomShopItems ({randomItemCount}개):");
            foreach (var item in randomItems)
            {
                LogManager.LogDebug(LogCategory.Debug, $"  - {item.itemId}: {item.itemName}");
            }
        }

        /// <summary>
        /// 타입별 랜덤 상점 아이템 선택 테스트
        /// </summary>
        [ContextMenu("Test: GetRandomShopItemsByType")]
        private void TestGetRandomShopItemsByType()
        {
            var globalItems = _itemPoolUseCase.GetRandomShopItemsByType(
                PawnSurvivors.Domain.ItemType.Global,
                2,
                excludeOwnedItems,
                null
            );
            
            LogManager.LogDebug(LogCategory.Debug, $"GetRandomShopItemsByType (Global, 2개):");
            foreach (var item in globalItems)
            {
                LogManager.LogDebug(LogCategory.Debug, $"  - {item.itemId}: {item.itemName}");
            }
        }

        /// <summary>
        /// 디버깅 JSON 데이터 소스로 재초기화합니다.
        /// </summary>
        [ContextMenu("Reinitialize with Debug JSON")]
        public void ReinitializeWithDebugJson()
        {
            jsonItemsPath = ""; // 빈 경로로 설정하면 자동으로 디버깅 경로 사용
            useJsonDataSource = true;
            InitializeDomainLayer();
            LogManager.LogInfo(LogCategory.Debug, "디버깅 JSON 데이터 소스로 재초기화 완료");
        }

        /// <summary>
        /// JSON 데이터 소스로 재초기화합니다. (커스텀 경로)
        /// </summary>
        [ContextMenu("Reinitialize with Custom JSON")]
        public void ReinitializeWithCustomJson()
        {
            if (string.IsNullOrEmpty(jsonItemsPath))
            {
                jsonItemsPath = "StreamingAssets/Debug/Items";
            }

            useJsonDataSource = true;
            InitializeDomainLayer();
            LogManager.LogInfo(LogCategory.Debug, $"커스텀 JSON 데이터 소스로 재초기화 완료 (경로: {jsonItemsPath})");
        }

        /// <summary>
        /// Mock 데이터로 재초기화합니다.
        /// </summary>
        [ContextMenu("Reinitialize with Mock")]
        public void ReinitializeWithMock()
        {
            useJsonDataSource = false;
            InitializeDomainLayer();
            LogManager.LogInfo(LogCategory.Debug, "Mock 데이터로 재초기화 완료");
        }
    }
}


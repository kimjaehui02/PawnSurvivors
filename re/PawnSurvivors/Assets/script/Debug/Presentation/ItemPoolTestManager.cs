using UnityEngine;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Domain.Repositories;
using PawnSurvivors.Debugging.Data;
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
        private IItemPoolRepository _repository;

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
            if (useJsonDataSource)
            {
                // JSON 데이터 소스 사용
                var jsonDataSource = new JsonItemPoolDataSource();
                
                // 경로가 지정되어 있으면 해당 경로 사용, 없으면 디버깅 경로 자동 사용
                if (!string.IsNullOrEmpty(jsonItemsPath))
                {
                    jsonDataSource.LoadItems(jsonItemsPath);
                    Debug.Log($"[ItemPoolTestManager] JSON 데이터 소스로 초기화 완료 (경로: {jsonItemsPath})");
                }
                else
                {
                    jsonDataSource.LoadDebugItems();
                    Debug.Log("[ItemPoolTestManager] 디버깅 JSON 데이터 소스로 초기화 완료");
                }
                
                _repository = new JsonItemPoolRepository(jsonDataSource);
            }
            else
            {
                // Mock Repository 사용
                _repository = new MockItemPoolRepository();
                Debug.Log("[ItemPoolTestManager] Mock 데이터로 초기화 완료");
            }
            
            // UseCase 생성 (Domain 계층만)
            _itemPoolUseCase = new ItemPoolUseCase(_repository);
            
            Debug.Log("[ItemPoolTestManager] Domain 계층 초기화 완료");
        }

        /// <summary>
        /// 모든 테스트를 실행합니다.
        /// </summary>
        [ContextMenu("Run All Tests")]
        public void RunAllTests()
        {
            if (_itemPoolUseCase == null)
            {
                Debug.LogError("[ItemPoolTestManager] UseCase가 초기화되지 않았습니다.");
                return;
            }

            Debug.Log("=== ItemPoolUseCase 테스트 시작 ===");
            
            TestGetAllItems();
            TestGetItem();
            TestGetItemsByType();
            TestGetRandomShopItems();
            TestGetRandomShopItemsByType();
            
            Debug.Log("=== ItemPoolUseCase 테스트 완료 ===");
        }

        /// <summary>
        /// 모든 아이템 가져오기 테스트
        /// </summary>
        [ContextMenu("Test: GetAllItems")]
        private void TestGetAllItems()
        {
            var allItems = _itemPoolUseCase.GetAllItems();
            Debug.Log($"[테스트] GetAllItems: {allItems.Count}개의 아이템");
            foreach (var item in allItems)
            {
                Debug.Log($"  - {item.itemId}: {item.itemName} ({item.itemType})");
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
                Debug.Log($"[테스트] GetItem: {item.itemId} - {item.itemName}");
                Debug.Log($"  - 타입: {item.itemType}");
                Debug.Log($"  - 가격: {item.cost}");
                Debug.Log($"  - 설명: {item.description}");
            }
            else
            {
                Debug.LogWarning($"[테스트] GetItem: 아이템 '{testItemId}'을 찾을 수 없습니다.");
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
            
            Debug.Log($"[테스트] GetItemsByType:");
            Debug.Log($"  - Global: {globalCount}개");
            Debug.Log($"  - Equipped: {equippedCount}개");
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
            
            Debug.Log($"[테스트] GetRandomShopItems ({randomItemCount}개):");
            foreach (var item in randomItems)
            {
                Debug.Log($"  - {item.itemId}: {item.itemName}");
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
            
            Debug.Log($"[테스트] GetRandomShopItemsByType (Global, 2개):");
            foreach (var item in globalItems)
            {
                Debug.Log($"  - {item.itemId}: {item.itemName}");
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
            Debug.Log("[ItemPoolTestManager] 디버깅 JSON 데이터 소스로 재초기화 완료");
        }

        /// <summary>
        /// JSON 데이터 소스로 재초기화합니다. (커스텀 경로)
        /// </summary>
        [ContextMenu("Reinitialize with Custom JSON")]
        public void ReinitializeWithCustomJson()
        {
            if (string.IsNullOrEmpty(jsonItemsPath))
            {
                jsonItemsPath = Path.Combine(Application.streamingAssetsPath, "Debug", "Items");
            }

            useJsonDataSource = true;
            InitializeDomainLayer();
            Debug.Log($"[ItemPoolTestManager] 커스텀 JSON 데이터 소스로 재초기화 완료 (경로: {jsonItemsPath})");
        }

        /// <summary>
        /// Mock 데이터로 재초기화합니다.
        /// </summary>
        [ContextMenu("Reinitialize with Mock")]
        public void ReinitializeWithMock()
        {
            useJsonDataSource = false;
            InitializeDomainLayer();
            Debug.Log("[ItemPoolTestManager] Mock 데이터로 재초기화 완료");
        }
    }
}


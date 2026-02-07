using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using PawnSurvivors.Managers;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Domain.States;
using PawnSurvivors.UI.Factory;
using PawnSurvivors.Data;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 상점 화면입니다. Builder를 사용하여 UI를 생성합니다.
    /// </summary>
    public class ShopScreen : MonoBehaviour
    {
        #region Constants
        private const int SLOT_COUNT = 4;
        private const int BASE_RESET_COST = 1;
        #endregion

        #region State
        private int _resetCost;
        private List<ShopSlotData> _currentSlots = new();
        private bool[] _lockedSlots;
        private bool[] _purchasedSlots;
        #endregion

        #region References
        private Canvas _canvas;
        private ShopUseCase _shopUseCase;
        private CurrencyUseCase _currencyUseCase;
        private ItemManagementUseCase _itemManagementUseCase;
        #endregion

        #region UI Components
        private ShopUIBuilder.ShopLayout _layout;
        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            SetupCanvas();
            _lockedSlots = new bool[SLOT_COUNT];
            _purchasedSlots = new bool[SLOT_COUNT];
        }

        private void Start()
        {
            InitializeUseCases();
            CreateUI();
            RefreshShop();
        }

        private void Update()
        {
            HandleInput();
        }

        #endregion

        #region Initialization

        private void SetupCanvas()
        {
            _canvas = UIFactory.SetupCanvas(gameObject);
        }

        private void InitializeUseCases()
        {
            if (GameManager.Instance == null) return;

            _shopUseCase = GameManager.Instance.ShopUseCase;
            _currencyUseCase = GameManager.Instance.CurrencyUseCase;
            _itemManagementUseCase = GameManager.Instance.ItemManagementUseCase;
            _resetCost = BASE_RESET_COST;
        }

        private void CreateUI()
        {
            _layout = ShopUIBuilder.CreateShopLayout(
                transform,
                SLOT_COUNT,
                onBuyClicked: OnItemBuyClicked,
                onLockClicked: OnItemLockClicked,
                onResetClicked: OnResetClicked,
                onNextWaveClicked: OnNextWaveClicked
            );

            UpdateTitle();
            UpdateGoldDisplay();
            UpdateNextWaveButton();
        }

        #endregion

        #region Shop Logic

        private void RefreshShop()
        {
            // 잠긴 슬롯의 아이템 ID 수집
            var lockedItemIds = new List<string>();
            for (int i = 0; i < SLOT_COUNT && i < _currentSlots.Count; i++)
            {
                if (_lockedSlots[i] && _currentSlots[i]?.itemData != null)
                {
                    lockedItemIds.Add(_currentSlots[i].itemData.itemId);
                }
            }

            // 필요한 슬롯 수 계산
            int neededCount = 0;
            for (int i = 0; i < SLOT_COUNT; i++)
            {
                if (!_lockedSlots[i] || i >= _currentSlots.Count)
                {
                    neededCount++;
                }
            }

            // 새로운 아이템 가져오기
            var newSlots = _shopUseCase?.GetRandomShopSlots(neededCount, excludeItemIds: lockedItemIds)
                           ?? new List<ShopSlotData>();

            // 슬롯 업데이트
            var updatedSlots = new List<ShopSlotData>();
            int newSlotIndex = 0;
            for (int i = 0; i < SLOT_COUNT; i++)
            {
                if (_lockedSlots[i] && i < _currentSlots.Count)
                {
                    // 잠긴 슬롯은 유지
                    updatedSlots.Add(_currentSlots[i]);
                }
                else if (newSlotIndex < newSlots.Count)
                {
                    updatedSlots.Add(newSlots[newSlotIndex++]);
                    _purchasedSlots[i] = false;
                }
                else
                {
                    // 빈 슬롯
                    updatedSlots.Add(null);
                    _purchasedSlots[i] = false;
                }
            }

            _currentSlots = updatedSlots;
            UpdateAllSlots();
            UpdateGoldDisplay();
        }

        private void UpdateAllSlots()
        {
            for (int i = 0; i < SLOT_COUNT && i < _currentSlots.Count; i++)
            {
                var slot = _layout.SlotContainer.GetSlot(i);
                var slotData = _currentSlots[i];

                if (slot == null) continue;

                if (slotData == null)
                {
                    slot.SetData("비어있음", "", "아이템이 없습니다", 0, null);
                    slot.SetCanAfford(false);
                    continue;
                }

                // 아이템 또는 캐릭터에 따라 정보 표시
                string name, typeText, description;
                int cost;
                Sprite icon = null;

                if (slotData.slotType == ShopSlotType.Item && slotData.itemData != null)
                {
                    var item = slotData.itemData;
                    name = item.itemName;
                    typeText = GetItemTypeText(item.itemType.ToString());
                    description = item.description;
                    cost = item.cost;
                    if (!string.IsNullOrEmpty(item.iconPath))
                    {
                        icon = Resources.Load<Sprite>(item.iconPath);
                    }
                }
                else if (slotData.slotType == ShopSlotType.Character && slotData.characterRecipe != null)
                {
                    name = slotData.characterRecipeName ?? "캐릭터";
                    typeText = "캐릭터";
                    description = slotData.characterRecipe.description ?? "";
                    cost = 50; // 캐릭터 기본 가격
                    // icon = slotData.characterRecipe.icon; // 필요시 설정
                }
                else
                {
                    slot.SetData("비어있음", "", "아이템이 없습니다", 0, null);
                    slot.SetCanAfford(false);
                    continue;
                }

                slot.SetData(name, typeText, description, cost, icon);
                slot.SetLocked(_lockedSlots[i]);
                slot.SetPurchased(_purchasedSlots[i]);

                int gold = _currencyUseCase?.GetGold() ?? 0;
                slot.SetCanAfford(gold >= cost);
            }
        }

        private string GetItemTypeText(string type)
        {
            return type switch
            {
                "Global" => "전역",
                "Equipped" => "장비",
                "Weapon" => "무기",
                "Armor" => "방어구",
                "Accessory" => "악세서리",
                "Consumable" => "소모품",
                _ => type
            };
        }

        #endregion

        #region Event Handlers

        private void OnItemBuyClicked(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _currentSlots.Count) return;
            if (_purchasedSlots[slotIndex]) return;

            var slotData = _currentSlots[slotIndex];
            if (slotData == null) return;

            int cost = GetSlotCost(slotData);
            int gold = _currencyUseCase?.GetGold() ?? 0;

            if (gold < cost)
            {
                LogManager.LogWarning(LogCategory.UI, "골드가 부족합니다.");
                return;
            }

            // 구매 처리
            bool success = false;
            if (slotData.slotType == ShopSlotType.Item && slotData.itemData != null)
            {
                success = _itemManagementUseCase?.BuyItem(slotData.itemData) ?? false;
            }
            else if (slotData.slotType == ShopSlotType.Character)
            {
                // 캐릭터 구매 로직 (나중에 구현)
                _currencyUseCase?.SpendGold(cost);
                success = true;
                LogManager.LogInfo(LogCategory.UI, $"캐릭터 구매: {slotData.characterRecipeName}");
            }

            if (success)
            {
                _purchasedSlots[slotIndex] = true;

                // UI 업데이트
                _layout.SlotContainer.GetSlot(slotIndex)?.SetPurchased(true);
                UpdateGoldDisplay();
                UpdateAllSlots();

                string itemName = slotData.slotType == ShopSlotType.Item
                    ? slotData.itemData?.itemName
                    : slotData.characterRecipeName;
                LogManager.LogInfo(LogCategory.UI, $"구매 완료: {itemName}");
            }
        }

        private int GetSlotCost(ShopSlotData slotData)
        {
            if (slotData.slotType == ShopSlotType.Item && slotData.itemData != null)
            {
                return slotData.itemData.cost;
            }
            else if (slotData.slotType == ShopSlotType.Character)
            {
                return 50; // 캐릭터 기본 가격
            }
            return 0;
        }

        private void OnItemLockClicked(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SLOT_COUNT) return;

            _lockedSlots[slotIndex] = !_lockedSlots[slotIndex];
            _layout.SlotContainer.GetSlot(slotIndex)?.SetLocked(_lockedSlots[slotIndex]);
        }

        private void OnResetClicked()
        {
            int gold = _currencyUseCase?.GetGold() ?? 0;

            if (gold < _resetCost)
            {
                LogManager.LogWarning(LogCategory.UI, "골드가 부족합니다.");
                return;
            }

            // 비용 지불
            _currencyUseCase?.SpendGold(_resetCost);
            _resetCost++;

            // 상점 새로고침
            RefreshShop();

            // 리셋 버튼 텍스트 업데이트
            _layout.TopBar.SetResetCost(_resetCost);

            LogManager.LogInfo(LogCategory.UI, "상점 초기화");
        }

        private void OnNextWaveClicked()
        {
            if (GameFlowController.Instance != null)
            {
                if (!GameFlowController.Instance.HasNextStage())
                {
                    // 모든 스테이지 완료
                    LogManager.LogInfo(LogCategory.Stage, "모든 스테이지를 완료했습니다!");
                    GameStateManager.Instance?.GoToGameOver();
                    return;
                }

                // 다음 스테이지 준비 및 이동
                string nextStageName = GameFlowController.Instance.GetNextStageName();
                GameManager.Instance?.StageManagementUseCase?.PrepareStageStart(nextStageName, shouldResetSession: false);
                GameStateManager.Instance?.GoToStage();
            }
            else
            {
                // Fallback
                string nextStageName = GameManager.Instance?.StageManagementUseCase?.GetNextStageName();
                if (string.IsNullOrEmpty(nextStageName))
                {
                    GameStateManager.Instance?.GoToGameOver();
                    return;
                }

                GameManager.Instance?.StageManagementUseCase?.PrepareStageStart(nextStageName, shouldResetSession: false);
                GameStateManager.Instance?.GoToStage();
            }
        }

        #endregion

        #region Input

        private void HandleInput()
        {
            // F키 - 상점 초기화
            if (Input.GetKeyDown(KeyCode.F))
            {
                OnResetClicked();
            }

            // E키 - 슬롯 잠금 (1~4)
            for (int i = 0; i < SLOT_COUNT; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i) && Input.GetKey(KeyCode.E))
                {
                    OnItemLockClicked(i);
                }
            }

            // Space 또는 Enter - 다음 웨이브
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                OnNextWaveClicked();
            }
        }

        #endregion

        #region UI Updates

        private void UpdateTitle()
        {
            if (_layout?.TopBar == null || GameManager.Instance?.StageManagementUseCase == null) return;

            int current = GameManager.Instance.StageManagementUseCase.GetCurrentRound();
            int total = GameManager.Instance.StageManagementUseCase.GetTotalRounds();

            _layout.TopBar.SetTitle($"상점 (라운드 {current}/{total})");
        }

        private void UpdateGoldDisplay()
        {
            int gold = _currencyUseCase?.GetGold() ?? 0;
            _layout?.TopBar?.SetGold(gold);
        }

        private void UpdateNextWaveButton()
        {
            if (_layout?.BottomBar == null || GameManager.Instance?.StageManagementUseCase == null) return;

            int current = GameManager.Instance.StageManagementUseCase.GetCurrentRound();
            int total = GameManager.Instance.StageManagementUseCase.GetTotalRounds();

            bool hasNext = GameFlowController.Instance?.HasNextStage() ??
                           !string.IsNullOrEmpty(GameManager.Instance.StageManagementUseCase.GetNextStageName());

            if (hasNext)
            {
                _layout.BottomBar.SetNextWaveText($"다음 라운드 ({current + 1}/{total})");
            }
            else
            {
                _layout.BottomBar.SetNextWaveText("게임 완료");
            }
        }

        #endregion
    }
}

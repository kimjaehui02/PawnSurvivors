using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

namespace PawnSurvivors.UI.Factory
{
    /// <summary>
    /// 상점 화면의 UI를 빌드합니다.
    /// </summary>
    public static class ShopUIBuilder
    {
        #region Item Slot

        /// <summary>
        /// 상점 아이템 슬롯을 생성합니다.
        /// </summary>
        public static ItemSlot CreateItemSlot(
            Transform parent,
            int index,
            Action<int> onBuyClicked,
            Action<int> onLockClicked)
        {
            var slot = new ItemSlot { Index = index };

            // 슬롯 루트
            slot.Root = UIFactory.CreatePanel($"ItemSlot_{index}", parent, CommonUIBuilder.BgSlot);

            // 아이콘 (상단 중앙)
            slot.IconImage = UIFactory.CreateImage("Icon", slot.Root, Color.white);
            var iconRect = slot.IconImage.GetComponent<RectTransform>();
            UIFactory.SetRect(iconRect,
                new Vector2(0.5f, 1), new Vector2(0.5f, 1),
                new Vector2(0.5f, 1),
                new Vector2(0, -40),
                new Vector2(120, 120));
            slot.IconImage.raycastTarget = false;

            // 이름 텍스트
            slot.NameText = UIFactory.CreateText("NameText", slot.Root, "아이템 이름", 32, CommonUIBuilder.TextWhite, TextAlignmentOptions.Left);
            var nameRect = slot.NameText.GetComponent<RectTransform>();
            UIFactory.SetRect(nameRect,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(20, -180),
                new Vector2(-40, 50));

            // 타입 텍스트
            slot.TypeText = UIFactory.CreateText("TypeText", slot.Root, "타입", 24, CommonUIBuilder.TextGray, TextAlignmentOptions.Left);
            var typeRect = slot.TypeText.GetComponent<RectTransform>();
            UIFactory.SetRect(typeRect,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(20, -230),
                new Vector2(-40, 35));

            // 설명 텍스트
            slot.DescriptionText = UIFactory.CreateText("DescriptionText", slot.Root, "설명", 22, CommonUIBuilder.TextWhite, TextAlignmentOptions.TopLeft);
            var descRect = slot.DescriptionText.GetComponent<RectTransform>();
            UIFactory.SetRect(descRect,
                new Vector2(0, 0.3f), new Vector2(1, 0.65f),
                new Vector2(0, 1),
                new Vector2(20, 0),
                new Vector2(-40, 0));
            slot.DescriptionText.enableWordWrapping = true;

            // 가격 텍스트 (좌측 하단)
            slot.CostText = UIFactory.CreateText("CostText", slot.Root, "0", 32, CommonUIBuilder.TextGreen, TextAlignmentOptions.Left);
            var costRect = slot.CostText.GetComponent<RectTransform>();
            UIFactory.SetRect(costRect,
                new Vector2(0, 0), new Vector2(0, 0),
                new Vector2(0, 0),
                new Vector2(20, 20),
                new Vector2(100, 50));

            // 구매 버튼 (슬롯 전체)
            slot.BuyButton = slot.Root.gameObject.AddComponent<Button>();
            var slotImage = slot.Root.GetComponent<Image>();
            if (slotImage == null)
            {
                slotImage = slot.Root.gameObject.AddComponent<Image>();
                slotImage.color = CommonUIBuilder.BgSlot;
            }
            slot.BuyButton.targetGraphic = slotImage;
            slot.BuyButton.onClick.AddListener(() => onBuyClicked?.Invoke(index));

            // 잠금 버튼 (우측 하단)
            slot.LockButton = UIFactory.CreateButton("LockButton", slot.Root, "E 잠금", 24, CommonUIBuilder.BgButton);
            var lockRect = slot.LockButton.GetComponent<RectTransform>();
            UIFactory.SetRect(lockRect,
                new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(1, 0),
                new Vector2(-20, 20),
                new Vector2(150, 50));
            slot.LockButton.onClick.AddListener(() => onLockClicked?.Invoke(index));
            slot.LockButtonText = UIFactory.GetButtonText(slot.LockButton);

            return slot;
        }

        public class ItemSlot
        {
            public int Index;
            public RectTransform Root;
            public Image IconImage;
            public TMP_Text NameText;
            public TMP_Text TypeText;
            public TMP_Text DescriptionText;
            public TMP_Text CostText;
            public Button BuyButton;
            public Button LockButton;
            public TMP_Text LockButtonText;

            public bool IsLocked { get; private set; }
            public bool IsPurchased { get; private set; }

            public void SetLocked(bool locked)
            {
                IsLocked = locked;
                if (LockButtonText != null)
                {
                    LockButtonText.text = locked ? "E 잠금 해제" : "E 잠금";
                }
            }

            public void SetPurchased(bool purchased)
            {
                IsPurchased = purchased;
                if (BuyButton != null)
                {
                    BuyButton.interactable = !purchased;
                }
                if (CostText != null)
                {
                    CostText.color = purchased ? Color.gray : CommonUIBuilder.TextGreen;
                }
            }

            public void SetCanAfford(bool canAfford)
            {
                if (IsPurchased) return;
                if (BuyButton != null)
                {
                    BuyButton.interactable = canAfford;
                }
                if (CostText != null)
                {
                    CostText.color = canAfford ? CommonUIBuilder.TextGreen : CommonUIBuilder.TextRed;
                }
            }

            public void SetData(string name, string type, string description, int cost, Sprite icon = null)
            {
                if (NameText != null) NameText.text = name;
                if (TypeText != null) TypeText.text = type;
                if (DescriptionText != null) DescriptionText.text = description;
                if (CostText != null) CostText.text = cost.ToString();
                if (IconImage != null && icon != null) IconImage.sprite = icon;
            }
        }

        #endregion

        #region Item Slot Container

        /// <summary>
        /// 아이템 슬롯 컨테이너를 생성합니다. (4개 슬롯 가로 배치)
        /// </summary>
        public static ItemSlotContainer CreateItemSlotContainer(
            Transform parent,
            int slotCount,
            Action<int> onBuyClicked,
            Action<int> onLockClicked)
        {
            var container = new ItemSlotContainer();
            container.Slots = new List<ItemSlot>();

            // 컨테이너 패널
            container.Root = UIFactory.CreatePanel("ItemSlotContainer", parent);
            UIFactory.SetRect(container.Root,
                new Vector2(0, 0.15f), new Vector2(1, 0.85f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero);

            // 레이아웃
            var layout = container.Root.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 30f;
            layout.padding = new RectOffset(60, 60, 20, 20);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            // 슬롯 생성
            for (int i = 0; i < slotCount; i++)
            {
                var slot = CreateItemSlot(container.Root, i, onBuyClicked, onLockClicked);
                container.Slots.Add(slot);
            }

            return container;
        }

        public class ItemSlotContainer
        {
            public RectTransform Root;
            public List<ItemSlot> Slots;

            public ItemSlot GetSlot(int index)
            {
                if (index >= 0 && index < Slots.Count)
                {
                    return Slots[index];
                }
                return null;
            }
        }

        #endregion

        #region Shop Top Bar

        /// <summary>
        /// 상점 상단 바를 생성합니다.
        /// </summary>
        public static ShopTopBar CreateShopTopBar(
            Transform parent,
            Action onResetClicked)
        {
            var bar = new ShopTopBar();

            // 기본 헤더 바
            bar.Header = CommonUIBuilder.CreateHeaderBar(parent, "상점", 100f);

            // 중앙에 골드 표시
            bar.GoldDisplay = CommonUIBuilder.CreateGoldDisplay(bar.Header.CenterPanel);
            var goldRect = bar.GoldDisplay.Root;
            UIFactory.SetAnchorPreset(goldRect, AnchorPreset.MiddleCenter, new Vector2(200, 60));

            // 우측에 리셋 버튼
            bar.ResetButton = UIFactory.CreateButton("ResetButton", bar.Header.RightPanel, "F 초기화 - 1", 28, CommonUIBuilder.BgButton);
            var resetRect = bar.ResetButton.GetComponent<RectTransform>();
            UIFactory.SetAnchorPreset(resetRect, AnchorPreset.MiddleRight, new Vector2(250, 60));
            bar.ResetButton.onClick.AddListener(() => onResetClicked?.Invoke());
            bar.ResetButtonText = UIFactory.GetButtonText(bar.ResetButton);

            return bar;
        }

        public class ShopTopBar
        {
            public CommonUIBuilder.HeaderBar Header;
            public CommonUIBuilder.GoldDisplay GoldDisplay;
            public Button ResetButton;
            public TMP_Text ResetButtonText;

            public void SetTitle(string title)
            {
                if (Header?.TitleText != null)
                {
                    Header.TitleText.text = title;
                }
            }

            public void SetGold(int gold)
            {
                GoldDisplay?.SetGold(gold);
            }

            public void SetResetCost(int cost)
            {
                if (ResetButtonText != null)
                {
                    ResetButtonText.text = $"F 초기화 - {cost}";
                }
            }
        }

        #endregion

        #region Shop Bottom Bar

        /// <summary>
        /// 상점 하단 바를 생성합니다. (인벤토리 + 다음 웨이브 버튼)
        /// </summary>
        public static ShopBottomBar CreateShopBottomBar(
            Transform parent,
            Action onNextWaveClicked)
        {
            var bar = new ShopBottomBar();

            // 하단 바
            bar.Root = CommonUIBuilder.CreateBottomBar(parent, 180f);

            // 3등분 레이아웃
            var layout = bar.Root.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 20f;
            layout.padding = new RectOffset(40, 40, 20, 20);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            // 아이템 인벤토리 패널
            bar.ItemsPanel = CreateInventoryPanel(bar.Root, "아이템");

            // Pawn 인벤토리 패널
            bar.PawnsPanel = CreateInventoryPanel(bar.Root, "Pawn (1/6)");
            bar.PawnsTitleText = bar.PawnsPanel.transform.Find("Title")?.GetComponent<TMP_Text>();

            // 웨이브 정보 패널
            bar.WavePanel = UIFactory.CreatePanel("WavePanel", bar.Root, CommonUIBuilder.BgSlot);

            // 웨이브 메시지
            bar.WaveMessageText = UIFactory.CreateText("WaveMessage", bar.WavePanel, "", 22, CommonUIBuilder.TextWhite, TextAlignmentOptions.TopLeft);
            var msgRect = bar.WaveMessageText.GetComponent<RectTransform>();
            UIFactory.SetRect(msgRect,
                new Vector2(0, 0.5f), new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(20, -20),
                new Vector2(-40, 0));

            // 다음 웨이브 버튼
            bar.NextWaveButton = UIFactory.CreateButton("NextWaveButton", bar.WavePanel, "이동", 28, CommonUIBuilder.BgButtonGreen);
            var btnRect = bar.NextWaveButton.GetComponent<RectTransform>();
            UIFactory.SetRect(btnRect,
                new Vector2(0, 0), new Vector2(1, 0.5f),
                new Vector2(0.5f, 0),
                new Vector2(0, 10),
                new Vector2(-40, -20));
            bar.NextWaveButton.onClick.AddListener(() => onNextWaveClicked?.Invoke());
            bar.NextWaveButtonText = UIFactory.GetButtonText(bar.NextWaveButton);

            return bar;
        }

        private static RectTransform CreateInventoryPanel(Transform parent, string title)
        {
            var panel = UIFactory.CreatePanel("InventoryPanel", parent, CommonUIBuilder.BgSlot);

            var titleText = UIFactory.CreateText("Title", panel, title, 28, CommonUIBuilder.TextWhite, TextAlignmentOptions.Left);
            var titleRect = titleText.GetComponent<RectTransform>();
            UIFactory.SetRect(titleRect,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(20, -15),
                new Vector2(-40, 40));

            return panel;
        }

        public class ShopBottomBar
        {
            public RectTransform Root;
            public RectTransform ItemsPanel;
            public RectTransform PawnsPanel;
            public TMP_Text PawnsTitleText;
            public RectTransform WavePanel;
            public TMP_Text WaveMessageText;
            public Button NextWaveButton;
            public TMP_Text NextWaveButtonText;

            public void SetPawnCount(int current, int max)
            {
                if (PawnsTitleText != null)
                {
                    PawnsTitleText.text = $"Pawn ({current}/{max})";
                }
            }

            public void SetNextWaveText(string text)
            {
                if (NextWaveButtonText != null)
                {
                    NextWaveButtonText.text = text;
                }
            }

            public void SetWaveMessage(string message)
            {
                if (WaveMessageText != null)
                {
                    WaveMessageText.text = message;
                }
            }
        }

        #endregion

        #region Complete Shop Layout

        /// <summary>
        /// 완전한 상점 레이아웃을 생성합니다.
        /// </summary>
        public static ShopLayout CreateShopLayout(
            Transform parent,
            int slotCount,
            Action<int> onBuyClicked,
            Action<int> onLockClicked,
            Action onResetClicked,
            Action onNextWaveClicked)
        {
            var layout = new ShopLayout();

            // 루트 패널 (배경)
            layout.Root = UIFactory.CreateFullScreenPanel("ShopRoot", parent, CommonUIBuilder.BgDark);

            // 상단 바
            layout.TopBar = CreateShopTopBar(layout.Root, onResetClicked);

            // 아이템 슬롯 컨테이너
            layout.SlotContainer = CreateItemSlotContainer(layout.Root, slotCount, onBuyClicked, onLockClicked);

            // 하단 바
            layout.BottomBar = CreateShopBottomBar(layout.Root, onNextWaveClicked);

            return layout;
        }

        public class ShopLayout
        {
            public RectTransform Root;
            public ShopTopBar TopBar;
            public ItemSlotContainer SlotContainer;
            public ShopBottomBar BottomBar;
        }

        #endregion
    }
}

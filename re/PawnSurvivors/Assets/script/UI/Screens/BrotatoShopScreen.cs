using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using PawnSurvivors.Managers;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Data;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// Brotato 스타일의 상점 화면입니다. 순수 코딩으로 생성됩니다.
    /// </summary>
    public class BrotatoShopScreen : MonoBehaviour
    {
        // UI 요소들
        private Canvas _canvas;
        private GameObject _rootPanel;
        private TMP_FontAsset _koreanFont;
        
        // 상단 바
        private TMP_Text _titleText;
        private TMP_Text _goldText;
        private Button _resetButton;
        private TMP_Text _resetButtonText;
        
        // 중앙 아이템 슬롯 (4개)
        private List<ShopItemSlot> _itemSlots = new List<ShopItemSlot>();
        
        // 우측 능력치 패널
        private GameObject _statsPanel;
        private TMP_Text _statsTitleText;
        private Toggle _basicStatsTab;
        private Toggle _secondaryStatsTab;
        private GameObject _basicStatsContent;
        private GameObject _secondaryStatsContent;
        private List<StatRow> _basicStatRows = new List<StatRow>();
        private List<StatRow> _secondaryStatRows = new List<StatRow>();
        
        // 하단 좌측: 아이템 인벤토리
        private GameObject _itemsInventoryPanel;
        private TMP_Text _itemsTitleText;
        
        // 하단 중앙: Pawn 인벤토리
        private GameObject _pawnsInventoryPanel;
        private TMP_Text _pawnsTitleText;
        
        // 하단 우측: 웨이브 정보
        private GameObject _waveInfoPanel;
        private TMP_Text _waveMessageText;
        private Button _nextWaveButton;
        private TMP_Text _nextWaveButtonText;
        
        private bool _uiCreated = false;
        private int _currentWave = 1;
        private int _resetCost = 1;
        
        // Pawn 선택 UI
        private GameObject _pawnSelectionPanel;
        private ItemData _pendingItemPurchase; // 구매 대기 중인 아이템

        // 아이템 슬롯 데이터 구조
        private class ShopItemSlot
        {
            public GameObject rootObject;
            public Image iconImage;
            public TMP_Text nameText;
            public TMP_Text typeText;
            public TMP_Text descriptionText;
            public TMP_Text costText;
            public Button buyButton;
            public Button lockButton;
            public TMP_Text lockButtonText;
            public bool isLocked = false;
            public bool isPurchased = false; // 구매 완료 여부
            public int cost = 0;
            public ItemData itemData; // 아이템 데이터 저장
        }

        // 통계 행 데이터 구조
        private class StatRow
        {
            public GameObject rootObject;
            public Image iconImage;
            public TMP_Text nameText;
            public TMP_Text valueText;
        }

        private void Start()
        {
            if (!_uiCreated)
            {
                CreateShopUI();
                _uiCreated = true;
            }
        }

        private void OnEnable()
        {
            if (!_uiCreated)
            {
                CreateShopUI();
                _uiCreated = true;
            }
            
            if (_rootPanel != null)
            {
                _rootPanel.SetActive(true);
            }
            
            RefreshUI();
        }

        private void OnDisable()
        {
            if (_rootPanel != null)
            {
                _rootPanel.SetActive(false);
            }
        }

        private void Update()
        {
            UpdateGoldDisplay();
        }

        private void CreateShopUI()
        {
            // 기존 UI가 있으면 정리
            if (_itemSlots != null && _itemSlots.Count > 0)
            {
                foreach (var slot in _itemSlots)
                {
                    if (slot.buyButton != null)
                    {
                        slot.buyButton.onClick.RemoveAllListeners();
                    }
                    if (slot.lockButton != null)
                    {
                        slot.lockButton.onClick.RemoveAllListeners();
                    }
                    if (slot.rootObject != null)
                    {
                        Destroy(slot.rootObject);
                    }
                }
                _itemSlots.Clear();
            }
            
            // 폰트 로드
            LoadKoreanFont();
            
            // Canvas 찾기 또는 생성
            SetupCanvas();
            
            // 루트 패널 생성
            CreateRootPanel();
            
            // 상단 바 생성
            CreateTopBar();
            
            // 중앙 아이템 슬롯 생성
            CreateItemSlots();
            
            // 하단 패널들 생성
            CreateBottomPanels();
            
            // 기본적으로 숨김
            gameObject.SetActive(false);
        }

        private void LoadKoreanFont()
        {
            _koreanFont = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
            #if UNITY_EDITOR
            if (_koreanFont == null)
            {
                _koreanFont = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/NanumGothic SDF.asset");
            }
            #endif
        }

        private void SetupCanvas()
        {
            _canvas = GetComponentInParent<Canvas>();
            if (_canvas == null)
            {
                // ScreenSpaceOverlay 또는 ScreenSpaceCamera Canvas만 찾기 (World Space 제외)
                Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
                foreach (var canvas in allCanvases)
                {
                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || 
                        canvas.renderMode == RenderMode.ScreenSpaceCamera)
                    {
                        _canvas = canvas;
                        break;
                    }
                }
            }
            
            if (_canvas == null)
            {
                GameObject canvasObj = new GameObject("BrotatoShopCanvas");
                _canvas = canvasObj.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                
                var scaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
                
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            else
            {
                // 기존 Canvas에 CanvasScaler가 있으면 업데이트
                var scaler = _canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
                if (scaler != null)
                {
                    scaler.referenceResolution = new Vector2(1920, 1080);
                }
            }
        }

        private void CreateRootPanel()
        {
            _rootPanel = new GameObject("BrotatoShopRootPanel");
            _rootPanel.transform.SetParent(_canvas.transform, false);
            
            var rect = _rootPanel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            
            var image = _rootPanel.AddComponent<Image>();
            image.color = new Color(0.05f, 0.05f, 0.08f, 1f); // 어두운 배경
        }

        private void CreateTopBar()
        {
            // 제목 (좌측 상단)
            var titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(_rootPanel.transform, false);
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(0f, 1f);
            titleRect.pivot = new Vector2(0f, 1f);
            titleRect.anchoredPosition = new Vector2(60f, -40f);
            titleRect.sizeDelta = new Vector2(600f, 80f);
            
            _titleText = titleObj.AddComponent<TextMeshProUGUI>();
            _titleText.text = "상점 (웨이브 1)";
            _titleText.fontSize = 56;
            _titleText.color = Color.white;
            _titleText.alignment = TextAlignmentOptions.Left;
            if (_koreanFont != null) _titleText.font = _koreanFont;
            
            // 골드 표시 (상단 중앙)
            var goldObj = new GameObject("GoldText");
            goldObj.transform.SetParent(_rootPanel.transform, false);
            var goldRect = goldObj.AddComponent<RectTransform>();
            goldRect.anchorMin = new Vector2(0.5f, 1f);
            goldRect.anchorMax = new Vector2(0.5f, 1f);
            goldRect.pivot = new Vector2(0.5f, 1f);
            goldRect.anchoredPosition = new Vector2(0f, -40f);
            goldRect.sizeDelta = new Vector2(300f, 80f);
            
            _goldText = goldObj.AddComponent<TextMeshProUGUI>();
            _goldText.text = "30";
            _goldText.fontSize = 48;
            _goldText.color = Color.green;
            _goldText.alignment = TextAlignmentOptions.Center;
            if (_koreanFont != null) _goldText.font = _koreanFont;
            
            // 골드 아이콘 (골드 텍스트 왼쪽)
            var goldIconObj = new GameObject("GoldIcon");
            goldIconObj.transform.SetParent(_rootPanel.transform, false);
            var goldIconRect = goldIconObj.AddComponent<RectTransform>();
            goldIconRect.anchorMin = new Vector2(0.5f, 1f);
            goldIconRect.anchorMax = new Vector2(0.5f, 1f);
            goldIconRect.pivot = new Vector2(1f, 0.5f);
            goldIconRect.anchoredPosition = new Vector2(-160f, -60f);
            goldIconRect.sizeDelta = new Vector2(60f, 60f);
            var goldIcon = goldIconObj.AddComponent<Image>();
            goldIcon.color = Color.green;
            
            // 초기화 버튼 (우측 상단)
            var resetObj = new GameObject("ResetButton");
            resetObj.transform.SetParent(_rootPanel.transform, false);
            var resetRect = resetObj.AddComponent<RectTransform>();
            resetRect.anchorMin = new Vector2(1f, 1f);
            resetRect.anchorMax = new Vector2(1f, 1f);
            resetRect.pivot = new Vector2(1f, 1f);
            resetRect.anchoredPosition = new Vector2(-60f, -40f);
            resetRect.sizeDelta = new Vector2(400f, 80f);
            
            _resetButton = resetObj.AddComponent<Button>();
            var resetImage = resetObj.AddComponent<Image>();
            resetImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            
            _resetButtonText = new GameObject("Text").AddComponent<TextMeshProUGUI>();
            _resetButtonText.transform.SetParent(resetObj.transform, false);
            var resetTextRect = _resetButtonText.GetComponent<RectTransform>();
            resetTextRect.anchorMin = Vector2.zero;
            resetTextRect.anchorMax = Vector2.one;
            resetTextRect.sizeDelta = Vector2.zero;
            _resetButtonText.text = $"F 초기화 - {_resetCost}";
            _resetButtonText.fontSize = 36;
            _resetButtonText.color = Color.white;
            _resetButtonText.alignment = TextAlignmentOptions.Center;
            if (_koreanFont != null) _resetButtonText.font = _koreanFont;
            
            _resetButton.targetGraphic = resetImage;
            _resetButton.onClick.AddListener(OnResetButtonClicked);
        }

        private void CreateItemSlots()
        {
            // 4개 슬롯이 전체 너비를 사용하도록 계산
            // 좌우 여백 120px씩, 슬롯 간 간격 40px
            float totalWidth = 1920f;
            float leftMargin = 120f;
            float rightMargin = 120f;
            float spacing = 40f;
            float availableWidth = totalWidth - leftMargin - rightMargin;
            float slotWidth = (availableWidth - (spacing * 3)) / 4f; // 4개 슬롯
            float slotHeight = 700f;
            float startX = leftMargin;
            float centerY = 65f; // 위로 65만큼 올림
            
            for (int i = 0; i < 4; i++)
            {
                int slotIndex = i; // 클로저 캡처 문제 해결을 위한 로컬 변수
                var slot = new ShopItemSlot();
                
                // 슬롯 루트
                slot.rootObject = new GameObject($"ItemSlot_{slotIndex}");
                slot.rootObject.transform.SetParent(_rootPanel.transform, false);
                var slotRect = slot.rootObject.AddComponent<RectTransform>();
                slotRect.anchorMin = new Vector2(0f, 0.5f);
                slotRect.anchorMax = new Vector2(0f, 0.5f);
                slotRect.pivot = new Vector2(0f, 0.5f);
                slotRect.anchoredPosition = new Vector2(startX + slotIndex * (slotWidth + spacing), centerY);
                slotRect.sizeDelta = new Vector2(slotWidth, slotHeight);
                
                // 배경
                var bg = slot.rootObject.AddComponent<Image>();
                bg.color = new Color(0.15f, 0.15f, 0.15f, 1f);
                
                // 아이콘
                var iconObj = new GameObject("Icon");
                iconObj.transform.SetParent(slot.rootObject.transform, false);
                var iconRect = iconObj.AddComponent<RectTransform>();
                iconRect.anchorMin = new Vector2(0.5f, 1f);
                iconRect.anchorMax = new Vector2(0.5f, 1f);
                iconRect.pivot = new Vector2(0.5f, 1f);
                iconRect.anchoredPosition = new Vector2(0f, -40f);
                iconRect.sizeDelta = new Vector2(140f, 140f);
                slot.iconImage = iconObj.AddComponent<Image>();
                slot.iconImage.color = Color.white;
                slot.iconImage.raycastTarget = false; // 아이콘은 클릭 방해 안 함
                
                // 이름
                var nameObj = new GameObject("NameText");
                nameObj.transform.SetParent(slot.rootObject.transform, false);
                var nameRect = nameObj.AddComponent<RectTransform>();
                nameRect.anchorMin = new Vector2(0f, 1f);
                nameRect.anchorMax = new Vector2(1f, 1f);
                nameRect.pivot = new Vector2(0f, 1f);
                nameRect.anchoredPosition = new Vector2(20f, -200f);
                nameRect.sizeDelta = new Vector2(-40f, 50f);
                slot.nameText = nameObj.AddComponent<TextMeshProUGUI>();
                slot.nameText.text = "아이템 이름";
                slot.nameText.fontSize = 36;
                slot.nameText.color = Color.white;
                slot.nameText.alignment = TextAlignmentOptions.Left;
                slot.nameText.raycastTarget = false; // 텍스트는 클릭 방해 안 함
                if (_koreanFont != null) slot.nameText.font = _koreanFont;
                
                // 타입
                var typeObj = new GameObject("TypeText");
                typeObj.transform.SetParent(slot.rootObject.transform, false);
                var typeRect = typeObj.AddComponent<RectTransform>();
                typeRect.anchorMin = new Vector2(0f, 1f);
                typeRect.anchorMax = new Vector2(1f, 1f);
                typeRect.pivot = new Vector2(0f, 1f);
                typeRect.anchoredPosition = new Vector2(20f, -250f);
                typeRect.sizeDelta = new Vector2(-40f, 40f);
                slot.typeText = typeObj.AddComponent<TextMeshProUGUI>();
                slot.typeText.text = "타입";
                slot.typeText.fontSize = 28;
                slot.typeText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                slot.typeText.alignment = TextAlignmentOptions.Left;
                slot.typeText.raycastTarget = false; // 텍스트는 클릭 방해 안 함
                if (_koreanFont != null) slot.typeText.font = _koreanFont;
                
                // 설명
                var descObj = new GameObject("DescriptionText");
                descObj.transform.SetParent(slot.rootObject.transform, false);
                var descRect = descObj.AddComponent<RectTransform>();
                descRect.anchorMin = new Vector2(0f, 0.5f);
                descRect.anchorMax = new Vector2(1f, 1f);
                descRect.pivot = new Vector2(0f, 1f);
                descRect.anchoredPosition = new Vector2(20f, -300f);
                descRect.sizeDelta = new Vector2(-40f, -300f);
                slot.descriptionText = descObj.AddComponent<TextMeshProUGUI>();
                slot.descriptionText.text = "설명";
                slot.descriptionText.fontSize = 24;
                slot.descriptionText.color = Color.white;
                slot.descriptionText.alignment = TextAlignmentOptions.TopLeft;
                slot.descriptionText.textWrappingMode = TextWrappingModes.Normal;
                slot.descriptionText.raycastTarget = false; // 텍스트는 클릭 방해 안 함
                if (_koreanFont != null) slot.descriptionText.font = _koreanFont;
                
                // 가격 (좌측 하단)
                var costObj = new GameObject("CostText");
                costObj.transform.SetParent(slot.rootObject.transform, false);
                var costRect = costObj.AddComponent<RectTransform>();
                costRect.anchorMin = new Vector2(0f, 0f);
                costRect.anchorMax = new Vector2(0f, 0f);
                costRect.pivot = new Vector2(0f, 0f);
                costRect.anchoredPosition = new Vector2(20f, 20f);
                costRect.sizeDelta = new Vector2(100f, 50f);
                slot.costText = costObj.AddComponent<TextMeshProUGUI>();
                slot.costText.text = "1";
                slot.costText.fontSize = 36;
                slot.costText.color = Color.green;
                slot.costText.alignment = TextAlignmentOptions.Left;
                slot.costText.raycastTarget = false; // 텍스트는 클릭 방해 안 함
                if (_koreanFont != null) slot.costText.font = _koreanFont;
                
                // 구매 버튼 (전체 슬롯)
                slot.buyButton = slot.rootObject.AddComponent<Button>();
                slot.buyButton.targetGraphic = bg; // 배경을 targetGraphic으로 설정
                // 클로저 캡처 문제 해결: slotIndex를 사용
                slot.buyButton.onClick.AddListener(() => OnItemBuyClicked(slotIndex));
                
                // 잠금 버튼 (우측 하단)
                var lockObj = new GameObject("LockButton");
                lockObj.transform.SetParent(slot.rootObject.transform, false);
                var lockRect = lockObj.AddComponent<RectTransform>();
                lockRect.anchorMin = new Vector2(1f, 0f);
                lockRect.anchorMax = new Vector2(1f, 0f);
                lockRect.pivot = new Vector2(1f, 0f);
                lockRect.anchoredPosition = new Vector2(-20f, 20f);
                lockRect.sizeDelta = new Vector2(180f, 50f);
                
                slot.lockButton = lockObj.AddComponent<Button>();
                var lockImage = lockObj.AddComponent<Image>();
                lockImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                
                slot.lockButtonText = new GameObject("Text").AddComponent<TextMeshProUGUI>();
                slot.lockButtonText.transform.SetParent(lockObj.transform, false);
                var lockTextRect = slot.lockButtonText.GetComponent<RectTransform>();
                lockTextRect.anchorMin = Vector2.zero;
                lockTextRect.anchorMax = Vector2.one;
                lockTextRect.sizeDelta = Vector2.zero;
                slot.lockButtonText.text = "E 잠금";
                slot.lockButtonText.fontSize = 28;
                slot.lockButtonText.color = Color.white;
                slot.lockButtonText.alignment = TextAlignmentOptions.Center;
                slot.lockButtonText.raycastTarget = false; // 텍스트는 클릭 방해 안 함
                if (_koreanFont != null) slot.lockButtonText.font = _koreanFont;
                
                slot.lockButton.targetGraphic = lockImage;
                // 클로저 캡처 문제 해결: slotIndex를 사용
                slot.lockButton.onClick.AddListener(() => OnItemLockClicked(slotIndex));
                
                // 잠금 버튼이 구매 버튼보다 위에 오도록 설정 (이미 자식 순서로 해결됨)
                lockObj.transform.SetAsLastSibling(); // 잠금 버튼을 가장 위로
                
                _itemSlots.Add(slot);
            }
        }

        private void CreateStatsPanel()
        {
            // 능력치 패널 루트
            _statsPanel = new GameObject("StatsPanel");
            _statsPanel.transform.SetParent(_rootPanel.transform, false);
            var statsRect = _statsPanel.AddComponent<RectTransform>();
            statsRect.anchorMin = new Vector2(1f, 0f);
            statsRect.anchorMax = new Vector2(1f, 1f);
            statsRect.pivot = new Vector2(1f, 0.5f);
            statsRect.anchoredPosition = new Vector2(-60f, 0f);
            statsRect.sizeDelta = new Vector2(450f, 0f);
            
            var statsBg = _statsPanel.AddComponent<Image>();
            statsBg.color = new Color(0.12f, 0.12f, 0.12f, 1f);
            
            // 제목
            var titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(_statsPanel.transform, false);
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0f, 1f);
            titleRect.anchoredPosition = new Vector2(30f, -40f);
            titleRect.sizeDelta = new Vector2(-60f, 60f);
            
            _statsTitleText = titleObj.AddComponent<TextMeshProUGUI>();
            _statsTitleText.text = "능력치";
            _statsTitleText.fontSize = 42;
            _statsTitleText.color = Color.white;
            _statsTitleText.alignment = TextAlignmentOptions.Left;
            if (_koreanFont != null) _statsTitleText.font = _koreanFont;
            
            // 탭 버튼들
            CreateStatsTabs();
            
            // 통계 내용
            CreateStatsContent();
        }

        private void CreateStatsTabs()
        {
            var tabsObj = new GameObject("Tabs");
            tabsObj.transform.SetParent(_statsPanel.transform, false);
            var tabsRect = tabsObj.AddComponent<RectTransform>();
            tabsRect.anchorMin = new Vector2(0f, 1f);
            tabsRect.anchorMax = new Vector2(1f, 1f);
            tabsRect.pivot = new Vector2(0f, 1f);
            tabsRect.anchoredPosition = new Vector2(30f, -120f);
            tabsRect.sizeDelta = new Vector2(-60f, 60f);
            
            // 기본적인 탭
            var basicTabObj = new GameObject("BasicTab");
            basicTabObj.transform.SetParent(tabsObj.transform, false);
            var basicTabRect = basicTabObj.AddComponent<RectTransform>();
            basicTabRect.anchorMin = new Vector2(0f, 0f);
            basicTabRect.anchorMax = new Vector2(0.5f, 1f);
            basicTabRect.sizeDelta = Vector2.zero;
            
            _basicStatsTab = basicTabObj.AddComponent<Toggle>();
            var basicTabImage = basicTabObj.AddComponent<Image>();
            basicTabImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            var basicTabText = new GameObject("Text").AddComponent<TextMeshProUGUI>();
            basicTabText.transform.SetParent(basicTabObj.transform, false);
            var basicTabTextRect = basicTabText.GetComponent<RectTransform>();
            basicTabTextRect.anchorMin = Vector2.zero;
            basicTabTextRect.anchorMax = Vector2.one;
            basicTabTextRect.sizeDelta = Vector2.zero;
            basicTabText.text = "기본적인";
            basicTabText.fontSize = 32;
            basicTabText.color = Color.white;
            basicTabText.alignment = TextAlignmentOptions.Center;
            if (_koreanFont != null) basicTabText.font = _koreanFont;
            
            _basicStatsTab.targetGraphic = basicTabImage;
            _basicStatsTab.isOn = true;
            _basicStatsTab.onValueChanged.AddListener(OnBasicTabToggled);
            
            // 2차적인 탭
            var secondaryTabObj = new GameObject("SecondaryTab");
            secondaryTabObj.transform.SetParent(tabsObj.transform, false);
            var secondaryTabRect = secondaryTabObj.AddComponent<RectTransform>();
            secondaryTabRect.anchorMin = new Vector2(0.5f, 0f);
            secondaryTabRect.anchorMax = new Vector2(1f, 1f);
            secondaryTabRect.sizeDelta = Vector2.zero;
            
            _secondaryStatsTab = secondaryTabObj.AddComponent<Toggle>();
            var secondaryTabImage = secondaryTabObj.AddComponent<Image>();
            secondaryTabImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            var secondaryTabText = new GameObject("Text").AddComponent<TextMeshProUGUI>();
            secondaryTabText.transform.SetParent(secondaryTabObj.transform, false);
            var secondaryTabTextRect = secondaryTabText.GetComponent<RectTransform>();
            secondaryTabTextRect.anchorMin = Vector2.zero;
            secondaryTabTextRect.anchorMax = Vector2.one;
            secondaryTabTextRect.sizeDelta = Vector2.zero;
            secondaryTabText.text = "2차적인";
            secondaryTabText.fontSize = 32;
            secondaryTabText.color = Color.white;
            secondaryTabText.alignment = TextAlignmentOptions.Center;
            if (_koreanFont != null) secondaryTabText.font = _koreanFont;
            
            _secondaryStatsTab.targetGraphic = secondaryTabImage;
            _secondaryStatsTab.isOn = false;
            _secondaryStatsTab.onValueChanged.AddListener(OnSecondaryTabToggled);
        }

        private void CreateStatsContent()
        {
            // 기본 통계 내용
            _basicStatsContent = new GameObject("BasicStatsContent");
            _basicStatsContent.transform.SetParent(_statsPanel.transform, false);
            var basicContentRect = _basicStatsContent.AddComponent<RectTransform>();
            basicContentRect.anchorMin = new Vector2(0f, 0f);
            basicContentRect.anchorMax = new Vector2(1f, 1f);
            basicContentRect.pivot = new Vector2(0f, 1f);
            basicContentRect.anchoredPosition = new Vector2(30f, -200f);
            basicContentRect.sizeDelta = new Vector2(-60f, -200f);
            
            var basicLayout = _basicStatsContent.AddComponent<VerticalLayoutGroup>();
            basicLayout.spacing = 10f;
            basicLayout.padding = new RectOffset(10, 10, 10, 10);
            basicLayout.childControlHeight = false;
            basicLayout.childControlWidth = true;
            basicLayout.childForceExpandWidth = true;
            
            // 2차 통계 내용
            _secondaryStatsContent = new GameObject("SecondaryStatsContent");
            _secondaryStatsContent.transform.SetParent(_statsPanel.transform, false);
            var secondaryContentRect = _secondaryStatsContent.AddComponent<RectTransform>();
            secondaryContentRect.anchorMin = new Vector2(0f, 0f);
            secondaryContentRect.anchorMax = new Vector2(1f, 1f);
            secondaryContentRect.pivot = new Vector2(0f, 1f);
            secondaryContentRect.anchoredPosition = new Vector2(30f, -200f);
            secondaryContentRect.sizeDelta = new Vector2(-60f, -200f);
            
            var secondaryLayout = _secondaryStatsContent.AddComponent<VerticalLayoutGroup>();
            secondaryLayout.spacing = 10f;
            secondaryLayout.padding = new RectOffset(10, 10, 10, 10);
            secondaryLayout.childControlHeight = false;
            secondaryLayout.childControlWidth = true;
            secondaryLayout.childForceExpandWidth = true;
            
            _secondaryStatsContent.SetActive(false);
            
            // 통계 행 생성
            CreateStatRows();
        }

        private void CreateStatRows()
        {
            // 기본 통계들
            string[] basicStatNames = {
                "현재 레벨", "최대 HP", "HP 재생", "% 생명 훔침", "% 대미지",
                "근거리 대미지", "원거리 대미지", "원소 대미지", "% 공격 속도", "% 치명타율"
            };
            
            foreach (var statName in basicStatNames)
            {
                var row = CreateStatRow(statName, _basicStatsContent);
                _basicStatRows.Add(row);
            }
            
            // 2차 통계들
            string[] secondaryStatNames = {
                "엔지니어링", "범위", "방어구", "% 회피", "% 속도", "행운", "수확"
            };
            
            foreach (var statName in secondaryStatNames)
            {
                var row = CreateStatRow(statName, _secondaryStatsContent);
                _secondaryStatRows.Add(row);
            }
        }

        private StatRow CreateStatRow(string statName, GameObject parent)
        {
            var row = new StatRow();
            
            row.rootObject = new GameObject($"StatRow_{statName}");
            row.rootObject.transform.SetParent(parent.transform, false);
            var rowRect = row.rootObject.AddComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(0f, 45f);
            
            // 아이콘
            var iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(row.rootObject.transform, false);
            var iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0f, 0f);
            iconRect.anchorMax = new Vector2(0f, 1f);
            iconRect.pivot = new Vector2(0f, 0.5f);
            iconRect.anchoredPosition = new Vector2(0f, 0f);
            iconRect.sizeDelta = new Vector2(36f, 36f);
            row.iconImage = iconObj.AddComponent<Image>();
            row.iconImage.color = Color.gray;
            
            // 이름
            var nameObj = new GameObject("NameText");
            nameObj.transform.SetParent(row.rootObject.transform, false);
            var nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0f, 0f);
            nameRect.anchorMax = new Vector2(0.7f, 1f);
            nameRect.pivot = new Vector2(0f, 0.5f);
            nameRect.anchoredPosition = new Vector2(45f, 0f);
            nameRect.sizeDelta = Vector2.zero;
            row.nameText = nameObj.AddComponent<TextMeshProUGUI>();
            row.nameText.text = statName;
            row.nameText.fontSize = 26;
            row.nameText.color = Color.white;
            row.nameText.alignment = TextAlignmentOptions.Left;
            if (_koreanFont != null) row.nameText.font = _koreanFont;
            
            // 값
            var valueObj = new GameObject("ValueText");
            valueObj.transform.SetParent(row.rootObject.transform, false);
            var valueRect = valueObj.AddComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(0.7f, 0f);
            valueRect.anchorMax = new Vector2(1f, 1f);
            valueRect.pivot = new Vector2(1f, 0.5f);
            valueRect.anchoredPosition = Vector2.zero;
            valueRect.sizeDelta = Vector2.zero;
            row.valueText = valueObj.AddComponent<TextMeshProUGUI>();
            row.valueText.text = "U";
            row.valueText.fontSize = 26;
            row.valueText.color = Color.white;
            row.valueText.alignment = TextAlignmentOptions.Right;
            if (_koreanFont != null) row.valueText.font = _koreanFont;
            
            return row;
        }

        private void CreateBottomPanels()
        {
            float panelHeight = 220f;
            float panelY = 40f;
            
            // 아이템 인벤토리 (좌측)
            _itemsInventoryPanel = new GameObject("ItemsInventoryPanel");
            _itemsInventoryPanel.transform.SetParent(_rootPanel.transform, false);
            var itemsRect = _itemsInventoryPanel.AddComponent<RectTransform>();
            itemsRect.anchorMin = new Vector2(0f, 0f);
            itemsRect.anchorMax = new Vector2(0.33f, 0f);
            itemsRect.pivot = new Vector2(0f, 0f);
            itemsRect.anchoredPosition = new Vector2(40f, panelY);
            itemsRect.sizeDelta = new Vector2(-60f, panelHeight);
            
            var itemsBg = _itemsInventoryPanel.AddComponent<Image>();
            itemsBg.color = new Color(0.12f, 0.12f, 0.12f, 1f);
            
            _itemsTitleText = CreatePanelTitle("아이템", _itemsInventoryPanel);
            
            // Pawn 인벤토리 (중앙)
            _pawnsInventoryPanel = new GameObject("PawnsInventoryPanel");
            _pawnsInventoryPanel.transform.SetParent(_rootPanel.transform, false);
            var pawnsRect = _pawnsInventoryPanel.AddComponent<RectTransform>();
            pawnsRect.anchorMin = new Vector2(0.33f, 0f);
            pawnsRect.anchorMax = new Vector2(0.66f, 0f);
            pawnsRect.pivot = new Vector2(0f, 0f);
            pawnsRect.anchoredPosition = new Vector2(20f, panelY);
            pawnsRect.sizeDelta = new Vector2(-40f, panelHeight);
            
            var pawnsBg = _pawnsInventoryPanel.AddComponent<Image>();
            pawnsBg.color = new Color(0.12f, 0.12f, 0.12f, 1f);
            
            _pawnsTitleText = CreatePanelTitle("Pawn (1/6)", _pawnsInventoryPanel);
            
            // 웨이브 정보 (우측)
            _waveInfoPanel = new GameObject("WaveInfoPanel");
            _waveInfoPanel.transform.SetParent(_rootPanel.transform, false);
            var waveRect = _waveInfoPanel.AddComponent<RectTransform>();
            waveRect.anchorMin = new Vector2(0.66f, 0f);
            waveRect.anchorMax = new Vector2(1f, 0f);
            waveRect.pivot = new Vector2(0f, 0f);
            waveRect.anchoredPosition = new Vector2(20f, panelY);
            waveRect.sizeDelta = new Vector2(-40f, panelHeight);
            
            var waveBg = _waveInfoPanel.AddComponent<Image>();
            waveBg.color = new Color(0.12f, 0.12f, 0.12f, 1f);
            
            _waveMessageText = new GameObject("WaveMessageText").AddComponent<TextMeshProUGUI>();
            _waveMessageText.transform.SetParent(_waveInfoPanel.transform, false);
            var waveMsgRect = _waveMessageText.GetComponent<RectTransform>();
            waveMsgRect.anchorMin = new Vector2(0f, 0.5f);
            waveMsgRect.anchorMax = new Vector2(1f, 1f);
            waveMsgRect.pivot = new Vector2(0f, 1f);
            waveMsgRect.anchoredPosition = new Vector2(20f, -60f);
            waveMsgRect.sizeDelta = new Vector2(-40f, -80f);
            _waveMessageText.text = "웨이브 12에 엘리트가 나타납니다";
            _waveMessageText.fontSize = 26;
            _waveMessageText.color = Color.white;
            _waveMessageText.alignment = TextAlignmentOptions.Left;
            _waveMessageText.textWrappingMode = TextWrappingModes.Normal;
            if (_koreanFont != null) _waveMessageText.font = _koreanFont;
            
            // 다음 웨이브 버튼
            var nextWaveObj = new GameObject("NextWaveButton");
            nextWaveObj.transform.SetParent(_waveInfoPanel.transform, false);
            var nextWaveRect = nextWaveObj.AddComponent<RectTransform>();
            nextWaveRect.anchorMin = new Vector2(0f, 0f);
            nextWaveRect.anchorMax = new Vector2(1f, 0.5f);
            nextWaveRect.pivot = new Vector2(0f, 0f);
            nextWaveRect.anchoredPosition = new Vector2(20f, 20f);
            nextWaveRect.sizeDelta = new Vector2(-40f, -20f);
            
            _nextWaveButton = nextWaveObj.AddComponent<Button>();
            var nextWaveImage = nextWaveObj.AddComponent<Image>();
            nextWaveImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);
            
            _nextWaveButtonText = new GameObject("Text").AddComponent<TextMeshProUGUI>();
            _nextWaveButtonText.transform.SetParent(nextWaveObj.transform, false);
            var nextWaveTextRect = _nextWaveButtonText.GetComponent<RectTransform>();
            nextWaveTextRect.anchorMin = Vector2.zero;
            nextWaveTextRect.anchorMax = Vector2.one;
            nextWaveTextRect.sizeDelta = Vector2.zero;
            _nextWaveButtonText.text = "이동 (웨이브 2)";
            _nextWaveButtonText.fontSize = 32;
            _nextWaveButtonText.color = Color.white;
            _nextWaveButtonText.alignment = TextAlignmentOptions.Center;
            if (_koreanFont != null) _nextWaveButtonText.font = _koreanFont;
            
            _nextWaveButton.targetGraphic = nextWaveImage;
            _nextWaveButton.onClick.AddListener(OnNextWaveButtonClicked);
        }

        private TMP_Text CreatePanelTitle(string title, GameObject parent)
        {
            var titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(parent.transform, false);
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0f, 1f);
            titleRect.anchoredPosition = new Vector2(20f, -20f);
            titleRect.sizeDelta = new Vector2(-40f, 45f);
            
            var titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = title;
            titleText.fontSize = 32;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Left;
            if (_koreanFont != null) titleText.font = _koreanFont;
            
            return titleText;
        }

        private void RefreshUI()
        {
            UpdateGoldDisplay();
            UpdateTitle();
            UpdatePawnsInventory();
            RefreshShopItems(); // 아이템 슬롯 업데이트
            // 스탯은 다른 방식으로 확인 (스탯 패널 제거됨)
        }
        
        /// <summary>
        /// 상점 아이템 슬롯을 새로고침합니다.
        /// ShopUseCase를 사용하여 중복 없는 랜덤 아이템을 가져옵니다.
        /// 리롤 시 구매한 슬롯도 초기화됩니다.
        /// </summary>
        private void RefreshShopItems()
        {
            if (GameManager.Instance?.ShopUseCase == null)
            {
                LogManager.LogWarning(LogCategory.UI, "ShopUseCase가 없어 상점 아이템을 가져올 수 없습니다.");
                return;
            }
            
            // 잠금되지 않은 슬롯만 새로고침 (구매한 슬롯도 포함)
            var unlockedSlots = new List<int>();
            var lockedItemIds = new List<string>(); // 잠긴 슬롯의 아이템 ID 수집
            
            for (int i = 0; i < _itemSlots.Count; i++)
            {
                if (!_itemSlots[i].isLocked)
                {
                    unlockedSlots.Add(i);
                    // 리롤 시 구매 상태 초기화
                    _itemSlots[i].isPurchased = false;
                }
                else if (_itemSlots[i].itemData != null && !string.IsNullOrEmpty(_itemSlots[i].itemData.itemId))
                {
                    // 잠긴 슬롯의 아이템 ID 수집 (중복 방지)
                    if (!lockedItemIds.Contains(_itemSlots[i].itemData.itemId))
                    {
                        lockedItemIds.Add(_itemSlots[i].itemData.itemId);
                    }
                }
            }
            
            if (unlockedSlots.Count == 0)
            {
                LogManager.LogInfo(LogCategory.UI, "모든 슬롯이 잠겨있어 아이템을 새로고침하지 않습니다.");
                return;
            }
            
            // ShopUseCase를 통해 중복 없는 랜덤 아이템 가져오기 (잠긴 아이템 제외)
            var randomItems = GameManager.Instance.ShopUseCase.GetShopItems(
                unlockedSlots.Count,
                excludeOwned: false, // 보유 아이템 제외 여부 (필요시 변경)
                excludeItemIds: lockedItemIds.Count > 0 ? lockedItemIds : null // 잠긴 슬롯의 아이템 제외
            );
            
            // 슬롯에 아이템 할당
            for (int i = 0; i < unlockedSlots.Count && i < randomItems.Count; i++)
            {
                int slotIndex = unlockedSlots[i];
                var slot = _itemSlots[slotIndex];
                var item = randomItems[i];
                
                // 아이템 데이터 저장
                slot.itemData = item;
                slot.cost = item.cost;
                
                // UI 업데이트
                if (slot.nameText != null)
                {
                    slot.nameText.text = item.itemName ?? "알 수 없음";
                }
                
                if (slot.typeText != null)
                {
                    slot.typeText.text = item.itemType == ItemType.Global ? "전역" : "장착";
                }
                
                if (slot.descriptionText != null)
                {
                    slot.descriptionText.text = item.description ?? "";
                }
                
                if (slot.costText != null)
                {
                    slot.costText.text = item.cost.ToString();
                }
                
                // 버튼 활성화/비활성화 (골드에 따라)
                UpdateSlotButtonState(slot);
            }
            
            LogManager.LogInfo(LogCategory.UI, $"상점 아이템 {unlockedSlots.Count}개 새로고침 완료");
        }
        
        /// <summary>
        /// 슬롯의 버튼 상태를 골드에 따라 업데이트합니다.
        /// </summary>
        private void UpdateSlotButtonState(ShopItemSlot slot)
        {
            if (slot.buyButton == null || slot.itemData == null) return;
            
            // 구매한 슬롯은 항상 비활성화
            if (slot.isPurchased)
            {
                slot.buyButton.interactable = false;
                if (slot.costText != null)
                {
                    slot.costText.color = Color.gray;
                }
                return;
            }
            
            if (GameManager.Instance?.ItemManagementUseCase == null)
            {
                slot.buyButton.interactable = false;
                return;
            }
            
            // UseCase를 통해 구매 가능 여부 확인
            bool canBuy = GameManager.Instance.ItemManagementUseCase.CanBuyItem(slot.itemData);
            slot.buyButton.interactable = canBuy;
            
            // 골드 부족 시 비용 텍스트 색상 변경
            if (slot.costText != null)
            {
                slot.costText.color = canBuy ? Color.green : Color.red;
            }
        }

        private void UpdateGoldDisplay()
        {
            if (_goldText == null || GameManager.Instance?.CurrencyUseCase == null) return;
            
            int gold = GameManager.Instance.CurrencyUseCase.GetGold();
            _goldText.text = gold.ToString();
            
            // 모든 슬롯의 버튼 상태 업데이트
            foreach (var slot in _itemSlots)
            {
                if (slot != null && slot.itemData != null)
                {
                    UpdateSlotButtonState(slot);
                }
            }
        }

        private void UpdateTitle()
        {
            if (_titleText != null)
            {
                _titleText.text = $"상점 (웨이브 {_currentWave})";
            }
        }

        private void UpdateStats()
        {
            // 플레이어 Pawn 데이터 가져오기
            if (GameManager.Instance?.PlayerController == null) return;
            
            var playerPawns = GameManager.Instance.PlayerController.playerPawns;
            if (playerPawns == null || playerPawns.Count == 0) return;
            
            // 모든 Pawn의 스탯 합산
            float totalMaxHP = 0f;
            float totalCurrentHP = 0f;
            float totalDamage = 0f;
            float totalFireRate = 0f;
            int totalLevel = 0;
            int validPawnCount = 0;
            
            // ✅ PawnStatCalculator 가져오기
            PawnStatCalculator statCalculator = GameManager.Instance?.PawnStatCalculator;
            
            foreach (var pawnObj in playerPawns)
            {
                if (pawnObj == null) continue;
                
                var pawnManager = pawnObj.GetComponent<PawnManager>();
                if (pawnManager?.PawnData == null) continue;
                
                var pawnData = pawnManager.PawnData;
                validPawnCount++;
                
                // ✅ HP 합산 (PawnStatCalculator 사용)
                if (pawnData.healthData != null)
                {
                    float effectiveMaxHP = statCalculator != null 
                        ? statCalculator.GetEffectiveMaxHealth(pawnData) 
                        : pawnData.healthData.maxHealth;
                    totalMaxHP += effectiveMaxHP;
                    totalCurrentHP += pawnData.healthData.currentHealth;
                }
                
                // ✅ 대미지 합산 (PawnStatCalculator 사용)
                if (pawnData.combatData != null)
                {
                    float effectiveDamage = statCalculator != null 
                        ? statCalculator.GetEffectiveDamage(pawnData) 
                        : pawnData.combatData.damage;
                    float effectiveFireRate = statCalculator != null 
                        ? statCalculator.GetEffectiveFireRate(pawnData) 
                        : pawnData.combatData.fireRate;
                    
                    totalDamage += effectiveDamage;
                    totalFireRate += effectiveFireRate;
                }
                
                // 레벨 합산
                var levelUpManager = pawnObj.GetComponent<LevelUpSubManager>();
                if (levelUpManager != null)
                {
                    totalLevel += levelUpManager.GetCurrentLevel();
                }
            }
            
            if (validPawnCount == 0) return;
            
            // 기본 통계 업데이트
            if (_basicStatRows.Count > 0)
            {
                // 현재 레벨 (평균)
                if (_basicStatRows.Count > 0)
                {
                    int avgLevel = validPawnCount > 0 ? totalLevel / validPawnCount : 0;
                    _basicStatRows[0].valueText.text = avgLevel.ToString();
                }
                
                // 최대 HP (합산)
                if (_basicStatRows.Count > 1)
                {
                    _basicStatRows[1].valueText.text = totalMaxHP.ToString("F0");
                }
                
                // HP 재생 (아직 구현 안 됨)
                if (_basicStatRows.Count > 2)
                {
                    _basicStatRows[2].valueText.text = "U";
                }
                
                // % 생명 훔침 (아직 구현 안 됨)
                if (_basicStatRows.Count > 3)
                {
                    _basicStatRows[3].valueText.text = "U";
                }
                
                // % 대미지 (아직 구현 안 됨)
                if (_basicStatRows.Count > 4)
                {
                    _basicStatRows[4].valueText.text = "U";
                }
                
                // 근거리 대미지 (아직 구현 안 됨)
                if (_basicStatRows.Count > 5)
                {
                    _basicStatRows[5].valueText.text = "U";
                }
                
                // 원거리 대미지 (합산)
                if (_basicStatRows.Count > 6)
                {
                    _basicStatRows[6].valueText.text = totalDamage.ToString("F1");
                }
                
                // 원소 대미지 (아직 구현 안 됨)
                if (_basicStatRows.Count > 7)
                {
                    _basicStatRows[7].valueText.text = "U";
                }
                
                // % 공격 속도 (평균)
                if (_basicStatRows.Count > 8)
                {
                    float avgFireRate = validPawnCount > 0 ? totalFireRate / validPawnCount : 0f;
                    _basicStatRows[8].valueText.text = avgFireRate.ToString("F1");
                }
                
                // % 치명타율 (아직 구현 안 됨)
                if (_basicStatRows.Count > 9)
                {
                    _basicStatRows[9].valueText.text = "U";
                }
            }
            
            // 2차 통계는 아직 구현 안 됨
            foreach (var row in _secondaryStatRows)
            {
                row.valueText.text = "U";
            }
        }
        
        private void UpdatePawnsInventory()
        {
            if (_pawnsTitleText == null || GameManager.Instance?.PlayerController == null) return;
            
            var playerPawns = GameManager.Instance.PlayerController.playerPawns;
            int currentCount = playerPawns != null ? playerPawns.Count : 0;
            int maxCount = 6;
            
            _pawnsTitleText.text = $"Pawn ({currentCount}/{maxCount})";
        }

        // 이벤트 핸들러들
        private void OnResetButtonClicked()
        {
            if (GameManager.Instance?.ShopUseCase == null) return;
            
            // 잠금되지 않은 슬롯 개수 계산 및 잠긴 아이템 ID 수집
            int unlockedSlotCount = 0;
            var lockedItemIds = new List<string>();
            
            for (int i = 0; i < _itemSlots.Count; i++)
            {
                if (!_itemSlots[i].isLocked)
                {
                    unlockedSlotCount++;
                }
                else if (_itemSlots[i].itemData != null && !string.IsNullOrEmpty(_itemSlots[i].itemData.itemId))
                {
                    // 잠긴 슬롯의 아이템 ID 수집 (중복 방지)
                    if (!lockedItemIds.Contains(_itemSlots[i].itemData.itemId))
                    {
                        lockedItemIds.Add(_itemSlots[i].itemData.itemId);
                    }
                }
            }
            
            // ShopUseCase를 통해 리롤 (골드 차감 및 새 아이템 가져오기, 잠긴 아이템 제외)
            var newItems = GameManager.Instance.ShopUseCase.RerollShop(
                _resetCost, 
                unlockedSlotCount, 
                excludeOwned: false,
                excludeItemIds: lockedItemIds.Count > 0 ? lockedItemIds : null
            );
            
            if (newItems != null)
            {
                LogManager.LogInfo(LogCategory.UI, $"상점 초기화 (비용: {_resetCost})");
                
                // 잠금되지 않은 슬롯에 새 아이템 할당
                int itemIndex = 0;
                for (int i = 0; i < _itemSlots.Count && itemIndex < newItems.Count; i++)
                {
                    if (!_itemSlots[i].isLocked)
                    {
                        var slot = _itemSlots[i];
                        var item = newItems[itemIndex];
                        
                        // 아이템 데이터 저장
                        slot.itemData = item;
                        slot.cost = item.cost;
                        slot.isPurchased = false; // 리롤 시 구매 상태 초기화
                        
                        // UI 업데이트
                        if (slot.nameText != null)
                        {
                            slot.nameText.text = item.itemName ?? "알 수 없음";
                        }
                        
                        if (slot.typeText != null)
                        {
                            slot.typeText.text = item.itemType == ItemType.Global ? "전역" : "장착";
                        }
                        
                        if (slot.descriptionText != null)
                        {
                            slot.descriptionText.text = item.description ?? "";
                        }
                        
                        if (slot.costText != null)
                        {
                            slot.costText.text = item.cost.ToString();
                        }
                        
                        // 버튼 활성화/비활성화 (골드에 따라)
                        UpdateSlotButtonState(slot);
                        
                        itemIndex++;
                    }
                }
                
                // 골드 표시 업데이트
                UpdateGoldDisplay();
            }
            else
            {
                LogManager.LogWarning(LogCategory.UI, "골드가 부족합니다.");
            }
        }

        private void OnItemBuyClicked(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _itemSlots.Count) return;
            
            var slot = _itemSlots[slotIndex];
            if (slot.itemData == null)
            {
                LogManager.LogWarning(LogCategory.UI, $"슬롯 {slotIndex}에 아이템이 없습니다.");
                return;
            }
            
            if (GameManager.Instance?.ItemManagementUseCase == null)
            {
                LogManager.LogError(LogCategory.UI, "ItemManagementUseCase가 없습니다.");
                return;
            }
            
            // UseCase를 통해 구매 가능 여부 확인 (골드 체크 포함)
            if (!GameManager.Instance.ItemManagementUseCase.CanBuyItem(slot.itemData))
            {
                LogManager.LogWarning(LogCategory.UI, $"골드가 부족하거나 아이템이 유효하지 않습니다.");
                return;
            }
            
            // 단일장착 아이템인 경우 Pawn 선택 UI 표시
            if (slot.itemData.itemType == ItemType.Equipped)
            {
                // Pawn 목록 확인
                if (GameManager.Instance?.PlayerController == null || 
                    GameManager.Instance.PlayerController.playerPawns == null ||
                    GameManager.Instance.PlayerController.playerPawns.Count == 0)
                {
                    LogManager.LogWarning(LogCategory.UI, "장착할 Pawn이 없습니다.");
                    return;
                }
                
                // 구매 대기 중인 아이템 저장
                _pendingItemPurchase = slot.itemData;
                
                // Pawn 선택 UI 표시
                ShowPawnSelectionUI();
            }
            else
            {
                // 전역 아이템은 바로 구매
                BuyItemDirectly(slot.itemData, slot);
            }
        }
        
        /// <summary>
        /// 아이템을 바로 구매합니다 (전역 아이템용).
        /// </summary>
        private void BuyItemDirectly(ItemData itemData, ShopItemSlot slot)
        {
            bool success = GameManager.Instance.ItemManagementUseCase.BuyItem(itemData);
            
            if (success)
            {
                LogManager.LogInfo(LogCategory.UI, $"아이템 구매 성공: {itemData.itemName} (비용: {slot.cost})");
                
                // 구매한 슬롯 표시 및 잠금 해제
                slot.isPurchased = true;
                slot.isLocked = false; // 구매 시 잠금 해제
                if (slot.lockButtonText != null)
                {
                    slot.lockButtonText.text = "E 잠금";
                }
                UpdateSlotButtonState(slot);
                
                // 구매한 슬롯은 새 아이템으로 교체하지 않음 (리롤 전까지 유지)
                // 잠금되지 않은 다른 슬롯만 새로고침
                // RefreshShopItems(); // 구매한 슬롯은 제외되므로 호출해도 됨
                
                // 골드 표시 업데이트
                UpdateGoldDisplay();
            }
            else
            {
                LogManager.LogWarning(LogCategory.UI, "아이템 구매 실패: 골드가 부족하거나 아이템이 유효하지 않습니다.");
            }
        }

        private void OnItemLockClicked(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _itemSlots.Count) return;
            
            var slot = _itemSlots[slotIndex];
            
            // 구매된 아이템은 잠글 수 없음
            if (slot.isPurchased)
            {
                LogManager.LogWarning(LogCategory.UI, "구매된 아이템은 잠글 수 없습니다.");
                return;
            }
            
            slot.isLocked = !slot.isLocked;
            
            if (slot.isLocked)
            {
                slot.lockButtonText.text = "E 잠금 해제";
                LogManager.LogInfo(LogCategory.UI, $"슬롯 {slotIndex} 잠금됨");
            }
            else
            {
                slot.lockButtonText.text = "E 잠금";
                LogManager.LogInfo(LogCategory.UI, $"슬롯 {slotIndex} 잠금 해제됨");
            }
        }

        private void OnBasicTabToggled(bool isOn)
        {
            if (_basicStatsContent != null)
            {
                _basicStatsContent.SetActive(isOn);
            }
        }

        private void OnSecondaryTabToggled(bool isOn)
        {
            if (_secondaryStatsContent != null)
            {
                _secondaryStatsContent.SetActive(isOn);
            }
        }

        private void OnNextWaveButtonClicked()
        {
            if (GameManager.Instance != null && GameManager.Instance.StageManagementUseCase != null)
            {
                // 다음 스테이지 이름 가져오기
                string nextStageName = GameManager.Instance.StageManagementUseCase.GetNextStageName();
                
                if (string.IsNullOrEmpty(nextStageName))
                {
                    // 마지막 스테이지면 메인 메뉴로 돌아가기 (또는 게임 종료 처리)
                    LogManager.LogInfo(LogCategory.Stage, "모든 스테이지를 완료했습니다!");
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ReturnToMainMenu();
                    }
                    return;
                }
                
                // 다음 스테이지로 이동
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowStageScreen();
                }
                
                GameManager.Instance.StartStage(nextStageName, resetSession: false);
            }
        }
        
        #region Pawn Selection UI
        
        /// <summary>
        /// Pawn 선택 UI를 표시합니다.
        /// </summary>
        private void ShowPawnSelectionUI()
        {
            if (_pendingItemPurchase == null)
            {
                LogManager.LogWarning(LogCategory.UI, "구매 대기 중인 아이템이 없습니다.");
                return;
            }
            
            if (GameManager.Instance?.PlayerController == null ||
                GameManager.Instance.PlayerController.playerPawns == null ||
                GameManager.Instance.PlayerController.playerPawns.Count == 0)
            {
                LogManager.LogWarning(LogCategory.UI, "선택할 Pawn이 없습니다.");
                return;
            }
            
            EnsurePawnSelectionPanel();
            if (_pawnSelectionPanel == null) return;
            
            _pawnSelectionPanel.SetActive(true);
        }
        
        /// <summary>
        /// Pawn 선택 패널을 생성합니다.
        /// </summary>
        private void EnsurePawnSelectionPanel()
        {
            if (_pawnSelectionPanel != null) return;
            
            if (_canvas == null)
            {
                LogManager.LogError(LogCategory.UI, "Canvas가 없어 Pawn 선택 UI를 생성할 수 없습니다.");
                return;
            }
            
            // 패널 생성
            _pawnSelectionPanel = new GameObject("PawnSelectionPanel");
            var panelRect = _pawnSelectionPanel.AddComponent<RectTransform>();
            _pawnSelectionPanel.transform.SetParent(_canvas.transform, false);
            
            // 전체 화면 덮기
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.anchoredPosition = Vector2.zero;
            
            // 배경 (반투명 검은색)
            var bg = _pawnSelectionPanel.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.7f);
            
            // 중앙 컨테이너
            var containerObj = new GameObject("Container");
            var containerRect = containerObj.AddComponent<RectTransform>();
            containerObj.transform.SetParent(_pawnSelectionPanel.transform, false);
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.pivot = new Vector2(0.5f, 0.5f);
            containerRect.anchoredPosition = Vector2.zero;
            containerRect.sizeDelta = new Vector2(600f, 400f);
            
            var containerBg = containerObj.AddComponent<Image>();
            containerBg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            // 제목
            var titleObj = new GameObject("Title");
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleObj.transform.SetParent(containerObj.transform, false);
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0f, -20f);
            titleRect.sizeDelta = new Vector2(-40f, 60f);
            
            var titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "장착할 Pawn을 선택하세요";
            titleText.fontSize = 28;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            if (_koreanFont != null) titleText.font = _koreanFont;
            
            // Pawn 버튼 컨테이너
            var buttonsContainerObj = new GameObject("ButtonsContainer");
            var buttonsContainerRect = buttonsContainerObj.AddComponent<RectTransform>();
            buttonsContainerObj.transform.SetParent(containerObj.transform, false);
            buttonsContainerRect.anchorMin = new Vector2(0f, 0f);
            buttonsContainerRect.anchorMax = new Vector2(1f, 1f);
            buttonsContainerRect.pivot = new Vector2(0.5f, 0.5f);
            buttonsContainerRect.anchoredPosition = new Vector2(0f, -40f);
            buttonsContainerRect.sizeDelta = new Vector2(-40f, -100f);
            
            var verticalLayout = buttonsContainerObj.AddComponent<VerticalLayoutGroup>();
            verticalLayout.spacing = 15f;
            verticalLayout.padding = new RectOffset(20, 20, 20, 20);
            verticalLayout.childControlWidth = true;
            verticalLayout.childControlHeight = false;
            verticalLayout.childForceExpandWidth = true;
            verticalLayout.childForceExpandHeight = false;
            
            // Pawn 버튼 생성
            var playerPawns = GameManager.Instance.PlayerController.playerPawns;
            for (int i = 0; i < playerPawns.Count; i++)
            {
                var pawn = playerPawns[i];
                if (pawn == null) continue;
                
                var pawnManager = pawn.GetComponent<PawnManager>();
                if (pawnManager == null || pawnManager.PawnData == null) continue;
                
                int playerIndex = pawnManager.PawnData.playerIndex;
                string pawnName = !string.IsNullOrEmpty(pawnManager.PawnData.recipeName) 
                    ? pawnManager.PawnData.recipeName 
                    : pawn.name;
                
                // 버튼 생성
                var buttonObj = new GameObject($"PawnButton_{i}");
                var buttonRect = buttonObj.AddComponent<RectTransform>();
                buttonObj.transform.SetParent(buttonsContainerObj.transform, false);
                buttonRect.sizeDelta = new Vector2(0f, 60f);
                
                var button = buttonObj.AddComponent<Button>();
                var buttonBg = buttonObj.AddComponent<Image>();
                buttonBg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
                button.targetGraphic = buttonBg;
                
                // 버튼 텍스트
                var buttonTextObj = new GameObject("Text");
                var buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
                buttonTextObj.transform.SetParent(buttonObj.transform, false);
                buttonTextRect.anchorMin = Vector2.zero;
                buttonTextRect.anchorMax = Vector2.one;
                buttonTextRect.sizeDelta = Vector2.zero;
                buttonTextRect.anchoredPosition = Vector2.zero;
                
                var buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
                buttonText.text = pawnName;
                buttonText.fontSize = 24;
                buttonText.color = Color.white;
                buttonText.alignment = TextAlignmentOptions.Center;
                buttonText.raycastTarget = false;
                if (_koreanFont != null) buttonText.font = _koreanFont;
                
                // 클릭 이벤트
                int capturedIndex = playerIndex; // 클로저 캡처 문제 해결
                button.onClick.AddListener(() => OnPawnSelected(capturedIndex));
            }
            
            // 취소 버튼
            var cancelButtonObj = new GameObject("CancelButton");
            var cancelButtonRect = cancelButtonObj.AddComponent<RectTransform>();
            cancelButtonObj.transform.SetParent(containerObj.transform, false);
            cancelButtonRect.anchorMin = new Vector2(0.5f, 0f);
            cancelButtonRect.anchorMax = new Vector2(0.5f, 0f);
            cancelButtonRect.pivot = new Vector2(0.5f, 0f);
            cancelButtonRect.anchoredPosition = new Vector2(0f, 20f);
            cancelButtonRect.sizeDelta = new Vector2(200f, 50f);
            
            var cancelButton = cancelButtonObj.AddComponent<Button>();
            var cancelBg = cancelButtonObj.AddComponent<Image>();
            cancelBg.color = new Color(0.5f, 0.2f, 0.2f, 1f);
            cancelButton.targetGraphic = cancelBg;
            
            var cancelTextObj = new GameObject("Text");
            var cancelTextRect = cancelTextObj.AddComponent<RectTransform>();
            cancelTextObj.transform.SetParent(cancelButtonObj.transform, false);
            cancelTextRect.anchorMin = Vector2.zero;
            cancelTextRect.anchorMax = Vector2.one;
            cancelTextRect.sizeDelta = Vector2.zero;
            cancelTextRect.anchoredPosition = Vector2.zero;
            
            var cancelText = cancelTextObj.AddComponent<TextMeshProUGUI>();
            cancelText.text = "취소";
            cancelText.fontSize = 20;
            cancelText.color = Color.white;
            cancelText.alignment = TextAlignmentOptions.Center;
            cancelText.raycastTarget = false;
            if (_koreanFont != null) cancelText.font = _koreanFont;
            
            cancelButton.onClick.AddListener(() => OnPawnSelectionCancelled());
            
            // 초기에는 숨김
            _pawnSelectionPanel.SetActive(false);
        }
        
        /// <summary>
        /// Pawn이 선택되었을 때 호출됩니다.
        /// </summary>
        private void OnPawnSelected(int playerIndex)
        {
            if (_pendingItemPurchase == null)
            {
                LogManager.LogWarning(LogCategory.UI, "구매 대기 중인 아이템이 없습니다.");
                HidePawnSelectionUI();
                return;
            }
            
            if (GameManager.Instance?.ItemManagementUseCase == null)
            {
                LogManager.LogError(LogCategory.UI, "ItemManagementUseCase가 없습니다.");
                HidePawnSelectionUI();
                return;
            }
            
            // 원자적 작업: 구매 및 장착 (UseCase 레벨에서 처리)
            bool success = GameManager.Instance.ItemManagementUseCase.BuyAndEquipItem(
                _pendingItemPurchase, 
                playerIndex
            );
            
            if (success)
            {
                LogManager.LogInfo(LogCategory.UI, 
                    $"아이템 구매 및 장착 성공: {_pendingItemPurchase.itemName} → Pawn {playerIndex}");
                
                // 구매한 슬롯 찾아서 구매 완료 표시 및 잠금 해제
                foreach (var slot in _itemSlots)
                {
                    if (slot.itemData != null && slot.itemData.itemId == _pendingItemPurchase.itemId)
                    {
                        slot.isPurchased = true;
                        slot.isLocked = false; // 구매 시 잠금 해제
                        if (slot.lockButtonText != null)
                        {
                            slot.lockButtonText.text = "E 잠금";
                        }
                        UpdateSlotButtonState(slot);
                        break;
                    }
                }
                
                // 골드 표시 업데이트
                UpdateGoldDisplay();
            }
            else
            {
                LogManager.LogWarning(LogCategory.UI, 
                    $"아이템 구매 및 장착 실패: {_pendingItemPurchase.itemName} → Pawn {playerIndex} " +
                    "(골드 부족, 아이템 유효하지 않음, 또는 장착 실패)");
            }
            
            HidePawnSelectionUI();
            _pendingItemPurchase = null;
        }
        
        /// <summary>
        /// Pawn 선택이 취소되었을 때 호출됩니다.
        /// </summary>
        private void OnPawnSelectionCancelled()
        {
            HidePawnSelectionUI();
            _pendingItemPurchase = null;
        }
        
        /// <summary>
        /// Pawn 선택 UI를 숨깁니다.
        /// </summary>
        private void HidePawnSelectionUI()
        {
            if (_pawnSelectionPanel != null)
            {
                _pawnSelectionPanel.SetActive(false);
            }
        }
        
        #endregion
    }
}


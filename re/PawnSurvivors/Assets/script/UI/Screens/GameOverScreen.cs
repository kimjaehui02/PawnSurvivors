using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using PawnSurvivors.Managers;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 게임 오버 화면입니다. 동적으로 UI를 생성합니다.
    /// </summary>
    public class GameOverScreen : MonoBehaviour
    {
        private Canvas _canvas;
        private TMP_FontAsset _font;
        private bool _isUICreated = false;
        
        // UI 요소들
        private TMP_Text _titleText;
        private TMP_Text _roundText;
        private TMP_Text _charactersText;
        private TMP_Text _itemsText;
        private Button _retryButton;
        private Button _characterSelectButton;

        private void Awake()
        {
            // UI가 이미 생성되었는지 확인 (GameObject가 재사용될 수 있음)
            if (!_isUICreated)
            {
                // UI 요소들이 이미 존재하는지 확인
                if (_titleText == null && transform.Find("MainPanel/TitleText") == null)
                {
                    LoadFont();
                    CreateUI();
                }
                else
                {
                    // UI 요소들이 이미 있으면 참조만 복원
                    RestoreUIReferences();
                }
                _isUICreated = true;
            }
        }
        
        private void Start()
        {
            // 씬이 로드되면 자동으로 게임오버 화면 표시
            // 씬은 이전 씬과 독립적으로 작동합니다
            InitializeGameOver();
        }
        
        /// <summary>
        /// 씬이 로드되면 게임오버 초기화를 수행합니다.
        /// </summary>
        private void InitializeGameOver()
        {
            // 게임 일시정지
            if (GameManager.Instance?.LifecycleManager != null && 
                !GameManager.Instance.LifecycleManager.IsPaused)
            {
                GameManager.Instance.LifecycleManager.TogglePause();
            }
            
            // 게임오버 시 캐릭터 선택 상태 저장
            if (GameManager.Instance?.CharacterSelectionUseCase != null)
            {
                GameManager.Instance.CharacterSelectionUseCase.SaveSelectedCharacters();
            }
            
            // 게임오버 화면 표시
            Show();
        }
        
        private void RestoreUIReferences()
        {
            // UI 요소 참조 복원
            Transform mainPanel = transform.Find("MainPanel");
            if (mainPanel != null)
            {
                Transform titleObj = mainPanel.Find("TitleText");
                if (titleObj != null) _titleText = titleObj.GetComponent<TMP_Text>();
                
                Transform roundObj = mainPanel.Find("RoundText");
                if (roundObj != null) _roundText = roundObj.GetComponent<TMP_Text>();
                
                Transform charsObj = mainPanel.Find("CharactersText");
                if (charsObj != null) _charactersText = charsObj.GetComponent<TMP_Text>();
                
                Transform itemsObj = mainPanel.Find("ItemsText");
                if (itemsObj != null) _itemsText = itemsObj.GetComponent<TMP_Text>();
                
                Transform retryObj = mainPanel.Find("RetryButton");
                if (retryObj != null)
                {
                    _retryButton = retryObj.GetComponent<Button>();
                    // onClick 리스너 재설정 (프리팹에서 복원된 버튼의 경우)
                    if (_retryButton != null)
                    {
                        _retryButton.onClick.RemoveAllListeners();
                        _retryButton.onClick.AddListener(OnRetryButtonClicked);
                    }
                }
                
                Transform charSelectObj = mainPanel.Find("CharacterSelectButton");
                if (charSelectObj != null)
                {
                    _characterSelectButton = charSelectObj.GetComponent<Button>();
                    // onClick 리스너 재설정 (프리팹에서 복원된 버튼의 경우)
                    if (_characterSelectButton != null)
                    {
                        _characterSelectButton.onClick.RemoveAllListeners();
                        _characterSelectButton.onClick.AddListener(OnCharacterSelectButtonClicked);
                    }
                }
            }
            
            _canvas = GetComponent<Canvas>();
        }

        private void LoadFont()
        {
            _font = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
        }

        private void CreateUI()
        {
            // Canvas가 이미 있으면 재사용
            _canvas = gameObject.GetComponent<Canvas>();
            if (_canvas == null)
            {
                _canvas = gameObject.AddComponent<Canvas>();
            }
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 200; // 다른 UI보다 위
            
            // GraphicRaycaster도 중복 추가 방지
            if (gameObject.GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }
            
            // CanvasScaler도 중복 추가 방지
            CanvasScaler scaler = gameObject.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = gameObject.AddComponent<CanvasScaler>();
            }
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            // EventSystem 확인
            if (EventSystem.current == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }

            // 배경 패널
            GameObject backgroundPanel = new GameObject("BackgroundPanel");
            backgroundPanel.transform.SetParent(transform, false);
            Image bgImage = backgroundPanel.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.8f);
            
            RectTransform bgRect = backgroundPanel.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            // 메인 패널
            GameObject mainPanel = new GameObject("MainPanel");
            mainPanel.transform.SetParent(transform, false);
            RectTransform mainRect = mainPanel.AddComponent<RectTransform>();
            mainRect.anchorMin = new Vector2(0.5f, 0.5f);
            mainRect.anchorMax = new Vector2(0.5f, 0.5f);
            mainRect.sizeDelta = new Vector2(800, 900);
            mainRect.anchoredPosition = Vector2.zero;

            Image panelImage = mainPanel.AddComponent<Image>();
            panelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);

            // 제목
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(mainPanel.transform, false);
            _titleText = titleObj.AddComponent<TextMeshProUGUI>();
            _titleText.text = "GAME OVER";
            _titleText.fontSize = 64;
            _titleText.color = Color.red;
            _titleText.alignment = TextAlignmentOptions.Center;
            if (_font != null) _titleText.font = _font;
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.sizeDelta = new Vector2(700, 100);
            titleRect.anchoredPosition = new Vector2(0, -50);

            // 라운드 정보
            GameObject roundObj = new GameObject("RoundText");
            roundObj.transform.SetParent(mainPanel.transform, false);
            _roundText = roundObj.AddComponent<TextMeshProUGUI>();
            _roundText.fontSize = 32;
            _roundText.color = Color.white;
            _roundText.alignment = TextAlignmentOptions.Left;
            if (_font != null) _roundText.font = _font;
            
            RectTransform roundRect = roundObj.GetComponent<RectTransform>();
            roundRect.anchorMin = new Vector2(0.5f, 1f);
            roundRect.anchorMax = new Vector2(0.5f, 1f);
            roundRect.sizeDelta = new Vector2(700, 40);
            roundRect.anchoredPosition = new Vector2(0, -180);

            // 캐릭터 정보
            GameObject charactersObj = new GameObject("CharactersText");
            charactersObj.transform.SetParent(mainPanel.transform, false);
            _charactersText = charactersObj.AddComponent<TextMeshProUGUI>();
            _charactersText.fontSize = 28;
            _charactersText.color = Color.white;
            _charactersText.alignment = TextAlignmentOptions.Left;
            if (_font != null) _charactersText.font = _font;
            
            RectTransform charactersRect = charactersObj.GetComponent<RectTransform>();
            charactersRect.anchorMin = new Vector2(0.5f, 1f);
            charactersRect.anchorMax = new Vector2(0.5f, 1f);
            charactersRect.sizeDelta = new Vector2(700, 200);
            charactersRect.anchoredPosition = new Vector2(0, -280);

            // 아이템 정보
            GameObject itemsObj = new GameObject("ItemsText");
            itemsObj.transform.SetParent(mainPanel.transform, false);
            _itemsText = itemsObj.AddComponent<TextMeshProUGUI>();
            _itemsText.fontSize = 28;
            _itemsText.color = Color.white;
            _itemsText.alignment = TextAlignmentOptions.Left;
            if (_font != null) _itemsText.font = _font;
            
            RectTransform itemsRect = itemsObj.GetComponent<RectTransform>();
            itemsRect.anchorMin = new Vector2(0.5f, 1f);
            itemsRect.anchorMax = new Vector2(0.5f, 1f);
            itemsRect.sizeDelta = new Vector2(700, 300);
            itemsRect.anchoredPosition = new Vector2(0, -520);

            // 재시작 버튼
            GameObject retryObj = new GameObject("RetryButton");
            retryObj.transform.SetParent(mainPanel.transform, false);
            _retryButton = retryObj.AddComponent<Button>();
            Image retryImage = retryObj.AddComponent<Image>();
            retryImage.color = new Color(0.3f, 0.6f, 0.3f);
            
            RectTransform retryRect = retryObj.GetComponent<RectTransform>();
            retryRect.anchorMin = new Vector2(0.5f, 0f);
            retryRect.anchorMax = new Vector2(0.5f, 0f);
            retryRect.sizeDelta = new Vector2(300, 60);
            retryRect.anchoredPosition = new Vector2(-160, 30);

            GameObject retryTextObj = new GameObject("Text");
            retryTextObj.transform.SetParent(retryObj.transform, false);
            TextMeshProUGUI retryText = retryTextObj.AddComponent<TextMeshProUGUI>();
            retryText.text = "재시작";
            retryText.fontSize = 32;
            retryText.color = Color.white;
            retryText.alignment = TextAlignmentOptions.Center;
            if (_font != null) retryText.font = _font;
            
            RectTransform retryTextRect = retryTextObj.GetComponent<RectTransform>();
            retryTextRect.anchorMin = Vector2.zero;
            retryTextRect.anchorMax = Vector2.one;
            retryTextRect.sizeDelta = Vector2.zero;

            _retryButton.onClick.AddListener(OnRetryButtonClicked);

            // 캐릭터 선택 버튼
            GameObject charSelectObj = new GameObject("CharacterSelectButton");
            charSelectObj.transform.SetParent(mainPanel.transform, false);
            _characterSelectButton = charSelectObj.AddComponent<Button>();
            Image charSelectImage = charSelectObj.AddComponent<Image>();
            charSelectImage.color = new Color(0.3f, 0.3f, 0.6f);
            
            RectTransform charSelectRect = charSelectObj.GetComponent<RectTransform>();
            charSelectRect.anchorMin = new Vector2(0.5f, 0f);
            charSelectRect.anchorMax = new Vector2(0.5f, 0f);
            charSelectRect.sizeDelta = new Vector2(300, 60);
            charSelectRect.anchoredPosition = new Vector2(160, 30);

            GameObject charSelectTextObj = new GameObject("Text");
            charSelectTextObj.transform.SetParent(charSelectObj.transform, false);
            TextMeshProUGUI charSelectText = charSelectTextObj.AddComponent<TextMeshProUGUI>();
            charSelectText.text = "캐릭터 선택";
            charSelectText.fontSize = 32;
            charSelectText.color = Color.white;
            charSelectText.alignment = TextAlignmentOptions.Center;
            if (_font != null) charSelectText.font = _font;
            
            RectTransform charSelectTextRect = charSelectTextObj.GetComponent<RectTransform>();
            charSelectTextRect.anchorMin = Vector2.zero;
            charSelectTextRect.anchorMax = Vector2.one;
            charSelectTextRect.sizeDelta = Vector2.zero;

            _characterSelectButton.onClick.AddListener(OnCharacterSelectButtonClicked);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f;

            // 정보 수집 및 표시
            UpdateGameOverInfo();
        }

        private void UpdateGameOverInfo()
        {
            // UI 요소들이 초기화되지 않았으면 복원 시도
            if (_roundText == null || _charactersText == null || _itemsText == null)
            {
                RestoreUIReferences();
            }
            
            // 라운드 정보
            string roundInfo = "라운드: ";
            if (GameManager.Instance?.StageFlowUseCase != null)
            {
                string stageName = GameManager.Instance.StageFlowUseCase.GetCurrentStageName();
                roundInfo += stageName;
            }
            else
            {
                roundInfo += "알 수 없음";
            }
            if (_roundText != null)
            {
                _roundText.text = roundInfo;
            }

            // 캐릭터 정보
            string charactersInfo = "캐릭터:\n";
            if (GameManager.Instance?.CharacterSelectionUseCase != null)
            {
                var selectedCharacters = GameManager.Instance.CharacterSelectionUseCase.GetSelectedCharacters();
                if (selectedCharacters.Count > 0)
                {
                    charactersInfo += string.Join(", ", selectedCharacters);
                }
                else
                {
                    charactersInfo += "없음";
                }
            }
            else
            {
                charactersInfo += "알 수 없음";
            }
            if (_charactersText != null)
            {
                _charactersText.text = charactersInfo;
            }

            // 아이템 정보
            string itemsInfo = "아이템:\n";
            if (GameManager.Instance?.ItemManagementUseCase != null)
            {
                var allItems = new List<PawnSurvivors.Data.ItemData>();
                
                // 전역 아이템 수집
                var globalItems = GameManager.Instance.ItemManagementUseCase.GetGlobalItems();
                allItems.AddRange(globalItems);
                
                // 장착 아이템도 수집
                if (GameManager.Instance?.PlayerController?.playerPawns != null)
                {
                    for (int i = 0; i < GameManager.Instance.PlayerController.playerPawns.Count; i++)
                    {
                        var equippedItems = GameManager.Instance.ItemManagementUseCase.GetEquippedItems(i);
                        allItems.AddRange(equippedItems);
                    }
                }
                
                if (allItems.Count > 0)
                {
                    // 아이템 그룹화 (중복 제거 및 개수 표시)
                    var itemGroups = allItems.GroupBy(item => item.itemId);
                    var itemList = new List<string>();
                    foreach (var group in itemGroups)
                    {
                        int count = group.Count();
                        string itemName = group.First().itemName ?? group.Key;
                        if (count > 1)
                        {
                            itemList.Add($"{itemName} x{count}");
                        }
                        else
                        {
                            itemList.Add(itemName);
                        }
                    }
                    itemsInfo += string.Join(", ", itemList);
                }
                else
                {
                    itemsInfo += "없음";
                }
            }
            else
            {
                itemsInfo += "알 수 없음";
            }
            if (_itemsText != null)
            {
                _itemsText.text = itemsInfo;
            }
        }

        private void OnRetryButtonClicked()
        {
            Time.timeScale = 1f;
            
            // 캐릭터 선택 상태 복원
            if (GameManager.Instance?.CharacterSelectionUseCase != null)
            {
                GameManager.Instance.CharacterSelectionUseCase.RestoreFromSaved();
            }
            
            // 재시작: StageState로 전환 (StageState.OnEnter()에서 RestartStage() 호출)
            // State 전환을 먼저 하고, State의 OnEnter()에서 스테이지 재시작 처리
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.GoToStage();
            }
            
            gameObject.SetActive(false);
            LogManager.LogInfo(LogCategory.UI, "Retry Button Clicked");
        }

        private void OnCharacterSelectButtonClicked()
        {
            Time.timeScale = 1f;
            
            // GameStateManager를 통해 CharacterSelectState로 전환
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.GoToCharacterSelect();
            }
            
            gameObject.SetActive(false);
            LogManager.LogInfo(LogCategory.UI, "Character Select Button Clicked");
        }
    }
}

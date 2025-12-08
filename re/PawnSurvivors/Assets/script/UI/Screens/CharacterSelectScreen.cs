using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using PawnSurvivors.Managers;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Usecases;

namespace PawnSurvivors.UI
{
    public class CharacterSelectScreen : MonoBehaviour
    {
        /// <summary>
        /// UI 요소 타입 enum (하드코딩된 string 제거)
        /// </summary>
        private enum UIElementType
        {
            Background,
            CharacterList,
            ConfirmButton,
            BackButton,
            SelectedCount
        }
        
        /// <summary>
        /// UI 요소 참조를 enum으로 관리 (내부적으로도 enum 사용)
        /// </summary>
        private Dictionary<UIElementType, GameObject> _uiElements = new Dictionary<UIElementType, GameObject>();
        
        private GameObject _characterListContainer => _uiElements.ContainsKey(UIElementType.CharacterList) 
            ? _uiElements[UIElementType.CharacterList] 
            : null;
        private GameObject _confirmButton => _uiElements.ContainsKey(UIElementType.ConfirmButton) 
            ? _uiElements[UIElementType.ConfirmButton] 
            : null;
        private GameObject _backButton => _uiElements.ContainsKey(UIElementType.BackButton) 
            ? _uiElements[UIElementType.BackButton] 
            : null;
        private GameObject _selectedCountText => _uiElements.ContainsKey(UIElementType.SelectedCount) 
            ? _uiElements[UIElementType.SelectedCount] 
            : null;
        
        [Header("Character Settings")]
        [SerializeField] private int minSelectCount = 1;
        [SerializeField] private int maxSelectCount = 1;

        private CharacterSelectionUseCase _characterSelectionUseCase;
        private Dictionary<string, GameObject> _characterIcons = new Dictionary<string, GameObject>();
        private TMP_FontAsset _koreanFont;

        private void Awake()
        {
            // 프리팹에 이미 Canvas가 있으므로 추가하지 않음
            // 씬에 배치된 프리팹은 이미 UI가 생성되어 있음
            
            _koreanFont = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
            
            // UI 요소들을 자식에서 찾아서 enum으로 매핑
            InitializeUIElements();
            
            // UI가 없으면 생성
            if (!_uiElements.ContainsKey(UIElementType.Background))
            {
                CreateUI();
            }
            
            // 이벤트 리스너 재설정
            SetupEventListeners();
        }
        
        private void Start()
        {
            // 씬이 독립적으로 작동해야 하므로 Start()에서 캐릭터 로드
            // GameManager가 준비될 때까지 기다림
            LoadCharacters();
        }
        
        /// <summary>
        /// 캐릭터를 불러옵니다. 씬이 독립적으로 작동하므로 스스로 불러와야 합니다.
        /// </summary>
        private void LoadCharacters()
        {
            // GameManager와 CharacterSelectionUseCase 확인
            if (GameManager.Instance?.CharacterSelectionUseCase == null)
            {
                Debug.LogWarning("[CharacterSelectScreen] GameManager 또는 CharacterSelectionUseCase가 아직 준비되지 않았습니다. 다음 프레임에 다시 시도합니다.");
                // 다음 프레임에 다시 시도
                Invoke(nameof(LoadCharacters), 0.1f);
                return;
            }
            
            _characterSelectionUseCase = GameManager.Instance.CharacterSelectionUseCase;
            LoadCharactersFromJSON();
            CreateCharacterIcons();
        }
        
        /// <summary>
        /// 자식 Transform들을 순회하면서 UI 요소를 enum으로 매핑합니다.
        /// </summary>
        private void InitializeUIElements()
        {
            _uiElements.Clear();
            
            // 모든 자식 Transform을 순회 (재귀적으로)
            MapUIElementsRecursive(transform);
        }
        
        /// <summary>
        /// 재귀적으로 Transform을 순회하면서 UI 요소를 enum으로 매핑합니다.
        /// </summary>
        private void MapUIElementsRecursive(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                string childName = child.name;
                
                // enum 값과 이름이 일치하는지 확인
                if (System.Enum.TryParse<UIElementType>(childName, out UIElementType elementType))
                {
                    _uiElements[elementType] = child.gameObject;
                }
                
                // 자식도 재귀적으로 확인
                MapUIElementsRecursive(child);
            }
        }
        
        /// <summary>
        /// UI 요소들의 이벤트 리스너를 설정합니다.
        /// </summary>
        private void SetupEventListeners()
        {
            // BackButton 이벤트 리스너 재설정
            if (_backButton != null)
            {
                Button backBtn = _backButton.GetComponent<Button>();
                if (backBtn != null)
                {
                    backBtn.onClick.RemoveAllListeners();
                    backBtn.onClick.AddListener(() => { 
                        if (GameStateManager.Instance != null) 
                        {
                            GameStateManager.Instance.GoToTitle();
                        }
                    });
                }
            }
            
            // ConfirmButton 이벤트 리스너 재설정
            if (_confirmButton != null)
            {
                Button confirmBtn = _confirmButton.GetComponent<Button>();
                if (confirmBtn != null)
                {
                    confirmBtn.onClick.RemoveAllListeners();
                    confirmBtn.onClick.AddListener(OnConfirmClicked);
                }
            }
        }

        private void CreateUI()
        {
            GameObject bg = new GameObject(UIElementType.Background.ToString());
            _uiElements[UIElementType.Background] = bg;
            bg.transform.SetParent(transform, false);
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bg.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.95f);

            GameObject title = new GameObject("Title");
            title.transform.SetParent(bg.transform, false);
            RectTransform titleRect = title.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0f, -50f);
            titleRect.sizeDelta = new Vector2(800f, 100f);
            TMP_Text titleText = title.AddComponent<TextMeshProUGUI>();
            titleText.text = "캐릭터 선택";
            titleText.fontSize = 60;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            if (_koreanFont != null) titleText.font = _koreanFont;

            GameObject backBtnObj = new GameObject(UIElementType.BackButton.ToString());
            _uiElements[UIElementType.BackButton] = backBtnObj;
            backBtnObj.transform.SetParent(bg.transform, false);
            RectTransform backRect = backBtnObj.AddComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0f, 1f);
            backRect.anchorMax = new Vector2(0f, 1f);
            backRect.pivot = new Vector2(0f, 1f);
            backRect.anchoredPosition = new Vector2(50f, -50f);
            backRect.sizeDelta = new Vector2(150f, 60f);
            backBtnObj.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            Button backBtn = backBtnObj.AddComponent<Button>();
            backBtn.onClick.AddListener(() => { 
                if (GameStateManager.Instance != null) 
                {
                    GameStateManager.Instance.GoToTitle();
                }
            });
            GameObject backText = new GameObject("Text");
            backText.transform.SetParent(backBtnObj.transform, false);
            RectTransform backTextRect = backText.AddComponent<RectTransform>();
            backTextRect.anchorMin = Vector2.zero;
            backTextRect.anchorMax = Vector2.one;
            backTextRect.sizeDelta = Vector2.zero;
            TMP_Text backTxt = backText.AddComponent<TextMeshProUGUI>();
            backTxt.text = "← 뒤로";
            backTxt.fontSize = 24;
            backTxt.color = Color.white;
            backTxt.alignment = TextAlignmentOptions.Center;
            if (_koreanFont != null) backTxt.font = _koreanFont;

            GameObject listContainer = new GameObject(UIElementType.CharacterList.ToString());
            _uiElements[UIElementType.CharacterList] = listContainer;
            listContainer.transform.SetParent(bg.transform, false);
            RectTransform listRect = listContainer.AddComponent<RectTransform>();
            listRect.anchorMin = new Vector2(0.5f, 0.5f);
            listRect.anchorMax = new Vector2(0.5f, 0.5f);
            listRect.pivot = new Vector2(0.5f, 0.5f);
            listRect.anchoredPosition = Vector2.zero;
            listRect.sizeDelta = new Vector2(1200f, 600f);
            GridLayoutGroup grid = listContainer.AddComponent<GridLayoutGroup>();
            // 40:48 비율 유지 (가로 125, 세로 150)
            grid.cellSize = new Vector2(125f, 150f);
            grid.spacing = new Vector2(20f, 20f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 6;

            GameObject selectedCountObj = new GameObject(UIElementType.SelectedCount.ToString());
            _uiElements[UIElementType.SelectedCount] = selectedCountObj;
            selectedCountObj.transform.SetParent(bg.transform, false);
            RectTransform countRect = selectedCountObj.AddComponent<RectTransform>();
            countRect.anchorMin = new Vector2(0.5f, 0f);
            countRect.anchorMax = new Vector2(0.5f, 0f);
            countRect.pivot = new Vector2(0.5f, 0f);
            countRect.anchoredPosition = new Vector2(0f, 150f);
            countRect.sizeDelta = new Vector2(400f, 50f);
            TMP_Text countTxt = selectedCountObj.AddComponent<TextMeshProUGUI>();
            countTxt.text = "선택: 0/1";
            countTxt.fontSize = 32;
            countTxt.color = Color.white;
            countTxt.alignment = TextAlignmentOptions.Center;
            if (_koreanFont != null) countTxt.font = _koreanFont;

            GameObject confirmBtnObj = new GameObject(UIElementType.ConfirmButton.ToString());
            _uiElements[UIElementType.ConfirmButton] = confirmBtnObj;
            confirmBtnObj.transform.SetParent(bg.transform, false);
            RectTransform confirmRect = confirmBtnObj.AddComponent<RectTransform>();
            confirmRect.anchorMin = new Vector2(0.5f, 0f);
            confirmRect.anchorMax = new Vector2(0.5f, 0f);
            confirmRect.pivot = new Vector2(0.5f, 0f);
            confirmRect.anchoredPosition = new Vector2(0f, 50f);
            confirmRect.sizeDelta = new Vector2(300f, 80f);
            confirmBtnObj.AddComponent<Image>().color = new Color(0.2f, 0.6f, 0.2f, 1f);
            Button confirmBtn = confirmBtnObj.AddComponent<Button>();
            confirmBtn.onClick.AddListener(OnConfirmClicked);
            GameObject confirmText = new GameObject("Text");
            confirmText.transform.SetParent(confirmBtnObj.transform, false);
            RectTransform confirmTextRect = confirmText.AddComponent<RectTransform>();
            confirmTextRect.anchorMin = Vector2.zero;
            confirmTextRect.anchorMax = Vector2.one;
            confirmTextRect.sizeDelta = Vector2.zero;
            TMP_Text confirmTxt = confirmText.AddComponent<TextMeshProUGUI>();
            confirmTxt.text = "시작";
            confirmTxt.fontSize = 36;
            confirmTxt.color = Color.white;
            confirmTxt.alignment = TextAlignmentOptions.Center;
            if (_koreanFont != null) confirmTxt.font = _koreanFont;
        }

        private void LoadCharactersFromJSON()
        {
            if (GameManager.Instance?.CreationManager == null) return;

            List<string> playerRecipes = GameManager.Instance.CreationManager.GetAllPlayerRecipeNames();
            _characterSelectionUseCase.SetAvailableCharacters(playerRecipes);
        }

        private void CreateCharacterIcons()
        {
            if (_characterListContainer == null || _characterSelectionUseCase == null) return;

            foreach (var character in _characterSelectionUseCase.GetAvailableCharacters())
            {
                string characterName = character.ToString();
                GameObject icon = new GameObject($"Icon_{characterName}");
                icon.transform.SetParent(_characterListContainer.transform, false);
                
                Image img = icon.AddComponent<Image>();
                img.preserveAspect = true; // 비율 유지
                string spritePath = GetSpritePath(characterName);
                Sprite sprite = Resources.Load<Sprite>(spritePath);
                if (sprite != null) img.sprite = sprite;
                else img.color = new Color(0.3f, 0.3f, 0.3f, 1f);
                
                Button btn = icon.AddComponent<Button>();
                PlayerCharacter capturedChar = character; // 클로저 캡처
                btn.onClick.AddListener(() => OnCharacterClicked(capturedChar));
                
                _characterIcons[characterName] = icon;
            }
        }

        private string GetSpritePath(string characterName)
        {
            string name = characterName.Replace("Player", "").ToLower();
            string korean = name switch
            {
                "erpin" => "에르핀",
                "butter" => "버터",
                "opal" => "오팔",
                "rufo" => "루포",
                "beni" => "베니",
                "tig" => "티그",
                "speaki" => "스피키",
                "epica" => "에피카",
                "elena" => "엘레나",
                "ui" => "우이",
                _ => name
            };
            return $"Sprites/player/{korean}";
        }

        private void OnCharacterClicked(PlayerCharacter character)
        {
            if (_characterSelectionUseCase == null) return;

            if (_characterSelectionUseCase.IsCharacterSelected(character))
            {
                _characterSelectionUseCase.DeselectCharacter(character);
            }
            else
            {
                if (_characterSelectionUseCase.GetSelectedCount() >= maxSelectCount)
                {
                    foreach (var ch in _characterSelectionUseCase.GetSelectedCharacters())
                    {
                        _characterSelectionUseCase.DeselectCharacter(ch);
                    }
                }
                _characterSelectionUseCase.SelectCharacter(character);
            }
            
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_characterSelectionUseCase == null) return;

            foreach (var kvp in _characterIcons)
            {
                bool selected = _characterSelectionUseCase.IsCharacterSelected(kvp.Key);
                Outline outline = kvp.Value.GetComponent<Outline>();
                if (selected)
                {
                    if (outline == null) outline = kvp.Value.AddComponent<Outline>();
                    outline.effectColor = Color.yellow;
                    outline.effectDistance = new Vector2(5f, 5f);
                }
                else
                {
                    if (outline != null) Destroy(outline);
                }
            }

            if (_selectedCountText != null)
            {
                TMP_Text txt = _selectedCountText.GetComponent<TMP_Text>();
                if (txt != null) txt.text = $"선택: {_characterSelectionUseCase.GetSelectedCount()}/{maxSelectCount}";
            }

            if (_confirmButton != null)
            {
                Button btn = _confirmButton.GetComponent<Button>();
                if (btn != null) btn.interactable = _characterSelectionUseCase.GetSelectedCount() >= minSelectCount;
            }
        }

        private void OnConfirmClicked()
        {
            if (_characterSelectionUseCase == null) return;

            var selected = _characterSelectionUseCase.GetSelectedCharacters();
            if (selected.Count < minSelectCount) return;

            _characterSelectionUseCase.ConfirmSelection();
            
            // 세션 데이터에 저장 (단일 소스: CharacterSelectionUseCase)
            _characterSelectionUseCase.SaveSelectedCharacters();
            
            // GameManager에도 동기화 (하위 호환성, 내부적으로 UseCase 사용)
            if (GameManager.Instance != null)
            {
                // enum 리스트를 string 리스트로 변환하여 GameManager에 설정
                // GameManager.SetSelectedCharacters()는 내부적으로 CharacterSelectionUseCase 사용
                List<string> selectedStrings = selected.Select(c => c.ToString()).ToList();
                GameManager.Instance.SetSelectedCharacters(selectedStrings);
            }

            // GameStateManager를 통해 StageSelectState로 전환
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.GoToStageSelect();
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            if (_characterSelectionUseCase != null)
            {
                _characterSelectionUseCase.ClearSelection();
            }
            UpdateUI();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

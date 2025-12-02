using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using PawnSurvivors.Managers;
using PawnSurvivors.Domain.Usecases;

namespace PawnSurvivors.UI
{
    public class CharacterSelectScreen : MonoBehaviour
    {
        [Header("Character Settings")]
        [SerializeField] private int minSelectCount = 1;
        [SerializeField] private int maxSelectCount = 1;

        private CharacterSelectionUseCase _characterSelectionUseCase;
        private Dictionary<string, GameObject> _characterIcons = new Dictionary<string, GameObject>();
        
        private GameObject _characterListContainer;
        private GameObject _confirmButton;
        private GameObject _backButton;
        private GameObject _selectedCountText;
        private TMP_FontAsset _koreanFont;

        private void Awake()
        {
            _koreanFont = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
            
            gameObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            gameObject.AddComponent<GraphicRaycaster>();
            
            CreateUI();
            
            if (GameManager.Instance?.CharacterSelectionUseCase != null)
            {
                _characterSelectionUseCase = GameManager.Instance.CharacterSelectionUseCase;
                LoadCharactersFromJSON();
            }

            CreateCharacterIcons();
        }

        private void CreateUI()
        {
            GameObject bg = new GameObject("Background");
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

            _backButton = new GameObject("BackButton");
            _backButton.transform.SetParent(bg.transform, false);
            RectTransform backRect = _backButton.AddComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0f, 1f);
            backRect.anchorMax = new Vector2(0f, 1f);
            backRect.pivot = new Vector2(0f, 1f);
            backRect.anchoredPosition = new Vector2(50f, -50f);
            backRect.sizeDelta = new Vector2(150f, 60f);
            _backButton.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            Button backBtn = _backButton.AddComponent<Button>();
            backBtn.onClick.AddListener(() => { if (UIManager.Instance != null) UIManager.Instance.ShowTitleScreen(); });
            GameObject backText = new GameObject("Text");
            backText.transform.SetParent(_backButton.transform, false);
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

            _characterListContainer = new GameObject("CharacterList");
            _characterListContainer.transform.SetParent(bg.transform, false);
            RectTransform listRect = _characterListContainer.AddComponent<RectTransform>();
            listRect.anchorMin = new Vector2(0.5f, 0.5f);
            listRect.anchorMax = new Vector2(0.5f, 0.5f);
            listRect.pivot = new Vector2(0.5f, 0.5f);
            listRect.anchoredPosition = Vector2.zero;
            listRect.sizeDelta = new Vector2(1200f, 600f);
            GridLayoutGroup grid = _characterListContainer.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(150f, 150f);
            grid.spacing = new Vector2(20f, 20f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 6;

            _selectedCountText = new GameObject("SelectedCount");
            _selectedCountText.transform.SetParent(bg.transform, false);
            RectTransform countRect = _selectedCountText.AddComponent<RectTransform>();
            countRect.anchorMin = new Vector2(0.5f, 0f);
            countRect.anchorMax = new Vector2(0.5f, 0f);
            countRect.pivot = new Vector2(0.5f, 0f);
            countRect.anchoredPosition = new Vector2(0f, 150f);
            countRect.sizeDelta = new Vector2(400f, 50f);
            TMP_Text countTxt = _selectedCountText.AddComponent<TextMeshProUGUI>();
            countTxt.text = "선택: 0/1";
            countTxt.fontSize = 32;
            countTxt.color = Color.white;
            countTxt.alignment = TextAlignmentOptions.Center;
            if (_koreanFont != null) countTxt.font = _koreanFont;

            _confirmButton = new GameObject("ConfirmButton");
            _confirmButton.transform.SetParent(bg.transform, false);
            RectTransform confirmRect = _confirmButton.AddComponent<RectTransform>();
            confirmRect.anchorMin = new Vector2(0.5f, 0f);
            confirmRect.anchorMax = new Vector2(0.5f, 0f);
            confirmRect.pivot = new Vector2(0.5f, 0f);
            confirmRect.anchoredPosition = new Vector2(0f, 50f);
            confirmRect.sizeDelta = new Vector2(300f, 80f);
            _confirmButton.AddComponent<Image>().color = new Color(0.2f, 0.6f, 0.2f, 1f);
            Button confirmBtn = _confirmButton.AddComponent<Button>();
            confirmBtn.onClick.AddListener(OnConfirmClicked);
            GameObject confirmText = new GameObject("Text");
            confirmText.transform.SetParent(_confirmButton.transform, false);
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

            foreach (string characterName in _characterSelectionUseCase.GetAvailableCharacters())
            {
                GameObject icon = new GameObject($"Icon_{characterName}");
                icon.transform.SetParent(_characterListContainer.transform, false);
                
                Image img = icon.AddComponent<Image>();
                string spritePath = GetSpritePath(characterName);
                Sprite sprite = Resources.Load<Sprite>(spritePath);
                if (sprite != null) img.sprite = sprite;
                else img.color = new Color(0.3f, 0.3f, 0.3f, 1f);
                
                Button btn = icon.AddComponent<Button>();
                string charName = characterName;
                btn.onClick.AddListener(() => OnCharacterClicked(charName));
                
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
                _ => name
            };
            return $"Sprites/player/{korean}";
        }

        private void OnCharacterClicked(string characterName)
        {
            if (_characterSelectionUseCase == null) return;

            if (_characterSelectionUseCase.IsCharacterSelected(characterName))
            {
                _characterSelectionUseCase.DeselectCharacter(characterName);
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
                _characterSelectionUseCase.SelectCharacter(characterName);
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

            List<string> selected = _characterSelectionUseCase.GetSelectedCharacters();
            if (selected.Count < minSelectCount) return;

            _characterSelectionUseCase.ConfirmSelection();
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetSelectedCharacters(selected);
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowMainMenuScreen();
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

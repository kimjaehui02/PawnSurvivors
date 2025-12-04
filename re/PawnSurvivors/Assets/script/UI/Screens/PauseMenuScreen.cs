using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using PawnSurvivors.Managers;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 일시정지 메뉴 화면입니다. 코드로 UI를 생성합니다.
    /// </summary>
    public class PauseMenuScreen : MonoBehaviour
    {
        private GameObject _optionsScreen;
        private TMP_FontAsset _font;

        private void Awake()
        {
            LoadFont();
            CreateUI();
        }

        private void LoadFont()
        {
            _font = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
        }

        private void CreateUI()
        {
            // Canvas 생성
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            gameObject.AddComponent<GraphicRaycaster>();
            
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            // EventSystem 생성 (버튼 클릭을 위해 필요)
            if (FindAnyObjectByType<EventSystem>() == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }
            
            // 배경 (반투명 검정)
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(transform, false);
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0f, 0f, 0f, 0.8f);
            
            // 패널
            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(500, 500);
            Image panelImg = panel.AddComponent<Image>();
            panelImg.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            
            // 제목
            CreateTitle(panel.transform);
            
            // 버튼들
            CreateButton(panel.transform, "계속하기", 0.7f, OnResumeClicked);
            CreateButton(panel.transform, "옵션", 0.55f, OnOptionsClicked);
            CreateButton(panel.transform, "메인 메뉴", 0.4f, OnMainMenuClicked);
            CreateButton(panel.transform, "종료", 0.25f, OnQuitClicked);
        }

        private void CreateTitle(Transform parent)
        {
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(parent, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.85f);
            titleRect.anchorMax = new Vector2(0.5f, 0.85f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = new Vector2(400, 60);
            
            TMP_Text titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "일시정지";
            titleText.fontSize = 50;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;
            if (_font != null) titleText.font = _font;
        }

        private void CreateButton(Transform parent, string text, float yAnchor, UnityEngine.Events.UnityAction callback)
        {
            GameObject btnObj = new GameObject($"{text}Button");
            btnObj.transform.SetParent(parent, false);
            RectTransform btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, yAnchor);
            btnRect.anchorMax = new Vector2(0.5f, yAnchor);
            btnRect.anchoredPosition = Vector2.zero;
            btnRect.sizeDelta = new Vector2(350, 70);
            
            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            
            Button btn = btnObj.AddComponent<Button>();
            btn.onClick.AddListener(callback);
            
            // 호버 효과
            ColorBlock colors = btn.colors;
            colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            colors.highlightedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            colors.pressedColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            btn.colors = colors;
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            TMP_Text btnText = textObj.AddComponent<TextMeshProUGUI>();
            btnText.text = text;
            btnText.fontSize = 36;
            btnText.alignment = TextAlignmentOptions.Center;
            btnText.color = Color.white;
            if (_font != null) btnText.font = _font;
        }

        private void OnResumeClicked()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HidePauseMenu();
            }
        }

        private void OnOptionsClicked()
        {
            // 옵션 화면 생성
            if (_optionsScreen == null)
            {
                _optionsScreen = new GameObject("OptionsScreen");
                _optionsScreen.transform.SetParent(transform.parent);
                _optionsScreen.AddComponent<OptionsScreen>();
            }
            
            // 일시정지 메뉴 숨기고 옵션 표시
            gameObject.SetActive(false);
            _optionsScreen.SetActive(true);
        }

        private void OnMainMenuClicked()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ReturnToMainMenu();
            }
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnEnable()
        {
            // 옵션 화면이 열려있으면 닫기
            if (_optionsScreen != null)
            {
                _optionsScreen.SetActive(false);
            }
        }
    }
}

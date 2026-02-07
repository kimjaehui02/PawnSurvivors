using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using PawnSurvivors.Managers;

namespace PawnSurvivors.UI.Legacy
{
    /// <summary>
    /// 옵션 메인 화면입니다. 하위 옵션 메뉴들을 선택할 수 있습니다.
    /// </summary>
    public class OptionsScreen : MonoBehaviour
    {
        private GameObject _audioSettingsScreen;
        private TMP_FontAsset _font;
        private bool _isUICreated = false;
        
        /// <summary>
        /// 현재 열려있는 하위 화면을 반환합니다.
        /// </summary>
        public GameObject ActiveSubScreen => _audioSettingsScreen != null && _audioSettingsScreen.activeSelf ? _audioSettingsScreen : null;

        private void Awake()
        {
            if (!_isUICreated)
            {
                LoadFont();
                CreateUI();
                _isUICreated = true;
            }
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
            canvas.sortingOrder = 110; // PauseMenu(100)보다 위
            
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
            
            // 배경
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
            panelRect.sizeDelta = new Vector2(500, 400);
            Image panelImg = panel.AddComponent<Image>();
            panelImg.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            
            // 제목
            CreateTitle(panel.transform);
            
            // 옵션 버튼들
            CreateButton(panel.transform, "소리 설정", 0.6f, OnAudioSettingsClicked);
            
            // 닫기 버튼
            CreateButton(panel.transform, "닫기", 0.2f, OnCloseClicked);
        }

        private void CreateTitle(Transform parent)
        {
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(parent, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.8f);
            titleRect.anchorMax = new Vector2(0.5f, 0.8f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = new Vector2(400, 60);
            
            TMP_Text titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "옵션";
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

        private void OnAudioSettingsClicked()
        {
            // 오디오 설정 화면 생성 (코드로)
            if (_audioSettingsScreen == null)
            {
                _audioSettingsScreen = new GameObject("AudioSettingsScreen");
                _audioSettingsScreen.transform.SetParent(transform.parent, false);
                _audioSettingsScreen.AddComponent<AudioSettingsScreen>();
                _audioSettingsScreen.SetActive(false); // 생성 직후 비활성화
            }
            
            // 메인 옵션 화면 숨기고 오디오 설정 표시
            gameObject.SetActive(false);
            _audioSettingsScreen.SetActive(true);
        }

        private void OnCloseClicked()
        {
            gameObject.SetActive(false);
            
            // 일시정지 메뉴로 돌아가기
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPauseMenu();
            }
        }

        private void OnEnable()
        {
            // 오디오 설정 화면이 열려있으면 닫기
            if (_audioSettingsScreen != null)
            {
                _audioSettingsScreen.SetActive(false);
            }
        }
    }
}


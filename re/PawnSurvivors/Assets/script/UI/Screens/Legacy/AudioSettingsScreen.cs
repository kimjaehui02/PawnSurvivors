using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PawnSurvivors.Managers;

namespace PawnSurvivors.UI.Legacy
{
    /// <summary>
    /// 오디오 설정 화면입니다. 볼륨 조절 기능을 제공합니다.
    /// </summary>
    public class AudioSettingsScreen : MonoBehaviour
    {
        private Slider _masterVolumeSlider;
        private Slider _bgmVolumeSlider;
        private Slider _sfxVolumeSlider;
        private TMP_FontAsset _font;
        private bool _isUICreated = false;

        private void Awake()
        {
            if (!_isUICreated)
            {
                LoadFont();
                CreateUI();
                _isUICreated = true;
            }
        }
        
        private void OnEnable()
        {
            // 화면 표시 시 현재 설정값으로 슬라이더 초기화
            UpdateSliders();
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
            canvas.sortingOrder = 120; // Options(110)보다 위
            
            gameObject.AddComponent<GraphicRaycaster>();
            
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f; // 다른 화면과 일관성 유지
            
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
            panelRect.sizeDelta = new Vector2(600, 500);
            Image panelImg = panel.AddComponent<Image>();
            panelImg.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            
            // 제목
            CreateTitle(panel.transform);
            
            // 볼륨 슬라이더들
            CreateVolumeSliders(panel.transform);
            
            // 닫기 버튼
            CreateCloseButton(panel.transform);
        }

        private void CreateTitle(Transform parent)
        {
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(parent, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.85f);
            titleRect.anchorMax = new Vector2(0.5f, 0.85f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = new Vector2(500, 80);
            
            TMP_Text titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "소리 설정";
            titleText.fontSize = 50;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;
            if (_font != null) titleText.font = _font;
        }

        private void CreateVolumeSliders(Transform parent)
        {
            // 마스터 볼륨
            _masterVolumeSlider = CreateSlider(parent, "마스터 볼륨", 0.65f, OnMasterVolumeChanged);
            
            // BGM 볼륨
            _bgmVolumeSlider = CreateSlider(parent, "배경음악", 0.5f, OnBGMVolumeChanged);
            
            // 효과음 볼륨
            _sfxVolumeSlider = CreateSlider(parent, "효과음", 0.35f, OnSFXVolumeChanged);
        }

        private Slider CreateSlider(Transform parent, string label, float yAnchor, UnityEngine.Events.UnityAction<float> callback)
        {
            GameObject sliderGroup = new GameObject($"{label}Group");
            sliderGroup.transform.SetParent(parent, false);
            RectTransform groupRect = sliderGroup.AddComponent<RectTransform>();
            groupRect.anchorMin = new Vector2(0.5f, yAnchor);
            groupRect.anchorMax = new Vector2(0.5f, yAnchor);
            groupRect.anchoredPosition = Vector2.zero;
            groupRect.sizeDelta = new Vector2(500, 60);
            
            // 라벨
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(sliderGroup.transform, false);
            RectTransform labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 0.5f);
            labelRect.anchorMax = new Vector2(0.3f, 0.5f);
            labelRect.anchoredPosition = Vector2.zero;
            labelRect.sizeDelta = new Vector2(0, 40);
            
            TMP_Text labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 28;
            labelText.alignment = TextAlignmentOptions.Left;
            labelText.color = Color.white;
            if (_font != null) labelText.font = _font;
            
            // 슬라이더
            GameObject sliderObj = new GameObject("Slider");
            sliderObj.transform.SetParent(sliderGroup.transform, false);
            RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.35f, 0.5f);
            sliderRect.anchorMax = new Vector2(1f, 0.5f);
            sliderRect.anchoredPosition = Vector2.zero;
            sliderRect.sizeDelta = new Vector2(0, 30);
            
            Slider slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;
            slider.onValueChanged.AddListener(callback);
            
            // 배경
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform, false);
            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            Image bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            slider.targetGraphic = bgImg;
            
            // Fill (채워지는 부분)
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform, false);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.sizeDelta = Vector2.zero;
            
            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillArea.transform, false);
            RectTransform fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;
            Image fillImg = fillObj.AddComponent<Image>();
            fillImg.color = new Color(0.2f, 0.6f, 0.9f, 1f);
            slider.fillRect = fillRect;
            
            // Handle (손잡이)
            GameObject handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(sliderObj.transform, false);
            RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.sizeDelta = Vector2.zero;
            
            GameObject handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(handleArea.transform, false);
            RectTransform handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 0);
            Image handleImg = handleObj.AddComponent<Image>();
            handleImg.color = Color.white;
            slider.handleRect = handleRect;
            
            return slider;
        }

        private void CreateCloseButton(Transform parent)
        {
            GameObject btnObj = new GameObject("CloseButton");
            btnObj.transform.SetParent(parent, false);
            RectTransform btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, 0.1f);
            btnRect.anchorMax = new Vector2(0.5f, 0.1f);
            btnRect.anchoredPosition = Vector2.zero;
            btnRect.sizeDelta = new Vector2(200, 60);
            
            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            
            Button btn = btnObj.AddComponent<Button>();
            btn.onClick.AddListener(OnCloseButtonClicked);
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            TMP_Text text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "닫기";
            text.fontSize = 32;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            if (_font != null) text.font = _font;
        }

        private void UpdateSliders()
        {
            if (GameManager.Instance?.AudioSettings == null) return;
            
            _masterVolumeSlider.value = GameManager.Instance.AudioSettings.masterVolume;
            _bgmVolumeSlider.value = GameManager.Instance.AudioSettings.bgmVolume;
            _sfxVolumeSlider.value = GameManager.Instance.AudioSettings.sfxVolume;
        }

        private void OnMasterVolumeChanged(float value)
        {
            if (GameManager.Instance?.AudioSettings != null)
            {
                GameManager.Instance.AudioSettings.SetMasterVolume(value);
                GameManager.Instance.UpdateBGMVolume();
            }
        }

        private void OnBGMVolumeChanged(float value)
        {
            if (GameManager.Instance?.AudioSettings != null)
            {
                GameManager.Instance.AudioSettings.SetBGMVolume(value);
                GameManager.Instance.UpdateBGMVolume();
            }
        }

        private void OnSFXVolumeChanged(float value)
        {
            if (GameManager.Instance?.AudioSettings != null)
            {
                GameManager.Instance.AudioSettings.SetSFXVolume(value);
                // 테스트 사운드 재생
                PawnSurvivors.Utilities.AudioHelper.PlayUISound("Audio/Select");
            }
        }

        private void OnCloseButtonClicked()
        {
            gameObject.SetActive(false);
            
            // 옵션 메뉴로 돌아가기
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowOptionsScreen();
            }
        }
    }
}


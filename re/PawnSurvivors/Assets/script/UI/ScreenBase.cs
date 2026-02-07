using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 모든 Screen의 공통 기능을 제공하는 베이스 클래스입니다.
    /// Canvas 설정, 폰트 로딩, 공통 UI 생성 메서드를 제공합니다.
    /// </summary>
    public abstract class ScreenBase : MonoBehaviour
    {
        protected Canvas _canvas;
        protected TMP_FontAsset _koreanFont;
        protected bool _isInitialized = false;

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            SetupCanvas();
            LoadKoreanFont();
        }

        protected virtual void Start()
        {
            EnsureEventSystem();
            OnScreenInitialize();
            _isInitialized = true;
        }

        protected virtual void OnEnable()
        {
            if (_isInitialized)
            {
                OnScreenShow();
            }
        }

        protected virtual void OnDisable()
        {
            OnScreenHide();
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Screen 초기화 시 호출됩니다. UI 생성 등을 수행합니다.
        /// </summary>
        protected abstract void OnScreenInitialize();

        /// <summary>
        /// Screen이 표시될 때 호출됩니다.
        /// </summary>
        protected virtual void OnScreenShow() { }

        /// <summary>
        /// Screen이 숨겨질 때 호출됩니다.
        /// </summary>
        protected virtual void OnScreenHide() { }

        #endregion

        #region Canvas Setup

        protected virtual void SetupCanvas()
        {
            _canvas = GetComponent<Canvas>();
            if (_canvas == null)
            {
                _canvas = gameObject.AddComponent<Canvas>();
            }

            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            if (GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }

            var scaler = GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = gameObject.AddComponent<CanvasScaler>();
            }
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
        }

        protected void EnsureEventSystem()
        {
            if (EventSystem.current == null)
            {
                var eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }
        }

        #endregion

        #region Font Loading

        protected void LoadKoreanFont()
        {
            _koreanFont = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");

            #if UNITY_EDITOR
            if (_koreanFont == null)
            {
                _koreanFont = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                    "Assets/Resources/Fonts/NanumGothic SDF.asset");
            }
            #endif
        }

        #endregion

        #region UI Factory Methods

        /// <summary>
        /// 텍스트 UI를 생성합니다.
        /// </summary>
        protected TMP_Text CreateText(
            string name,
            Transform parent,
            string text,
            float fontSize = 24,
            Color? color = null,
            TextAlignmentOptions alignment = TextAlignmentOptions.Left)
        {
            var textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);

            var rectTransform = textObj.AddComponent<RectTransform>();
            var tmpText = textObj.AddComponent<TextMeshProUGUI>();

            tmpText.text = text;
            tmpText.fontSize = fontSize;
            tmpText.color = color ?? Color.white;
            tmpText.alignment = alignment;

            if (_koreanFont != null)
            {
                tmpText.font = _koreanFont;
            }

            return tmpText;
        }

        /// <summary>
        /// 버튼을 생성합니다.
        /// </summary>
        protected Button CreateButton(
            string name,
            Transform parent,
            string buttonText,
            float fontSize = 24,
            Color? bgColor = null,
            Color? textColor = null)
        {
            var buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);

            var rectTransform = buttonObj.AddComponent<RectTransform>();
            var image = buttonObj.AddComponent<Image>();
            image.color = bgColor ?? new Color(0.3f, 0.3f, 0.3f, 1f);

            var button = buttonObj.AddComponent<Button>();
            button.targetGraphic = image;

            // 버튼 텍스트
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            var tmpText = textObj.AddComponent<TextMeshProUGUI>();
            tmpText.text = buttonText;
            tmpText.fontSize = fontSize;
            tmpText.color = textColor ?? Color.white;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.raycastTarget = false;

            if (_koreanFont != null)
            {
                tmpText.font = _koreanFont;
            }

            return button;
        }

        /// <summary>
        /// 패널(컨테이너)을 생성합니다.
        /// </summary>
        protected GameObject CreatePanel(
            string name,
            Transform parent,
            Color? bgColor = null)
        {
            var panelObj = new GameObject(name);
            panelObj.transform.SetParent(parent, false);

            var rectTransform = panelObj.AddComponent<RectTransform>();

            if (bgColor.HasValue)
            {
                var image = panelObj.AddComponent<Image>();
                image.color = bgColor.Value;
            }

            return panelObj;
        }

        /// <summary>
        /// Slider를 생성합니다.
        /// </summary>
        protected Slider CreateSlider(
            string name,
            Transform parent,
            Color? bgColor = null,
            Color? fillColor = null)
        {
            var sliderObj = new GameObject(name);
            sliderObj.transform.SetParent(parent, false);

            var sliderRect = sliderObj.AddComponent<RectTransform>();
            var slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0.5f;

            // Background
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform, false);
            var bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = bgColor ?? new Color(0.2f, 0.2f, 0.2f, 1f);

            // Fill Area
            var fillAreaObj = new GameObject("Fill Area");
            fillAreaObj.transform.SetParent(sliderObj.transform, false);
            var fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.sizeDelta = Vector2.zero;

            // Fill
            var fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillAreaObj.transform, false);
            var fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;
            var fillImage = fillObj.AddComponent<Image>();
            fillImage.color = fillColor ?? new Color(0f, 1f, 0.5f, 1f);

            slider.fillRect = fillRect;
            slider.targetGraphic = fillImage;

            return slider;
        }

        /// <summary>
        /// RectTransform의 앵커와 위치를 설정합니다.
        /// </summary>
        protected void SetRectTransform(
            RectTransform rect,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// 게임 시간 델타를 가져옵니다 (일시정지 고려).
        /// </summary>
        protected float GetGameDeltaTime()
        {
            if (Managers.GameManager.Instance?.LifecycleManager != null)
            {
                return Managers.GameManager.Instance.LifecycleManager.GameDeltaTime;
            }
            return Time.deltaTime;
        }

        /// <summary>
        /// 게임 시간을 가져옵니다 (일시정지 고려).
        /// </summary>
        protected float GetGameTime()
        {
            if (Managers.GameManager.Instance?.LifecycleManager != null)
            {
                return Managers.GameManager.Instance.LifecycleManager.GameTime;
            }
            return Time.time;
        }

        #endregion
    }
}

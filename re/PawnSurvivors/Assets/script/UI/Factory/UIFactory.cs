using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PawnSurvivors.UI.Factory
{
    /// <summary>
    /// UI 요소를 코드로 생성하는 팩토리 클래스입니다.
    /// 모든 UI는 이 팩토리를 통해 일관된 방식으로 생성됩니다.
    /// </summary>
    public static class UIFactory
    {
        private static TMP_FontAsset _cachedFont;

        /// <summary>
        /// 한글 폰트를 로드합니다. (캐시됨)
        /// </summary>
        public static TMP_FontAsset GetKoreanFont()
        {
            if (_cachedFont == null)
            {
                _cachedFont = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");

                #if UNITY_EDITOR
                if (_cachedFont == null)
                {
                    _cachedFont = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                        "Assets/Resources/Fonts/NanumGothic SDF.asset");
                }
                #endif
            }
            return _cachedFont;
        }

        #region Canvas

        /// <summary>
        /// ScreenSpace Overlay Canvas를 생성합니다.
        /// </summary>
        public static Canvas CreateCanvas(string name, int sortingOrder = 0)
        {
            var canvasObj = new GameObject(name);
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            return canvas;
        }

        /// <summary>
        /// 기존 GameObject에 Canvas를 설정합니다.
        /// </summary>
        public static Canvas SetupCanvas(GameObject obj, int sortingOrder = 0)
        {
            var canvas = obj.GetComponent<Canvas>() ?? obj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            var scaler = obj.GetComponent<CanvasScaler>() ?? obj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            if (obj.GetComponent<GraphicRaycaster>() == null)
            {
                obj.AddComponent<GraphicRaycaster>();
            }

            return canvas;
        }

        #endregion

        #region Panel

        /// <summary>
        /// 패널(컨테이너)을 생성합니다.
        /// </summary>
        public static RectTransform CreatePanel(
            string name,
            Transform parent,
            Color? bgColor = null)
        {
            var panelObj = new GameObject(name);
            panelObj.transform.SetParent(parent, false);

            var rect = panelObj.AddComponent<RectTransform>();

            if (bgColor.HasValue)
            {
                var image = panelObj.AddComponent<Image>();
                image.color = bgColor.Value;
            }

            return rect;
        }

        /// <summary>
        /// 전체 화면을 채우는 패널을 생성합니다.
        /// </summary>
        public static RectTransform CreateFullScreenPanel(
            string name,
            Transform parent,
            Color? bgColor = null)
        {
            var rect = CreatePanel(name, parent, bgColor);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            return rect;
        }

        #endregion

        #region Text

        /// <summary>
        /// 텍스트를 생성합니다.
        /// </summary>
        public static TMP_Text CreateText(
            string name,
            Transform parent,
            string text,
            float fontSize = 24,
            Color? color = null,
            TextAlignmentOptions alignment = TextAlignmentOptions.Left,
            bool raycastTarget = false)
        {
            var textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);

            textObj.AddComponent<RectTransform>();
            var tmpText = textObj.AddComponent<TextMeshProUGUI>();

            tmpText.text = text;
            tmpText.fontSize = fontSize;
            tmpText.color = color ?? Color.white;
            tmpText.alignment = alignment;
            tmpText.raycastTarget = raycastTarget;

            var font = GetKoreanFont();
            if (font != null)
            {
                tmpText.font = font;
            }

            return tmpText;
        }

        #endregion

        #region Button

        /// <summary>
        /// 버튼을 생성합니다.
        /// </summary>
        public static Button CreateButton(
            string name,
            Transform parent,
            string buttonText,
            float fontSize = 24,
            Color? bgColor = null,
            Color? textColor = null)
        {
            var buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);

            buttonObj.AddComponent<RectTransform>();
            var image = buttonObj.AddComponent<Image>();
            image.color = bgColor ?? new Color(0.3f, 0.3f, 0.3f, 1f);

            var button = buttonObj.AddComponent<Button>();
            button.targetGraphic = image;

            // 버튼 텍스트
            var textTmp = CreateText("Text", buttonObj.transform, buttonText, fontSize, textColor ?? Color.white, TextAlignmentOptions.Center);
            var textRect = textTmp.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            return button;
        }

        /// <summary>
        /// 버튼의 텍스트를 가져옵니다.
        /// </summary>
        public static TMP_Text GetButtonText(Button button)
        {
            return button.GetComponentInChildren<TMP_Text>();
        }

        #endregion

        #region Image

        /// <summary>
        /// 이미지를 생성합니다.
        /// </summary>
        public static Image CreateImage(
            string name,
            Transform parent,
            Color? color = null,
            Sprite sprite = null)
        {
            var imageObj = new GameObject(name);
            imageObj.transform.SetParent(parent, false);

            imageObj.AddComponent<RectTransform>();
            var image = imageObj.AddComponent<Image>();
            image.color = color ?? Color.white;
            image.sprite = sprite;

            return image;
        }

        #endregion

        #region Slider

        /// <summary>
        /// 슬라이더를 생성합니다.
        /// </summary>
        public static Slider CreateSlider(
            string name,
            Transform parent,
            Color? bgColor = null,
            Color? fillColor = null,
            float minValue = 0f,
            float maxValue = 1f,
            float value = 0.5f)
        {
            var sliderObj = new GameObject(name);
            sliderObj.transform.SetParent(parent, false);

            var sliderRect = sliderObj.AddComponent<RectTransform>();
            var slider = sliderObj.AddComponent<Slider>();
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.value = value;

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
            fillRect.anchorMax = new Vector2(0, 1);
            fillRect.sizeDelta = Vector2.zero;
            var fillImage = fillObj.AddComponent<Image>();
            fillImage.color = fillColor ?? new Color(0f, 1f, 0.5f, 1f);

            slider.fillRect = fillRect;
            slider.targetGraphic = fillImage;

            return slider;
        }

        #endregion

        #region Toggle

        /// <summary>
        /// 토글을 생성합니다.
        /// </summary>
        public static Toggle CreateToggle(
            string name,
            Transform parent,
            string labelText,
            bool isOn = false,
            float fontSize = 24,
            Color? bgColor = null)
        {
            var toggleObj = new GameObject(name);
            toggleObj.transform.SetParent(parent, false);

            toggleObj.AddComponent<RectTransform>();
            var toggle = toggleObj.AddComponent<Toggle>();

            // Background
            var bgImage = CreateImage("Background", toggleObj.transform, bgColor ?? new Color(0.2f, 0.2f, 0.2f, 1f));
            var bgRect = bgImage.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0.5f);
            bgRect.anchorMax = new Vector2(0, 0.5f);
            bgRect.sizeDelta = new Vector2(30, 30);
            bgRect.anchoredPosition = new Vector2(15, 0);

            // Checkmark
            var checkImage = CreateImage("Checkmark", bgImage.transform, Color.green);
            var checkRect = checkImage.GetComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0.1f, 0.1f);
            checkRect.anchorMax = new Vector2(0.9f, 0.9f);
            checkRect.sizeDelta = Vector2.zero;

            // Label
            var label = CreateText("Label", toggleObj.transform, labelText, fontSize, Color.white, TextAlignmentOptions.Left);
            var labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(1, 1);
            labelRect.offsetMin = new Vector2(40, 0);
            labelRect.offsetMax = Vector2.zero;

            toggle.targetGraphic = bgImage;
            toggle.graphic = checkImage;
            toggle.isOn = isOn;

            return toggle;
        }

        #endregion

        #region ScrollView

        /// <summary>
        /// 스크롤뷰를 생성합니다.
        /// </summary>
        public static ScrollRect CreateScrollView(
            string name,
            Transform parent,
            bool horizontal = false,
            bool vertical = true,
            Color? bgColor = null)
        {
            var scrollObj = new GameObject(name);
            scrollObj.transform.SetParent(parent, false);

            var scrollRect = scrollObj.AddComponent<RectTransform>();
            var scroll = scrollObj.AddComponent<ScrollRect>();
            scroll.horizontal = horizontal;
            scroll.vertical = vertical;

            if (bgColor.HasValue)
            {
                var bgImage = scrollObj.AddComponent<Image>();
                bgImage.color = bgColor.Value;
            }

            // Viewport
            var viewportObj = new GameObject("Viewport");
            viewportObj.transform.SetParent(scrollObj.transform, false);
            var viewportRect = viewportObj.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = Vector2.zero;
            var viewportMask = viewportObj.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            var viewportImage = viewportObj.AddComponent<Image>();

            // Content
            var contentObj = new GameObject("Content");
            contentObj.transform.SetParent(viewportObj.transform, false);
            var contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.sizeDelta = new Vector2(0, 0);

            scroll.viewport = viewportRect;
            scroll.content = contentRect;

            return scroll;
        }

        /// <summary>
        /// ScrollRect의 Content에 VerticalLayoutGroup을 추가합니다.
        /// </summary>
        public static VerticalLayoutGroup AddVerticalLayout(
            ScrollRect scrollRect,
            float spacing = 10f,
            RectOffset padding = null)
        {
            var layout = scrollRect.content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = padding ?? new RectOffset(10, 10, 10, 10);
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var fitter = scrollRect.content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return layout;
        }

        #endregion

        #region Layout Groups

        /// <summary>
        /// Vertical Layout Group을 가진 패널을 생성합니다.
        /// </summary>
        public static VerticalLayoutGroup CreateVerticalLayoutPanel(
            string name,
            Transform parent,
            float spacing = 10f,
            RectOffset padding = null,
            Color? bgColor = null)
        {
            var rect = CreatePanel(name, parent, bgColor);
            var layout = rect.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = padding ?? new RectOffset(10, 10, 10, 10);
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childAlignment = TextAnchor.UpperCenter;

            return layout;
        }

        /// <summary>
        /// Horizontal Layout Group을 가진 패널을 생성합니다.
        /// </summary>
        public static HorizontalLayoutGroup CreateHorizontalLayoutPanel(
            string name,
            Transform parent,
            float spacing = 10f,
            RectOffset padding = null,
            Color? bgColor = null)
        {
            var rect = CreatePanel(name, parent, bgColor);
            var layout = rect.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = padding ?? new RectOffset(10, 10, 10, 10);
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            layout.childAlignment = TextAnchor.MiddleCenter;

            return layout;
        }

        /// <summary>
        /// Grid Layout Group을 가진 패널을 생성합니다.
        /// </summary>
        public static GridLayoutGroup CreateGridLayoutPanel(
            string name,
            Transform parent,
            Vector2 cellSize,
            Vector2 spacing,
            RectOffset padding = null,
            Color? bgColor = null)
        {
            var rect = CreatePanel(name, parent, bgColor);
            var layout = rect.gameObject.AddComponent<GridLayoutGroup>();
            layout.cellSize = cellSize;
            layout.spacing = spacing;
            layout.padding = padding ?? new RectOffset(10, 10, 10, 10);

            return layout;
        }

        #endregion

        #region Utility

        /// <summary>
        /// RectTransform 설정을 간편하게 합니다.
        /// </summary>
        public static void SetRect(
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

        /// <summary>
        /// 앵커 프리셋으로 RectTransform을 설정합니다.
        /// </summary>
        public static void SetAnchorPreset(RectTransform rect, AnchorPreset preset, Vector2 size)
        {
            switch (preset)
            {
                case AnchorPreset.TopLeft:
                    SetRect(rect, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), Vector2.zero, size);
                    break;
                case AnchorPreset.TopCenter:
                    SetRect(rect, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), Vector2.zero, size);
                    break;
                case AnchorPreset.TopRight:
                    SetRect(rect, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), Vector2.zero, size);
                    break;
                case AnchorPreset.MiddleLeft:
                    SetRect(rect, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), Vector2.zero, size);
                    break;
                case AnchorPreset.MiddleCenter:
                    SetRect(rect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, size);
                    break;
                case AnchorPreset.MiddleRight:
                    SetRect(rect, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), Vector2.zero, size);
                    break;
                case AnchorPreset.BottomLeft:
                    SetRect(rect, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), Vector2.zero, size);
                    break;
                case AnchorPreset.BottomCenter:
                    SetRect(rect, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), Vector2.zero, size);
                    break;
                case AnchorPreset.BottomRight:
                    SetRect(rect, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), Vector2.zero, size);
                    break;
                case AnchorPreset.StretchTop:
                    SetRect(rect, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), Vector2.zero, new Vector2(0, size.y));
                    break;
                case AnchorPreset.StretchMiddle:
                    SetRect(rect, new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(0, size.y));
                    break;
                case AnchorPreset.StretchBottom:
                    SetRect(rect, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), Vector2.zero, new Vector2(0, size.y));
                    break;
                case AnchorPreset.StretchAll:
                    SetRect(rect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
                    break;
            }
        }

        #endregion
    }

    /// <summary>
    /// 앵커 프리셋
    /// </summary>
    public enum AnchorPreset
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
        StretchTop,
        StretchMiddle,
        StretchBottom,
        StretchAll
    }
}

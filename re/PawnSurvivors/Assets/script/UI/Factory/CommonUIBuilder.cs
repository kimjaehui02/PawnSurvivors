using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace PawnSurvivors.UI.Factory
{
    /// <summary>
    /// 여러 Screen에서 재사용되는 공통 UI 컴포넌트를 빌드합니다.
    /// </summary>
    public static class CommonUIBuilder
    {
        #region Colors (게임 테마)

        public static readonly Color BgDark = new Color(0.05f, 0.05f, 0.08f, 1f);
        public static readonly Color BgPanel = new Color(0.12f, 0.12f, 0.12f, 1f);
        public static readonly Color BgSlot = new Color(0.15f, 0.15f, 0.15f, 1f);
        public static readonly Color BgButton = new Color(0.3f, 0.3f, 0.3f, 1f);
        public static readonly Color BgButtonGreen = new Color(0.2f, 0.6f, 0.2f, 1f);
        public static readonly Color BgButtonRed = new Color(0.6f, 0.2f, 0.2f, 1f);
        public static readonly Color BgOverlay = new Color(0f, 0f, 0f, 0.7f);

        public static readonly Color TextWhite = Color.white;
        public static readonly Color TextGray = new Color(0.8f, 0.8f, 0.8f, 1f);
        public static readonly Color TextGreen = Color.green;
        public static readonly Color TextRed = Color.red;
        public static readonly Color TextGold = new Color(1f, 0.84f, 0f, 1f);

        #endregion

        #region Header Bar

        /// <summary>
        /// 상단 헤더 바를 생성합니다. (타이틀 + 중앙 정보 + 우측 버튼)
        /// </summary>
        public static HeaderBar CreateHeaderBar(
            Transform parent,
            string title,
            float height = 100f)
        {
            var header = new HeaderBar();

            // 헤더 패널
            header.Root = UIFactory.CreatePanel("HeaderBar", parent, BgPanel);
            UIFactory.SetRect(header.Root,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0.5f, 1),
                new Vector2(0, 0),
                new Vector2(0, height));

            // 타이틀 (좌측)
            header.TitleText = UIFactory.CreateText("Title", header.Root, title, 48, TextWhite, TextAlignmentOptions.Left);
            var titleRect = header.TitleText.GetComponent<RectTransform>();
            UIFactory.SetRect(titleRect,
                new Vector2(0, 0), new Vector2(0.3f, 1),
                new Vector2(0, 0.5f),
                new Vector2(40, 0),
                new Vector2(0, 0));

            // 중앙 영역 (정보 표시용)
            header.CenterPanel = UIFactory.CreatePanel("CenterPanel", header.Root);
            UIFactory.SetRect(header.CenterPanel,
                new Vector2(0.3f, 0), new Vector2(0.7f, 1),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero);

            // 우측 영역 (버튼용)
            header.RightPanel = UIFactory.CreatePanel("RightPanel", header.Root);
            UIFactory.SetRect(header.RightPanel,
                new Vector2(0.7f, 0), new Vector2(1, 1),
                new Vector2(1, 0.5f),
                new Vector2(-40, 0),
                Vector2.zero);

            return header;
        }

        public class HeaderBar
        {
            public RectTransform Root;
            public TMP_Text TitleText;
            public RectTransform CenterPanel;
            public RectTransform RightPanel;
        }

        #endregion

        #region Gold Display

        /// <summary>
        /// 골드 표시 UI를 생성합니다.
        /// </summary>
        public static GoldDisplay CreateGoldDisplay(Transform parent, int initialGold = 0)
        {
            var display = new GoldDisplay();

            display.Root = UIFactory.CreatePanel("GoldDisplay", parent);
            var layout = display.Root.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;

            // 골드 아이콘
            display.Icon = UIFactory.CreateImage("GoldIcon", display.Root, TextGold);
            var iconRect = display.Icon.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(40, 40);

            // 골드 텍스트
            display.Text = UIFactory.CreateText("GoldText", display.Root, initialGold.ToString(), 36, TextGreen, TextAlignmentOptions.Left);
            var textRect = display.Text.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(150, 50);

            return display;
        }

        public class GoldDisplay
        {
            public RectTransform Root;
            public Image Icon;
            public TMP_Text Text;

            public void SetGold(int amount)
            {
                if (Text != null)
                {
                    Text.text = amount.ToString();
                }
            }
        }

        #endregion

        #region Round/Stage Display

        /// <summary>
        /// 라운드/스테이지 표시 UI를 생성합니다.
        /// </summary>
        public static TMP_Text CreateRoundDisplay(Transform parent, int current = 1, int total = 0)
        {
            string text = total > 0 ? $"라운드 {current}/{total}" : $"라운드 {current}";
            var roundText = UIFactory.CreateText("RoundDisplay", parent, text, 32, TextWhite, TextAlignmentOptions.Center);
            return roundText;
        }

        #endregion

        #region Timer Display

        /// <summary>
        /// 타이머 표시 UI를 생성합니다.
        /// </summary>
        public static TimerDisplay CreateTimerDisplay(Transform parent, float initialTime = 0f)
        {
            var display = new TimerDisplay();

            display.Root = UIFactory.CreatePanel("TimerDisplay", parent, BgPanel);

            display.TimeText = UIFactory.CreateText("TimeText", display.Root, "00:00", 48, TextWhite, TextAlignmentOptions.Center);
            var textRect = display.TimeText.GetComponent<RectTransform>();
            UIFactory.SetAnchorPreset(textRect, AnchorPreset.StretchAll, Vector2.zero);

            display.SetTime(initialTime);

            return display;
        }

        public class TimerDisplay
        {
            public RectTransform Root;
            public TMP_Text TimeText;

            public void SetTime(float seconds)
            {
                if (TimeText != null)
                {
                    int minutes = Mathf.FloorToInt(seconds / 60f);
                    int secs = Mathf.FloorToInt(seconds % 60f);
                    TimeText.text = $"{minutes:00}:{secs:00}";
                }
            }
        }

        #endregion

        #region Progress Bar

        /// <summary>
        /// 프로그레스 바를 생성합니다. (체력바, 경험치바 등)
        /// </summary>
        public static ProgressBar CreateProgressBar(
            Transform parent,
            string name,
            Color? bgColor = null,
            Color? fillColor = null,
            float height = 20f)
        {
            var bar = new ProgressBar();

            bar.Root = UIFactory.CreatePanel(name, parent, bgColor ?? new Color(0.2f, 0.2f, 0.2f, 1f));

            // Fill
            bar.Fill = UIFactory.CreateImage("Fill", bar.Root, fillColor ?? Color.green);
            var fillRect = bar.Fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(1, 1);
            fillRect.sizeDelta = Vector2.zero;
            fillRect.pivot = new Vector2(0, 0.5f);

            bar._fillRect = fillRect;

            return bar;
        }

        public class ProgressBar
        {
            public RectTransform Root;
            public Image Fill;
            internal RectTransform _fillRect;

            public void SetProgress(float normalized)
            {
                if (_fillRect != null)
                {
                    _fillRect.anchorMax = new Vector2(Mathf.Clamp01(normalized), 1);
                }
            }
        }

        #endregion

        #region Modal Dialog

        /// <summary>
        /// 모달 다이얼로그를 생성합니다.
        /// </summary>
        public static ModalDialog CreateModalDialog(
            Transform parent,
            string title,
            string message,
            string confirmText = "확인",
            string cancelText = null,
            Action onConfirm = null,
            Action onCancel = null)
        {
            var dialog = new ModalDialog();

            // 오버레이 배경
            dialog.Root = UIFactory.CreateFullScreenPanel("ModalDialog", parent, BgOverlay);

            // 다이얼로그 박스
            dialog.DialogBox = UIFactory.CreatePanel("DialogBox", dialog.Root, BgPanel);
            UIFactory.SetAnchorPreset(dialog.DialogBox, AnchorPreset.MiddleCenter, new Vector2(500, 300));

            // 타이틀
            dialog.TitleText = UIFactory.CreateText("Title", dialog.DialogBox, title, 32, TextWhite, TextAlignmentOptions.Center);
            var titleRect = dialog.TitleText.GetComponent<RectTransform>();
            UIFactory.SetRect(titleRect,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0.5f, 1),
                new Vector2(0, -20),
                new Vector2(0, 50));

            // 메시지
            dialog.MessageText = UIFactory.CreateText("Message", dialog.DialogBox, message, 24, TextGray, TextAlignmentOptions.Center);
            var msgRect = dialog.MessageText.GetComponent<RectTransform>();
            UIFactory.SetRect(msgRect,
                new Vector2(0, 0.3f), new Vector2(1, 0.8f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(-40, 0));

            // 버튼 영역
            var buttonArea = UIFactory.CreateHorizontalLayoutPanel("Buttons", dialog.DialogBox, 20f);
            var buttonAreaRect = buttonArea.GetComponent<RectTransform>();
            UIFactory.SetRect(buttonAreaRect,
                new Vector2(0, 0), new Vector2(1, 0.3f),
                new Vector2(0.5f, 0),
                Vector2.zero,
                Vector2.zero);

            // 확인 버튼
            dialog.ConfirmButton = UIFactory.CreateButton("ConfirmButton", buttonArea.transform, confirmText, 24, BgButtonGreen);
            var confirmRect = dialog.ConfirmButton.GetComponent<RectTransform>();
            confirmRect.sizeDelta = new Vector2(150, 50);
            if (onConfirm != null)
            {
                dialog.ConfirmButton.onClick.AddListener(() => onConfirm());
            }

            // 취소 버튼 (선택사항)
            if (!string.IsNullOrEmpty(cancelText))
            {
                dialog.CancelButton = UIFactory.CreateButton("CancelButton", buttonArea.transform, cancelText, 24, BgButtonRed);
                var cancelRect = dialog.CancelButton.GetComponent<RectTransform>();
                cancelRect.sizeDelta = new Vector2(150, 50);
                if (onCancel != null)
                {
                    dialog.CancelButton.onClick.AddListener(() => onCancel());
                }
            }

            return dialog;
        }

        public class ModalDialog
        {
            public RectTransform Root;
            public RectTransform DialogBox;
            public TMP_Text TitleText;
            public TMP_Text MessageText;
            public Button ConfirmButton;
            public Button CancelButton;

            public void Show() => Root?.gameObject.SetActive(true);
            public void Hide() => Root?.gameObject.SetActive(false);
            public void Destroy() => UnityEngine.Object.Destroy(Root?.gameObject);
        }

        #endregion

        #region Selection List

        /// <summary>
        /// 선택 리스트를 생성합니다. (Pawn 선택 등)
        /// </summary>
        public static SelectionList CreateSelectionList(
            Transform parent,
            string title,
            Action onCancel = null)
        {
            var list = new SelectionList();

            // 오버레이 배경
            list.Root = UIFactory.CreateFullScreenPanel("SelectionList", parent, BgOverlay);

            // 컨테이너
            list.Container = UIFactory.CreatePanel("Container", list.Root, BgPanel);
            UIFactory.SetAnchorPreset(list.Container, AnchorPreset.MiddleCenter, new Vector2(500, 500));

            // 타이틀
            list.TitleText = UIFactory.CreateText("Title", list.Container, title, 28, TextWhite, TextAlignmentOptions.Center);
            var titleRect = list.TitleText.GetComponent<RectTransform>();
            UIFactory.SetRect(titleRect,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0.5f, 1),
                new Vector2(0, -20),
                new Vector2(0, 50));

            // 버튼 컨테이너 (스크롤 가능)
            var scroll = UIFactory.CreateScrollView("ButtonScroll", list.Container);
            var scrollRect = scroll.GetComponent<RectTransform>();
            UIFactory.SetRect(scrollRect,
                new Vector2(0, 0.15f), new Vector2(1, 0.85f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(-40, 0));
            UIFactory.AddVerticalLayout(scroll, 10f);
            list.ButtonContainer = scroll.content;

            // 취소 버튼
            list.CancelButton = UIFactory.CreateButton("CancelButton", list.Container, "취소", 24, BgButtonRed);
            var cancelRect = list.CancelButton.GetComponent<RectTransform>();
            UIFactory.SetRect(cancelRect,
                new Vector2(0.5f, 0), new Vector2(0.5f, 0),
                new Vector2(0.5f, 0),
                new Vector2(0, 20),
                new Vector2(200, 50));

            if (onCancel != null)
            {
                list.CancelButton.onClick.AddListener(() => onCancel());
            }
            else
            {
                list.CancelButton.onClick.AddListener(() => list.Hide());
            }

            list.Root.gameObject.SetActive(false);

            return list;
        }

        public class SelectionList
        {
            public RectTransform Root;
            public RectTransform Container;
            public TMP_Text TitleText;
            public RectTransform ButtonContainer;
            public Button CancelButton;

            public Button AddOption(string text, Action onClick)
            {
                var button = UIFactory.CreateButton($"Option_{text}", ButtonContainer, text, 24, BgButton);
                var rect = button.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(0, 60);

                if (onClick != null)
                {
                    button.onClick.AddListener(() => onClick());
                }

                return button;
            }

            public void ClearOptions()
            {
                if (ButtonContainer == null) return;
                for (int i = ButtonContainer.childCount - 1; i >= 0; i--)
                {
                    UnityEngine.Object.Destroy(ButtonContainer.GetChild(i).gameObject);
                }
            }

            public void Show() => Root?.gameObject.SetActive(true);
            public void Hide() => Root?.gameObject.SetActive(false);
        }

        #endregion

        #region Bottom Bar

        /// <summary>
        /// 하단 바를 생성합니다.
        /// </summary>
        public static RectTransform CreateBottomBar(Transform parent, float height = 200f)
        {
            var bar = UIFactory.CreatePanel("BottomBar", parent, BgPanel);
            UIFactory.SetRect(bar,
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(0.5f, 0),
                Vector2.zero,
                new Vector2(0, height));

            return bar;
        }

        #endregion
    }
}

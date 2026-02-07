using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace PawnSurvivors.UI.Factory
{
    /// <summary>
    /// 스테이지 플레이 화면의 UI를 빌드합니다.
    /// </summary>
    public static class StageUIBuilder
    {
        #region HUD (Heads-Up Display)

        /// <summary>
        /// 스테이지 HUD를 생성합니다. (타이머, 라운드, 킬 수 등)
        /// </summary>
        public static StageHUD CreateStageHUD(Transform parent)
        {
            var hud = new StageHUD();

            // HUD 루트 (상단)
            hud.Root = UIFactory.CreatePanel("StageHUD", parent);
            UIFactory.SetRect(hud.Root,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0.5f, 1),
                Vector2.zero,
                new Vector2(0, 120));

            // 타이머 (상단 중앙)
            hud.TimerDisplay = CommonUIBuilder.CreateTimerDisplay(hud.Root);
            var timerRect = hud.TimerDisplay.Root;
            UIFactory.SetRect(timerRect,
                new Vector2(0.5f, 1), new Vector2(0.5f, 1),
                new Vector2(0.5f, 1),
                new Vector2(0, -10),
                new Vector2(200, 60));

            // 라운드 표시 (타이머 위)
            hud.RoundText = CommonUIBuilder.CreateRoundDisplay(hud.Root);
            var roundRect = hud.RoundText.GetComponent<RectTransform>();
            UIFactory.SetRect(roundRect,
                new Vector2(0.5f, 1), new Vector2(0.5f, 1),
                new Vector2(0.5f, 1),
                new Vector2(0, -75),
                new Vector2(300, 40));

            // 킬 수 (좌측 상단)
            hud.KillText = UIFactory.CreateText("KillCount", hud.Root, "Kill: 0", 28, CommonUIBuilder.TextWhite, TextAlignmentOptions.Left);
            var killRect = hud.KillText.GetComponent<RectTransform>();
            UIFactory.SetRect(killRect,
                new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(0, 1),
                new Vector2(20, -20),
                new Vector2(200, 40));

            // 골드 (좌측 상단, 킬 아래)
            hud.GoldDisplay = CommonUIBuilder.CreateGoldDisplay(hud.Root);
            var goldRect = hud.GoldDisplay.Root;
            UIFactory.SetRect(goldRect,
                new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(0, 1),
                new Vector2(20, -60),
                new Vector2(200, 40));

            return hud;
        }

        public class StageHUD
        {
            public RectTransform Root;
            public CommonUIBuilder.TimerDisplay TimerDisplay;
            public TMP_Text RoundText;
            public TMP_Text KillText;
            public CommonUIBuilder.GoldDisplay GoldDisplay;

            public void SetTimer(float seconds)
            {
                TimerDisplay?.SetTime(seconds);
            }

            public void SetRound(int current, int total)
            {
                if (RoundText != null)
                {
                    RoundText.text = total > 0 ? $"라운드 {current}/{total}" : $"라운드 {current}";
                }
            }

            public void SetKillCount(int count)
            {
                if (KillText != null)
                {
                    KillText.text = $"Kill: {count}";
                }
            }

            public void SetGold(int gold)
            {
                GoldDisplay?.SetGold(gold);
            }
        }

        #endregion

        #region Player Status Bar

        /// <summary>
        /// 플레이어 상태 바를 생성합니다. (체력바, 경험치바)
        /// </summary>
        public static PlayerStatusBar CreatePlayerStatusBar(
            Transform parent,
            int playerIndex,
            string characterName)
        {
            var bar = new PlayerStatusBar { PlayerIndex = playerIndex };

            // 루트 패널
            bar.Root = UIFactory.CreatePanel($"PlayerStatus_{playerIndex}", parent, new Color(0, 0, 0, 0.5f));

            // 캐릭터 이름
            bar.NameText = UIFactory.CreateText("CharacterName", bar.Root, characterName, 20, CommonUIBuilder.TextWhite, TextAlignmentOptions.Left);
            var nameRect = bar.NameText.GetComponent<RectTransform>();
            UIFactory.SetRect(nameRect,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(5, -5),
                new Vector2(-10, 20));

            // 체력바
            bar.HealthBar = CommonUIBuilder.CreateProgressBar(bar.Root, "HealthBar", null, Color.red);
            var healthRect = bar.HealthBar.Root;
            UIFactory.SetRect(healthRect,
                new Vector2(0, 0.4f), new Vector2(1, 0.7f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(-10, 0));

            // 경험치바
            bar.ExpBar = CommonUIBuilder.CreateProgressBar(bar.Root, "ExpBar", null, Color.cyan);
            var expRect = bar.ExpBar.Root;
            UIFactory.SetRect(expRect,
                new Vector2(0, 0.1f), new Vector2(1, 0.35f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(-10, 0));

            return bar;
        }

        public class PlayerStatusBar
        {
            public int PlayerIndex;
            public RectTransform Root;
            public TMP_Text NameText;
            public CommonUIBuilder.ProgressBar HealthBar;
            public CommonUIBuilder.ProgressBar ExpBar;

            public void SetHealth(float current, float max)
            {
                HealthBar?.SetProgress(max > 0 ? current / max : 0);
            }

            public void SetExp(float current, float max)
            {
                ExpBar?.SetProgress(max > 0 ? current / max : 0);
            }

            public void SetName(string name)
            {
                if (NameText != null)
                {
                    NameText.text = name;
                }
            }
        }

        #endregion

        #region Player Status Container

        /// <summary>
        /// 플레이어 상태 컨테이너를 생성합니다. (하단에 모든 플레이어 표시)
        /// </summary>
        public static PlayerStatusContainer CreatePlayerStatusContainer(Transform parent)
        {
            var container = new PlayerStatusContainer();

            // 컨테이너 (하단 중앙)
            container.Root = UIFactory.CreatePanel("PlayerStatusContainer", parent);
            UIFactory.SetRect(container.Root,
                new Vector2(0.2f, 0), new Vector2(0.8f, 0),
                new Vector2(0.5f, 0),
                new Vector2(0, 20),
                new Vector2(0, 80));

            var layout = container.Root.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10f;
            layout.padding = new RectOffset(10, 10, 5, 5);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            return container;
        }

        public class PlayerStatusContainer
        {
            public RectTransform Root;

            public PlayerStatusBar AddPlayer(int index, string characterName)
            {
                return CreatePlayerStatusBar(Root, index, characterName);
            }

            public void Clear()
            {
                if (Root == null) return;
                for (int i = Root.childCount - 1; i >= 0; i--)
                {
                    UnityEngine.Object.Destroy(Root.GetChild(i).gameObject);
                }
            }
        }

        #endregion

        #region Pause Menu

        /// <summary>
        /// 일시정지 메뉴를 생성합니다.
        /// </summary>
        public static PauseMenu CreatePauseMenu(
            Transform parent,
            Action onResume,
            Action onOptions,
            Action onMainMenu)
        {
            var menu = new PauseMenu();

            // 오버레이
            menu.Root = UIFactory.CreateFullScreenPanel("PauseMenu", parent, CommonUIBuilder.BgOverlay);

            // 메뉴 박스
            menu.MenuBox = UIFactory.CreatePanel("MenuBox", menu.Root, CommonUIBuilder.BgPanel);
            UIFactory.SetAnchorPreset(menu.MenuBox, AnchorPreset.MiddleCenter, new Vector2(400, 400));

            // 타이틀
            var titleText = UIFactory.CreateText("Title", menu.MenuBox, "일시정지", 40, CommonUIBuilder.TextWhite, TextAlignmentOptions.Center);
            var titleRect = titleText.GetComponent<RectTransform>();
            UIFactory.SetRect(titleRect,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0.5f, 1),
                new Vector2(0, -30),
                new Vector2(0, 60));

            // 버튼 컨테이너
            var buttonLayout = UIFactory.CreateVerticalLayoutPanel("Buttons", menu.MenuBox, 15f, new RectOffset(50, 50, 0, 0));
            var buttonLayoutRect = buttonLayout.GetComponent<RectTransform>();
            UIFactory.SetRect(buttonLayoutRect,
                new Vector2(0, 0.15f), new Vector2(1, 0.75f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero);

            // 계속하기 버튼
            menu.ResumeButton = UIFactory.CreateButton("ResumeButton", buttonLayout.transform, "계속하기", 28, CommonUIBuilder.BgButtonGreen);
            var resumeRect = menu.ResumeButton.GetComponent<RectTransform>();
            resumeRect.sizeDelta = new Vector2(0, 60);
            menu.ResumeButton.onClick.AddListener(() => onResume?.Invoke());

            // 옵션 버튼
            menu.OptionsButton = UIFactory.CreateButton("OptionsButton", buttonLayout.transform, "옵션", 28, CommonUIBuilder.BgButton);
            var optionsRect = menu.OptionsButton.GetComponent<RectTransform>();
            optionsRect.sizeDelta = new Vector2(0, 60);
            menu.OptionsButton.onClick.AddListener(() => onOptions?.Invoke());

            // 메인 메뉴 버튼
            menu.MainMenuButton = UIFactory.CreateButton("MainMenuButton", buttonLayout.transform, "메인 메뉴", 28, CommonUIBuilder.BgButtonRed);
            var mainMenuRect = menu.MainMenuButton.GetComponent<RectTransform>();
            mainMenuRect.sizeDelta = new Vector2(0, 60);
            menu.MainMenuButton.onClick.AddListener(() => onMainMenu?.Invoke());

            menu.Root.gameObject.SetActive(false);

            return menu;
        }

        public class PauseMenu
        {
            public RectTransform Root;
            public RectTransform MenuBox;
            public Button ResumeButton;
            public Button OptionsButton;
            public Button MainMenuButton;

            public void Show() => Root?.gameObject.SetActive(true);
            public void Hide() => Root?.gameObject.SetActive(false);
            public bool IsVisible => Root != null && Root.gameObject.activeSelf;
        }

        #endregion

        #region Stage Clear / Game Over Overlay

        /// <summary>
        /// 스테이지 클리어 오버레이를 생성합니다.
        /// </summary>
        public static ResultOverlay CreateStageClearOverlay(
            Transform parent,
            Action onContinue)
        {
            return CreateResultOverlay(parent, "스테이지 클리어!", CommonUIBuilder.TextGreen, "계속", onContinue);
        }

        /// <summary>
        /// 게임 오버 오버레이를 생성합니다.
        /// </summary>
        public static ResultOverlay CreateGameOverOverlay(
            Transform parent,
            Action onRetry,
            Action onMainMenu)
        {
            var overlay = new ResultOverlay();

            // 오버레이
            overlay.Root = UIFactory.CreateFullScreenPanel("GameOverOverlay", parent, CommonUIBuilder.BgOverlay);

            // 타이틀
            overlay.TitleText = UIFactory.CreateText("Title", overlay.Root, "게임 오버", 64, CommonUIBuilder.TextRed, TextAlignmentOptions.Center);
            var titleRect = overlay.TitleText.GetComponent<RectTransform>();
            UIFactory.SetAnchorPreset(titleRect, AnchorPreset.MiddleCenter, new Vector2(600, 100));
            titleRect.anchoredPosition = new Vector2(0, 100);

            // 버튼 컨테이너
            var buttonLayout = UIFactory.CreateHorizontalLayoutPanel("Buttons", overlay.Root, 30f);
            var buttonLayoutRect = buttonLayout.GetComponent<RectTransform>();
            UIFactory.SetAnchorPreset(buttonLayoutRect, AnchorPreset.MiddleCenter, new Vector2(500, 80));
            buttonLayoutRect.anchoredPosition = new Vector2(0, -50);

            // 재시도 버튼
            overlay.PrimaryButton = UIFactory.CreateButton("RetryButton", buttonLayout.transform, "재시도", 28, CommonUIBuilder.BgButtonGreen);
            var retryRect = overlay.PrimaryButton.GetComponent<RectTransform>();
            retryRect.sizeDelta = new Vector2(200, 60);
            overlay.PrimaryButton.onClick.AddListener(() => onRetry?.Invoke());

            // 메인 메뉴 버튼
            overlay.SecondaryButton = UIFactory.CreateButton("MainMenuButton", buttonLayout.transform, "메인 메뉴", 28, CommonUIBuilder.BgButton);
            var mainMenuRect = overlay.SecondaryButton.GetComponent<RectTransform>();
            mainMenuRect.sizeDelta = new Vector2(200, 60);
            overlay.SecondaryButton.onClick.AddListener(() => onMainMenu?.Invoke());

            overlay.Root.gameObject.SetActive(false);

            return overlay;
        }

        private static ResultOverlay CreateResultOverlay(
            Transform parent,
            string title,
            Color titleColor,
            string buttonText,
            Action onButtonClick)
        {
            var overlay = new ResultOverlay();

            // 오버레이
            overlay.Root = UIFactory.CreateFullScreenPanel("ResultOverlay", parent, CommonUIBuilder.BgOverlay);

            // 타이틀
            overlay.TitleText = UIFactory.CreateText("Title", overlay.Root, title, 64, titleColor, TextAlignmentOptions.Center);
            var titleRect = overlay.TitleText.GetComponent<RectTransform>();
            UIFactory.SetAnchorPreset(titleRect, AnchorPreset.MiddleCenter, new Vector2(600, 100));
            titleRect.anchoredPosition = new Vector2(0, 50);

            // 버튼
            overlay.PrimaryButton = UIFactory.CreateButton("PrimaryButton", overlay.Root, buttonText, 32, CommonUIBuilder.BgButtonGreen);
            var buttonRect = overlay.PrimaryButton.GetComponent<RectTransform>();
            UIFactory.SetAnchorPreset(buttonRect, AnchorPreset.MiddleCenter, new Vector2(250, 70));
            buttonRect.anchoredPosition = new Vector2(0, -50);
            overlay.PrimaryButton.onClick.AddListener(() => onButtonClick?.Invoke());

            overlay.Root.gameObject.SetActive(false);

            return overlay;
        }

        public class ResultOverlay
        {
            public RectTransform Root;
            public TMP_Text TitleText;
            public Button PrimaryButton;
            public Button SecondaryButton;

            public void Show() => Root?.gameObject.SetActive(true);
            public void Hide() => Root?.gameObject.SetActive(false);
        }

        #endregion

        #region Complete Stage Layout

        /// <summary>
        /// 완전한 스테이지 레이아웃을 생성합니다.
        /// </summary>
        public static StageLayout CreateStageLayout(
            Transform parent,
            Action onPauseResume,
            Action onPauseOptions,
            Action onPauseMainMenu)
        {
            var layout = new StageLayout();

            // HUD (Canvas 위에 배치)
            layout.HUD = CreateStageHUD(parent);

            // 플레이어 상태 컨테이너
            layout.PlayerStatusContainer = CreatePlayerStatusContainer(parent);

            // 일시정지 메뉴
            layout.PauseMenu = CreatePauseMenu(parent, onPauseResume, onPauseOptions, onPauseMainMenu);

            return layout;
        }

        public class StageLayout
        {
            public StageHUD HUD;
            public PlayerStatusContainer PlayerStatusContainer;
            public PauseMenu PauseMenu;
        }

        #endregion
    }
}

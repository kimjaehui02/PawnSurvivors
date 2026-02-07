using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

namespace PawnSurvivors.UI.Factory
{
    /// <summary>
    /// 메뉴 화면들의 UI를 빌드합니다. (캐릭터 선택, 게임오버 등)
    /// </summary>
    public static class MenuUIBuilder
    {
        #region Character Select

        /// <summary>
        /// 캐릭터 선택 카드를 생성합니다.
        /// </summary>
        public static CharacterCard CreateCharacterCard(
            Transform parent,
            string characterName,
            string description,
            Sprite portrait = null,
            Action onClick = null)
        {
            var card = new CharacterCard();

            // 카드 루트
            card.Root = UIFactory.CreatePanel($"Card_{characterName}", parent, CommonUIBuilder.BgSlot);
            var button = card.Root.gameObject.AddComponent<Button>();
            card.Button = button;

            var image = card.Root.GetComponent<Image>() ?? card.Root.gameObject.AddComponent<Image>();
            image.color = CommonUIBuilder.BgSlot;
            button.targetGraphic = image;

            if (onClick != null)
            {
                button.onClick.AddListener(() => onClick());
            }

            // 초상화
            card.Portrait = UIFactory.CreateImage("Portrait", card.Root, Color.gray);
            var portraitRect = card.Portrait.GetComponent<RectTransform>();
            UIFactory.SetRect(portraitRect,
                new Vector2(0.5f, 1), new Vector2(0.5f, 1),
                new Vector2(0.5f, 1),
                new Vector2(0, -20),
                new Vector2(150, 150));
            if (portrait != null)
            {
                card.Portrait.sprite = portrait;
                card.Portrait.color = Color.white;
            }

            // 이름
            card.NameText = UIFactory.CreateText("Name", card.Root, characterName, 28, CommonUIBuilder.TextWhite, TextAlignmentOptions.Center);
            var nameRect = card.NameText.GetComponent<RectTransform>();
            UIFactory.SetRect(nameRect,
                new Vector2(0, 0.3f), new Vector2(1, 0.45f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero);

            // 설명
            card.DescriptionText = UIFactory.CreateText("Description", card.Root, description, 20, CommonUIBuilder.TextGray, TextAlignmentOptions.Center);
            var descRect = card.DescriptionText.GetComponent<RectTransform>();
            UIFactory.SetRect(descRect,
                new Vector2(0, 0.05f), new Vector2(1, 0.3f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(-20, 0));
            card.DescriptionText.enableWordWrapping = true;

            // 선택 표시 (기본 숨김)
            card.SelectionBorder = UIFactory.CreateImage("SelectionBorder", card.Root, Color.clear);
            var borderRect = card.SelectionBorder.GetComponent<RectTransform>();
            UIFactory.SetAnchorPreset(borderRect, AnchorPreset.StretchAll, Vector2.zero);
            card.SelectionBorder.raycastTarget = false;

            return card;
        }

        public class CharacterCard
        {
            public RectTransform Root;
            public Button Button;
            public Image Portrait;
            public TMP_Text NameText;
            public TMP_Text DescriptionText;
            public Image SelectionBorder;

            private bool _isSelected;

            public void SetSelected(bool selected)
            {
                _isSelected = selected;
                if (SelectionBorder != null)
                {
                    SelectionBorder.color = selected ? new Color(0f, 1f, 0f, 0.5f) : Color.clear;
                }

                var rootImage = Root?.GetComponent<Image>();
                if (rootImage != null)
                {
                    rootImage.color = selected ? new Color(0.2f, 0.4f, 0.2f, 1f) : CommonUIBuilder.BgSlot;
                }
            }

            public bool IsSelected => _isSelected;
        }

        /// <summary>
        /// 캐릭터 선택 화면 레이아웃을 생성합니다.
        /// </summary>
        public static CharacterSelectLayout CreateCharacterSelectLayout(
            Transform parent,
            Action onStartGame,
            Action onBack)
        {
            var layout = new CharacterSelectLayout();

            // 배경
            layout.Root = UIFactory.CreateFullScreenPanel("CharacterSelectRoot", parent, CommonUIBuilder.BgDark);

            // 타이틀
            layout.TitleText = UIFactory.CreateText("Title", layout.Root, "캐릭터 선택", 48, CommonUIBuilder.TextWhite, TextAlignmentOptions.Center);
            var titleRect = layout.TitleText.GetComponent<RectTransform>();
            UIFactory.SetRect(titleRect,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0.5f, 1),
                new Vector2(0, -50),
                new Vector2(0, 80));

            // 캐릭터 카드 컨테이너
            layout.CardContainer = UIFactory.CreatePanel("CardContainer", layout.Root);
            UIFactory.SetRect(layout.CardContainer,
                new Vector2(0.1f, 0.2f), new Vector2(0.9f, 0.85f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero);

            var gridLayout = layout.CardContainer.gameObject.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(250, 350);
            gridLayout.spacing = new Vector2(30, 30);
            gridLayout.padding = new RectOffset(20, 20, 20, 20);
            gridLayout.childAlignment = TextAnchor.MiddleCenter;

            // 하단 버튼 영역
            var buttonArea = UIFactory.CreateHorizontalLayoutPanel("ButtonArea", layout.Root, 30f);
            var buttonAreaRect = buttonArea.GetComponent<RectTransform>();
            UIFactory.SetRect(buttonAreaRect,
                new Vector2(0.3f, 0), new Vector2(0.7f, 0),
                new Vector2(0.5f, 0),
                new Vector2(0, 60),
                new Vector2(0, 70));

            // 뒤로가기 버튼
            layout.BackButton = UIFactory.CreateButton("BackButton", buttonArea.transform, "뒤로", 28, CommonUIBuilder.BgButton);
            var backRect = layout.BackButton.GetComponent<RectTransform>();
            backRect.sizeDelta = new Vector2(200, 60);
            layout.BackButton.onClick.AddListener(() => onBack?.Invoke());

            // 시작 버튼
            layout.StartButton = UIFactory.CreateButton("StartButton", buttonArea.transform, "게임 시작", 28, CommonUIBuilder.BgButtonGreen);
            var startRect = layout.StartButton.GetComponent<RectTransform>();
            startRect.sizeDelta = new Vector2(200, 60);
            layout.StartButton.onClick.AddListener(() => onStartGame?.Invoke());

            return layout;
        }

        public class CharacterSelectLayout
        {
            public RectTransform Root;
            public TMP_Text TitleText;
            public RectTransform CardContainer;
            public Button BackButton;
            public Button StartButton;
            public List<CharacterCard> Cards = new();

            public CharacterCard AddCharacter(string name, string description, Sprite portrait, Action onClick)
            {
                var card = CreateCharacterCard(CardContainer, name, description, portrait, onClick);
                Cards.Add(card);
                return card;
            }

            public void ClearCards()
            {
                foreach (var card in Cards)
                {
                    if (card.Root != null)
                    {
                        UnityEngine.Object.Destroy(card.Root.gameObject);
                    }
                }
                Cards.Clear();
            }
        }

        #endregion

        #region Game Over

        /// <summary>
        /// 게임오버 화면 레이아웃을 생성합니다.
        /// </summary>
        public static GameOverLayout CreateGameOverLayout(
            Transform parent,
            bool isVictory,
            Action onRetry,
            Action onMainMenu)
        {
            var layout = new GameOverLayout();

            // 배경
            layout.Root = UIFactory.CreateFullScreenPanel("GameOverRoot", parent, CommonUIBuilder.BgDark);

            // 타이틀
            string titleText = isVictory ? "승리!" : "게임 오버";
            Color titleColor = isVictory ? CommonUIBuilder.TextGreen : CommonUIBuilder.TextRed;

            layout.TitleText = UIFactory.CreateText("Title", layout.Root, titleText, 72, titleColor, TextAlignmentOptions.Center);
            var titleRect = layout.TitleText.GetComponent<RectTransform>();
            UIFactory.SetAnchorPreset(titleRect, AnchorPreset.MiddleCenter, new Vector2(600, 100));
            titleRect.anchoredPosition = new Vector2(0, 150);

            // 통계 패널
            layout.StatsPanel = UIFactory.CreatePanel("StatsPanel", layout.Root, CommonUIBuilder.BgPanel);
            UIFactory.SetAnchorPreset(layout.StatsPanel, AnchorPreset.MiddleCenter, new Vector2(500, 200));
            layout.StatsPanel.anchoredPosition = new Vector2(0, 0);

            var statsLayout = layout.StatsPanel.gameObject.AddComponent<VerticalLayoutGroup>();
            statsLayout.spacing = 15f;
            statsLayout.padding = new RectOffset(30, 30, 20, 20);
            statsLayout.childControlWidth = true;
            statsLayout.childControlHeight = false;

            // 통계 텍스트들
            layout.KillCountText = UIFactory.CreateText("KillCount", layout.StatsPanel, "처치 수: 0", 28, CommonUIBuilder.TextWhite, TextAlignmentOptions.Left);
            layout.KillCountText.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40);

            layout.SurvivalTimeText = UIFactory.CreateText("SurvivalTime", layout.StatsPanel, "생존 시간: 00:00", 28, CommonUIBuilder.TextWhite, TextAlignmentOptions.Left);
            layout.SurvivalTimeText.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40);

            layout.GoldEarnedText = UIFactory.CreateText("GoldEarned", layout.StatsPanel, "획득 골드: 0", 28, CommonUIBuilder.TextWhite, TextAlignmentOptions.Left);
            layout.GoldEarnedText.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40);

            // 버튼 영역
            var buttonArea = UIFactory.CreateHorizontalLayoutPanel("ButtonArea", layout.Root, 30f);
            var buttonAreaRect = buttonArea.GetComponent<RectTransform>();
            UIFactory.SetAnchorPreset(buttonAreaRect, AnchorPreset.MiddleCenter, new Vector2(500, 80));
            buttonAreaRect.anchoredPosition = new Vector2(0, -150);

            // 재시도 버튼
            layout.RetryButton = UIFactory.CreateButton("RetryButton", buttonArea.transform, "재시도", 28, CommonUIBuilder.BgButtonGreen);
            var retryRect = layout.RetryButton.GetComponent<RectTransform>();
            retryRect.sizeDelta = new Vector2(200, 60);
            layout.RetryButton.onClick.AddListener(() => onRetry?.Invoke());

            // 메인 메뉴 버튼
            layout.MainMenuButton = UIFactory.CreateButton("MainMenuButton", buttonArea.transform, "메인 메뉴", 28, CommonUIBuilder.BgButton);
            var mainMenuRect = layout.MainMenuButton.GetComponent<RectTransform>();
            mainMenuRect.sizeDelta = new Vector2(200, 60);
            layout.MainMenuButton.onClick.AddListener(() => onMainMenu?.Invoke());

            return layout;
        }

        public class GameOverLayout
        {
            public RectTransform Root;
            public TMP_Text TitleText;
            public RectTransform StatsPanel;
            public TMP_Text KillCountText;
            public TMP_Text SurvivalTimeText;
            public TMP_Text GoldEarnedText;
            public Button RetryButton;
            public Button MainMenuButton;

            public void SetStats(int kills, float survivalTime, int gold)
            {
                if (KillCountText != null)
                    KillCountText.text = $"처치 수: {kills}";
                if (SurvivalTimeText != null)
                {
                    int minutes = Mathf.FloorToInt(survivalTime / 60f);
                    int seconds = Mathf.FloorToInt(survivalTime % 60f);
                    SurvivalTimeText.text = $"생존 시간: {minutes:00}:{seconds:00}";
                }
                if (GoldEarnedText != null)
                    GoldEarnedText.text = $"획득 골드: {gold}";
            }
        }

        #endregion

        #region Main Menu

        /// <summary>
        /// 메인 메뉴 레이아웃을 생성합니다.
        /// </summary>
        public static MainMenuLayout CreateMainMenuLayout(
            Transform parent,
            Action onNewGame,
            Action onContinue,
            Action onOptions,
            Action onQuit)
        {
            var layout = new MainMenuLayout();

            // 배경
            layout.Root = UIFactory.CreateFullScreenPanel("MainMenuRoot", parent, CommonUIBuilder.BgDark);

            // 타이틀
            layout.TitleText = UIFactory.CreateText("Title", layout.Root, "Pawn Survivors", 72, CommonUIBuilder.TextWhite, TextAlignmentOptions.Center);
            var titleRect = layout.TitleText.GetComponent<RectTransform>();
            UIFactory.SetAnchorPreset(titleRect, AnchorPreset.TopCenter, new Vector2(600, 100));
            titleRect.anchoredPosition = new Vector2(0, -100);

            // 버튼 컨테이너
            var buttonLayout = UIFactory.CreateVerticalLayoutPanel("Buttons", layout.Root, 20f);
            var buttonLayoutRect = buttonLayout.GetComponent<RectTransform>();
            UIFactory.SetAnchorPreset(buttonLayoutRect, AnchorPreset.MiddleCenter, new Vector2(300, 350));

            // 새 게임 버튼
            layout.NewGameButton = UIFactory.CreateButton("NewGameButton", buttonLayout.transform, "새 게임", 32, CommonUIBuilder.BgButtonGreen);
            layout.NewGameButton.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 70);
            layout.NewGameButton.onClick.AddListener(() => onNewGame?.Invoke());

            // 계속하기 버튼
            layout.ContinueButton = UIFactory.CreateButton("ContinueButton", buttonLayout.transform, "계속하기", 32, CommonUIBuilder.BgButton);
            layout.ContinueButton.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 70);
            layout.ContinueButton.onClick.AddListener(() => onContinue?.Invoke());

            // 옵션 버튼
            layout.OptionsButton = UIFactory.CreateButton("OptionsButton", buttonLayout.transform, "옵션", 32, CommonUIBuilder.BgButton);
            layout.OptionsButton.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 70);
            layout.OptionsButton.onClick.AddListener(() => onOptions?.Invoke());

            // 종료 버튼
            layout.QuitButton = UIFactory.CreateButton("QuitButton", buttonLayout.transform, "종료", 32, CommonUIBuilder.BgButtonRed);
            layout.QuitButton.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 70);
            layout.QuitButton.onClick.AddListener(() => onQuit?.Invoke());

            return layout;
        }

        public class MainMenuLayout
        {
            public RectTransform Root;
            public TMP_Text TitleText;
            public Button NewGameButton;
            public Button ContinueButton;
            public Button OptionsButton;
            public Button QuitButton;
        }

        #endregion
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 스테이지 클리어 화면입니다.
    /// </summary>
    public class StageClearScreen : UIScreen
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text clearText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private Button nextStageButton;
        [SerializeField] private Button mainMenuButton;

        protected override void Awake()
        {
            base.Awake();

            if (nextStageButton != null)
                nextStageButton.onClick.AddListener(OnNextStageButtonClicked);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }

        protected override void OnShow()
        {
            base.OnShow();
            
            if (clearText != null)
                clearText.text = "STAGE CLEAR!";
        }

        private void OnNextStageButtonClicked()
        {
            // TODO: 다음 스테이지 로드
            UIManager.Instance?.StartStage();
        }

        private void OnMainMenuButtonClicked()
        {
            UIManager.Instance?.ReturnToMainMenu();
        }

        /// <summary>
        /// 클리어 정보를 설정합니다.
        /// </summary>
        public void SetClearInfo(int score, float time)
        {
            if (scoreText != null)
                scoreText.text = $"Score: {score}";
            
            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                timeText.text = $"Time: {minutes:00}:{seconds:00}";
            }
        }
    }
}


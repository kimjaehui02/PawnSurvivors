using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 게임 오버 화면입니다.
    /// </summary>
    public class GameOverScreen : UIScreen
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text gameOverText;
        [SerializeField] private TMP_Text finalScoreText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button mainMenuButton;

        protected override void Awake()
        {
            base.Awake();

            if (retryButton != null)
                retryButton.onClick.AddListener(OnRetryButtonClicked);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }

        protected override void OnShow()
        {
            base.OnShow();
            
            if (gameOverText != null)
                gameOverText.text = "GAME OVER";
            
            // TODO: 최종 점수 표시
            if (finalScoreText != null)
                finalScoreText.text = "Final Score: 0";
        }

        private void OnRetryButtonClicked()
        {
            UIManager.Instance?.StartStage();
        }

        private void OnMainMenuButtonClicked()
        {
            UIManager.Instance?.ReturnToMainMenu();
        }

        /// <summary>
        /// 최종 점수를 설정합니다.
        /// </summary>
        public void SetFinalScore(int score)
        {
            if (finalScoreText != null)
                finalScoreText.text = $"Final Score: {score}";
        }
    }
}


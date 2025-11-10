using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 게임 오버 화면입니다. 씬에 배치하면 자동으로 작동합니다.
    /// </summary>
    public class GameOverScreen : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text gameOverText;
        [SerializeField] private TMP_Text finalScoreText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            if (retryButton != null)
                retryButton.onClick.AddListener(OnRetryButtonClicked);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);

            // 기본적으로 숨김
            gameObject.SetActive(false);
        }

        public void Show(int score = 0)
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f;

            if (gameOverText != null)
                gameOverText.text = "GAME OVER";
            
            if (finalScoreText != null)
                finalScoreText.text = $"Final Score: {score}";
        }

        private void OnRetryButtonClicked()
        {
            Time.timeScale = 1f;
            // TODO: 게임 재시작
            Debug.Log("Retry Button Clicked");
        }

        private void OnMainMenuButtonClicked()
        {
            Time.timeScale = 1f;
            // TODO: 메인 메뉴로 이동
            Debug.Log("Main Menu Button Clicked");
        }
    }
}

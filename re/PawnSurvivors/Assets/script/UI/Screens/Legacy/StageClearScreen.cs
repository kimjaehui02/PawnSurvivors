using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PawnSurvivors.Managers;

namespace PawnSurvivors.UI.Legacy
{
    /// <summary>
    /// 스테이지 클리어 화면입니다. 씬에 배치하면 자동으로 작동합니다.
    /// </summary>
    public class StageClearScreen : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text clearText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private Button nextStageButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            if (nextStageButton != null)
                nextStageButton.onClick.AddListener(OnNextStageButtonClicked);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);

            // 기본적으로 숨김
            gameObject.SetActive(false);
        }

        public void Show(int score = 0, float time = 0f)
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f;

            if (clearText != null)
                clearText.text = "STAGE CLEAR!";
            
            if (scoreText != null)
                scoreText.text = $"Score: {score}";
            
            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                timeText.text = $"Time: {minutes:00}:{seconds:00}";
            }
        }

        private void OnNextStageButtonClicked()
        {
            Time.timeScale = 1f;
            // TODO: 다음 스테이지 시작
            LogManager.LogInfo(LogCategory.UI, "Next Stage Button Clicked");
        }

        private void OnMainMenuButtonClicked()
        {
            Time.timeScale = 1f;
            // TODO: 메인 메뉴로 이동
            LogManager.LogInfo(LogCategory.UI, "Main Menu Button Clicked");
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 일시정지 메뉴 화면입니다.
    /// </summary>
    public class PauseMenuScreen : UIScreen
    {
        [Header("UI Elements")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;

        protected override void Awake()
        {
            base.Awake();

            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeButtonClicked);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void Update()
        {
            // 일시정지 상태에서 ESC 키로 재개
            if (UIManager.Instance?.CurrentState == GameState.Paused)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    OnResumeButtonClicked();
                }
            }
        }

        private void OnResumeButtonClicked()
        {
            UIManager.Instance?.ResumeGame();
        }

        private void OnMainMenuButtonClicked()
        {
            UIManager.Instance?.ReturnToMainMenu();
        }

        private void OnQuitButtonClicked()
        {
            UIManager.Instance?.QuitGame();
        }
    }
}


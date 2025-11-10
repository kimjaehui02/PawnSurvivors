using UnityEngine;
using UnityEngine.UI;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 게임 시작 시 표시되는 타이틀 화면입니다.
    /// </summary>
    public class TitleScreen : UIScreen
    {
        [Header("UI Elements")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;

        protected override void Awake()
        {
            base.Awake();

            if (startButton != null)
                startButton.onClick.AddListener(OnStartButtonClicked);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnStartButtonClicked()
        {
            UIManager.Instance?.StartGame();
        }

        private void OnQuitButtonClicked()
        {
            UIManager.Instance?.QuitGame();
        }

        protected override void OnShow()
        {
            base.OnShow();
            Debug.Log("Title Screen Shown");
        }
    }
}


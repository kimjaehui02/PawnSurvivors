using UnityEngine;
using UnityEngine.UI;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 타이틀 화면입니다. 씬에 배치하면 자동으로 작동합니다.
    /// </summary>
    public class TitleScreen : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartButtonClicked);
            }
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitButtonClicked);
            }
        }

        private void OnStartButtonClicked()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowCharacterSelectScreen();
            }
        }

        private void OnQuitButtonClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

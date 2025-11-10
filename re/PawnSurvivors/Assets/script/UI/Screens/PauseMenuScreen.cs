using UnityEngine;
using UnityEngine.UI;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 일시정지 메뉴 화면입니다. 씬에 배치하면 자동으로 작동합니다.
    /// </summary>
    public class PauseMenuScreen : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeButtonClicked);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitButtonClicked);

            // 기본적으로 숨김
            gameObject.SetActive(false);
        }

        private void Update()
        {
            // ESC 키로 일시정지 토글
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (gameObject.activeSelf)
                {
                    OnResumeButtonClicked();
                }
                else
                {
                    Show();
                }
            }
        }

        private void Show()
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f;
        }

        private void OnResumeButtonClicked()
        {
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

        private void OnMainMenuButtonClicked()
        {
            Time.timeScale = 1f;
            // TODO: 메인 메뉴 씬으로 이동
            Debug.Log("Main Menu Button Clicked");
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

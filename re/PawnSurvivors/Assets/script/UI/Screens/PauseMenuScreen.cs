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

        private void Awake()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeButtonClicked);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        // Inspector에서 이 GameObject를 처음부터 비활성화 상태로 설정하세요!

    // Update()와 Show() 제거: StageScreen에서 이미 ESC 처리하므로 불필요

    private void OnResumeButtonClicked()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HidePauseMenu();
        }
        else
        {
            // 폴백: UIManager가 없을 경우
            gameObject.SetActive(false);
            if (GameManager.Instance?.LifecycleManager != null)
            {
                GameManager.Instance.LifecycleManager.TogglePause();
            }
        }
    }

    private void OnMainMenuButtonClicked()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ReturnToMainMenu();
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

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 메인 메뉴 화면입니다. 씬에 배치하면 자동으로 작동합니다.
    /// </summary>
    public class MainMenuScreen : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button startStageButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_Text stageNameText;

        private string _selectedStage = "Stage1";

        private void Start()
        {
            if (startStageButton != null)
                startStageButton.onClick.AddListener(OnStartStageButtonClicked);
            
            if (backButton != null)
                backButton.onClick.AddListener(OnBackButtonClicked);

            UpdateStageInfo();
        }

        private void UpdateStageInfo()
        {
            if (stageNameText != null)
            {
                stageNameText.text = $"Selected Stage: {_selectedStage}";
            }
        }

        private void OnStartStageButtonClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartStage(_selectedStage);
            }
        }

        private void OnBackButtonClicked()
        {
            // TODO: 타이틀 씬으로 이동
            Debug.Log("Back Button Clicked");
        }

        public void SelectStage(string stageName)
        {
            _selectedStage = stageName;
            UpdateStageInfo();
        }
    }
}

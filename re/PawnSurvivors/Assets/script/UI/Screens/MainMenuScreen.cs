using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 메인 메뉴 화면입니다. 스테이지 선택 등의 기능을 제공합니다.
    /// </summary>
    public class MainMenuScreen : UIScreen
    {
        [Header("UI Elements")]
        [SerializeField] private Button startStageButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_Text stageNameText;

        private string _selectedStage = "Stage1"; // 기본 스테이지

        protected override void Awake()
        {
            base.Awake();

            if (startStageButton != null)
                startStageButton.onClick.AddListener(OnStartStageButtonClicked);
            
            if (backButton != null)
                backButton.onClick.AddListener(OnBackButtonClicked);
        }

        protected override void OnShow()
        {
            base.OnShow();
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
            // TODO: 선택된 스테이지 정보를 GameManager에 전달
            UIManager.Instance?.StartStage();
        }

        private void OnBackButtonClicked()
        {
            UIManager.Instance?.ReturnToTitle();
        }

        /// <summary>
        /// 스테이지를 선택합니다.
        /// </summary>
        public void SelectStage(string stageName)
        {
            _selectedStage = stageName;
            UpdateStageInfo();
        }
    }
}


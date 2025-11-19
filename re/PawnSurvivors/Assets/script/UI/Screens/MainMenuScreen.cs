using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PawnSurvivors.Managers;

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
        [SerializeField] private TMP_Dropdown stageDropdown;

        private string _selectedStage = "Stage1";

        private void Start()
        {
            if (startStageButton != null)
                startStageButton.onClick.AddListener(OnStartStageButtonClicked);
            
            if (backButton != null)
                backButton.onClick.AddListener(OnBackButtonClicked);

            if (stageDropdown != null)
                stageDropdown.onValueChanged.AddListener(OnStageDropdownChanged);

            InitializeStageDropdown();
            UpdateStageInfo();
        }

        private void InitializeStageDropdown()
        {
            if (stageDropdown != null && GameManager.Instance?._stageLoader != null)
            {
                // 스테이지 목록 가져오기
                var stageNames = GameManager.Instance._stageLoader.GetAllStageNames();
                
                // Dropdown 옵션 설정
                stageDropdown.ClearOptions();
                stageDropdown.AddOptions(stageNames);

                // 첫 번째 스테이지를 기본 선택
                if (stageNames.Count > 0)
                {
                    _selectedStage = stageNames[0];
                    stageDropdown.value = 0;
                }
            }
        }

        private void UpdateStageInfo()
        {
            if (stageNameText != null)
            {
                stageNameText.text = $"Selected Stage: {_selectedStage}";
            }
        }

        private void OnStageDropdownChanged(int index)
        {
            if (GameManager.Instance?._stageLoader != null)
            {
                var stageNames = GameManager.Instance._stageLoader.GetAllStageNames();
                if (index >= 0 && index < stageNames.Count)
                {
                    SelectStage(stageNames[index]);
                }
            }
        }

        private void OnStartStageButtonClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartStage(_selectedStage);
                
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowStageScreen();
                }
            }
        }

        private void OnBackButtonClicked()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowTitleScreen();
            }
        }

        public void SelectStage(string stageName)
        {
            _selectedStage = stageName;
            UpdateStageInfo();
        }
    }
}

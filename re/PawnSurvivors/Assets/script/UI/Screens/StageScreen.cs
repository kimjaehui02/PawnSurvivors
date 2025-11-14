using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 메인 메뉴 화면입니다. 씬에 배치하면 자동으로 작동합니다.
    /// </summary>
    public class StageScreen : MonoBehaviour
    {


        [SerializeField] private string _selectedStage = "Stage1";

        [SerializeField] private float stageTime = 0f;
        
        #region UI Elements
        [Header("UI Elements")]

        [SerializeField] private Button OptionButton;

        [SerializeField] private TMP_Text stageTimeText;
        #endregion


        #region Unity Lifecycle
        private void Start()
        {


            if (OptionButton != null)
            {
                OptionButton.onClick.AddListener(OnOptionButtonClicked);
            }


        }

        private void Update()
        {
            UpdateStageTime();
        }
        #endregion

        #region Button Clicked Events
        public void OnOptionButtonClicked()
        {
            Debug.Log("Option Button Clicked");
        }


        #endregion

        #region Update methods
        private void UpdateStageTime()
        {
            if (stageTimeText != null)
            {
                stageTimeText.text = $"Time: {Time.time:00}:{Time.time:00}";
            }
        }
        #endregion

        public void StageStart()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartStage(_selectedStage);
            }
        }

        public void StageEnd()
        {
            if (GameManager.Instance != null)
            {
                // GameManager.Instance.EndStage(_selectedStage);
            }
        }

        public void StagePause()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LifecycleManager.TogglePause();
            }
        }
    }
}

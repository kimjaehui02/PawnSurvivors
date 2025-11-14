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

        [SerializeField] private float stageTime = 600f;
        
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
            HandleInput();
        }
        #endregion

        #region Update methods
        private void UpdateStageTime()
        {
            float deltaTime = GetGameDeltaTime();
            
            if (stageTimeText != null)
            {
                float gameTime = GetGameTime();
                int minutes = Mathf.FloorToInt(gameTime / 60f);
                int seconds = Mathf.FloorToInt(gameTime % 60f);
                stageTimeText.text = $"Time: {minutes:00}:{seconds:00}";
            }

            stageTime -= deltaTime;
            if (stageTime <= 0)
            {
                StageEnd();
            }
        }
        
        private float GetGameDeltaTime()
        {
            if (GameManager.Instance?.LifecycleManager != null)
            {
                return GameManager.Instance.LifecycleManager.GameDeltaTime;
            }
            return Time.deltaTime; // 폴백
        }
        
        private float GetGameTime()
        {
            if (GameManager.Instance?.LifecycleManager != null)
            {
                return GameManager.Instance.LifecycleManager.GameTime;
            }
            return Time.time; // 폴백
        }


        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OptionAndPause();
            }
        }

        
        #endregion

        #region Button Clicked Events
        public void OnOptionButtonClicked()
        {
            Debug.Log("Option Button Clicked");
            OptionAndPause();
        }


        #endregion




        #region Usecase methods

        #region 1단계
        public void OptionAndPause()
        {
            OpenOptionMenu();
            StagePause();
        }
        #endregion

        #region 0단계
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

        public void OpenOptionMenu()
        {
            Debug.Log("Option Menu Opened");
        }
        #endregion

        #endregion
    }
}

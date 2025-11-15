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

        private float stageTime = 0f; // JSON에서 초기화됨
        
        #region UI Elements
        [Header("UI Elements")]

        [SerializeField] private Button OptionButton;

        [SerializeField] private TMP_Text stageTimeText;
        [SerializeField] private TMP_Text healthText;
        #endregion


        #region Unity Lifecycle
        private void Start()
        {
            if (OptionButton != null)
            {
                OptionButton.onClick.AddListener(OnOptionButtonClicked);
            }

            InitializeStageData();
        }

        private void Update()
        {
            UpdateStageTime();
            UpdateHealth();
            // HandleInput() 제거: UIManager에서 ESC 처리
        }
        #endregion

        #region Initialization
        private void InitializeStageData()
        {
            if (GameManager.Instance != null)
            {
                PawnSurvivors.Managers.StageData stageData = GameManager.Instance.LoadStage(_selectedStage);
                if (stageData != null)
                {
                    stageTime = stageData.stageDuration;
                    Debug.Log($"Stage '{_selectedStage}' loaded. Duration: {stageTime}s");
                }
                else
                {
                    Debug.LogWarning($"StageData for '{_selectedStage}' not found. Using default duration.");
                    stageTime = 300f; // 기본값
                }
            }
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

        private void UpdateHealth()
        {
            if (healthText != null && GameManager.Instance?.CreationManager != null)
            {
                // Player 폰을 찾아서 체력 표시
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    var pawnManager = playerObject.GetComponent<PawnManager>();
                    if (pawnManager != null && pawnManager.PawnData != null)
                    {
                        var healthData = pawnManager.PawnData.healthData;
                        healthText.text = $"HP: {healthData.currentHealth:F0}/{healthData.maxHealth:F0}";
                    }
                }
                else
                {
                    healthText.text = "HP: --/--";
                }
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
        
        #endregion

        #region Button Clicked Events
        public void OnOptionButtonClicked()
        {
            // 옵션 버튼 클릭 시 UIManager를 통해 일시정지 메뉴 표시
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPauseMenu();
            }
        }
        #endregion

        #region Usecase methods

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
                GameManager.Instance.StageManager?.EndStage();
                
                // TODO: 클리어인지 게임오버인지에 따라 다른 화면 표시
                // 현재는 임시로 메인 메뉴로 이동
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ReturnToMainMenu();
                }
            }
        }

        // StagePause(), OpenOptionMenu(), OptionAndPause() 제거
        // UIManager가 ESC와 일시정지를 중앙에서 관리
        #endregion

        #endregion
    }
}

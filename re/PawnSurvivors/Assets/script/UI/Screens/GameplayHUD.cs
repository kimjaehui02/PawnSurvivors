using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 게임 플레이 중 표시되는 HUD입니다. 씬에 배치하면 자동으로 작동합니다.
    /// </summary>
    public class GameplayHUD : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private Slider healthSlider;

        private PawnManager _playerPawn;
        private float _gameTime;

        private void Start()
        {
            _gameTime = 0f;
            FindPlayerPawn();
        }

        private void Update()
        {
            UpdateGameTime();
            UpdatePlayerHealth();
        }

        /// <summary>
        /// 플레이어 Pawn을 찾습니다.
        /// </summary>
        private void FindPlayerPawn()
        {
            foreach (var pawn in PawnManager.AllPawnManagers)
            {
                if (pawn.gameObject.CompareTag("Player"))
                {
                    _playerPawn = pawn;
                    break;
                }
            }
        }

        /// <summary>
        /// 게임 시간을 업데이트합니다.
        /// </summary>
        private void UpdateGameTime()
        {
            _gameTime += Time.deltaTime;
            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(_gameTime / 60f);
                int seconds = Mathf.FloorToInt(_gameTime % 60f);
                timeText.text = $"Time: {minutes:00}:{seconds:00}";
            }
        }

        /// <summary>
        /// 플레이어 체력을 업데이트합니다.
        /// </summary>
        private void UpdatePlayerHealth()
        {
            if (_playerPawn == null || _playerPawn.PawnData == null)
            {
                FindPlayerPawn();
                return;
            }

            float currentHealth = _playerPawn.PawnData.healthData.currentHealth;
            float maxHealth = _playerPawn.PawnData.healthData.maxHealth;

            if (healthText != null)
            {
                healthText.text = $"HP: {currentHealth:F0}/{maxHealth:F0}";
            }

            if (healthSlider != null)
            {
                healthSlider.value = maxHealth > 0 ? currentHealth / maxHealth : 0f;
            }
        }

        /// <summary>
        /// 점수를 업데이트합니다.
        /// </summary>
        public void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }
        }
    }
}

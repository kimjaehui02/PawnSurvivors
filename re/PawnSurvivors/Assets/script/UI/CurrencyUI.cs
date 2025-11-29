using UnityEngine;
using TMPro;
using PawnSurvivors.Managers;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 골드(재화)를 표시하는 간단한 UI 컴포넌트입니다.
    /// StageScreen 스타일로 가볍게 구현되었습니다.
    /// </summary>
    public class CurrencyUI : MonoBehaviour
    {
        private TMP_Text _goldText;
        private bool _uiCreated = false;

        [Header("UI Settings")]
        [Tooltip("골드 텍스트의 위치 (화면 기준, 0~1)")]
        public Vector2 anchorPosition = new Vector2(0.95f, 0.95f); // 우측 상단
        
        [Tooltip("골드 텍스트 크기")]
        public Vector2 textSize = new Vector2(200f, 40f);
        
        [Tooltip("폰트 크기")]
        public int fontSize = 24;

        private void Start()
        {
            if (!_uiCreated)
            {
                CreateUI();
                _uiCreated = true;
            }
        }

        private void OnEnable()
        {
            if (!_uiCreated)
            {
                CreateUI();
                _uiCreated = true;
            }
            
            if (_goldText != null)
            {
                _goldText.gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            if (_goldText != null)
            {
                _goldText.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            UpdateGoldDisplay();
        }

        private void CreateUI()
        {
            // Canvas 찾기
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = FindFirstObjectByType<Canvas>();
            }

            if (canvas == null)
            {
                LogManager.LogError(LogCategory.UI, "Canvas를 찾을 수 없어 UI를 생성할 수 없습니다.");
                return;
            }

            // 골드 텍스트 생성
            GameObject goldTextObj = new GameObject("GoldText");
            goldTextObj.transform.SetParent(canvas.transform, false);
            
            var goldRect = goldTextObj.AddComponent<RectTransform>();
            _goldText = goldTextObj.AddComponent<TextMeshProUGUI>();
            
            // 위치 설정
            goldRect.anchorMin = anchorPosition;
            goldRect.anchorMax = anchorPosition;
            goldRect.pivot = new Vector2(0.5f, 0.5f);
            goldRect.anchoredPosition = Vector2.zero;
            goldRect.sizeDelta = textSize;
            
            // 텍스트 설정
            _goldText.text = "골드: 0";
            _goldText.fontSize = fontSize;
            _goldText.color = Color.yellow;
            _goldText.alignment = TextAlignmentOptions.Center;
            
            // 한글 폰트 적용 (있으면)
            TMP_FontAsset nanumFont = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
            #if UNITY_EDITOR
            if (nanumFont == null)
            {
                nanumFont = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/NanumGothic SDF.asset");
            }
            #endif
            if (nanumFont != null)
            {
                _goldText.font = nanumFont;
            }
        }

        private void UpdateGoldDisplay()
        {
            if (_goldText == null) return;
            if (GameManager.Instance?.CurrencyUseCase == null) return;

            int gold = GameManager.Instance.CurrencyUseCase.GetGold();
            _goldText.text = $"골드: {gold}";
        }
    }
}


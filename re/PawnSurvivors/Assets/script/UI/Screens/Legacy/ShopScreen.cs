using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PawnSurvivors.Managers;

namespace PawnSurvivors.UI.Legacy
{
    /// <summary>
    /// 상점 화면입니다. 순수 코딩으로 생성됩니다.
    /// </summary>
    public class ShopScreen : MonoBehaviour
    {
        private Canvas _canvas;
        private GameObject _rootPanel;
        private Button _nextStageButton;
        private Button _exitButton;
        private bool _uiCreated = false;

        private void Start()
        {
            if (!_uiCreated)
            {
                CreateShopUI();
                _uiCreated = true;
            }
        }

        private void OnEnable()
        {
            // 화면이 활성화될 때 UI가 생성되어 있지 않으면 생성
            if (!_uiCreated)
            {
                CreateShopUI();
                _uiCreated = true;
            }
            
            // UI가 이미 생성되어 있으면 _rootPanel 활성화
            if (_rootPanel != null)
            {
                _rootPanel.SetActive(true);
            }
        }

        private void OnDisable()
        {
            // 화면이 비활성화될 때 _rootPanel도 함께 숨김
            if (_rootPanel != null)
            {
                _rootPanel.SetActive(false);
            }
        }

        private void CreateShopUI()
        {
            // Canvas 찾기 또는 생성
            _canvas = GetComponentInParent<Canvas>();
            if (_canvas == null)
            {
                _canvas = FindFirstObjectByType<Canvas>();
            }
            
            if (_canvas == null)
            {
                GameObject canvasObj = new GameObject("ShopCanvas");
                _canvas = canvasObj.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            
            // 골드 UI 추가
            if (GetComponent<CurrencyUI>() == null)
            {
                var currencyUI = gameObject.AddComponent<CurrencyUI>();
                currencyUI.anchorPosition = new Vector2(0.95f, 0.95f); // 우측 상단
            }

            // 한글 폰트 로드 (Assets/Fonts/NanumGothic SDF.asset)
            TMP_FontAsset nanumFont = null;
            // Resources에서 먼저 찾기
            nanumFont = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
            if (nanumFont == null)
            {
                // Resources에 없으면 Assets/Fonts에서 직접 찾기 (에디터 전용)
                #if UNITY_EDITOR
                nanumFont = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/NanumGothic SDF.asset");
                #endif
            }
            
            if (nanumFont == null)
            {
                LogManager.LogWarning(LogCategory.UI, "NanumGothic SDF 폰트를 찾을 수 없습니다. Assets/Fonts/NanumGothic SDF.asset 파일을 확인하세요.");
            }

            // 루트 패널 생성
            _rootPanel = new GameObject("ShopPanel");
            _rootPanel.transform.SetParent(_canvas.transform, false);
            
            var panelRect = _rootPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.anchoredPosition = Vector2.zero;
            
            var panelImage = _rootPanel.AddComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.15f, 1f); // 어두운 배경

            // 제목 텍스트
            var titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(_rootPanel.transform, false);
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.8f);
            titleRect.anchorMax = new Vector2(0.5f, 0.8f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(400f, 80f);
            titleRect.anchoredPosition = Vector2.zero;
            
            var titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "상점";
            titleText.fontSize = 48;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            if (nanumFont != null)
            {
                titleText.font = nanumFont;
            }

            // 다음 스테이지 버튼
            var buttonObj = new GameObject("NextStageButton");
            buttonObj.transform.SetParent(_rootPanel.transform, false);
            var buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0.2f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.2f);
            buttonRect.pivot = new Vector2(0.5f, 0.5f);
            buttonRect.sizeDelta = new Vector2(300f, 60f);
            buttonRect.anchoredPosition = Vector2.zero;
            
            _nextStageButton = buttonObj.AddComponent<Button>();
            var buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.6f, 0.2f, 1f); // 녹색 버튼
            
            // 버튼 텍스트
            var buttonTextObj = new GameObject("Text");
            buttonTextObj.transform.SetParent(buttonObj.transform, false);
            var buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.sizeDelta = Vector2.zero;
            buttonTextRect.anchoredPosition = Vector2.zero;
            
            var buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = "다음 스테이지";
            buttonText.fontSize = 24;
            buttonText.color = Color.white;
            buttonText.alignment = TextAlignmentOptions.Center;
            if (nanumFont != null)
            {
                buttonText.font = nanumFont;
            }
            
            _nextStageButton.targetGraphic = buttonImage;
            _nextStageButton.onClick.AddListener(OnNextStageButtonClicked);

            // 종료 버튼 (메인 메뉴로)
            var exitButtonObj = new GameObject("ExitButton");
            exitButtonObj.transform.SetParent(_rootPanel.transform, false);
            var exitButtonRect = exitButtonObj.AddComponent<RectTransform>();
            exitButtonRect.anchorMin = new Vector2(0.5f, 0.1f);
            exitButtonRect.anchorMax = new Vector2(0.5f, 0.1f);
            exitButtonRect.pivot = new Vector2(0.5f, 0.5f);
            exitButtonRect.sizeDelta = new Vector2(300f, 60f);
            exitButtonRect.anchoredPosition = Vector2.zero;
            
            _exitButton = exitButtonObj.AddComponent<Button>();
            var exitButtonImage = exitButtonObj.AddComponent<Image>();
            exitButtonImage.color = new Color(0.6f, 0.2f, 0.2f, 1f); // 빨간색 버튼
            
            // 종료 버튼 텍스트
            var exitButtonTextObj = new GameObject("Text");
            exitButtonTextObj.transform.SetParent(exitButtonObj.transform, false);
            var exitButtonTextRect = exitButtonTextObj.AddComponent<RectTransform>();
            exitButtonTextRect.anchorMin = Vector2.zero;
            exitButtonTextRect.anchorMax = Vector2.one;
            exitButtonTextRect.sizeDelta = Vector2.zero;
            exitButtonTextRect.anchoredPosition = Vector2.zero;
            
            var exitButtonText = exitButtonTextObj.AddComponent<TextMeshProUGUI>();
            exitButtonText.text = "메인 메뉴로";
            exitButtonText.fontSize = 24;
            exitButtonText.color = Color.white;
            exitButtonText.alignment = TextAlignmentOptions.Center;
            if (nanumFont != null)
            {
                exitButtonText.font = nanumFont;
            }
            
            _exitButton.targetGraphic = exitButtonImage;
            _exitButton.onClick.AddListener(OnExitButtonClicked);

            // 기본적으로 숨김
            gameObject.SetActive(false);
        }

        private void OnNextStageButtonClicked()
        {
            if (GameManager.Instance != null && GameManager.Instance.StageManagementUseCase != null)
            {
                // 다음 스테이지 이름 가져오기
                string nextStageName = GameManager.Instance.StageManagementUseCase.GetNextStageName();
                
                if (string.IsNullOrEmpty(nextStageName))
                {
                    // 마지막 스테이지면 게임오버 화면으로 (모든 스테이지 클리어)
                    LogManager.LogInfo(LogCategory.Stage, "모든 스테이지를 완료했습니다!");
                    if (GameStateManager.Instance != null)
                    {
                        GameStateManager.Instance.GoToGameOver();
                    }
                    return;
                }
                
                // 다음 스테이지로 이동: StageState로 전환 (씬 전환)
                // StageState.OnEnter()에서 스테이지 시작 처리
                if (GameStateManager.Instance != null)
                {
                    // 다음 스테이지 이름 설정
                    GameManager.Instance.StageManagementUseCase.PrepareStageStart(nextStageName, shouldResetSession: false);
                    GameStateManager.Instance.GoToStage();
                }
            }
        }

        private void OnExitButtonClicked()
        {
            // GameStateManager를 통해 CharacterSelectState로 전환 (씬 전환)
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.GoToCharacterSelect();
            }
        }
    }
}


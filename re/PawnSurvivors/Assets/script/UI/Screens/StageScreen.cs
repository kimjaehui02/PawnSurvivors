using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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
        
        [Header("LevelUp UI")]
        [SerializeField] private RectTransform levelUpContainer; // 모든 캐릭터 정보를 담을 컨테이너
        [Tooltip("한글 폰트 (나눔고딕 등). 없으면 기본 폰트 사용")]
        [SerializeField] private TMP_FontAsset koreanFontAsset; // 한글 폰트 에셋
        
        // 동적으로 생성되는 UI 요소들
        private class CharacterLevelUI
        {
            public GameObject rootObject;
            public TMP_Text nameAndLevelText;
            public TMP_Text progressText;
            public Slider progressBar;
        }
        
        private List<CharacterLevelUI> _characterUIs = new List<CharacterLevelUI>();
        #endregion


        #region Unity Lifecycle
        private void Start()
        {
            if (OptionButton != null)
            {
                OptionButton.onClick.AddListener(OnOptionButtonClicked);
                LoadOptionButtonSprite();
            }

            InitializeStageData();
        }

        private void Update()
        {
            UpdateStageTime();
            UpdateHealth();
            UpdateLevelUpProgress();
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

        private void LoadOptionButtonSprite()
        {
            // Resources에서 옵션 아이콘 스프라이트 로드
            Sprite optionSprite = Resources.Load<Sprite>("Asprite/Option");
            
            if (optionSprite != null)
            {
                // 버튼의 Image 컴포넌트 찾기
                Image buttonImage = OptionButton.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.sprite = optionSprite;
                }
                else
                {
                    Debug.LogWarning("[StageScreen] OptionButton에 Image 컴포넌트가 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning("[StageScreen] 'Resources/Asprite/Option' 스프라이트를 찾을 수 없습니다.");
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
        
        /// <summary>
        /// 모든 캐릭터의 레벨업 진행 상태를 UI에 표시합니다.
        /// </summary>
        private void UpdateLevelUpProgress()
        {
            if (GameManager.Instance?.PlayerController == null) return;
            
            // levelUpContainer가 없으면 자동 생성
            if (levelUpContainer == null)
            {
                EnsureLevelUpContainer();
            }
            
            if (levelUpContainer == null)
            {
                Debug.LogWarning("[StageScreen] LevelUpContainer를 생성할 수 없습니다.");
                return;
            }
            
            var playerController = GameManager.Instance.PlayerController;
            var playerPawns = playerController.playerPawns;
            
            if (playerPawns == null || playerPawns.Count == 0)
            {
                // 플레이어가 없으면 모든 UI 비활성화
                foreach (var ui in _characterUIs)
                {
                    if (ui?.rootObject != null)
                    {
                        ui.rootObject.SetActive(false);
                    }
                }
                return;
            }
            
            // 캐릭터 수가 변경되었으면 UI 재생성
            if (_characterUIs.Count != playerPawns.Count)
            {
                RebuildCharacterUIs(playerPawns.Count);
            }
            
            // 각 캐릭터의 정보 업데이트
            int activeUICount = 0;
            bool shouldLogDetails = (Time.frameCount % 300 == 0); // 5초마다 상세 로그
            
            for (int i = 0; i < playerPawns.Count && i < _characterUIs.Count; i++)
            {
                var pawn = playerPawns[i];
                var ui = _characterUIs[i];
                
                if (pawn == null)
                {
                    if (shouldLogDetails)
                    {
                        Debug.LogWarning($"[StageScreen] 캐릭터 {i}: Pawn이 null입니다.");
                    }
                    if (ui?.rootObject != null)
                    {
                        ui.rootObject.SetActive(false);
                    }
                    continue;
                }
                
                if (ui == null || ui.rootObject == null)
                {
                    if (shouldLogDetails)
                    {
                        Debug.LogWarning($"[StageScreen] 캐릭터 {i}: UI가 null입니다.");
                    }
                    continue;
                }
                
                var pawnManager = pawn.GetComponent<PawnManager>();
                var levelUpManager = pawn.GetComponent<LevelUpSubManager>();
                
                if (shouldLogDetails)
                {
                    string pawnName = pawn.name;
                    string recipeName = pawnManager?.PawnData?.recipeName ?? "Unknown";
                    bool hasPawnManager = pawnManager != null;
                    bool hasLevelUpManager = levelUpManager != null;
                    
                    Debug.Log($"[StageScreen] 캐릭터 {i}: {pawnName} (Recipe: {recipeName}), " +
                             $"PawnManager: {hasPawnManager}, LevelUpManager: {hasLevelUpManager}");
                }
                
                if (pawnManager != null)
                {
                    string pawnName = pawnManager.PawnData?.recipeName ?? pawnManager.name;
                    
                    if (levelUpManager != null)
                    {
                        // 레벨업 시스템이 있는 경우
                        string progressText = levelUpManager.GetCurrentProgressText();
                        float progress = levelUpManager.GetCurrentProgress();
                        
                        if (ui.nameAndLevelText != null)
                        {
                            ui.nameAndLevelText.text = $"{pawnName} Lv.{levelUpManager.GetCurrentLevel()}";
                        }
                        
                        if (ui.progressText != null)
                        {
                            ui.progressText.text = progressText;
                            
                            // 디버깅: 5초마다 UI 텍스트 로그
                            if (shouldLogDetails)
                            {
                                Debug.Log($"[StageScreen] 캐릭터 {i} ({pawnName}) UI 텍스트: '{progressText}', 진행도: {progress:F2}");
                            }
                        }
                        
                        if (ui.progressBar != null)
                        {
                            ui.progressBar.value = progress;
                        }
                    }
                    else
                    {
                        // 레벨업 시스템이 없는 경우 - 기본 정보만 표시
                        if (ui.nameAndLevelText != null)
                        {
                            ui.nameAndLevelText.text = $"{pawnName} Lv.1";
                        }
                        
                        if (ui.progressText != null)
                        {
                            ui.progressText.text = "레벨업 시스템 없음";
                        }
                        
                        if (ui.progressBar != null)
                        {
                            ui.progressBar.value = 0f;
                        }
                    }
                    
                    // UI 활성화 (레벨업 시스템이 있든 없든 표시)
                    ui.rootObject.SetActive(true);
                    activeUICount++;
                }
                else
                {
                    // PawnManager가 없는 경우만 비활성화
                    ui.rootObject.SetActive(false);
                }
            }
            
            // 디버깅: 5초마다 요약 로그
            if (shouldLogDetails)
            {
                Debug.Log($"[StageScreen] 레벨업 UI 업데이트: 총 {playerPawns.Count}명, 활성 UI {activeUICount}개");
            }
        }
        
        /// <summary>
        /// LevelUpContainer가 없으면 자동으로 생성합니다.
        /// </summary>
        private void EnsureLevelUpContainer()
        {
            if (levelUpContainer != null) return;
            
            // Canvas 찾기
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = FindFirstObjectByType<Canvas>();
            }
            
            if (canvas == null)
            {
                Debug.LogError("[StageScreen] Canvas를 찾을 수 없어 LevelUpContainer를 생성할 수 없습니다.");
                return;
            }
            
            // LevelUpContainer 생성
            GameObject containerObj = new GameObject("LevelUpContainer");
            levelUpContainer = containerObj.AddComponent<RectTransform>();
            containerObj.transform.SetParent(canvas.transform, false);
            
            // 위치 설정 (좌측 상단, 더 넓게)
            levelUpContainer.anchorMin = new Vector2(0f, 1f);
            levelUpContainer.anchorMax = new Vector2(0f, 1f);
            levelUpContainer.pivot = new Vector2(0f, 1f);
            levelUpContainer.anchoredPosition = new Vector2(20f, -20f);
            levelUpContainer.sizeDelta = new Vector2(500f, 600f); // 더 넓고 높게
            
            // VerticalLayoutGroup 추가 (자동 정렬)
            var layoutGroup = containerObj.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.spacing = 15f; // 10 -> 15 (더 넓게)
            layoutGroup.padding = new RectOffset(15, 15, 15, 15); // 10 -> 15 (더 넓게)
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            
            Debug.Log("[StageScreen] LevelUpContainer를 자동으로 생성했습니다.");
        }
        
        /// <summary>
        /// 캐릭터 수에 맞게 UI를 재생성합니다.
        /// </summary>
        private void RebuildCharacterUIs(int count)
        {
            if (count <= 0)
            {
                // 기존 UI만 삭제
                foreach (var ui in _characterUIs)
                {
                    if (ui?.rootObject != null)
                    {
                        Destroy(ui.rootObject);
                    }
                }
                _characterUIs.Clear();
                return;
            }
            
            // levelUpContainer 확인
            if (levelUpContainer == null)
            {
                EnsureLevelUpContainer();
            }
            
            if (levelUpContainer == null)
            {
                Debug.LogError("[StageScreen] LevelUpContainer가 없어 UI를 생성할 수 없습니다.");
                return;
            }
            
            // 기존 UI 삭제
            foreach (var ui in _characterUIs)
            {
                if (ui?.rootObject != null)
                {
                    Destroy(ui.rootObject);
                }
            }
            _characterUIs.Clear();
            
            // 새 UI 생성 (각각 다른 위치에 배치)
            for (int i = 0; i < count; i++)
            {
                var newUI = CreateCharacterLevelUI(i);
                if (newUI != null)
                {
                    _characterUIs.Add(newUI);
                }
            }
            
            Debug.Log($"[StageScreen] {count}개의 캐릭터 UI를 재생성했습니다.");
        }
        
        /// <summary>
        /// 개별 캐릭터의 레벨 UI를 생성합니다.
        /// </summary>
        /// <param name="index">캐릭터 인덱스 (위치 계산용)</param>
        private CharacterLevelUI CreateCharacterLevelUI(int index)
        {
            if (levelUpContainer == null)
            {
                Debug.LogError("[StageScreen] CreateCharacterLevelUI: levelUpContainer가 null입니다.");
                return null;
            }
            
            var ui = new CharacterLevelUI();
            
            // 루트 GameObject
            ui.rootObject = new GameObject($"CharacterLevelUI_{index}");
            ui.rootObject.transform.SetParent(levelUpContainer, false);
            
            var rectTransform = ui.rootObject.AddComponent<RectTransform>();
            
            // 각 UI의 위치를 인덱스에 따라 계산
            float uiHeight = 90f; // 각 UI의 높이
            float spacing = 15f; // UI 간 간격
            float startY = -15f; // 시작 Y 위치 (상단에서)
            
            // Anchor와 Pivot 설정
            rectTransform.anchorMin = new Vector2(0f, 1f); // 좌측 상단
            rectTransform.anchorMax = new Vector2(0f, 1f); // 좌측 상단
            rectTransform.pivot = new Vector2(0f, 1f); // 좌측 상단 기준
            
            // 위치 계산: 각 UI를 세로로 배치
            float yPosition = startY - (index * (uiHeight + spacing));
            rectTransform.anchoredPosition = new Vector2(15f, yPosition);
            rectTransform.sizeDelta = new Vector2(480f, uiHeight); // 너비 480, 높이 90
            
            // 배경 이미지 (선택적, 더 보기 좋게)
            var bgImage = ui.rootObject.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.5f); // 반투명 어두운 배경
            
            // 이름 + 레벨 텍스트
            var nameTextObj = new GameObject("NameText");
            var nameRect = nameTextObj.AddComponent<RectTransform>();
            nameTextObj.transform.SetParent(ui.rootObject.transform, false);
            ui.nameAndLevelText = nameTextObj.AddComponent<TextMeshProUGUI>();
            ui.nameAndLevelText.text = "Player Lv.1";
            ui.nameAndLevelText.fontSize = 24; // 16 -> 24 (더 크게)
            ui.nameAndLevelText.color = Color.white;
            ui.nameAndLevelText.alignment = TextAlignmentOptions.Left;
            
            // 한글 폰트 적용 (있으면, 없으면 자동으로 찾기)
            if (koreanFontAsset != null)
            {
                ui.nameAndLevelText.font = koreanFontAsset;
            }
            else
            {
                // Resources에서 나눔고딕 폰트 찾기
                var nanumFont = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
                if (nanumFont == null)
                {
                    // Resources에 없으면 Assets/Fonts에서 직접 찾기 (에디터 전용)
                    #if UNITY_EDITOR
                    nanumFont = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/NanumGothic SDF.asset");
                    #endif
                }
                
                if (nanumFont != null)
                {
                    ui.nameAndLevelText.font = nanumFont;
                }
                else
                {
                    Debug.LogWarning("[StageScreen] 한글 폰트를 찾을 수 없습니다. Inspector에서 koreanFontAsset을 할당하거나, Assets/Fonts/NanumGothic SDF.asset 파일을 확인하세요.");
                }
            }
            
            // 이름 텍스트 (상단)
            nameRect.anchorMin = new Vector2(0f, 1f);
            nameRect.anchorMax = new Vector2(1f, 1f);
            nameRect.pivot = new Vector2(0f, 1f);
            nameRect.anchoredPosition = new Vector2(15f, -15f);
            nameRect.sizeDelta = new Vector2(-30f, 30f);
            
            // 진행도 텍스트 (중간, 이름 텍스트와 간격)
            var progressTextObj = new GameObject("ProgressText");
            var progressTextRect = progressTextObj.AddComponent<RectTransform>();
            progressTextObj.transform.SetParent(ui.rootObject.transform, false);
            ui.progressText = progressTextObj.AddComponent<TextMeshProUGUI>();
            ui.progressText.text = "0/100";
            ui.progressText.fontSize = 20;
            ui.progressText.color = Color.yellow;
            ui.progressText.alignment = TextAlignmentOptions.Left;
            
            // 한글 폰트 적용 (있으면, 없으면 자동으로 찾기)
            if (koreanFontAsset != null)
            {
                ui.progressText.font = koreanFontAsset;
            }
            else
            {
                // Resources에서 나눔고딕 폰트 찾기
                var nanumFont = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
                if (nanumFont == null)
                {
                    // Resources에 없으면 Assets/Fonts에서 직접 찾기 (에디터 전용)
                    #if UNITY_EDITOR
                    nanumFont = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/NanumGothic SDF.asset");
                    #endif
                }
                
                if (nanumFont != null)
                {
                    ui.progressText.font = nanumFont;
                }
                else
                {
                    Debug.LogWarning("[StageScreen] 한글 폰트를 찾을 수 없습니다. Inspector에서 koreanFontAsset을 할당하거나, Assets/Fonts/NanumGothic SDF.asset 파일을 확인하세요.");
                }
            }
            
            progressTextRect.anchorMin = new Vector2(0f, 0.5f);
            progressTextRect.anchorMax = new Vector2(1f, 0.5f);
            progressTextRect.pivot = new Vector2(0f, 0.5f);
            progressTextRect.anchoredPosition = new Vector2(15f, -5f); // 중간 위치, 간격 조정
            progressTextRect.sizeDelta = new Vector2(-30f, 25f);
            
            // 진행도 바 (하단, 진행도 텍스트와 간격)
            var sliderObj = new GameObject("ProgressBar");
            var sliderRect = sliderObj.AddComponent<RectTransform>();
            sliderObj.transform.SetParent(ui.rootObject.transform, false);
            ui.progressBar = sliderObj.AddComponent<Slider>();
            ui.progressBar.minValue = 0f;
            ui.progressBar.maxValue = 1f;
            ui.progressBar.value = 0f;
            sliderRect.anchorMin = new Vector2(0f, 0f);
            sliderRect.anchorMax = new Vector2(1f, 0f);
            sliderRect.pivot = new Vector2(0.5f, 0f);
            sliderRect.anchoredPosition = new Vector2(0f, 15f); // 하단에서 더 위로, 간격 조정
            sliderRect.sizeDelta = new Vector2(-30f, 18f);
            
            // Slider 배경
            var bgObj = new GameObject("Background");
            var bgRect = bgObj.AddComponent<RectTransform>();
            bgObj.transform.SetParent(sliderObj.transform, false);
            var bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            
            // Slider 채우기 영역
            var fillAreaObj = new GameObject("Fill Area");
            var fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
            fillAreaObj.transform.SetParent(sliderObj.transform, false);
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.sizeDelta = Vector2.zero;
            
            var fillObj = new GameObject("Fill");
            var fillRect = fillObj.AddComponent<RectTransform>();
            fillObj.transform.SetParent(fillAreaObj.transform, false);
            var fillImg = fillObj.AddComponent<Image>();
            fillImg.color = new Color(0f, 1f, 0.5f, 1f); // 녹색 진행바
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;
            
            ui.progressBar.fillRect = fillRect;
            ui.progressBar.targetGraphic = fillImg;
            
            return ui;
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

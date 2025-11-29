using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using PawnSurvivors.Data.Recipes;
using PawnSurvivors.Managers;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Data;
using PawnSurvivors.Domain;

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
            public TMP_Text healthText;
            public Slider healthBar;
            public TMP_Text dpsText;
            public TMP_Text progressText;
            public Slider progressBar;
            // 아이템 표시 UI
            public GameObject equippedItemsContainer; // 장착 아이템 컨테이너
            public List<GameObject> equippedItemIcons = new List<GameObject>(); // 장착 아이템 아이콘들
        }
        
        private List<CharacterLevelUI> _characterUIs = new List<CharacterLevelUI>();
        
        // 전역 아이템 표시 UI
        private GameObject _globalItemsContainer;
        private List<GameObject> _globalItemIcons;
        
        // 툴팁 UI
        private GameObject _tooltipPanel;
        private TMP_Text _tooltipText;
        private GameObject _currentHoveredIcon; // 현재 마우스가 올라간 아이콘
        #endregion


        #region Unity Lifecycle
        private void Start()
        {
            if (OptionButton != null)
            {
                OptionButton.onClick.AddListener(OnOptionButtonClicked);
                LoadOptionButtonSprite();
            }
            
            // 골드 UI 추가
            if (GetComponent<CurrencyUI>() == null)
            {
                var currencyUI = gameObject.AddComponent<CurrencyUI>();
                currencyUI.anchorPosition = new Vector2(0.95f, 0.95f); // 우측 상단
            }
        }

        private void OnEnable()
        {
            // 스테이지 화면이 활성화될 때마다 타이머 리셋
            InitializeStageData();
        }

        private void Update()
        {
            UpdateStageTime();
            UpdateTooltipPosition(); // 툴팁 위치를 마우스에 따라 실시간 업데이트
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
                    LogManager.LogInfo(LogCategory.Stage, $"Stage '{_selectedStage}' loaded. Duration: {stageTime}s");
                }
                else
                {
                    LogManager.LogWarning(LogCategory.Stage, $"StageData for '{_selectedStage}' not found. Using default duration.");
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
                    LogManager.LogWarning(LogCategory.UI, "OptionButton에 Image 컴포넌트가 없습니다.");
                }
            }
            else
            {
                LogManager.LogWarning(LogCategory.UI, "'Resources/Asprite/Option' 스프라이트를 찾을 수 없습니다.");
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
                LogManager.LogWarning(LogCategory.UI, "LevelUpContainer를 생성할 수 없습니다.");
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
            // bool shouldLogDetails = (Time.frameCount % 300 == 0); // 5초마다 상세 로그 (주석 처리)
            
            for (int i = 0; i < playerPawns.Count && i < _characterUIs.Count; i++)
            {
                var pawn = playerPawns[i];
                var ui = _characterUIs[i];
                
                if (pawn == null)
                {
                    // if (shouldLogDetails)
                    // {
                    //     Debug.LogWarning($"[StageScreen] 캐릭터 {i}: Pawn이 null입니다.");
                    // }
                    if (ui?.rootObject != null)
                    {
                        ui.rootObject.SetActive(false);
                    }
                    continue;
                }
                
                if (ui == null || ui.rootObject == null)
                {
                    // if (shouldLogDetails)
                    // {
                    //     Debug.LogWarning($"[StageScreen] 캐릭터 {i}: UI가 null입니다.");
                    // }
                    continue;
                }
                
                var pawnManager = pawn.GetComponent<PawnManager>();
                var levelUpManager = pawn.GetComponent<LevelUpSubManager>();
                
                // if (shouldLogDetails)
                // {
                //     string pawnName = pawn.name;
                //     string recipeName = pawnManager?.PawnData?.recipeName ?? "Unknown";
                //     bool hasPawnManager = pawnManager != null;
                //     bool hasLevelUpManager = levelUpManager != null;
                //     
                //     Debug.Log($"[StageScreen] 캐릭터 {i}: {pawnName} (Recipe: {recipeName}), " +
                //              $"PawnManager: {hasPawnManager}, LevelUpManager: {hasLevelUpManager}");
                // }
                
                if (pawnManager != null)
                {
                    string pawnName = pawnManager.PawnData?.recipeName ?? pawnManager.name;
                    var pawnData = pawnManager.PawnData;
                    int playerIndex = pawnData?.playerIndex ?? -1;
                    
                    // 체력 표시 및 체력바 업데이트 (null-safe)
                    if (pawnData?.healthData != null)
                    {
                        float currentHealth = pawnData.healthData.currentHealth;
                        
                        // ✅ PawnStatCalculator를 사용하여 계산된 최대 체력 사용
                        var statCalculator = GameManager.Instance?.PawnStatCalculator;
                        float maxHealth = statCalculator != null 
                            ? statCalculator.GetEffectiveMaxHealth(pawnData) 
                            : pawnData.healthData.maxHealth;
                        
                        // 체력 텍스트 업데이트
                        if (ui.healthText != null)
                        {
                            ui.healthText.text = $"HP: {currentHealth:F0}/{maxHealth:F0}";
                        }
                        
                        // 체력바 업데이트
                        if (ui.healthBar != null)
                        {
                            ui.healthBar.value = maxHealth > 0 ? currentHealth / maxHealth : 0f;
                        }
                    }
                    else
                    {
                        if (ui.healthText != null)
                        {
                            ui.healthText.text = "HP: --/--";
                        }
                        if (ui.healthBar != null)
                        {
                            ui.healthBar.value = 0f;
                        }
                    }
                    
                    // 초당피해량 표시 (null-safe)
                    // ✅ PawnStatCalculator를 사용하여 계산된 데미지와 공격 속도 사용
                    if (ui.dpsText != null)
                    {
                        if (pawnData?.combatData != null)
                        {
                            // ✅ PawnStatCalculator를 사용하여 실제 데미지와 공격 속도 계산
                            var statCalculator = GameManager.Instance?.PawnStatCalculator;
                            if (statCalculator != null)
                            {
                                float effectiveDamage = statCalculator.GetEffectiveDamage(pawnData);
                                float effectiveFireRate = statCalculator.GetEffectiveFireRate(pawnData);
                                float dps = effectiveDamage * effectiveFireRate;
                                
                                // 디버그 로그 (문제 해결용)
                                if (Time.frameCount % 60 == 0)
                                {
                                    LogManager.LogDebug(LogCategory.UI, 
                                        $"[DPS] playerIndex={playerIndex}, damage={effectiveDamage}, fireRate={effectiveFireRate}, dps={dps}, baseDamage={pawnData.combatData.damage}, baseFireRate={pawnData.combatData.fireRate}");
                                }
                                
                                ui.dpsText.text = $"DPS: {dps:F1}";
                            }
                            else
                            {
                                // Fallback: 기본값 사용
                                float dps = pawnData.combatData.damage * pawnData.combatData.fireRate;
                                ui.dpsText.text = $"DPS: {dps:F1}";
                            }
                        }
                        else
                        {
                            ui.dpsText.text = "DPS: --";
                        }
                    }
                    
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
                            
                            // 디버깅: 5초마다 UI 텍스트 로그 (주석 처리)
                            // if (shouldLogDetails)
                            // {
                            //     Debug.Log($"[StageScreen] 캐릭터 {i} ({pawnName}) UI 텍스트: '{progressText}', 진행도: {progress:F2}");
                            // }
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
                    
                    // 장착 아이템 표시 업데이트
                    UpdateEquippedItemsUI(ui, playerIndex);
                    
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
            
            // 전역 아이템 표시 업데이트
            UpdateGlobalItemsUI();
            
            // 디버깅: 5초마다 요약 로그 (주석 처리)
            // if (shouldLogDetails)
            // {
            //     Debug.Log($"[StageScreen] 레벨업 UI 업데이트: 총 {playerPawns.Count}명, 활성 UI {activeUICount}개");
            // }
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
                LogManager.LogError(LogCategory.UI, "Canvas를 찾을 수 없어 LevelUpContainer를 생성할 수 없습니다.");
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
            
            LogManager.LogInfo(LogCategory.UI, "LevelUpContainer를 자동으로 생성했습니다.");
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
                LogManager.LogError(LogCategory.UI, "LevelUpContainer가 없어 UI를 생성할 수 없습니다.");
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
            
            // Debug.Log($"[StageScreen] {count}개의 캐릭터 UI를 재생성했습니다.");
        }
        
        /// <summary>
        /// 개별 캐릭터의 레벨 UI를 생성합니다.
        /// </summary>
        /// <param name="index">캐릭터 인덱스 (위치 계산용)</param>
        private CharacterLevelUI CreateCharacterLevelUI(int index)
        {
            if (levelUpContainer == null)
            {
                LogManager.LogError(LogCategory.UI, "CreateCharacterLevelUI: levelUpContainer가 null입니다.");
                return null;
            }
            
            var ui = new CharacterLevelUI();
            
            // 루트 GameObject
            ui.rootObject = new GameObject($"CharacterLevelUI_{index}");
            ui.rootObject.transform.SetParent(levelUpContainer, false);
            
            var rectTransform = ui.rootObject.AddComponent<RectTransform>();
            
            // 각 UI의 위치를 인덱스에 따라 계산
            float uiHeight = 110f; // 각 UI의 높이 (겹침으로 인해 줄어듦)
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
                    LogManager.LogWarning(LogCategory.UI, "한글 폰트를 찾을 수 없습니다. Inspector에서 koreanFontAsset을 할당하거나, Assets/Fonts/NanumGothic SDF.asset 파일을 확인하세요.");
                }
            }
            
            // 이름 텍스트 (상단)
            nameRect.anchorMin = new Vector2(0f, 1f);
            nameRect.anchorMax = new Vector2(1f, 1f);
            nameRect.pivot = new Vector2(0f, 1f);
            nameRect.anchoredPosition = new Vector2(15f, -15f);
            nameRect.sizeDelta = new Vector2(-30f, 30f);
            
            // 체력바 (이름 아래, 좌측)
            var healthBarObj = new GameObject("HealthBar");
            var healthBarRect = healthBarObj.AddComponent<RectTransform>();
            healthBarObj.transform.SetParent(ui.rootObject.transform, false);
            ui.healthBar = healthBarObj.AddComponent<Slider>();
            ui.healthBar.minValue = 0f;
            ui.healthBar.maxValue = 1f;
            ui.healthBar.value = 1f;
            healthBarRect.anchorMin = new Vector2(0f, 1f);
            healthBarRect.anchorMax = new Vector2(0.5f, 1f);
            healthBarRect.pivot = new Vector2(0f, 1f);
            healthBarRect.anchoredPosition = new Vector2(15f, -45f);
            healthBarRect.sizeDelta = new Vector2(-15f, 20f);
            
            // 체력바 배경
            var healthBgObj = new GameObject("Background");
            var healthBgRect = healthBgObj.AddComponent<RectTransform>();
            healthBgObj.transform.SetParent(healthBarObj.transform, false);
            var healthBgImg = healthBgObj.AddComponent<Image>();
            healthBgImg.color = new Color(0.2f, 0.1f, 0.1f, 1f); // 어두운 빨간 배경
            healthBgRect.anchorMin = Vector2.zero;
            healthBgRect.anchorMax = Vector2.one;
            healthBgRect.sizeDelta = Vector2.zero;
            
            // 체력바 채우기 영역
            var healthFillAreaObj = new GameObject("Fill Area");
            var healthFillAreaRect = healthFillAreaObj.AddComponent<RectTransform>();
            healthFillAreaObj.transform.SetParent(healthBarObj.transform, false);
            healthFillAreaRect.anchorMin = Vector2.zero;
            healthFillAreaRect.anchorMax = Vector2.one;
            healthFillAreaRect.sizeDelta = Vector2.zero;
            
            var healthFillObj = new GameObject("Fill");
            var healthFillRect = healthFillObj.AddComponent<RectTransform>();
            healthFillObj.transform.SetParent(healthFillAreaObj.transform, false);
            var healthFillImg = healthFillObj.AddComponent<Image>();
            healthFillImg.color = new Color(1f, 0.2f, 0.2f, 1f); // 빨간색
            healthFillRect.anchorMin = Vector2.zero;
            healthFillRect.anchorMax = Vector2.one;
            healthFillRect.sizeDelta = Vector2.zero;
            
            ui.healthBar.fillRect = healthFillRect;
            ui.healthBar.targetGraphic = healthFillImg;
            
            // 체력 텍스트 (체력바의 자식으로 배치하여 하나로 합침)
            var healthTextObj = new GameObject("HealthText");
            var healthTextRect = healthTextObj.AddComponent<RectTransform>();
            healthTextObj.transform.SetParent(healthBarObj.transform, false); // 체력바의 자식으로 설정
            ui.healthText = healthTextObj.AddComponent<TextMeshProUGUI>();
            ui.healthText.text = "HP: 100/100";
            ui.healthText.fontSize = 16;
            ui.healthText.color = Color.white; // 흰색으로 변경 (빨간 바 위에서 잘 보이도록)
            ui.healthText.alignment = TextAlignmentOptions.Center; // 중앙 정렬
            ui.healthText.fontStyle = FontStyles.Bold; // 굵게
            
            // 한글 폰트 적용
            if (koreanFontAsset != null)
            {
                ui.healthText.font = koreanFontAsset;
            }
            else
            {
                var nanumFont = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
                #if UNITY_EDITOR
                if (nanumFont == null)
                    nanumFont = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/NanumGothic SDF.asset");
                #endif
                if (nanumFont != null)
                    ui.healthText.font = nanumFont;
            }
            
            // 체력 텍스트를 체력바 전체 영역에 맞춰 배치
            healthTextRect.anchorMin = Vector2.zero;
            healthTextRect.anchorMax = Vector2.one;
            healthTextRect.pivot = new Vector2(0.5f, 0.5f);
            healthTextRect.anchoredPosition = Vector2.zero; // 체력바 중앙
            healthTextRect.sizeDelta = Vector2.zero; // 체력바 전체 영역 사용
            
            // 초당피해량 텍스트 (체력 옆, 우측)
            var dpsTextObj = new GameObject("DPSText");
            var dpsTextRect = dpsTextObj.AddComponent<RectTransform>();
            dpsTextObj.transform.SetParent(ui.rootObject.transform, false);
            ui.dpsText = dpsTextObj.AddComponent<TextMeshProUGUI>();
            ui.dpsText.text = "DPS: 0";
            ui.dpsText.fontSize = 18;
            ui.dpsText.color = Color.cyan;
            ui.dpsText.alignment = TextAlignmentOptions.Left;
            
            // 한글 폰트 적용
            if (koreanFontAsset != null)
            {
                ui.dpsText.font = koreanFontAsset;
            }
            else
            {
                var nanumFont = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
                #if UNITY_EDITOR
                if (nanumFont == null)
                    nanumFont = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/NanumGothic SDF.asset");
                #endif
                if (nanumFont != null)
                    ui.dpsText.font = nanumFont;
            }
            
            dpsTextRect.anchorMin = new Vector2(0.5f, 1f);
            dpsTextRect.anchorMax = new Vector2(1f, 1f);
            dpsTextRect.pivot = new Vector2(0f, 1f);
            dpsTextRect.anchoredPosition = new Vector2(15f, -45f);
            dpsTextRect.sizeDelta = new Vector2(-15f, 25f);
            
            // 레벨업 진행도 바 (하단)
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
            sliderRect.anchoredPosition = new Vector2(0f, 15f);
            sliderRect.sizeDelta = new Vector2(-30f, 20f);
            
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
            
            // 레벨업 진행도 텍스트 (진행도 바 위에 겹쳐서 배치)
            var progressTextObj = new GameObject("ProgressText");
            var progressTextRect = progressTextObj.AddComponent<RectTransform>();
            progressTextObj.transform.SetParent(ui.rootObject.transform, false);
            ui.progressText = progressTextObj.AddComponent<TextMeshProUGUI>();
            ui.progressText.text = "0/100";
            ui.progressText.fontSize = 16;
            ui.progressText.color = Color.white; // 흰색으로 변경 (바 위에서 잘 보이도록)
            ui.progressText.alignment = TextAlignmentOptions.Center; // 중앙 정렬
            ui.progressText.fontStyle = FontStyles.Bold; // 굵게
            
            // 한글 폰트 적용
            if (koreanFontAsset != null)
            {
                ui.progressText.font = koreanFontAsset;
            }
            else
            {
                var nanumFont = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
                #if UNITY_EDITOR
                if (nanumFont == null)
                    nanumFont = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/NanumGothic SDF.asset");
                #endif
                if (nanumFont != null)
                    ui.progressText.font = nanumFont;
            }
            
            // 진행도 텍스트를 진행도 바와 같은 위치에 겹쳐서 배치
            progressTextRect.anchorMin = new Vector2(0f, 0f);
            progressTextRect.anchorMax = new Vector2(1f, 0f);
            progressTextRect.pivot = new Vector2(0.5f, 0.5f);
            progressTextRect.anchoredPosition = new Vector2(0f, 25f); // 진행도 바 중앙
            progressTextRect.sizeDelta = new Vector2(-30f, 20f);
            
            // 장착 아이템 컨테이너 생성 (체력바 옆, 우측)
            var equippedItemsObj = new GameObject("EquippedItems");
            var equippedItemsRect = equippedItemsObj.AddComponent<RectTransform>();
            equippedItemsObj.transform.SetParent(ui.rootObject.transform, false);
            ui.equippedItemsContainer = equippedItemsObj;
            
            // equippedItemIcons 리스트 초기화
            if (ui.equippedItemIcons == null)
            {
                ui.equippedItemIcons = new List<GameObject>();
            }
            
            // Pawn 이름과 같은 줄에 배치 (이름은 0~0.5, 아이템은 0.5~1)
            equippedItemsRect.anchorMin = new Vector2(0.5f, 1f);
            equippedItemsRect.anchorMax = new Vector2(1f, 1f);
            equippedItemsRect.pivot = new Vector2(0f, 1f);
            equippedItemsRect.anchoredPosition = new Vector2(15f, -15f); // 이름과 같은 높이
            equippedItemsRect.sizeDelta = new Vector2(-15f, 20f);
            
            // HorizontalLayoutGroup 추가 (아이템들을 가로로 배치)
            var horizontalLayout = equippedItemsObj.AddComponent<HorizontalLayoutGroup>();
            horizontalLayout.spacing = 5f;
            horizontalLayout.childControlWidth = false;
            horizontalLayout.childControlHeight = false;
            horizontalLayout.childForceExpandWidth = false;
            horizontalLayout.childForceExpandHeight = false;
            
            return ui;
        }
        
        /// <summary>
        /// 특정 Pawn의 장착 아이템 UI를 업데이트합니다.
        /// </summary>
        private void UpdateEquippedItemsUI(CharacterLevelUI ui, int playerIndex)
        {
            if (ui == null || ui.rootObject == null || playerIndex < 0)
            {
                return;
            }
            
            // equippedItemsContainer가 없으면 생성
            if (ui.equippedItemsContainer == null)
            {
                // 장착 아이템 컨테이너 생성 (체력바 옆, 우측)
                var equippedItemsObj = new GameObject("EquippedItems");
                var equippedItemsRect = equippedItemsObj.AddComponent<RectTransform>();
                equippedItemsObj.transform.SetParent(ui.rootObject.transform, false);
                ui.equippedItemsContainer = equippedItemsObj;
                
                // 체력바 옆에 배치 (체력바는 0~0.5, 아이템은 0.5~1)
                equippedItemsRect.anchorMin = new Vector2(0.5f, 1f);
                equippedItemsRect.anchorMax = new Vector2(1f, 1f);
                equippedItemsRect.pivot = new Vector2(0f, 1f);
                equippedItemsRect.anchoredPosition = new Vector2(15f, -45f);
                equippedItemsRect.sizeDelta = new Vector2(-15f, 20f);
                
                // HorizontalLayoutGroup 추가 (아이템들을 가로로 배치)
                var horizontalLayout = equippedItemsObj.AddComponent<HorizontalLayoutGroup>();
                horizontalLayout.spacing = 5f;
                horizontalLayout.childControlWidth = false;
                horizontalLayout.childControlHeight = false;
                horizontalLayout.childForceExpandWidth = false;
                horizontalLayout.childForceExpandHeight = false;
            }
            
            // equippedItemIcons가 null이면 초기화
            if (ui.equippedItemIcons == null)
            {
                ui.equippedItemIcons = new List<GameObject>();
            }
            
            // 기존 아이콘 제거
            foreach (var icon in ui.equippedItemIcons)
            {
                if (icon != null)
                {
                    Destroy(icon);
                }
            }
            ui.equippedItemIcons.Clear();
            
            // ItemManagementUseCase에서 장착 아이템 가져오기
            if (GameManager.Instance?.ItemManagementUseCase == null)
            {
                return;
            }
            
            var equippedItems = GameManager.Instance.ItemManagementUseCase.GetEquippedItems(playerIndex);
            
            if (equippedItems == null)
            {
                return;
            }
            
            // 아이템 아이콘 생성
            foreach (var item in equippedItems)
            {
                if (item == null || string.IsNullOrEmpty(item.itemId))
                {
                    continue;
                }
                
                if (ui.equippedItemsContainer == null || ui.equippedItemsContainer.transform == null)
                {
                    continue;
                }
                
                var iconObj = new GameObject($"ItemIcon_{item.itemId}");
                if (iconObj == null)
                {
                    continue;
                }
                
                var iconTransform = iconObj.transform;
                if (iconTransform == null || ui.equippedItemsContainer == null || ui.equippedItemsContainer.transform == null)
                {
                    Destroy(iconObj);
                    continue;
                }
                
                iconTransform.SetParent(ui.equippedItemsContainer.transform, false);
                var iconRect = iconObj.AddComponent<RectTransform>();
                if (iconRect == null)
                {
                    Destroy(iconObj);
                    continue;
                }
                iconRect.sizeDelta = new Vector2(18f, 18f);
                
                var iconImage = iconObj.AddComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.color = new Color(0.8f, 0.8f, 0.2f, 1f); // 노란색 (임시, 나중에 실제 아이콘으로 교체)
                    iconImage.raycastTarget = true; // 마우스 이벤트를 받기 위해 필요
                }
                
                // 마우스 오버 이벤트 추가
                var eventTrigger = iconObj.AddComponent<EventTrigger>();
                if (eventTrigger != null)
                {
                    // PointerEnter 이벤트
                    var pointerEnter = new EventTrigger.Entry();
                    pointerEnter.eventID = EventTriggerType.PointerEnter;
                    pointerEnter.callback.AddListener((data) => { OnItemIconPointerEnter(item, iconObj); });
                    eventTrigger.triggers.Add(pointerEnter);
                    
                    // PointerExit 이벤트
                    var pointerExit = new EventTrigger.Entry();
                    pointerExit.eventID = EventTriggerType.PointerExit;
                    pointerExit.callback.AddListener((data) => { OnItemIconPointerExit(iconObj); });
                    eventTrigger.triggers.Add(pointerExit);
                }
                
                ui.equippedItemIcons.Add(iconObj);
            }
        }
        
        /// <summary>
        /// 전역 아이템 UI를 생성합니다.
        /// </summary>
        private void EnsureGlobalItemsContainer()
        {
            if (_globalItemsContainer != null) return;
            
            if (levelUpContainer == null)
            {
                EnsureLevelUpContainer();
            }
            
            if (levelUpContainer == null) return;
            
            // 전역 아이템 컨테이너 생성 (LevelUpContainer 아래에 배치)
            var globalItemsObj = new GameObject("GlobalItemsContainer");
            var globalItemsRect = globalItemsObj.AddComponent<RectTransform>();
            globalItemsObj.transform.SetParent(levelUpContainer.parent, false);
            _globalItemsContainer = globalItemsObj;
            
            // LevelUpContainer 아래에 배치
            globalItemsRect.anchorMin = new Vector2(0f, 1f);
            globalItemsRect.anchorMax = new Vector2(0f, 1f);
            globalItemsRect.pivot = new Vector2(0f, 1f);
            globalItemsRect.anchoredPosition = new Vector2(20f, -620f); // LevelUpContainer 아래
            globalItemsRect.sizeDelta = new Vector2(500f, 100f);
            
            // 배경
            var bg = globalItemsObj.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            
            // 제목
            var titleObj = new GameObject("Title");
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleObj.transform.SetParent(globalItemsObj.transform, false);
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0f, 1f);
            titleRect.anchoredPosition = new Vector2(10f, -10f);
            titleRect.sizeDelta = new Vector2(-20f, 30f);
            
            var titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "전역 아이템";
            titleText.fontSize = 20;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Left;
            
            if (koreanFontAsset != null)
            {
                titleText.font = koreanFontAsset;
            }
            
            // HorizontalLayoutGroup 추가
            var horizontalLayout = globalItemsObj.AddComponent<HorizontalLayoutGroup>();
            horizontalLayout.spacing = 10f;
            horizontalLayout.padding = new RectOffset(10, 10, 40, 10); // 상단 패딩 40 (제목 공간)
            horizontalLayout.childControlWidth = false;
            horizontalLayout.childControlHeight = false;
            horizontalLayout.childForceExpandWidth = false;
            horizontalLayout.childForceExpandHeight = false;
        }
        
        /// <summary>
        /// 전역 아이템 UI를 업데이트합니다.
        /// </summary>
        private void UpdateGlobalItemsUI()
        {
            EnsureGlobalItemsContainer();
            
            if (_globalItemsContainer == null)
            {
                return;
            }
            
            // _globalItemIcons가 null이면 초기화
            if (_globalItemIcons == null)
            {
                _globalItemIcons = new List<GameObject>();
            }
            
            // 기존 아이콘 제거
            foreach (var icon in _globalItemIcons)
            {
                if (icon != null)
                {
                    Destroy(icon);
                }
            }
            _globalItemIcons.Clear();
            
            // ItemManagementUseCase에서 전역 아이템 가져오기
            if (GameManager.Instance?.ItemManagementUseCase == null)
            {
                return;
            }
            
            var globalItems = GameManager.Instance.ItemManagementUseCase.GetGlobalItems();
            
            if (globalItems == null)
            {
                return;
            }
            
            // 아이템 아이콘 생성
            foreach (var item in globalItems)
            {
                if (item == null || string.IsNullOrEmpty(item.itemId))
                {
                    continue;
                }
                
                if (_globalItemsContainer == null || _globalItemsContainer.transform == null)
                {
                    continue;
                }
                
                var iconObj = new GameObject($"GlobalItemIcon_{item.itemId}");
                if (iconObj == null)
                {
                    continue;
                }
                
                var iconTransform = iconObj.transform;
                if (iconTransform == null)
                {
                    Destroy(iconObj);
                    continue;
                }
                
                iconTransform.SetParent(_globalItemsContainer.transform, false);
                var iconRect = iconObj.AddComponent<RectTransform>();
                if (iconRect == null)
                {
                    Destroy(iconObj);
                    continue;
                }
                iconRect.sizeDelta = new Vector2(30f, 30f);
                
                var iconImage = iconObj.AddComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.color = new Color(0.2f, 0.8f, 0.2f, 1f); // 초록색 (임시, 나중에 실제 아이콘으로 교체)
                    iconImage.raycastTarget = true; // 마우스 이벤트를 받기 위해 필요
                }
                
                // 마우스 오버 이벤트 추가
                var eventTrigger = iconObj.AddComponent<EventTrigger>();
                if (eventTrigger != null)
                {
                    // PointerEnter 이벤트
                    var pointerEnter = new EventTrigger.Entry();
                    pointerEnter.eventID = EventTriggerType.PointerEnter;
                    pointerEnter.callback.AddListener((data) => { OnItemIconPointerEnter(item, iconObj); });
                    eventTrigger.triggers.Add(pointerEnter);
                    
                    // PointerExit 이벤트
                    var pointerExit = new EventTrigger.Entry();
                    pointerExit.eventID = EventTriggerType.PointerExit;
                    pointerExit.callback.AddListener((data) => { OnItemIconPointerExit(iconObj); });
                    eventTrigger.triggers.Add(pointerExit);
                }
                
                _globalItemIcons.Add(iconObj);
            }
        }
        
        #endregion

        #region Button Clicked Events
        public void OnOptionButtonClicked()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPauseMenu();
            }
        }
        #endregion
        
        #region Tooltip Methods
        
        /// <summary>
        /// 툴팁 UI를 생성합니다.
        /// </summary>
        private void EnsureTooltipPanel()
        {
            if (_tooltipPanel != null) return;
            
            // Canvas 찾기
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = FindFirstObjectByType<Canvas>();
            }
            
            if (canvas == null) return;
            
            // 툴팁 패널 생성
            _tooltipPanel = new GameObject("ItemTooltip");
            var tooltipRect = _tooltipPanel.AddComponent<RectTransform>();
            _tooltipPanel.transform.SetParent(canvas.transform, false);
            
            // 배경
            var bg = _tooltipPanel.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            bg.raycastTarget = false; // 툴팁이 마우스 이벤트를 차단하지 않도록
            
            // 텍스트
            var textObj = new GameObject("TooltipText");
            textObj.transform.SetParent(_tooltipPanel.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = new Vector2(-20f, -20f);
            textRect.anchoredPosition = Vector2.zero;
            
            _tooltipText = textObj.AddComponent<TextMeshProUGUI>();
            if (_tooltipText != null)
            {
                _tooltipText.fontSize = 14;
                _tooltipText.color = Color.white;
                _tooltipText.alignment = TextAlignmentOptions.Left;
                _tooltipText.raycastTarget = false;
                
                if (koreanFontAsset != null)
                {
                    _tooltipText.font = koreanFontAsset;
                }
            }
            
            // 초기에는 숨김
            _tooltipPanel.SetActive(false);
        }
        
        /// <summary>
        /// 아이템 아이콘에 마우스를 올렸을 때 호출됩니다.
        /// </summary>
        private void OnItemIconPointerEnter(ItemData item, GameObject iconObj)
        {
            if (item == null || iconObj == null) return;
            
            EnsureTooltipPanel();
            if (_tooltipPanel == null || _tooltipText == null) return;
            
            // 현재 호버된 아이콘 추적
            _currentHoveredIcon = iconObj;
            
            // 툴팁 텍스트 구성
            string tooltipContent = "";
            if (!string.IsNullOrEmpty(item.itemName))
            {
                tooltipContent += $"<b>{item.itemName}</b>\n";
            }
            else if (!string.IsNullOrEmpty(item.itemId))
            {
                tooltipContent += $"<b>{item.itemId}</b>\n";
            }
            
            if (!string.IsNullOrEmpty(item.description))
            {
                tooltipContent += $"\n{item.description}";
            }
            
            if (item.cost > 0)
            {
                tooltipContent += $"\n\n<color=yellow>가격: {item.cost}</color>";
            }
            
            _tooltipText.text = tooltipContent;
            
            // 툴팁 크기 조정
            _tooltipText.ForceMeshUpdate();
            var textSize = _tooltipText.GetPreferredValues();
            var tooltipRect = _tooltipPanel.GetComponent<RectTransform>();
            tooltipRect.sizeDelta = new Vector2(textSize.x + 20f, textSize.y + 20f);
            
            // 마우스 위치에 따라 툴팁 위치 조정
            UpdateTooltipPosition();
            
            _tooltipPanel.SetActive(true);
        }
        
        /// <summary>
        /// 아이템 아이콘에서 마우스가 벗어났을 때 호출됩니다.
        /// </summary>
        private void OnItemIconPointerExit(GameObject iconObj)
        {
            // 나간 아이콘이 현재 호버된 아이콘과 같으면 툴팁 숨김
            if (_currentHoveredIcon == iconObj)
            {
                _currentHoveredIcon = null;
                if (_tooltipPanel != null)
                {
                    _tooltipPanel.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// 툴팁 위치를 마우스 위치에 맞춰 업데이트합니다.
        /// </summary>
        private void UpdateTooltipPosition()
        {
            if (_tooltipPanel == null || !_tooltipPanel.activeSelf) return;
            
            // 마우스가 실제로 아이템 아이콘 위에 있는지 확인
            if (_currentHoveredIcon == null)
            {
                // 호버된 아이콘이 없으면 툴팁 숨김
                _tooltipPanel.SetActive(false);
                return;
            }
            
            // EventSystem을 사용하여 마우스 아래에 있는 UI 요소 확인
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            
            // 마우스 아래에 현재 호버된 아이콘이 있는지 확인
            bool isOverIcon = false;
            foreach (var result in results)
            {
                if (result.gameObject == _currentHoveredIcon || 
                    result.gameObject.transform.IsChildOf(_currentHoveredIcon.transform))
                {
                    isOverIcon = true;
                    break;
                }
            }
            
            if (!isOverIcon)
            {
                // 아이콘 위에 없으면 툴팁 숨김
                _currentHoveredIcon = null;
                _tooltipPanel.SetActive(false);
                return;
            }
            
            Vector2 mousePos = Input.mousePosition;
            Canvas canvas = _tooltipPanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                RectTransform tooltipRect = _tooltipPanel.GetComponent<RectTransform>();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    mousePos,
                    canvas.worldCamera,
                    out Vector2 localPoint
                );
                
                tooltipRect.anchoredPosition = new Vector2(localPoint.x + 20f, localPoint.y - 20f);
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
                // 스테이지 종료 시 남은 적 모두 파괴
                DestroyAllEnemies();
                
                // GameManager를 통해 스테이지 종료 (UseCase 사용)
                GameManager.Instance.EndStage();
                
                // 상점으로 이동 (일시정지는 ShowShopScreen()에서 처리)
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowShopScreen();
                }
            }
        }

        /// <summary>
        /// Enemy 태그를 가진 모든 Pawn과 적 투사체를 파괴합니다.
        /// </summary>
        private void DestroyAllEnemies()
        {
            var allPawns = PawnManager.AllPawnManagers.ToArray();
            int destroyedCount = 0;
            
            foreach (var pawnManager in allPawns)
            {
                if (pawnManager != null && pawnManager.gameObject != null)
                {
                    // Enemy 태그 또는 EnemyBullet 태그를 가진 모든 Pawn 파괴
                    if (pawnManager.gameObject.CompareTag("Enemy") || 
                        pawnManager.gameObject.CompareTag("EnemyBullet"))
                    {
                        Destroy(pawnManager.gameObject);
                        destroyedCount++;
                    }
                }
            }
            
            if (destroyedCount > 0)
            {
                LogManager.LogInfo(LogCategory.Stage, $"스테이지 종료: {destroyedCount}개의 적/적 투사체 파괴");
            }
        }

        // StagePause(), OpenOptionMenu(), OptionAndPause() 제거
        // UIManager가 ESC와 일시정지를 중앙에서 관리
        #endregion

        #endregion
    }
}

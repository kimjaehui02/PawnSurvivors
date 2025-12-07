using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using PawnSurvivors.Managers;
using Newtonsoft.Json;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 캠페인 선택 화면입니다. (구 메인 메뉴)
    /// </summary>
    public class CampaignSelectScreen : MonoBehaviour
    {
        /// <summary>
        /// UI 요소 타입 enum (하드코딩된 string 제거)
        /// </summary>
        private enum UIElementType
        {
            Title,
            CampaignListContainer,
            BackButton
        }
        
        /// <summary>
        /// UI 요소 참조를 enum으로 관리 (내부적으로도 enum 사용)
        /// </summary>
        private Dictionary<UIElementType, GameObject> _uiElements = new Dictionary<UIElementType, GameObject>();
        
        private GameObject _campaignListContainer => _uiElements.ContainsKey(UIElementType.CampaignListContainer) 
            ? _uiElements[UIElementType.CampaignListContainer] 
            : null;
        
        private Dictionary<string, CampaignData> _campaigns = new Dictionary<string, CampaignData>();
        private TMP_FontAsset _font;
        private bool _campaignsLoaded = false;

        private void Awake()
        {
            LoadFont();
            
            // UI 요소들을 자식에서 찾아서 enum으로 매핑
            InitializeUIElements();
            
            // UI가 없으면 생성
            if (!_uiElements.ContainsKey(UIElementType.CampaignListContainer))
            {
                CreateUI();
            }
            
            // _campaignListContainer가 여전히 null이면 에러
            if (_campaignListContainer == null)
            {
                Debug.LogError("[CampaignSelectScreen] CampaignListContainer를 찾을 수 없습니다. 캠페인 버튼을 생성할 수 없습니다.");
                return;
            }
        }
        
        private void Start()
        {
            // 씬이 독립적으로 작동하므로 Start()에서 캠페인 로드
            LoadCampaigns();
        }
        
        /// <summary>
        /// 자식 Transform들을 순회하면서 UI 요소를 enum으로 매핑합니다.
        /// </summary>
        private void InitializeUIElements()
        {
            _uiElements.Clear();
            
            // 모든 자식 Transform을 순회
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                string childName = child.name;
                
                // enum 값과 이름이 일치하는지 확인
                if (System.Enum.TryParse<UIElementType>(childName, out UIElementType elementType))
                {
                    _uiElements[elementType] = child.gameObject;
                }
            }
        }
        
        private void LoadFont()
        {
            _font = Resources.Load<TMP_FontAsset>("Fonts/NanumGothic SDF");
            if (_font == null)
            {
                Debug.LogWarning("[CampaignSelectScreen] NanumGothic SDF 폰트를 찾을 수 없습니다.");
            }
        }
        

        private void CreateUI()
        {
            // Canvas가 이미 있으면 재사용, 없으면 생성
            Canvas canvas = gameObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // GraphicRaycaster 중복 추가 방지
            if (gameObject.GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            
            // CanvasScaler 중복 추가 방지
            CanvasScaler scaler = gameObject.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = gameObject.AddComponent<CanvasScaler>();
            }
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f; // 화면 비율 맞춤
            
            // EventSystem 확인 및 생성
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            
            // 제목
            GameObject titleObj = new GameObject(UIElementType.Title.ToString());
            _uiElements[UIElementType.Title] = titleObj;
            titleObj.transform.SetParent(transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.9f);
            titleRect.anchorMax = new Vector2(0.5f, 0.9f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = new Vector2(800, 100);
            
            TMP_Text titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "캠페인 선택";
            titleText.fontSize = 60;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;
            if (_font != null) titleText.font = _font;
            
            // 캠페인 리스트 컨테이너
            GameObject containerObj = new GameObject(UIElementType.CampaignListContainer.ToString());
            containerObj.transform.SetParent(transform, false);
            RectTransform containerRect = containerObj.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.anchoredPosition = Vector2.zero;
            containerRect.sizeDelta = new Vector2(800, 600);
            
            GridLayoutGroup grid = containerObj.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(700, 100); // 크기 축소
            grid.spacing = new Vector2(10, 15);
            grid.childAlignment = TextAnchor.MiddleCenter;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 1; // 1열로 고정
            
            _uiElements[UIElementType.CampaignListContainer] = containerObj;
            
            // 뒤로 가기 버튼
            GameObject backBtnObj = new GameObject(UIElementType.BackButton.ToString());
            _uiElements[UIElementType.BackButton] = backBtnObj;
            backBtnObj.transform.SetParent(transform, false);
            RectTransform backRect = backBtnObj.AddComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0.5f, 0.1f);
            backRect.anchorMax = new Vector2(0.5f, 0.1f);
            backRect.anchoredPosition = Vector2.zero;
            backRect.sizeDelta = new Vector2(300, 80);
            
            Image backImg = backBtnObj.AddComponent<Image>();
            backImg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            
            Button backBtn = backBtnObj.AddComponent<Button>();
            backBtn.onClick.AddListener(OnBackButtonClicked);
            
            GameObject backTextObj = new GameObject("Text");
            backTextObj.transform.SetParent(backBtnObj.transform, false);
            TMP_Text backText = backTextObj.AddComponent<TextMeshProUGUI>();
            backText.text = "뒤로";
            backText.fontSize = 36;
            backText.alignment = TextAlignmentOptions.Center;
            backText.color = Color.white;
            if (_font != null) backText.font = _font;
            RectTransform backTextRect = backTextObj.GetComponent<RectTransform>();
            backTextRect.anchorMin = Vector2.zero;
            backTextRect.anchorMax = Vector2.one;
            backTextRect.sizeDelta = Vector2.zero;
        }

        /// <summary>
        /// JSON에서 캠페인을 읽어와서 버튼을 생성합니다.
        /// 씬이 독립적으로 작동하므로 CampaignSelectScreen이 직접 담당합니다.
        /// </summary>
        private void LoadCampaigns()
        {
            if (_campaignsLoaded) return; // 이미 로드했으면 중복 방지
            
            if (_campaignListContainer == null)
            {
                Debug.LogError("[CampaignSelectScreen] CampaignListContainer를 찾을 수 없습니다.");
                return;
            }
            
            // 기존 캠페인 버튼 모두 제거 (중복 방지)
            ClearCampaignButtons();
            
            // CampaignList.json 로드
            TextAsset jsonFile = Resources.Load<TextAsset>("StreamingAssets/Campaigns/CampaignList");
            if (jsonFile == null)
            {
                Debug.LogError("[CampaignSelectScreen] CampaignList.json not found!");
                return;
            }

            var campaignList = JsonConvert.DeserializeObject<CampaignListData>(jsonFile.text);
            if (campaignList?.campaigns == null)
            {
                Debug.LogError("[CampaignSelectScreen] Failed to parse CampaignList.json");
                return;
            }

            // 캠페인 데이터 저장 및 버튼 생성
            _campaigns.Clear();
            foreach (var campaign in campaignList.campaigns)
            {
                _campaigns[campaign.id] = campaign;
                CreateCampaignButton(campaign);
            }
            
            _campaignsLoaded = true;
            Debug.Log($"[CampaignSelectScreen] {_campaigns.Count}개의 캠페인을 로드했습니다.");
        }
        
        /// <summary>
        /// 기존 캠페인 버튼들을 모두 제거합니다.
        /// </summary>
        private void ClearCampaignButtons()
        {
            if (_campaignListContainer == null) return;
            
            // CampaignListContainer의 모든 자식 제거
            for (int i = _campaignListContainer.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = _campaignListContainer.transform.GetChild(i);
                // Campaign_ 접두사로 시작하는 버튼만 제거
                if (child.name.StartsWith("Campaign_"))
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private void CreateCampaignButton(CampaignData campaign)
        {
            GameObject btnObj = new GameObject($"Campaign_{campaign.id}");
            btnObj.transform.SetParent(_campaignListContainer.transform, false);
            
            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(0.2f, 0.4f, 0.6f, 1f);
            
            Button btn = btnObj.AddComponent<Button>();
            btn.onClick.AddListener(() => OnCampaignSelected(campaign.id));
            
            // 캠페인 이름
            GameObject nameObj = new GameObject("Name");
            nameObj.transform.SetParent(btnObj.transform, false);
            RectTransform nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.05f, 0.6f);
            nameRect.anchorMax = new Vector2(0.95f, 0.95f);
            nameRect.sizeDelta = Vector2.zero;
            
            TMP_Text nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = campaign.name;
            nameText.fontSize = 40;
            nameText.alignment = TextAlignmentOptions.Left;
            nameText.color = Color.white;
            if (_font != null) nameText.font = _font;
            
            // 캠페인 설명
            GameObject descObj = new GameObject("Description");
            descObj.transform.SetParent(btnObj.transform, false);
            RectTransform descRect = descObj.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0.05f, 0.05f);
            descRect.anchorMax = new Vector2(0.95f, 0.55f);
            descRect.sizeDelta = Vector2.zero;
            
            TMP_Text descText = descObj.AddComponent<TextMeshProUGUI>();
            descText.text = campaign.description;
            descText.fontSize = 24;
            descText.alignment = TextAlignmentOptions.Left;
            descText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            if (_font != null) descText.font = _font;
        }

        /// <summary>
        /// 캠페인 선택 시 호출됩니다.
        /// CampaignSelectScreen이 씬의 로직을 담당하므로 직접 처리합니다.
        /// </summary>
        private void OnCampaignSelected(string campaignId)
        {
            if (!_campaigns.ContainsKey(campaignId))
            {
                Debug.LogError($"[CampaignSelectScreen] Campaign '{campaignId}' not found!");
                return;
            }

            CampaignData campaign = _campaigns[campaignId];
            
            if (campaign.stages == null || campaign.stages.Length == 0)
            {
                Debug.LogError($"[CampaignSelectScreen] Campaign '{campaignId}'에 스테이지가 없습니다.");
                return;
            }
            
            // GameManager가 준비될 때까지 기다림
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("[CampaignSelectScreen] GameManager가 아직 준비되지 않았습니다. 다음 프레임에 다시 시도합니다.");
                StartCoroutine(RetryCampaignSelection(campaignId));
                return;
            }
            
            // 선택한 캠페인을 GameManager에 전달
            if (GameManager.Instance.StageFlowUseCase != null)
            {
                GameManager.Instance.StageFlowUseCase.SetCampaignStages(campaign.stages);
            }
            
            // 첫 스테이지 이름을 세션 데이터에 저장
            if (GameManager.Instance.StageManagementUseCase != null)
            {
                GameManager.Instance.StageManagementUseCase.PrepareStageStart(campaign.stages[0], shouldResetSession: false);
            }
            
            // GameStateManager를 통해 StageState로 전환
            // StageState.OnEnter()에서 StartStage() 호출
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.GoToStage();
            }
        }

        /// <summary>
        /// GameManager가 준비될 때까지 기다렸다가 캠페인 선택을 재시도합니다.
        /// </summary>
        private IEnumerator RetryCampaignSelection(string campaignId)
        {
            float timeout = 5f; // 최대 5초 대기
            float elapsed = 0f;
            
            while (GameManager.Instance == null && elapsed < timeout)
            {
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
            }
            
            if (GameManager.Instance != null)
            {
                OnCampaignSelected(campaignId);
            }
            else
            {
                Debug.LogError("[CampaignSelectScreen] GameManager를 기다리는 중 타임아웃이 발생했습니다.");
            }
        }
        
        private void OnBackButtonClicked()
        {
            // GameStateManager를 통해 CharacterSelectState로 전환 (씬 전환)
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.GoToCharacterSelect();
            }
        }
    }

    [System.Serializable]
    public class CampaignListData
    {
        public CampaignData[] campaigns;
    }

    [System.Serializable]
    public class CampaignData
    {
        public string id;
        public string name;
        public string description;
        public string[] stages;
    }
}


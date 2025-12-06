using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
        private GameObject _campaignListContainer;
        private Dictionary<string, CampaignData> _campaigns = new Dictionary<string, CampaignData>();
        private TMP_FontAsset _font;

        private void Awake()
        {
            LoadFont();
            CreateUI();
            LoadCampaigns();
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
            // Canvas 생성
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // GraphicRaycaster 추가 (클릭 감지용)
            gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
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
            GameObject titleObj = new GameObject("Title");
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
            GameObject containerObj = new GameObject("CampaignListContainer");
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
            
            _campaignListContainer = containerObj;
            
            // 뒤로 가기 버튼
            GameObject backBtnObj = new GameObject("BackButton");
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

        private void LoadCampaigns()
        {
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

            // 캠페인 버튼 생성
            foreach (var campaign in campaignList.campaigns)
            {
                _campaigns[campaign.id] = campaign;
                CreateCampaignButton(campaign);
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

        private void OnCampaignSelected(string campaignId)
        {
            if (!_campaigns.ContainsKey(campaignId))
            {
                Debug.LogError($"[CampaignSelectScreen] Campaign '{campaignId}' not found!");
                return;
            }

            CampaignData campaign = _campaigns[campaignId];
            
            // 선택한 캠페인을 GameManager에 전달
            if (GameManager.Instance?.StageFlowUseCase != null)
            {
                GameManager.Instance.StageFlowUseCase.SetCampaignStages(campaign.stages);
            }
            
            // 첫 스테이지 시작
            // 주의: StartStage()는 StageState.OnEnter()에서 호출되므로 여기서는 호출하지 않음
            // 대신 StageState로 전환만 하고, StageState.OnEnter()에서 StartStage() 호출
            if (campaign.stages.Length > 0 && GameManager.Instance != null)
            {
                // 스테이지 이름 설정 (StartStage는 StageState에서 호출)
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
        }

        private void OnBackButtonClicked()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowCharacterSelectScreen();
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


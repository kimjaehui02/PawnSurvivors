using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using PawnSurvivors.Domain;
using PawnSurvivors.Managers;

namespace PawnSurvivors.UI
{
    public class ShopPawnListScreen : MonoBehaviour
    {
        public Button _closeButton;
        [Tooltip("프리팹으로 미리 만든 버튼 6개를 Inspector에서 할당하세요")]
        public List<Button> _pawnButtons = new List<Button>();
        
        [Tooltip("플레이어 증감을 자동으로 감지하여 UI를 업데이트합니다")]
        public bool autoUpdateOnPawnChange = true;
        
        [Tooltip("아이템 장착 상태에 따라 버튼을 필터링합니다 (필요시 활성화)")]
        public bool filterByEquippedItems = false;
        
        private const int MAX_PAWN_BUTTONS = 6;
        private int _lastPawnCount = -1; // 이전 프레임의 플레이어 수

        private void OnEnable()
        {
            // 화면이 활성화될 때 초기화 및 첫 업데이트
            _lastPawnCount = -1; // 강제로 업데이트되도록 설정
            RefreshPawnButtons();
        }

        private void Update()
        {
            // 자동 업데이트가 활성화되어 있으면 플레이어 수 변경 감지
            if (autoUpdateOnPawnChange)
            {
                CheckAndUpdatePawnCount();
            }
        }

        /// <summary>
        /// 플레이어 수가 변경되었는지 확인하고 UI를 업데이트합니다.
        /// </summary>
        private void CheckAndUpdatePawnCount()
        {
            if (GameManager.Instance?.PlayerController == null) return;
            
            var playerPawns = GameManager.Instance.PlayerController.playerPawns;
            int currentCount = playerPawns != null ? playerPawns.Count : 0;
            
            // 플레이어 수가 변경되었으면 UI 업데이트
            if (currentCount != _lastPawnCount)
            {
                _lastPawnCount = currentCount;
                RefreshPawnButtons();
            }
        }

        /// <summary>
        /// 플레이어가 가진 모든 Pawn의 데이터를 가져옵니다.
        /// </summary>
        public List<PawnData> GetPawnList()
        {
            var pawnDataList = new List<PawnData>();
            
            if (GameManager.Instance?.PlayerController == null) 
                return pawnDataList;
            
            var playerPawns = GameManager.Instance.PlayerController.playerPawns;
            if (playerPawns == null) 
                return pawnDataList;
            
            foreach (var pawn in playerPawns)
            {
                if (pawn == null) continue;
                
                // ✅ PawnManager를 통해 PawnData 가져오기
                var pawnManager = pawn.GetComponent<PawnManager>();
                if (pawnManager == null || pawnManager.PawnData == null) continue;
                
                pawnDataList.Add(pawnManager.PawnData);
            }
            
            return pawnDataList;
        }
        
        /// <summary>
        /// 실제 폰 개수에 따라 버튼을 활성화/비활성화하고 데이터를 바인딩합니다.
        /// </summary>
        public void RefreshPawnButtons()
        {
            var pawnDataList = GetPawnList();
            
            // 아이템 장착 필터링이 활성화되어 있으면 필터링
            if (filterByEquippedItems)
            {
                pawnDataList = FilterPawnsByEquippedItems(pawnDataList);
            }
            
            int pawnCount = pawnDataList.Count;
            
            // 버튼 리스트가 비어있거나 부족하면 경고
            if (_pawnButtons == null || _pawnButtons.Count < MAX_PAWN_BUTTONS)
            {
                Debug.LogWarning($"[ShopPawnListScreen] 버튼이 {MAX_PAWN_BUTTONS}개 필요합니다. 현재: {_pawnButtons?.Count ?? 0}개");
                return;
            }
            
            // 실제 폰 개수만큼 버튼 활성화 및 데이터 바인딩
            for (int i = 0; i < MAX_PAWN_BUTTONS; i++)
            {
                if (_pawnButtons[i] == null) continue;
                
                if (i < pawnCount)
                {
                    // 버튼 활성화 및 데이터 바인딩
                    _pawnButtons[i].gameObject.SetActive(true);
                    BindPawnDataToButton(_pawnButtons[i], pawnDataList[i]);
                }
                else
                {
                    // 폰이 없는 슬롯은 비활성화
                    _pawnButtons[i].gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// 아이템 장착 상태에 따라 Pawn 리스트를 필터링합니다.
        /// </summary>
        private List<PawnData> FilterPawnsByEquippedItems(List<PawnData> pawnDataList)
        {
            if (pawnDataList == null || pawnDataList.Count == 0) 
                return pawnDataList;
            
            if (GameManager.Instance?.ItemManagementUseCase == null)
                return pawnDataList;
            
            var filteredList = new List<PawnData>();
            
            foreach (var pawnData in pawnDataList)
            {
                if (pawnData == null) continue;
                
                // 아이템이 장착되어 있는지 확인
                var equippedItems = GameManager.Instance.ItemManagementUseCase.GetEquippedItems(pawnData.playerIndex);
                
                // 아이템이 하나라도 장착되어 있으면 표시 (필요에 따라 조건 변경 가능)
                if (equippedItems != null && equippedItems.Count > 0)
                {
                    filteredList.Add(pawnData);
                }
            }
            
            return filteredList;
        }
        
        /// <summary>
        /// 버튼에 Pawn 데이터를 바인딩합니다.
        /// </summary>
        private void BindPawnDataToButton(Button button, PawnData pawnData)
        {
            if (button == null || pawnData == null) return;
            
            // ✅ Enum 기반 캐릭터 타입 확인
            if (!pawnData.characterType.HasValue) return;
            
            PlayerCharacter character = pawnData.characterType.Value;
            string pawnName = pawnData.recipeName;
            int playerIndex = pawnData.playerIndex;
            
            // 버튼 텍스트 업데이트 (TextMeshProUGUI 찾기)
            var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                // 아이템 장착 개수 표시 (선택사항)
                string displayText = pawnName;
                if (GameManager.Instance?.ItemManagementUseCase != null)
                {
                    var equippedItems = GameManager.Instance.ItemManagementUseCase.GetEquippedItems(playerIndex);
                    if (equippedItems != null && equippedItems.Count > 0)
                    {
                        displayText = $"{pawnName} ({equippedItems.Count})";
                    }
                }
                buttonText.text = displayText;
            }
            
            // 기존 클릭 이벤트 제거 후 새로 등록
            button.onClick.RemoveAllListeners();
            PlayerCharacter capturedChar = character;
            int capturedIndex = playerIndex;
            button.onClick.AddListener(() => OnPawnButtonClicked(capturedChar, capturedIndex));
        }
        
        /// <summary>
        /// Pawn 버튼 클릭 시 호출됩니다.
        /// </summary>
        private void OnPawnButtonClicked(PlayerCharacter character, int playerIndex)
        {
            // 여기에 클릭 처리 로직 추가
            Debug.Log($"Pawn 선택: {character}, Index: {playerIndex}");
        }
    }
}
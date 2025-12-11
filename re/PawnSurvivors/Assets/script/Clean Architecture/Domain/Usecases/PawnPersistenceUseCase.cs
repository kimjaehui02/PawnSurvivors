using System.Collections.Generic;
using System.Linq;
using PawnSurvivors.Domain;
using PawnSurvivors.Data;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// 플레이어 Pawn의 영구 데이터 관리를 담당하는 UseCase입니다.
    /// 라운드 간 유지되어야 하는 데이터(경험치, 레벨 등)를 저장/복원합니다.
    /// </summary>
    public class PawnPersistenceUseCase
    {
        private readonly GameSessionData _sessionData;

        public PawnPersistenceUseCase(GameSessionData sessionData)
        {
            _sessionData = sessionData;
        }

        /// <summary>
        /// PawnData에서 영구 데이터를 추출하여 저장합니다.
        /// 스테이지 종료 시 호출합니다.
        /// </summary>
        public void SavePawnPersistentData(PawnData pawnData, int currentLevel)
        {
            if (pawnData == null || !pawnData.characterType.HasValue) return;

            string key = _sessionData.GetPawnPersistentDataKey(pawnData.recipeName, pawnData.playerIndex);
            
            // 기존 데이터 가져오기 또는 새로 생성
            if (!_sessionData.playerPawnPersistentData.ContainsKey(key))
            {
                _sessionData.playerPawnPersistentData[key] = new PawnPersistentData
                {
                    characterType = pawnData.characterType.Value,
                    playerIndex = pawnData.playerIndex
                };
            }

            var persistentData = _sessionData.playerPawnPersistentData[key];
            
            // 경험치 저장
            if (pawnData.experienceData != null)
            {
                persistentData.experienceProgress = pawnData.experienceData.currentProgress;
            }
            
            // 레벨 저장
            persistentData.currentLevel = currentLevel;
            
            // 강화 레벨 저장 (중복 구매 강화 시스템)
            persistentData.upgradeLevel = pawnData.upgradeLevel;
            
            // 업그레이드 저장 (PawnData에 upgrades 필드가 있는 경우)
            // TODO: PawnData에 upgrades 필드가 추가되면 이 부분을 활성화
            // if (pawnData.upgrades != null)
            // {
            //     persistentData.upgrades.Clear();
            //     foreach (var kvp in pawnData.upgrades)
            //     {
            //         persistentData.upgrades[kvp.Key.ToString()] = kvp.Value;
            //     }
            // }
            
            // 영구 스탯 저장 (PawnData에 permanentStats 필드가 있는 경우)
            // TODO: PawnData에 permanentStats 필드가 추가되면 이 부분을 활성화
            // if (pawnData.permanentStats != null)
            // {
            //     persistentData.permanentStats.Clear();
            //     foreach (var kvp in pawnData.permanentStats)
            //     {
            //         persistentData.permanentStats[kvp.Key.ToString()] = kvp.Value;
            //     }
            // }
        }

        /// <summary>
        /// 저장된 영구 데이터를 PawnData에 복원합니다.
        /// 스테이지 시작 시 호출합니다.
        /// </summary>
        public void RestorePawnPersistentData(PawnData pawnData)
        {
            if (pawnData == null) return;

            string key = _sessionData.GetPawnPersistentDataKey(pawnData.recipeName, pawnData.playerIndex);
            
            if (!_sessionData.playerPawnPersistentData.ContainsKey(key))
            {
                return; // 저장된 데이터가 없음
            }

            var persistentData = _sessionData.playerPawnPersistentData[key];

            // 경험치 복원
            if (pawnData.experienceData != null)
            {
                pawnData.experienceData.currentProgress = persistentData.experienceProgress;
            }
            
            // 강화 레벨 복원 (중복 구매 강화 시스템)
            // 실제 스탯 적용은 CharacterUpgradeSubManager의 SubStart()에서 처리됨
            pawnData.upgradeLevel = persistentData.upgradeLevel;
            
            // 업그레이드 복원 (PawnData에 upgrades 필드가 있는 경우)
            // TODO: PawnData에 upgrades 필드가 추가되면 이 부분을 활성화
            // if (persistentData.upgrades != null && pawnData.upgrades != null)
            // {
            //     foreach (var kvp in persistentData.upgrades)
            //     {
            //         if (int.TryParse(kvp.Key, out int enumKey))
            //         {
            //             pawnData.upgrades[enumKey] = kvp.Value;
            //         }
            //     }
            // }
            
            // 영구 스탯 복원 (PawnData에 permanentStats 필드가 있는 경우)
            // TODO: PawnData에 permanentStats 필드가 추가되면 이 부분을 활성화
            // if (persistentData.permanentStats != null && pawnData.permanentStats != null)
            // {
            //     foreach (var kvp in persistentData.permanentStats)
            //     {
            //         if (int.TryParse(kvp.Key, out int enumKey))
            //         {
            //             pawnData.permanentStats[enumKey] = kvp.Value;
            //         }
            //     }
            // }
        }

        /// <summary>
        /// 특정 Pawn의 영구 데이터를 가져옵니다. (enum 기반, 권장)
        /// </summary>
        public PawnPersistentData GetPersistentData(PlayerCharacter character, int playerIndex)
        {
            string recipeName = character.ToString();
            string key = _sessionData.GetPawnPersistentDataKey(recipeName, playerIndex);
            return _sessionData.playerPawnPersistentData.ContainsKey(key) 
                ? _sessionData.playerPawnPersistentData[key] 
                : null;
        }

        /// <summary>
        /// 특정 Pawn의 영구 데이터를 가져옵니다. (하위 호환성)
        /// </summary>
        public PawnPersistentData GetPersistentData(string recipeName, int playerIndex)
        {
            string key = _sessionData.GetPawnPersistentDataKey(recipeName, playerIndex);
            return _sessionData.playerPawnPersistentData.ContainsKey(key) 
                ? _sessionData.playerPawnPersistentData[key] 
                : null;
        }

        /// <summary>
        /// 모든 플레이어 Pawn의 영구 데이터를 가져옵니다.
        /// </summary>
        public List<PawnPersistentData> GetAllPersistentData()
        {
            return _sessionData.playerPawnPersistentData.Values.ToList();
        }

        /// <summary>
        /// 특정 캐릭터의 모든 Pawn 영구 데이터를 가져옵니다. (enum 기반, 권장)
        /// </summary>
        public List<PawnPersistentData> GetPersistentDataByCharacter(PlayerCharacter character)
        {
            return _sessionData.playerPawnPersistentData.Values
                .Where(data => data.characterType == character)
                .ToList();
        }

        /// <summary>
        /// 특정 레시피의 모든 Pawn 영구 데이터를 가져옵니다. (하위 호환성)
        /// </summary>
        public List<PawnPersistentData> GetPersistentDataByRecipe(string recipeName)
        {
            return _sessionData.playerPawnPersistentData.Values
                .Where(data => data.recipeName == recipeName)
                .ToList();
        }

        /// <summary>
        /// 플레이어 Pawn의 영구 데이터를 제거합니다. (enum 기반, 권장)
        /// </summary>
        public bool RemovePersistentData(PlayerCharacter character, int playerIndex)
        {
            string recipeName = character.ToString();
            string key = _sessionData.GetPawnPersistentDataKey(recipeName, playerIndex);
            return _sessionData.playerPawnPersistentData.Remove(key);
        }

        /// <summary>
        /// 플레이어 Pawn의 영구 데이터를 제거합니다. (하위 호환성)
        /// </summary>
        public bool RemovePersistentData(string recipeName, int playerIndex)
        {
            string key = _sessionData.GetPawnPersistentDataKey(recipeName, playerIndex);
            return _sessionData.playerPawnPersistentData.Remove(key);
        }

        // ========================================
        // 활성 Pawn 데이터 관리 (씬 전환 시 사용)
        // ========================================

        /// <summary>
        /// 현재 활성화된 모든 Pawn의 상태를 저장합니다.
        /// 스테이지 종료 시 호출하여 다음 씬에서 복원할 수 있도록 합니다.
        /// </summary>
        /// <param name="playerPawns">PlayerController의 playerPawns 리스트</param>
        public void SaveActivePawns(List<UnityEngine.GameObject> playerPawns)
        {
            _sessionData.activePawnDataList.Clear();

            if (playerPawns == null || playerPawns.Count == 0)
            {
                UnityEngine.Debug.Log("[PawnPersistenceUseCase] 저장할 활성 Pawn이 없습니다.");
                return;
            }

            foreach (var pawn in playerPawns)
            {
                if (pawn == null) continue;

                var pawnManager = pawn.GetComponent<PawnManager>();
                if (pawnManager == null || pawnManager.PawnData == null) continue;

                bool isAlive = pawn.activeInHierarchy;
                var activePawnData = ActivePawnData.FromPawnData(pawnManager.PawnData, isAlive);

                if (activePawnData != null)
                {
                    _sessionData.activePawnDataList.Add(activePawnData);
                    UnityEngine.Debug.Log($"[PawnPersistenceUseCase] 활성 Pawn 저장: {activePawnData.characterType} (Index: {activePawnData.playerIndex}, Alive: {isAlive})");
                }
            }

            UnityEngine.Debug.Log($"[PawnPersistenceUseCase] 총 {_sessionData.activePawnDataList.Count}개의 활성 Pawn 저장 완료");
        }

        /// <summary>
        /// 저장된 활성 Pawn 데이터 목록을 가져옵니다.
        /// 스테이지 시작 시 호출하여 Pawn을 복원하는 데 사용합니다.
        /// </summary>
        public List<ActivePawnData> GetActivePawns()
        {
            return _sessionData.activePawnDataList;
        }

        /// <summary>
        /// 활성 Pawn 목록이 있는지 확인합니다.
        /// </summary>
        public bool HasActivePawns()
        {
            return _sessionData.activePawnDataList != null && _sessionData.activePawnDataList.Count > 0;
        }

        /// <summary>
        /// 활성 Pawn 목록을 초기화합니다.
        /// 새 게임 시작 시 호출합니다.
        /// </summary>
        public void ClearActivePawns()
        {
            _sessionData.activePawnDataList.Clear();
            UnityEngine.Debug.Log("[PawnPersistenceUseCase] 활성 Pawn 목록 초기화됨");
        }

        /// <summary>
        /// 새 캐릭터를 활성 Pawn 목록에 추가합니다.
        /// 상점에서 캐릭터 구매 시 호출합니다.
        /// </summary>
        public void AddActivePawn(PlayerCharacter characterType)
        {
            int newIndex = _sessionData.activePawnDataList.Count;

            var newPawnData = new ActivePawnData
            {
                characterType = characterType,
                playerIndex = newIndex,
                currentHealth = 100f, // 기본값, 실제로는 레시피에서 로드 필요
                maxHealth = 100f,
                experienceProgress = 0f,
                upgradeLevel = 0,
                isAlive = true
            };

            _sessionData.activePawnDataList.Add(newPawnData);
            UnityEngine.Debug.Log($"[PawnPersistenceUseCase] 활성 Pawn 추가: {characterType} (Index: {newIndex})");
        }

        /// <summary>
        /// 활성 Pawn 개수를 반환합니다.
        /// </summary>
        public int GetActivePawnCount()
        {
            return _sessionData.activePawnDataList?.Count ?? 0;
        }
    }
}


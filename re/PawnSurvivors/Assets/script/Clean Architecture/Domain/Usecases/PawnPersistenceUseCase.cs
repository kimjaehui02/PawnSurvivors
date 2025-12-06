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
    }
}


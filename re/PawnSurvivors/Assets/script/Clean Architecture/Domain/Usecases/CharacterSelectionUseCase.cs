using System.Collections.Generic;
using System.Linq;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// 캐릭터 선택 관련 비즈니스 로직을 담당하는 UseCase입니다.
    /// </summary>
    public class CharacterSelectionUseCase
    {
        private readonly ISessionDataRepository _sessionRepository;
        
        // 선택 가능한 캐릭터 목록 (enum 기반)
        private List<PlayerCharacter> _availableCharacters;
        
        // 현재 선택된 캐릭터들 (enum 기반)
        private List<PlayerCharacter> _selectedCharacters;

        public CharacterSelectionUseCase(ISessionDataRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
            _availableCharacters = new List<PlayerCharacter>();
            _selectedCharacters = new List<PlayerCharacter>();
        }

        /// <summary>
        /// 선택 가능한 캐릭터를 enum 리스트로 설정합니다. (enum 기반, 권장)
        /// </summary>
        public void SetAvailableCharacters(List<PlayerCharacter> characters)
        {
            _availableCharacters = characters != null ? new List<PlayerCharacter>(characters) : new List<PlayerCharacter>();
        }

        /// <summary>
        /// 선택 가능한 캐릭터를 string 리스트로 설정합니다. (하위 호환성)
        /// </summary>
        public void SetAvailableCharacters(List<string> characterNames)
        {
            _availableCharacters = new List<PlayerCharacter>();
            if (characterNames == null || characterNames.Count == 0)
            {
                UnityEngine.Debug.LogWarning("[CharacterSelectionUseCase] SetAvailableCharacters: characterNames가 null이거나 비어있습니다.");
                return;
            }
            
            foreach (var name in characterNames)
            {
                if (System.Enum.TryParse<PlayerCharacter>(name, true, out PlayerCharacter character))
                {
                    _availableCharacters.Add(character);
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"[CharacterSelectionUseCase] 캐릭터 이름을 enum으로 변환 실패: '{name}'. 사용 가능한 enum 값: {string.Join(", ", System.Enum.GetNames(typeof(PlayerCharacter)))}");
                }
            }
            
            UnityEngine.Debug.Log($"[CharacterSelectionUseCase] SetAvailableCharacters 완료: {_availableCharacters.Count}개 캐릭터 설정됨 (입력: {characterNames.Count}개)");
        }

        /// <summary>
        /// 선택 가능한 캐릭터를 enum 리스트로 가져옵니다. (enum 기반, 권장)
        /// </summary>
        public List<PlayerCharacter> GetAvailableCharacters()
        {
            return new List<PlayerCharacter>(_availableCharacters);
        }

        /// <summary>
        /// 선택 가능한 캐릭터를 string 리스트로 가져옵니다. (하위 호환성)
        /// </summary>
        public List<string> GetAvailableCharactersAsString()
        {
            return _availableCharacters.Select(c => c.ToString()).ToList();
        }

        /// <summary>
        /// 캐릭터를 선택합니다. (enum 기반, 권장)
        /// </summary>
        public bool SelectCharacter(PlayerCharacter character)
        {
            if (!_availableCharacters.Contains(character))
            {
                return false;
            }

            if (!_selectedCharacters.Contains(character))
            {
                _selectedCharacters.Add(character);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 캐릭터를 선택합니다. (하위 호환성)
        /// </summary>
        public bool SelectCharacter(string characterName)
        {
            if (!System.Enum.TryParse<PlayerCharacter>(characterName, out PlayerCharacter character))
            {
                return false;
            }
            return SelectCharacter(character);
        }

        /// <summary>
        /// 캐릭터 선택을 해제합니다. (enum 기반, 권장)
        /// </summary>
        public bool DeselectCharacter(PlayerCharacter character)
        {
            return _selectedCharacters.Remove(character);
        }

        /// <summary>
        /// 캐릭터 선택을 해제합니다. (하위 호환성)
        /// </summary>
        public bool DeselectCharacter(string characterName)
        {
            if (!System.Enum.TryParse<PlayerCharacter>(characterName, out PlayerCharacter character))
            {
                return false;
            }
            return DeselectCharacter(character);
        }

        /// <summary>
        /// 캐릭터가 선택되었는지 확인합니다. (enum 기반, 권장)
        /// </summary>
        public bool IsCharacterSelected(PlayerCharacter character)
        {
            return _selectedCharacters.Contains(character);
        }

        /// <summary>
        /// 캐릭터가 선택되었는지 확인합니다. (하위 호환성)
        /// </summary>
        public bool IsCharacterSelected(string characterName)
        {
            if (!System.Enum.TryParse<PlayerCharacter>(characterName, out PlayerCharacter character))
            {
                return false;
            }
            return IsCharacterSelected(character);
        }

        /// <summary>
        /// 선택된 캐릭터를 enum 리스트로 가져옵니다. (enum 기반, 권장)
        /// </summary>
        public List<PlayerCharacter> GetSelectedCharacters()
        {
            return new List<PlayerCharacter>(_selectedCharacters);
        }

        /// <summary>
        /// 선택된 캐릭터를 string 리스트로 가져옵니다. (하위 호환성)
        /// </summary>
        public List<string> GetSelectedCharactersAsString()
        {
            return _selectedCharacters.Select(c => c.ToString()).ToList();
        }

        public int GetSelectedCount()
        {
            return _selectedCharacters.Count;
        }

        public void ClearSelection()
        {
            _selectedCharacters.Clear();
        }

        public void ConfirmSelection()
        {
            // 선택 확정
        }

        /// <summary>
        /// 선택된 캐릭터를 세션 데이터에 저장합니다. (enum 기반 플래그 세트 사용)
        /// </summary>
        public void SaveSelectedCharacters()
        {
            // 기존 플래그 제거
            var existingFlags = _sessionRepository.GetFlags<PlayerCharacter>("selectedCharacters");
            foreach (var flag in existingFlags)
            {
                _sessionRepository.RemoveFlag("selectedCharacters", flag);
            }
            
            // 선택된 캐릭터를 플래그로 저장
            foreach (var character in _selectedCharacters)
            {
                _sessionRepository.AddFlag("selectedCharacters", character);
            }
        }

        /// <summary>
        /// 세션 데이터에서 선택된 캐릭터를 불러옵니다. (enum 기반 플래그 세트 사용)
        /// </summary>
        public void LoadSelectedCharacters()
        {
            var savedFlags = _sessionRepository.GetFlags<PlayerCharacter>("selectedCharacters");
            _selectedCharacters = new List<PlayerCharacter>(savedFlags);
        }

        /// <summary>
        /// 저장된 선택을 현재 선택으로 복원합니다.
        /// </summary>
        public bool RestoreFromSaved()
        {
            LoadSelectedCharacters();
            return _selectedCharacters.Count > 0;
        }
    }
}

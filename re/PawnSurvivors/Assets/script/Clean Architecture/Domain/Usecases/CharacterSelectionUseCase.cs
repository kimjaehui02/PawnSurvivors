using System.Collections.Generic;
using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// 캐릭터 선택 관련 비즈니스 로직을 담당하는 UseCase입니다.
    /// </summary>
    public class CharacterSelectionUseCase
    {
        private readonly ISessionDataRepository _sessionRepository;
        
        // 선택 가능한 캐릭터 목록
        private List<string> _availableCharacters;
        
        // 현재 선택된 캐릭터들
        private List<string> _selectedCharacters;

        public CharacterSelectionUseCase(ISessionDataRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
            _availableCharacters = new List<string>();
            _selectedCharacters = new List<string>();
        }

        public void SetAvailableCharacters(List<string> characters)
        {
            _availableCharacters = new List<string>(characters);
        }

        public List<string> GetAvailableCharacters()
        {
            return new List<string>(_availableCharacters);
        }

        public bool SelectCharacter(string characterName)
        {
            if (!_availableCharacters.Contains(characterName))
            {
                return false;
            }

            if (!_selectedCharacters.Contains(characterName))
            {
                _selectedCharacters.Add(characterName);
                return true;
            }

            return false;
        }

        public bool DeselectCharacter(string characterName)
        {
            return _selectedCharacters.Remove(characterName);
        }

        public bool IsCharacterSelected(string characterName)
        {
            return _selectedCharacters.Contains(characterName);
        }

        public List<string> GetSelectedCharacters()
        {
            return new List<string>(_selectedCharacters);
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
    }
}

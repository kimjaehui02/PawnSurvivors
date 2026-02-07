using UnityEngine;
using System.Collections.Generic;
using PawnSurvivors.Managers;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Domain;
using PawnSurvivors.UI.Factory;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 캐릭터 선택 화면입니다. Builder를 사용하여 UI를 생성합니다.
    /// </summary>
    public class CharacterSelectScreen : MonoBehaviour
    {
        #region State
        private List<PlayerCharacter> _selectedCharacters = new();
        private const int MAX_SELECTION = 3;
        #endregion

        #region References
        private Canvas _canvas;
        private CharacterSelectionUseCase _characterSelectionUseCase;
        #endregion

        #region UI
        private MenuUIBuilder.CharacterSelectLayout _layout;
        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            SetupCanvas();
        }

        private void Start()
        {
            InitializeUseCases();
            CreateUI();
            LoadCharacters();
        }

        #endregion

        #region Initialization

        private void SetupCanvas()
        {
            _canvas = UIFactory.SetupCanvas(gameObject);
        }

        private void InitializeUseCases()
        {
            _characterSelectionUseCase = GameManager.Instance?.CharacterSelectionUseCase;
        }

        private void CreateUI()
        {
            _layout = MenuUIBuilder.CreateCharacterSelectLayout(
                transform,
                onStartGame: OnStartGameClicked,
                onBack: OnBackClicked
            );

            // 시작 버튼 비활성화 (캐릭터 선택 전)
            _layout.StartButton.interactable = false;
        }

        private void LoadCharacters()
        {
            _layout.ClearCards();

            // 사용 가능한 캐릭터 목록 (실제 PlayerCharacter enum 값 사용)
            var characters = new[]
            {
                new CharacterInfo(PlayerCharacter.PlayerBeni, "베니", "균형 잡힌 기본 캐릭터"),
                new CharacterInfo(PlayerCharacter.PlayerButter, "버터", "빠른 공격에 특화된 캐릭터"),
                new CharacterInfo(PlayerCharacter.PlayerElena, "엘레나", "마법 공격에 특화된 캐릭터"),
                new CharacterInfo(PlayerCharacter.PlayerEpica, "에피카", "강력한 일격에 특화된 캐릭터"),
                new CharacterInfo(PlayerCharacter.PlayerErpin, "에르핀", "민첩한 회피에 특화된 캐릭터"),
                new CharacterInfo(PlayerCharacter.PlayerOpal, "오팔", "방어에 특화된 캐릭터"),
                new CharacterInfo(PlayerCharacter.PlayerRufo, "루포", "원거리 공격에 특화된 캐릭터"),
                new CharacterInfo(PlayerCharacter.PlayerSpeaki, "스피키", "지원에 특화된 캐릭터"),
                new CharacterInfo(PlayerCharacter.PlayerTig, "티그", "광역 공격에 특화된 캐릭터"),
            };

            foreach (var charInfo in characters)
            {
                // 캐릭터 초상화 로드 시도
                Sprite portrait = Resources.Load<Sprite>($"Sprites/Characters/{charInfo.type}");

                var card = _layout.AddCharacter(
                    charInfo.name,
                    charInfo.description,
                    portrait,
                    () => OnCharacterClicked(charInfo.type)
                );
            }
        }

        #endregion

        #region Event Handlers

        private void OnCharacterClicked(PlayerCharacter character)
        {
            // 이미 선택된 캐릭터면 선택 해제
            if (_selectedCharacters.Contains(character))
            {
                _selectedCharacters.Remove(character);
                UpdateCardSelection(character, false);
            }
            // 최대 선택 수 미만이면 선택
            else if (_selectedCharacters.Count < MAX_SELECTION)
            {
                _selectedCharacters.Add(character);
                UpdateCardSelection(character, true);
            }
            else
            {
                LogManager.LogWarning(LogCategory.System, $"최대 {MAX_SELECTION}명까지 선택할 수 있습니다.");
            }

            // 시작 버튼 활성화 여부
            _layout.StartButton.interactable = _selectedCharacters.Count > 0;

            // 타이틀 업데이트
            _layout.TitleText.text = $"캐릭터 선택 ({_selectedCharacters.Count}/{MAX_SELECTION})";
        }

        private void UpdateCardSelection(PlayerCharacter character, bool selected)
        {
            // 해당 캐릭터의 카드 찾아서 선택 상태 변경
            foreach (var card in _layout.Cards)
            {
                if (card.NameText.text == GetCharacterName(character))
                {
                    card.SetSelected(selected);
                    break;
                }
            }
        }

        private string GetCharacterName(PlayerCharacter character)
        {
            return character switch
            {
                PlayerCharacter.PlayerBeni => "베니",
                PlayerCharacter.PlayerButter => "버터",
                PlayerCharacter.PlayerElena => "엘레나",
                PlayerCharacter.PlayerEpica => "에피카",
                PlayerCharacter.PlayerErpin => "에르핀",
                PlayerCharacter.PlayerOpal => "오팔",
                PlayerCharacter.PlayerRufo => "루포",
                PlayerCharacter.PlayerSpeaki => "스피키",
                PlayerCharacter.PlayerTig => "티그",
                _ => character.ToString()
            };
        }

        private void OnStartGameClicked()
        {
            if (_selectedCharacters.Count == 0)
            {
                LogManager.LogWarning(LogCategory.System, "최소 1명의 캐릭터를 선택해주세요.");
                return;
            }

            // UseCase에 선택 저장
            if (_characterSelectionUseCase != null)
            {
                _characterSelectionUseCase.ClearSelection();
                foreach (var character in _selectedCharacters)
                {
                    _characterSelectionUseCase.SelectCharacter(character);
                }
                _characterSelectionUseCase.ConfirmSelection();
                _characterSelectionUseCase.SaveSelectedCharacters();
            }

            // 스테이지 시작
            StartGame();
        }

        private void StartGame()
        {
            // 첫 스테이지 이름 가져오기
            string firstStageName = "Stage_01";
            if (GameManager.Instance?.StageManagementUseCase != null)
            {
                var stageList = GameManager.Instance.StageManagementUseCase;
                // StageListDataSource에서 첫 스테이지 가져오기
            }

            // 스테이지 준비 및 시작
            GameManager.Instance?.StageManagementUseCase?.PrepareStageStart(firstStageName, shouldResetSession: true);

            // StageState로 전환
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.GoToStage();
            }
        }

        private void OnBackClicked()
        {
            // 메인 메뉴로 돌아가기
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.TransitionTo<Domain.States.TitleState>();
            }
        }

        #endregion

        #region Helper

        private struct CharacterInfo
        {
            public PlayerCharacter type;
            public string name;
            public string description;

            public CharacterInfo(PlayerCharacter type, string name, string description)
            {
                this.type = type;
                this.name = name;
                this.description = description;
            }
        }

        #endregion
    }
}

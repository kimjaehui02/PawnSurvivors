using UnityEngine;
using PawnSurvivors.UI;
using PawnSurvivors.Managers;

namespace PawnSurvivors.Domain.States
{
    /// <summary>
    /// 캐릭터 선택 화면 상태입니다.
    /// </summary>
    public class CharacterSelectState : IGameState
    {
        public string StateName => "CharacterSelect";

        public void OnEnter()
        {
            // 씬에 UI가 이미 배치되어 있으므로 UIManager 호출 불필요
            // 씬 전환 시 Unity가 자동으로 정리하므로 CleanupGameState() 불필요
            // 단, 게임 상태 리셋만 필요 (UseCase 상태 등)
            if (GameManager.Instance?.StageFlowUseCase != null)
            {
                GameManager.Instance.StageFlowUseCase.ResetState();
            }
        }

        public void OnExit()
        {
            // 캐릭터 선택 화면 종료 시 정리 작업
        }

        public void OnUpdate()
        {
            // 캐릭터 선택 화면 업데이트 로직 (필요시)
        }

        public bool CanTransitionTo(IGameState nextState)
        {
            // CharacterSelect → StageSelect만 허용
            return nextState is StageSelectState;
        }
    }
}


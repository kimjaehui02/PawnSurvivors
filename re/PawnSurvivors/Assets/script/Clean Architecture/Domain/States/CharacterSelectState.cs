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
            // 재시작 시 완전히 새로 시작하기 위해 PlayerController 제거
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ClearPlayerController();
            }
            
            // BGM 정지 (재시작 시 이전 BGM이 남아있지 않도록)
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StopBGM();
            }
            
            // 게임 일시정지 해제 (재시작 시 정상 상태로)
            if (GameManager.Instance?.LifecycleManager != null)
            {
                if (GameManager.Instance.LifecycleManager.IsPaused)
                {
                    GameManager.Instance.LifecycleManager.TogglePause();
                }
            }
            
            // 게임 상태 리셋 (UseCase 상태 등)
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


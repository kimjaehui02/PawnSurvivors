using UnityEngine;
using PawnSurvivors.UI;
using PawnSurvivors.Managers;

namespace PawnSurvivors.Domain.States
{
    /// <summary>
    /// 게임 오버 화면 상태입니다.
    /// </summary>
    public class GameOverState : IGameState
    {
        public string StateName => "GameOver";

        public void OnEnter()
        {
            // 게임 일시정지
            if (GameManager.Instance?.LifecycleManager != null && 
                !GameManager.Instance.LifecycleManager.IsPaused)
            {
                GameManager.Instance.LifecycleManager.TogglePause();
            }
            
            // 게임오버 시 캐릭터 선택 상태 저장
            if (GameManager.Instance?.CharacterSelectionUseCase != null)
            {
                GameManager.Instance.CharacterSelectionUseCase.SaveSelectedCharacters();
            }
            
            // 게임오버 화면 표시
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOverScreen();
            }
        }

        public void OnExit()
        {
            // 게임오버 화면 종료 시 정리 작업
        }

        public void OnUpdate()
        {
            // 게임오버 화면 업데이트 로직 (필요시)
        }

        public bool CanTransitionTo(IGameState nextState)
        {
            // GameOver → CharacterSelect (재시작) 또는 Stage (리트라이) 허용
            return nextState is CharacterSelectState || 
                   nextState is StageState;
        }
    }
}


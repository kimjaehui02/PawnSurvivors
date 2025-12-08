using UnityEngine;
using PawnSurvivors.UI;
using PawnSurvivors.Managers;

namespace PawnSurvivors.Domain.States
{
    /// <summary>
    /// 일시정지 상태입니다.
    /// </summary>
    public class PausedState : IGameState
    {
        public string StateName => "Paused";

        public void OnEnter()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPauseMenu();
            }
            
            // 게임 일시정지
            if (GameManager.Instance?.LifecycleManager != null)
            {
                if (!GameManager.Instance.LifecycleManager.IsPaused)
                {
                    GameManager.Instance.LifecycleManager.TogglePause();
                }
            }
        }

        public void OnExit()
        {
            // 일시정지 메뉴 숨기기 (updateState=false로 호출하여 무한 루프 방지)
            // GameStateManager.TransitionTo()에서 이미 상태 전환이 진행 중이므로
            // HidePauseMenu()에서 다시 GoToStage()를 호출하면 안 됨
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HidePauseMenu(updateState: false);
            }
        }

        public void OnUpdate()
        {
            // 일시정지 업데이트 로직 (필요시)
        }

        public bool CanTransitionTo(IGameState nextState)
        {
            // Paused → Stage (재개), Shop (스테이지 완료 후 상점), CharacterSelect (메인 메뉴) 허용
            return nextState is StageState || 
                   nextState is ShopState ||
                   nextState is CharacterSelectState;
        }
    }
}


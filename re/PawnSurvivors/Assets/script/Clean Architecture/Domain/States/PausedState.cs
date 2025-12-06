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
            // 일시정지 해제는 StageState에서 처리
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HidePauseMenu();
            }
        }

        public void OnUpdate()
        {
            // 일시정지 업데이트 로직 (필요시)
        }

        public bool CanTransitionTo(IGameState nextState)
        {
            // Paused → Stage (재개) 또는 CharacterSelect (메인 메뉴) 허용
            return nextState is StageState || 
                   nextState is CharacterSelectState;
        }
    }
}


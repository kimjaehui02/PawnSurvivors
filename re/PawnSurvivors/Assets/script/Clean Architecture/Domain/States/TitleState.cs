using UnityEngine;
using PawnSurvivors.UI;
using PawnSurvivors.Managers;

namespace PawnSurvivors.Domain.States
{
    /// <summary>
    /// 타이틀 화면 상태입니다.
    /// </summary>
    public class TitleState : IGameState
    {
        public string StateName => "Title";

        public void OnEnter()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowTitleScreen();
            }
        }

        public void OnExit()
        {
            // 타이틀 화면 종료 시 정리 작업
        }

        public void OnUpdate()
        {
            // 타이틀 화면 업데이트 로직 (필요시)
        }

        public bool CanTransitionTo(IGameState nextState)
        {
            // Title → CharacterSelect만 허용
            return nextState is CharacterSelectState;
        }
    }
}


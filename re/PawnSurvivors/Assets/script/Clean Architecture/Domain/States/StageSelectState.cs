using UnityEngine;
using PawnSurvivors.UI;
using PawnSurvivors.Managers;

namespace PawnSurvivors.Domain.States
{
    /// <summary>
    /// 스테이지(캠페인) 선택 화면 상태입니다.
    /// </summary>
    public class StageSelectState : IGameState
    {
        public string StateName => "StageSelect";

        public void OnEnter()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowCampaignSelectScreen();
            }
        }

        public void OnExit()
        {
            // 스테이지 선택 화면 종료 시 정리 작업
        }

        public void OnUpdate()
        {
            // 스테이지 선택 화면 업데이트 로직 (필요시)
        }

        public bool CanTransitionTo(IGameState nextState)
        {
            // StageSelect → Stage만 허용
            return nextState is StageState;
        }
    }
}


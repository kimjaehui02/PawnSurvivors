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
            // 씬에 UI가 이미 배치되어 있으므로 UIManager 호출 불필요
            // 씬 로드는 GameStateManager에서 처리
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


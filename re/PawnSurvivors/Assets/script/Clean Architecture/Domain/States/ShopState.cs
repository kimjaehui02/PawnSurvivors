using UnityEngine;
using PawnSurvivors.UI;
using PawnSurvivors.Managers;

namespace PawnSurvivors.Domain.States
{
    /// <summary>
    /// 상점 화면 상태입니다.
    /// </summary>
    public class ShopState : IGameState
    {
        public string StateName => "Shop";

        public void OnEnter()
        {
            // 씬이 로드되면 씬 내부의 초기화 로직이 실행됨
            // ShopScene의 MonoBehaviour들이 Awake/Start에서 상점 초기화를 처리
            // State는 씬 전환만 담당하고, 게임 로직은 씬이 독립적으로 처리
        }

        public void OnExit()
        {
            // 상점 종료 시 정리 작업
        }

        public void OnUpdate()
        {
            // 상점 업데이트 로직 (필요시)
        }

        public bool CanTransitionTo(IGameState nextState)
        {
            // Shop → Stage (다음 스테이지), GameOver (모든 스테이지 완료), CharacterSelect (메인 메뉴로 돌아가기) 허용
            return nextState is StageState || 
                   nextState is GameOverState ||
                   nextState is CharacterSelectState;
        }
    }
}


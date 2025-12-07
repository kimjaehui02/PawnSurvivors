using UnityEngine;
using PawnSurvivors.UI;
using PawnSurvivors.Managers;

namespace PawnSurvivors.Domain.States
{
    /// <summary>
    /// 스테이지 플레이 중 상태입니다.
    /// </summary>
    public class StageState : IGameState
    {
        public string StateName => "Stage";

        public void OnEnter()
        {
            // 씬이 로드되면 씬 내부의 초기화 로직이 실행됨
            // StageScene의 MonoBehaviour들이 Awake/Start에서 스테이지 초기화를 처리
            // State는 씬 전환만 담당하고, 게임 로직은 씬이 독립적으로 처리
        }

        public void OnExit()
        {
            // 스테이지 종료 시 정리 작업
        }

        public void OnUpdate()
        {
            // 스테이지 업데이트 로직 (필요시)
        }

        public bool CanTransitionTo(IGameState nextState)
        {
            // Stage → Paused, Shop, GameOver, CharacterSelect (메인 메뉴) 허용
            return nextState is PausedState || 
                   nextState is ShopState || 
                   nextState is GameOverState ||
                   nextState is CharacterSelectState; // 일시정지 메뉴에서 메인 메뉴로 가기 위해
        }
    }
}


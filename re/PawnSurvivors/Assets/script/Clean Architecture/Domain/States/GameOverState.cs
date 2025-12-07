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
            // 씬이 로드되면 씬 내부의 초기화 로직이 실행됨
            // GameOverScene의 MonoBehaviour들이 Awake/Start에서 게임오버 초기화를 처리
            // State는 씬 전환만 담당하고, 게임 로직은 씬이 독립적으로 처리
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


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
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowCharacterSelectScreen();
            }
            
            // 게임 상태 초기화 (메인 메뉴로 돌아올 때)
            if (GameManager.Instance != null)
            {
                CleanupGameState();
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
        
        private void CleanupGameState()
        {
            // 일시정지 해제
            if (GameManager.Instance.LifecycleManager != null && 
                GameManager.Instance.LifecycleManager.IsPaused)
            {
                GameManager.Instance.LifecycleManager.TogglePause();
            }

            // 모든 Pawn 파괴
            if (GameManager.Instance.CreationManager != null)
            {
                GameManager.Instance.CreationManager.DestroyAllPawns();
            }
            
            // PlayerController 제거
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ClearPlayerController();
            }

            // 스테이지 종료
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndStage();
            }
            
            // 스테이지 상태 리셋
            if (GameManager.Instance?.StageFlowUseCase != null)
            {
                GameManager.Instance.StageFlowUseCase.ResetState();
            }
        }
    }
}


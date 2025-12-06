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
        private IGameState _previousState;
        
        public string StateName => "Stage";

        public StageState(IGameState previousState = null)
        {
            _previousState = previousState;
        }

        public void OnEnter()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowStageScreen();
            }
            
            if (GameManager.Instance == null) return;
            
            // 일시정지 해제 (일시정지나 상점에서 올 때)
            if (_previousState is PausedState || _previousState is ShopState)
            {
                if (GameManager.Instance.LifecycleManager != null && 
                    GameManager.Instance.LifecycleManager.IsPaused)
                {
                    GameManager.Instance.LifecycleManager.TogglePause();
                }
                // 일시정지/상점에서 돌아올 때는 StartStage() 호출하지 않음 (이미 실행 중)
                return;
            }
            
            // 재시작인 경우 (GameOverState에서 올 때)
            // 현재 pawn들을 유지한 채로 스테이지만 재시작 (정상 시작 플로우와 동일)
            if (_previousState is GameOverState)
            {
                // 재시작: 현재 스테이지 이름을 가져와서 스테이지 재시작
                // resetSession: false (pawn 유지)
                string currentStageName = GameManager.Instance?.StageManagementUseCase?.GetCurrentStageName() ?? "Stage1";
                
                if (GameManager.Instance.StageFlowUseCase != null)
                {
                    // 현재 pawn들을 유지하면서 스테이지만 재시작
                    // HandleStageStartRequested에서 PrepareExistingPlayers()가 호출되어 pawn들이 부활함
                    GameManager.Instance.StageFlowUseCase.StartStage(currentStageName, resetSession: false);
                }
                return;
            }
            
            // 스테이지 시작 (새로 시작하는 경우: CharacterSelectState나 TitleState에서 올 때)
            // 선택된 캐릭터 복원 (세션 데이터에서)
            if (GameManager.Instance.CharacterSelectionUseCase != null)
            {
                GameManager.Instance.CharacterSelectionUseCase.LoadSelectedCharacters();
            }
            
            string stageName = GameManager.Instance?.StageManagementUseCase?.GetCurrentStageName() ?? "Stage1";
            // resetSession: CharacterSelect나 Title에서 올 때만 리셋 (새 게임 시작)
            bool resetSession = _previousState is CharacterSelectState || _previousState is TitleState;
            
            // StageFlowUseCase를 통해 스테이지 시작
            if (GameManager.Instance.StageFlowUseCase != null)
            {
                GameManager.Instance.StageFlowUseCase.StartStage(stageName, resetSession);
            }
            else
            {
                // Fallback: UseCase가 없으면 기존 방식
                GameManager.Instance.StartStage(stageName, resetSession);
            }
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


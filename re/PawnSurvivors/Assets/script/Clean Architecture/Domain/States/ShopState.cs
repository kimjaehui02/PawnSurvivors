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
            UnityEngine.Debug.Log("[ShopState] OnEnter() 호출됨");
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowShopScreen();
            }
            
            // 스테이지 종료 처리 (HandleStageCompletedToShop에서 이미 호출했을 수 있음)
            // 하지만 중복 호출해도 안전하므로 그대로 둠
            if (GameManager.Instance != null)
            {
                // EndStage()는 이미 HandleStageCompletedToShop에서 호출했을 수 있음
                // 하지만 안전을 위해 다시 호출 (중복 호출해도 안전)
                if (!GameManager.Instance.StageManager.IsStageRunning())
                {
                    // 스테이지가 실행 중이 아니면 EndStage() 호출하지 않음
                    UnityEngine.Debug.Log("[ShopState] 스테이지가 이미 종료됨. EndStage() 호출하지 않음");
                }
                else
                {
                    GameManager.Instance.EndStage();
                }
            }
            
            // 상점으로 갈 때 게임 일시정지
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


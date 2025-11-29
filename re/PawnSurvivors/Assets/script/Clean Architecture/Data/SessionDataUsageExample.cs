using UnityEngine;
using PawnSurvivors.Domain.Usecases;

namespace PawnSurvivors.Data
{
    /// <summary>
    /// 세션 데이터 사용 예시 템플릿 (UseCase를 통한 접근)
    /// 실제로 이 클래스를 사용하지는 않고, 참고용으로만 사용하세요.
    /// 
    /// 주의: 엄격한 클린 아키텍처에서는 Repository를 직접 접근하지 않고
    /// UseCase를 통해서만 데이터에 접근합니다.
    /// </summary>
    public class SessionDataUsageExample : MonoBehaviour
    {
        // ========================================
        // 예시 1: 기본 통계 수집 (UseCase 사용)
        // ========================================
        
        void Example_BasicStatistics()
        {
            // UseCase를 통한 접근 (권장)
            var killUseCase = GameManager.Instance.KillTrackingUseCase;
            var damageUseCase = GameManager.Instance.DamageTrackingUseCase;
            
            // 적 처치
            killUseCase.RecordKill();  // +1
            
            // 데미지 기록 (플레이어 ID 필요)
            int playerId = 123; // 실제로는 PawnManager의 InstanceID 사용
            damageUseCase.RecordDamage(playerId, 123.5f);
            
            // 읽기
            int kills = killUseCase.GetTotalKills();
            float damage = damageUseCase.GetTotalDamage(playerId);
            Debug.Log($"Total Kills: {kills}, Damage: {damage}");
        }
        
        // ========================================
        // 예시 2: 스테이지 관리 (UseCase 사용)
        // ========================================
        
        void Example_StageManagement()
        {
            var sessionUseCase = GameManager.Instance.SessionManagementUseCase;
            
            // 스테이지 시작
            sessionUseCase.SetCurrentStageName("Stage2");
            
            // 현재 스테이지 확인
            string currentStage = sessionUseCase.GetCurrentStageName();
            Debug.Log($"Current Stage: {currentStage}");
            
            // 세션 리셋
            sessionUseCase.ResetSession();
        }
        
        // ========================================
        // 예시 3: 생존 시간 추적 (UseCase 사용)
        // ========================================
        
        void Example_SurvivalTime()
        {
            var survivalUseCase = GameManager.Instance.SurvivalTimeTrackingUseCase;
            
            // 현재 생존 시간
            float currentTime = survivalUseCase.GetCurrentSurvivalTime();
            Debug.Log($"Survival Time: {currentTime:F1}s");
            
            // 특정 시점 이후 경과 시간
            float startTime = 10f;
            float elapsed = survivalUseCase.GetElapsedTimeSinceStart(startTime);
            Debug.Log($"Elapsed since {startTime}s: {elapsed:F1}s");
        }
        
        // ========================================
        // 예시 4: 적 처치 이벤트 (UseCase 사용)
        // ========================================
        
        void Example_OnEnemyKilled()
        {
            var killUseCase = GameManager.Instance.KillTrackingUseCase;
            
            // 적 처치 기록
            killUseCase.RecordKill();
            
            // 전체 처치 수 확인
            int totalKills = killUseCase.GetTotalKills();
            Debug.Log($"Total Kills: {totalKills}");
        }
        
        // ========================================
        // 예시 5: 데미지 추적 (UseCase 사용)
        // ========================================
        
        void Example_DamageTracking(int playerId)
        {
            var damageUseCase = GameManager.Instance.DamageTrackingUseCase;
            
            // 데미지 기록
            damageUseCase.RecordDamage(playerId, 50f);
            
            // 총 데미지 확인
            float totalDamage = damageUseCase.GetTotalDamage(playerId);
            Debug.Log($"Player {playerId} Total Damage: {totalDamage}");
        }
        
        // ========================================
        // 예시 6: 게임 오버 시 결과 (UseCase 사용)
        // ========================================
        
        void Example_GameOver()
        {
            var killUseCase = GameManager.Instance.KillTrackingUseCase;
            var survivalUseCase = GameManager.Instance.SurvivalTimeTrackingUseCase;
            
            // 최종 통계
            int kills = killUseCase.GetTotalKills();
            float time = survivalUseCase.GetCurrentSurvivalTime();
            
            Debug.Log($"=== Game Over ===");
            Debug.Log($"Enemies Killed: {kills}");
            Debug.Log($"Survival Time: {time:F1}s");
            
            // (나중에) 영구 저장
            // SaveBestScore(kills, time);
        }
    }
}

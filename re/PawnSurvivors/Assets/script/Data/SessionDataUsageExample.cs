using UnityEngine;

namespace PawnSurvivors.Data
{
    /// <summary>
    /// GameSessionData 사용 예시 템플릿
    /// 실제로 이 클래스를 사용하지는 않고, 참고용으로만 사용하세요.
    /// </summary>
    public class SessionDataUsageExample : MonoBehaviour
    {
        // ========================================
        // 예시 1: 기본 통계 수집
        // ========================================
        
        void Example_BasicStatistics()
        {
            var session = GameManager.Instance.SessionData;
            
            // 적 처치
            session.AddInt("enemiesKilled");  // +1
            session.AddInt("bossKilled");     // +1
            
            // 데미지 누적
            session.AddFloat("totalDamage", 123.5f);
            
            // 읽기
            int kills = session.GetInt("enemiesKilled");
            Debug.Log($"Total Kills: {kills}");
        }
        
        // ========================================
        // 예시 2: 업그레이드 시스템
        // ========================================
        
        void Example_UpgradeSystem()
        {
            var session = GameManager.Instance.SessionData;
            
            // 업그레이드 획득 (레벨 1)
            session.AddCounter("upgrades", "AttackSpeed");
            
            // 업그레이드 레벨업 (+2)
            session.AddCounter("upgrades", "AttackSpeed", 2);
            
            // 현재 레벨 확인
            int level = session.GetCounter("upgrades", "AttackSpeed");
            
            // 효과 적용
            float bonus = level * 0.1f;  // 10% per level
            Debug.Log($"Attack Speed Bonus: +{bonus * 100}%");
        }
        
        // ========================================
        // 예시 3: 아이템 수집
        // ========================================
        
        void Example_ItemCollection()
        {
            var session = GameManager.Instance.SessionData;
            
            // 아이템 획득 (중복 불가)
            session.AddFlag("collectedItems", "Sword");
            session.AddFlag("collectedItems", "Shield");
            
            // 보유 여부 확인
            if (session.HasFlag("collectedItems", "Sword"))
            {
                Debug.Log("Player has Sword!");
            }
            
            // 모든 아이템 목록
            var items = session.GetFlags("collectedItems");
            Debug.Log($"Total Items: {items.Count}");
        }
        
        // ========================================
        // 예시 4: 여러 카테고리 관리
        // ========================================
        
        void Example_MultipleCategories()
        {
            var session = GameManager.Instance.SessionData;
            
            // 적 타입별 처치 수
            session.AddCounter("killsByType", "Zombie");
            session.AddCounter("killsByType", "Boss");
            session.AddCounter("killsByType", "Elite", 3);  // +3
            
            // 아이템 개수 관리
            session.SetCounter("inventory", "HealthPotion", 5);
            session.AddCounter("inventory", "HealthPotion", -1);  // 사용
            
            // 통계 확인
            int zombieKills = session.GetCounter("killsByType", "Zombie");
            int potions = session.GetCounter("inventory", "HealthPotion");
            
            Debug.Log($"Zombies killed: {zombieKills}, Potions left: {potions}");
        }
        
        // ========================================
        // 예시 5: 실전 통합 - 적 처치
        // ========================================
        
        void Example_OnEnemyKilled(string enemyType, int goldReward, int expReward)
        {
            var session = GameManager.Instance.SessionData;
            
            // 전체 처치 수
            session.AddInt("enemiesKilled");
            
            // 타입별 처치 수 (선택)
            session.AddCounter("killsByType", enemyType);
            
            // 보상 (기획 완료 후)
            // session.AddInt("gold", goldReward);
            // session.AddInt("exp", expReward);
            
            Debug.Log($"Killed {enemyType}. Total kills: {session.GetInt("enemiesKilled")}");
        }
        
        // ========================================
        // 예시 6: UI 업데이트
        // ========================================
        
        void Example_UpdateUI()
        {
            var session = GameManager.Instance.SessionData;
            
            // UI 텍스트 업데이트
            // killsText.text = $"Kills: {session.GetInt("enemiesKilled")}";
            // timeText.text = $"Time: {session.GetSurvivalTime():F1}s";
            // levelText.text = $"Level: {session.GetInt("level", 1)}";
            // goldText.text = $"Gold: {session.GetInt("gold")}";
            
            Debug.Log($"Game Time: {session.GetSurvivalTime():F1}s");
        }
        
        // ========================================
        // 예시 7: 게임 오버 시 결과
        // ========================================
        
        void Example_GameOver()
        {
            var session = GameManager.Instance.SessionData;
            
            // 최종 통계
            int kills = session.GetInt("enemiesKilled");
            float time = session.GetSurvivalTime();
            int gold = session.GetInt("gold");
            
            Debug.Log($"=== Game Over ===");
            Debug.Log($"Enemies Killed: {kills}");
            Debug.Log($"Survival Time: {time:F1}s");
            Debug.Log($"Gold Collected: {gold}");
            
            // (나중에) 영구 저장
            // SaveBestScore(kills, time);
        }
    }
}


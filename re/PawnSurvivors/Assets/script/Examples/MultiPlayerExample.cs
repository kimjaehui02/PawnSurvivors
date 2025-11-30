using UnityEngine;

namespace PawnSurvivors.Examples
{
    /// <summary>
    /// 여러 플레이어 추가 예시
    /// (실제 사용 시 이 클래스를 사용하지 말고, 필요한 부분만 참고하세요)
    /// </summary>
    public class MultiPlayerExample : MonoBehaviour
    {
        // ========================================
        // 예시 1: 게임 시작 시 여러 플레이어 추가
        // ========================================
        
        void Example_StartWithMultiplePlayers()
        {
            // 방법 A: 하나씩 추가
            GameManager.Instance.AddPlayerPawn("PlayerErpin");   // Recipe 이름
            GameManager.Instance.AddPlayerPawn("Warrior");
            GameManager.Instance.AddPlayerPawn("Mage");
            
            // 방법 B: 배열로 한 번에 추가
            string[] party = new string[] { "PlayerErpin", "Warrior", "Mage", "Archer" };
            GameManager.Instance.AddMultiplePlayerPawns(party);
        }
        
        // ========================================
        // 예시 2: 게임 중 플레이어 추가
        // ========================================
        
        void Example_AddPlayerDuringGame()
        {
            // 새 유닛 획득 시 (레벨업, 상점 구매 등)
            GameObject newPlayer = GameManager.Instance.AddPlayerPawn("Archer");
            
            if (newPlayer != null)
            {
                Debug.Log("새 플레이어 추가됨!");
            }
        }
        
        // ========================================
        // 예시 3: 대열 동적 변경
        // ========================================
        
        void Example_ChangFormation()
        {
            var controller = GameManager.Instance.PlayerController;
            
            // V자 대형
            controller.formationPositions = new Vector3[]
            {
                new Vector3(0f, 1f, 0f),
                new Vector3(-1f, 0f, 0f),
                new Vector3(1f, 0f, 0f),
                new Vector3(-2f, -1f, 0f),
                new Vector3(2f, -1f, 0f)
            };
            
            controller.RearrangeFormation();
        }
        
        // ========================================
        // 예시 4: 전투 중 대형 전환
        // ========================================
        
        void Example_SwitchFormationInBattle()
        {
            var controller = GameManager.Instance.PlayerController;
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                // 1번: 공격 대형 (V자)
                controller.formationPositions = new Vector3[]
                {
                    new Vector3(0f, 0f, 0f),
                    new Vector3(-1.5f, -0.5f, 0f),
                    new Vector3(1.5f, -0.5f, 0f)
                };
                controller.RearrangeFormation();
                Debug.Log("공격 대형!");
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                // 2번: 방어 대형 (일렬)
                controller.formationPositions = new Vector3[]
                {
                    new Vector3(-1.5f, 0f, 0f),
                    new Vector3(0f, 0f, 0f),
                    new Vector3(1.5f, 0f, 0f)
                };
                controller.RearrangeFormation();
                Debug.Log("방어 대형!");
            }
        }
        
        // ========================================
        // 예시 5: 플레이어 수에 따른 자동 대열
        // ========================================
        
        void Example_AutoFormation()
        {
            var controller = GameManager.Instance.PlayerController;
            int playerCount = controller.playerPawns.Count;
            
            if (playerCount == 1)
            {
                // 1명: 중앙
                controller.formationPositions = new Vector3[]
                {
                    Vector3.zero
                };
            }
            else if (playerCount == 2)
            {
                // 2명: 좌우
                controller.formationPositions = new Vector3[]
                {
                    new Vector3(-0.8f, 0f, 0f),
                    new Vector3(0.8f, 0f, 0f)
                };
            }
            else if (playerCount == 3)
            {
                // 3명: V자
                controller.formationPositions = new Vector3[]
                {
                    new Vector3(0f, 0.5f, 0f),
                    new Vector3(-1f, -0.5f, 0f),
                    new Vector3(1f, -0.5f, 0f)
                };
            }
            else
            {
                // 4명 이상: 2열 대형
                controller.formationPositions = new Vector3[]
                {
                    new Vector3(-0.8f, 0.5f, 0f),
                    new Vector3(0.8f, 0.5f, 0f),
                    new Vector3(-0.8f, -0.5f, 0f),
                    new Vector3(0.8f, -0.5f, 0f)
                };
            }
            
            controller.RearrangeFormation();
        }
        
        // ========================================
        // 예시 6: 특정 위치에 플레이어 배치
        // ========================================
        
        void Example_ManualPositioning()
        {
            var controller = GameManager.Instance.PlayerController;
            
            // 수동으로 위치 설정
            if (controller.playerPawns.Count > 0)
            {
                controller.playerPawns[0].transform.localPosition = new Vector3(0f, 0f, 0f);     // 중앙
            }
            if (controller.playerPawns.Count > 1)
            {
                controller.playerPawns[1].transform.localPosition = new Vector3(2f, 0f, 0f);     // 오른쪽
            }
            if (controller.playerPawns.Count > 2)
            {
                controller.playerPawns[2].transform.localPosition = new Vector3(-2f, 0f, 0f);    // 왼쪽
            }
        }
        
        // ========================================
        // 예시 7: 원형 대열
        // ========================================
        
        void Example_CircularFormation()
        {
            var controller = GameManager.Instance.PlayerController;
            int count = controller.playerPawns.Count;
            float radius = 2f;
            
            Vector3[] circleFormation = new Vector3[count];
            
            for (int i = 0; i < count; i++)
            {
                float angle = (360f / count) * i * Mathf.Deg2Rad;
                circleFormation[i] = new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0f
                );
            }
            
            controller.formationPositions = circleFormation;
            controller.RearrangeFormation();
        }
    }
}


using UnityEngine;
using System.Collections.Generic;

namespace PawnSurvivors.Player
{
    /// <summary>
    /// 대열 타입
    /// </summary>
    public enum FormationType
    {
        Polygon,     // 정다각형 (1명=중앙, 2명=선, 3명=삼각형, 4명=사각형, 5명=오각형, 6명=육각형)
        VShape,      // V자 대형
        TwoRows,     // 2열 대형
        Horizontal,  // 횡대
        Circle       // 원형 (동그랗게)
    }
    
    /// <summary>
    /// 플레이어 입력을 받아서 이동하는 중심 컨트롤러입니다.
    /// 자식으로 붙은 플레이어블 폰들과 카메라가 자동으로 따라옵니다.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("이동 설정")]
        [Tooltip("이동 속도")]
        public float moveSpeed = 5f;
        
        [Header("대열 설정")]
        [Tooltip("플레이어 폰들의 대열 위치 (localPosition)")]
        public Vector3[] formationPositions = new Vector3[]
        {
            new Vector3(0f, 0f, 0f),    // 중앙
            new Vector3(1f, -0.5f, 0f), // 오른쪽 뒤
            new Vector3(-1f, -0.5f, 0f) // 왼쪽 뒤
        };
        
        [Tooltip("자동 대열 생성 사용 여부")]
        public bool useAutomaticFormation = true;
        
        [Tooltip("대열 타입")]
        public FormationType formationType = FormationType.Polygon;
        
        [Tooltip("정다각형 반지름 (Polygon 타입일 때)")]
        public float polygonRadius = 1.5f;
        
        [Tooltip("플레이어 폰의 크기 배율 (1.0 = 원본 크기)")]
        public float pawnScale = 0.6f;
        
        [Header("참조")]
        [Tooltip("플레이어 폰들 (수동 할당 또는 런타임 추가)")]
        public List<GameObject> playerPawns = new List<GameObject>();
        
        private Vector2 _moveInput;
        
        private void Update()
        {
            // 입력 받기
            HandleInput();
            
            // 이동
            Move();
        }
        
        /// <summary>
        /// 키보드 입력 처리
        /// </summary>
        private void HandleInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal"); // A/D, Left/Right
            float vertical = Input.GetAxisRaw("Vertical");     // W/S, Up/Down
            
            _moveInput = new Vector2(horizontal, vertical).normalized;
        }
        
        /// <summary>
        /// 이동 처리
        /// </summary>
        private void Move()
        {
            if (_moveInput.magnitude > 0.01f)
            {
                // GameDeltaTime 사용 (정지 시 멈춤)
                float deltaTime = GameManager.Instance.LifecycleManager.GameDeltaTime;
                
                Vector3 movement = new Vector3(_moveInput.x, _moveInput.y, 0f) * moveSpeed * deltaTime;
                transform.position += movement;
            }
        }
        
        /// <summary>
        /// 플레이어 폰을 자식으로 추가하고 대열 위치에 배치합니다.
        /// </summary>
        public void AddPlayerPawn(GameObject pawn)
        {
            if (pawn == null) return;
            
            // 자식으로 설정
            pawn.transform.SetParent(transform);
            pawn.transform.localRotation = Quaternion.identity;
            
            // 크기 설정 (스프라이트, 충돌체, 그림자 모두 자동 스케일됨!)
            pawn.transform.localScale = Vector3.one * pawnScale;
            
            // 리스트에 추가
            playerPawns.Add(pawn);
            
            // 자동 대열 생성
            if (useAutomaticFormation)
            {
                UpdateFormationPositions();
            }
            
            // 대열 위치 설정
            int index = playerPawns.Count - 1;
            if (index < formationPositions.Length)
            {
                pawn.transform.localPosition = formationPositions[index];
            }
            else
            {
                pawn.transform.localPosition = Vector3.zero;
                Debug.LogWarning($"[PlayerController] 대열 위치 부족! Pawn {index}를 기본 위치에 배치합니다.");
            }
            
            Debug.Log($"[PlayerController] Pawn 추가됨: {pawn.name} at localPosition {pawn.transform.localPosition}, scale {pawnScale}");
        }
        
        /// <summary>
        /// 플레이어 폰을 제거합니다.
        /// </summary>
        public void RemovePlayerPawn(GameObject pawn)
        {
            if (playerPawns.Contains(pawn))
            {
                playerPawns.Remove(pawn);
                pawn.transform.SetParent(null); // 부모 해제
                
                Debug.Log($"[PlayerController] Pawn 제거됨: {pawn.name}");
            }
        }
        
        /// <summary>
        /// 모든 플레이어 폰을 제거합니다.
        /// </summary>
        public void ClearAllPawns()
        {
            foreach (var pawn in playerPawns)
            {
                if (pawn != null)
                {
                    pawn.transform.SetParent(null);
                }
            }
            
            playerPawns.Clear();
            Debug.Log("[PlayerController] 모든 Pawn 제거됨");
        }
        
        /// <summary>
        /// 대열 재정렬
        /// </summary>
        public void RearrangeFormation()
        {
            if (useAutomaticFormation)
            {
                UpdateFormationPositions();
            }
            
            for (int i = 0; i < playerPawns.Count; i++)
            {
                if (playerPawns[i] != null && i < formationPositions.Length)
                {
                    playerPawns[i].transform.localPosition = formationPositions[i];
                }
            }
        }
        
        /// <summary>
        /// 플레이어 수에 따라 자동으로 대열 위치를 생성합니다.
        /// </summary>
        private void UpdateFormationPositions()
        {
            int count = playerPawns.Count;
            
            switch (formationType)
            {
                case FormationType.Polygon:
                    formationPositions = GeneratePolygonFormation(count, polygonRadius);
                    break;
                case FormationType.VShape:
                    formationPositions = GenerateVShapeFormation(count);
                    break;
                case FormationType.TwoRows:
                    formationPositions = GenerateTwoRowsFormation(count);
                    break;
                case FormationType.Horizontal:
                    formationPositions = GenerateHorizontalFormation(count);
                    break;
                case FormationType.Circle:
                    formationPositions = GenerateCircleFormation(count);
                    break;
            }
        }
        
        /// <summary>
        /// 정다각형 대열 생성 (1명=중앙, 2명=선, 3명=삼각형, 4명=사각형, 5명=오각형, 6명=육각형)
        /// 미리 계산된 좌표값 사용 (삼각함수 연산 없음)
        /// </summary>
        private Vector3[] GeneratePolygonFormation(int count, float radius)
        {
            if (count == 0) return new Vector3[0];
            
            Vector3[] positions = new Vector3[count];
            
            // 미리 계산된 정규화 좌표 (반지름 1 기준)
            // 위쪽(90도)부터 시계방향으로 배치
            switch (count)
            {
                case 1:
                    // 1명: 중앙
                    positions[0] = Vector3.zero;
                    break;
                    
                case 2:
                    // 2명: 선분 (위-아래)
                    positions[0] = new Vector3(0f, 1f, 0f);
                    positions[1] = new Vector3(0f, -1f, 0f);
                    break;
                    
                case 3:
                    // 3명: 정삼각형 (위쪽부터)
                    positions[0] = new Vector3(0f, 1f, 0f);
                    positions[1] = new Vector3(0.866f, -0.5f, 0f);
                    positions[2] = new Vector3(-0.866f, -0.5f, 0f);
                    break;
                    
                case 4:
                    // 4명: 정사각형 (위쪽부터)
                    positions[0] = new Vector3(0f, 1f, 0f);
                    positions[1] = new Vector3(1f, 0f, 0f);
                    positions[2] = new Vector3(0f, -1f, 0f);
                    positions[3] = new Vector3(-1f, 0f, 0f);
                    break;
                    
                case 5:
                    // 5명: 정오각형 (위쪽부터)
                    positions[0] = new Vector3(0f, 1f, 0f);
                    positions[1] = new Vector3(0.951f, 0.309f, 0f);
                    positions[2] = new Vector3(0.588f, -0.809f, 0f);
                    positions[3] = new Vector3(-0.588f, -0.809f, 0f);
                    positions[4] = new Vector3(-0.951f, 0.309f, 0f);
                    break;
                    
                case 6:
                    // 6명: 정육각형 (위쪽부터)
                    positions[0] = new Vector3(0f, 1f, 0f);
                    positions[1] = new Vector3(0.866f, 0.5f, 0f);
                    positions[2] = new Vector3(0.866f, -0.5f, 0f);
                    positions[3] = new Vector3(0f, -1f, 0f);
                    positions[4] = new Vector3(-0.866f, -0.5f, 0f);
                    positions[5] = new Vector3(-0.866f, 0.5f, 0f);
                    break;
                    
                default:
                    // 7명 이상은 중앙에 배치 (fallback)
                    Debug.LogWarning($"[PlayerController] {count}명은 지원하지 않습니다. 중앙에 배치합니다.");
                    for (int i = 0; i < count; i++)
                    {
                        positions[i] = Vector3.zero;
                    }
                    return positions;
            }
            
            // 반지름 적용
            for (int i = 0; i < count; i++)
            {
                positions[i] *= radius;
            }
            
            return positions;
        }
        
        /// <summary>
        /// V자 대열 생성
        /// </summary>
        private Vector3[] GenerateVShapeFormation(int count)
        {
            Vector3[] positions = new Vector3[count];
            
            if (count == 1)
            {
                positions[0] = Vector3.zero;
            }
            else if (count == 2)
            {
                positions[0] = new Vector3(-0.8f, 0f, 0f);
                positions[1] = new Vector3(0.8f, 0f, 0f);
            }
            else
            {
                positions[0] = new Vector3(0f, 0.5f, 0f); // 선봉
                for (int i = 1; i < count; i++)
                {
                    float side = (i % 2 == 1) ? -1f : 1f;
                    int row = (i + 1) / 2;
                    positions[i] = new Vector3(side * row * 1f, -0.5f * row, 0f);
                }
            }
            
            return positions;
        }
        
        /// <summary>
        /// 2열 대열 생성
        /// </summary>
        private Vector3[] GenerateTwoRowsFormation(int count)
        {
            Vector3[] positions = new Vector3[count];
            int halfCount = (count + 1) / 2;
            
            for (int i = 0; i < count; i++)
            {
                int row = i < halfCount ? 0 : 1;
                int col = i < halfCount ? i : i - halfCount;
                
                float xOffset = (col - (halfCount - 1) / 2f) * 1.5f;
                float yOffset = row * -1.5f;
                
                positions[i] = new Vector3(xOffset, yOffset, 0f);
            }
            
            return positions;
        }
        
        /// <summary>
        /// 횡대 대열 생성
        /// </summary>
        private Vector3[] GenerateHorizontalFormation(int count)
        {
            Vector3[] positions = new Vector3[count];
            
            for (int i = 0; i < count; i++)
            {
                float xOffset = (i - (count - 1) / 2f) * 1.5f;
                positions[i] = new Vector3(xOffset, 0f, 0f);
            }
            
            return positions;
        }
        
        /// <summary>
        /// 원형 대열 생성
        /// </summary>
        private Vector3[] GenerateCircleFormation(int count)
        {
            return GeneratePolygonFormation(count, 2f); // 반지름만 다름
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // 대열 위치 시각화
            Gizmos.color = Color.cyan;
            foreach (var pos in formationPositions)
            {
                Vector3 worldPos = transform.TransformPoint(pos);
                Gizmos.DrawWireSphere(worldPos, 0.2f);
            }
        }
#endif
    }
}


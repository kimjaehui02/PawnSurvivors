using System.Collections.Generic;
using UnityEngine;

namespace PawnCore.Presentation.SubManagers.Movement
{
    /// <summary>
    /// 같은 태그를 가진 다른 Pawn들과 일정 거리 이상 떨어지도록 밀어내는 SubManager입니다.
    /// (적들이 한 점에 뭉치는 것을 방지)
    /// </summary>
    public class SeparationSubManager : PawnSubManager
    {
        [Header("분리 설정")]
        [Tooltip("분리를 적용할 최소 거리")]
        public float separationRadius = 1.0f;
        
        [Tooltip("분리 힘의 강도")]
        public float separationStrength = 2.0f;
        
        [Tooltip("분리를 체크할 레이어 마스크 (보통 자신과 같은 레이어)")]
        public LayerMask separationLayerMask;
        
        [Tooltip("분리 체크 빈도 (초, 성능 최적화용)")]
        public float checkFrequency = 0.1f;
        
        private float _checkTimer = 0f;
        private Vector3 _separationForce;

        public override void SubStart()
        {
            // 레이어 마스크가 설정되지 않았으면 자신의 레이어로 설정
            if (separationLayerMask.value == 0)
            {
                separationLayerMask = 1 << gameObject.layer;
            }
        }

        public override void SubUpdate()
        {
            _checkTimer -= GetGameDeltaTime();
            
            if (_checkTimer <= 0f)
            {
                _checkTimer = checkFrequency;
                CalculateSeparationForce();
            }
            
            // 분리 힘 적용
            if (_separationForce.sqrMagnitude > 0.01f)
            {
                transform.position += _separationForce * GetGameDeltaTime();
            }
        }

        /// <summary>
        /// 주변 Pawn들로부터 밀려나는 힘을 계산합니다.
        /// </summary>
        private void CalculateSeparationForce()
        {
            _separationForce = Vector3.zero;
            
            // 주변의 같은 태그 Pawn들 찾기 (최신 API 사용)
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(
                transform.position,
                separationRadius,
                separationLayerMask
            );
            
            int neighborCount = 0;
            
            foreach (Collider2D other in nearbyColliders)
            {
                // 자신은 제외
                if (other.gameObject == gameObject)
                    continue;
                
                // 같은 태그만 처리
                if (!other.CompareTag(gameObject.tag))
                    continue;
                
                Vector3 diff = transform.position - other.transform.position;
                float distance = diff.magnitude;
                
                if (distance < 0.01f) // 너무 가까우면 랜덤 방향으로
                {
                    diff = new Vector3(
                        Random.Range(-1f, 1f),
                        Random.Range(-1f, 1f),
                        0f
                    );
                    distance = 1f;
                }
                
                // 거리가 가까울수록 강한 힘
                float strength = 1f - (distance / separationRadius);
                _separationForce += diff.normalized * strength * separationStrength;
                neighborCount++;
            }
            
            // 평균 계산
            if (neighborCount > 0)
            {
                _separationForce /= neighborCount;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // 분리 반경 표시
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, separationRadius);
            
            // 분리 힘 방향 표시
            if (Application.isPlaying && _separationForce.sqrMagnitude > 0.01f)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + _separationForce);
            }
        }
#endif
    }
}


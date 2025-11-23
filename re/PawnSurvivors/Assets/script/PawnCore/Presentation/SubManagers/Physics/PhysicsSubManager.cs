using UnityEngine;

namespace PawnCore.Presentation.SubManagers.Physics
{
    public class PhysicsSubManager : PawnSubManager
    {
        private PawnCore.Domain.PhysicsData _physicsData;

        public override void SubStart()
        {
            // PhysicsData 가져오기 또는 생성
            _physicsData = _pawnManager.PawnData.GetOrCreatePhysicsData();

            // Collider2D 추가
            Collider2D collider = null;
            switch (_physicsData.colliderType)
            {
                case PawnCore.Domain.ColliderType.Box2D:
                    collider = gameObject.AddComponent<BoxCollider2D>();
                    break;
                case PawnCore.Domain.ColliderType.Circle2D:
                    collider = gameObject.AddComponent<CircleCollider2D>();
                    break;
                case PawnCore.Domain.ColliderType.Capsule2D:
                    collider = gameObject.AddComponent<CapsuleCollider2D>();
                    break;
                // None의 경우 기본 사례가 필요 없으며 충돌체는 null로 유지됩니다.
            }
            if (collider != null)
            {
                collider.isTrigger = _physicsData.isTrigger;
            }

            // Rigidbody2D 추가
            Rigidbody2D rigidbody = null;
            switch (_physicsData.rigidbodyType)
            {
                case PawnCore.Domain.RigidbodyType.Dynamic:
                    rigidbody = gameObject.AddComponent<Rigidbody2D>();
                    break;
                case PawnCore.Domain.RigidbodyType.Kinematic:
                    rigidbody = gameObject.AddComponent<Rigidbody2D>();
                    rigidbody.bodyType = RigidbodyType2D.Kinematic;
                    break;
                case PawnCore.Domain.RigidbodyType.Static:
                    rigidbody = gameObject.AddComponent<Rigidbody2D>();
                    rigidbody.bodyType = RigidbodyType2D.Static;
                    break;
                // None의 경우 기본 사례가 필요 없으며 리지드바디는 null로 유지됩니다.
            }
            if (rigidbody != null)
            {
                rigidbody.gravityScale = _physicsData.gravityScale;
            }

            // 레이어 설정
            if (!string.IsNullOrEmpty(_physicsData.physicsLayerName) && _physicsData.physicsLayerName != "Default")
            {
                int layerIndex = LayerMask.NameToLayer(_physicsData.physicsLayerName);
                if (layerIndex != -1)
                {
                    gameObject.layer = layerIndex;
                }
                else
                {
                    Debug.LogWarning($"Physics layer '{_physicsData.physicsLayerName}' not found. Using Default layer.");
                }
            }

            // 태그 설정
            if (!string.IsNullOrEmpty(_physicsData.physicsTag) && _physicsData.physicsTag != "Untagged")
            {
                // 태그가 존재하는지 확인
                try
                {
                    gameObject.tag = _physicsData.physicsTag;
                }
                catch (UnityException)
                {
                    Debug.LogWarning($"Tag '{_physicsData.physicsTag}' not defined in Unity. Using Untagged.");
                    gameObject.tag = "Untagged";
                }
            }
        }

        public override void SubUpdate()
        {
            // 물리 업데이트는 Unity의 물리 시스템에서 처리되므로 여기서는 사용자 지정 업데이트가 필요하지 않습니다.
        }
    }
}










using UnityEngine;

namespace PawnCore.Presentation.SubManagers.Physics
{
    public class PhysicsSubManager : PawnSubManager
    {
        private PawnCore.Domain.PhysicsData _physicsData;

        public override void SubStart()
        {
            _physicsData = _pawnManager.PawnData.physicsData;

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
                gameObject.layer = LayerMask.NameToLayer(_physicsData.physicsLayerName);
            }

            // 태그 설정
            if (!string.IsNullOrEmpty(_physicsData.physicsTag) && _physicsData.physicsTag != "Untagged")
            {
                gameObject.tag = _physicsData.physicsTag;
            }
        }

        public override void SubUpdate()
        {
            // 물리 업데이트는 Unity의 물리 시스템에서 처리되므로 여기서는 사용자 지정 업데이트가 필요하지 않습니다.
        }
    }
}


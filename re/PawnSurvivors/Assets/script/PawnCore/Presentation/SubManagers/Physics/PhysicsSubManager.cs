using UnityEngine;

namespace PawnCore.Presentation.SubManagers.Physics
{
    public class PhysicsSubManager : PawnSubManager
    {
        private PawnCore.Domain.PhysicsData _physicsData;

        public override void SubStart()
        {
            _physicsData = _pawnManager.PawnData.physicsData;

            // Add Collider2D
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
                // No default case needed for None, collider remains null
            }
            if (collider != null)
            {
                collider.isTrigger = _physicsData.isTrigger;
            }

            // Add Rigidbody2D
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
                // No default case needed for None, rigidbody remains null
            }
            if (rigidbody != null)
            {
                rigidbody.gravityScale = _physicsData.gravityScale;
            }

            // Set Layer
            if (!string.IsNullOrEmpty(_physicsData.physicsLayerName) && _physicsData.physicsLayerName != "Default")
            {
                gameObject.layer = LayerMask.NameToLayer(_physicsData.physicsLayerName);
            }

            // Set Tag
            if (!string.IsNullOrEmpty(_physicsData.physicsTag) && _physicsData.physicsTag != "Untagged")
            {
                gameObject.tag = _physicsData.physicsTag;
            }
        }

        public override void SubUpdate()
        {
            // Physics updates are handled by Unity's physics system, no custom update needed here.
        }
    }
}


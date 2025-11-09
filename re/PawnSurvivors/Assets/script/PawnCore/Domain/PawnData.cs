
using System;
using System.Collections.Generic;
using UnityEngine;
using PawnCore.Recipes.Json;

namespace PawnCore.Domain
{
    [Serializable]
    public class PawnData
    {
        // 피해 가능
        public float maxHealth = 100f;
        public float currentHealth = 100f;

        // 시각적
        public string visualSpriteName; // visualPrefabName에서 변경됨
        public Color visualColor = Color.white; // 선택 사항: 스프라이트 색상 추가

        // 충돌 피해
        public float damage = 10f;

        // 발사체 발사기
        public string projectileRecipeName;
        public float fireRate = 2f;

        // 이동 가능
        public MovableData movableData = new MovableData();

        // 물리
        public PhysicsData physicsData = new PhysicsData();
    }

    [Serializable]
    public class MovableData
    {
        public KeyboardMovementData keyboardMovement = new KeyboardMovementData();
        public DirectionalMovementData directionalMovement = new DirectionalMovementData();
        public HomingMovementData homingMovement = new HomingMovementData();
        public TargetMovementData targetMovement = new TargetMovementData();
    }

    [Serializable]
    public class KeyboardMovementData
    {
        public float moveSpeed = 5f;
    }

    [Serializable]
    public class DirectionalMovementData
    {
        public float speed = 20f;
        public float lifetime = 5f;
        public Vector3 moveDirection;
    }

    [Serializable]
    public class HomingMovementData
    {
        public float speed = 5f;
        public string targetTag = "Enemy";
        public float detectionRange = 10f;
        public float retargetFrequency = 0.5f;
    }

    [Serializable]
    public class TargetMovementData
    {
        public float speed;
    }

    [Serializable]
    public class PhysicsData
    {
        public ColliderType colliderType = ColliderType.None;
        public bool isTrigger = false;
        public RigidbodyType rigidbodyType = RigidbodyType.None;
        public float gravityScale = 1f;
        public string physicsLayerName = "Default";
        public string physicsTag = "Untagged";
    }

    public enum ColliderType
    {
        None,
        Box2D,
        Circle2D,
        Capsule2D
    }

    public enum RigidbodyType
    {
        None,
        Dynamic,
        Kinematic,
        Static
    }
}

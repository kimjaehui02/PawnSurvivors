
using System;
using System.Collections.Generic;
using UnityEngine;
using PawnCore.Recipes.Json;

namespace PawnCore.Domain
{
    [Serializable]
    public class PawnData
    {
        // Damageable
        public float maxHealth = 100f;
        public float currentHealth = 100f;

        // Visual
        public string visualSpriteName; // Changed from visualPrefabName
        public Color visualColor = Color.white; // Optional: add color for sprite

        // CollisionDamage
        public float damage = 10f;

        // ProjectileShooter
        public string projectileRecipeName;
        public float fireRate = 2f;

        // Movable
        public MovableData movableData = new MovableData();

        // Physics
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


using System;
using System.Collections.Generic;
using UnityEngine;
using PawnCore.Recipes.Json;

namespace PawnCore.Domain
{
    [Serializable]
    public class PawnData
    {
        // 모듈화된 데이터 (nullable로 필요한 것만 할당)
        public HealthData healthData;
        public VisualData visualData;
        public CombatData combatData;
        public MovableData movableData;
        public PhysicsData physicsData;

        // 하위 호환성을 위한 속성들 (deprecated 예정)
        // public float maxHealth
        // {
        //     get => healthData?.maxHealth ?? 0f;
        //     set { if (healthData == null) healthData = new HealthData(); healthData.maxHealth = value; }
        // }

        // public float currentHealth
        // {
        //     get => healthData?.currentHealth ?? 0f;
        //     set { if (healthData == null) healthData = new HealthData(); healthData.currentHealth = value; }
        // }

        // public string visualSpriteName
        // {
        //     get => visualData?.spriteName;
        //     set { if (visualData == null) visualData = new VisualData(); visualData.spriteName = value; }
        // }

        // public Color visualColor
        // {
        //     get => visualData?.color ?? Color.white;
        //     set { if (visualData == null) visualData = new VisualData(); visualData.color = value; }
        // }

        // public float damage
        // {
        //     get => combatData?.damage ?? 0f;
        //     set { if (combatData == null) combatData = new CombatData(); combatData.damage = value; }
        // }

        // public string projectileRecipeName
        // {
        //     get => combatData?.projectileRecipeName;
        //     set { if (combatData == null) combatData = new CombatData(); combatData.projectileRecipeName = value; }
        // }

        // public float fireRate
        // {
        //     get => combatData?.fireRate ?? 0f;
        //     set { if (combatData == null) combatData = new CombatData(); combatData.fireRate = value; }
        // }
    }

    [Serializable]
    public class HealthData
    {
        public float maxHealth = 100f;
        public float currentHealth = 100f;
    }

    [Serializable]
    public class VisualData
    {
        public string visualSpriteName;
        public Color visualColor = Color.white;
    }

    [Serializable]
    public class CombatData
    {
        public float damage = 10f;
        public string projectileRecipeName;
        public float fireRate = 2f;
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

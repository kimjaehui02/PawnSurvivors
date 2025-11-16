
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
        
        // 스프라이트 인덱스 (-1이면 이름 기반, 0 이상이면 인덱스 기반)
        // 예: "Temporary/mini"와 index=0이면 mini의 첫 번째 슬라이스
        public int visualSpriteIndex = -1;
        
        // 그림자 프리셋 이름 (null이면 그림자 없음)
        public string shadowPresetName = null;
        
        // 런타임에 로드된 그림자 프리셋 데이터 (JSON에 저장 안 됨)
        [System.NonSerialized]
        public ShadowPresetData shadowPreset = null;
    }

    [Serializable]
    public class CombatData
    {
        public float damage = 10f;
        public string projectileRecipeName;
        public float fireRate = 2f;
        
        /// <summary>
        /// 충돌 시 자신을 파괴할지 여부입니다.
        /// 발사체: true (기본값), 근접 공격 유닛: false
        /// </summary>
        public bool destroyOnHit = true;
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

using System;
using System.Collections.Generic;
using UnityEngine;
using PawnCore.Domain;

namespace PawnCore.Recipes.Json
{
    [Serializable]
    public class PawnRecipeData
    {
        public string pawnName;
        [SerializeReference]
        public List<SubManagerSetupData> subManagerSetups;

        public PawnData ToPawnData()
        {
            PawnData pawnData = new PawnData();
            foreach (var setup in subManagerSetups)
            {
                setup.ApplyToPawnData(pawnData);
            }
            return pawnData;
        }
    }

    [Serializable]
    public abstract class SubManagerSetupData
    {
        public abstract void ApplyToPawnData(PawnData pawnData);
        public abstract MonoBehaviour AddSubManagerComponent(GameObject pawnObject);
    }

    [Serializable]
    public class CollisionDamageSubManagerSetupData : SubManagerSetupData
    {
        public float damage;
        
        /// <summary>
        /// 충돌 시 자신을 파괴할지 여부입니다.
        /// 발사체: true (기본값), 근접 공격 유닛: false
        /// </summary>
        public bool destroyOnHit = true;

        public override void ApplyToPawnData(PawnData pawnData)
        {
            // CombatData가 없으면 생성
            if (pawnData.combatData == null)
            {
                pawnData.combatData = new CombatData();
            }

            pawnData.combatData.damage = damage;
            pawnData.combatData.destroyOnHit = destroyOnHit;
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            return pawnObject.AddComponent<CollisionDamageSubManager>();
        }
    }

    [Serializable]
    public class DamageableSubManagerSetupData : SubManagerSetupData
    {
        public float maxHealth;

        public override void ApplyToPawnData(PawnData pawnData)
        {
            // HealthData가 없으면 생성
            if (pawnData.healthData == null)
            {
                pawnData.healthData = new HealthData();
            }

            pawnData.healthData.maxHealth = maxHealth;
            pawnData.healthData.currentHealth = maxHealth;
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            return pawnObject.AddComponent<DamageableSubManager>();
        }
    }

    [Serializable]
    public class MovableSubManagerSetupData : SubManagerSetupData
    {
        [SerializeReference]
        public List<MovementStrategySetupData> strategySetups;

        public override void ApplyToPawnData(PawnData pawnData)
        {
            // MovableData가 없으면 생성
            if (pawnData.movableData == null)
            {
                pawnData.movableData = new MovableData();
            }

            foreach (var setup in strategySetups)
            {
                setup.ApplyToMovableData(pawnData.movableData);
            }
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            MovableSubManager movableSubManager = pawnObject.AddComponent<MovableSubManager>();
            foreach (var strategySetup in strategySetups)
            {
                strategySetup.AddMovementStrategyComponent(pawnObject);
            }
            return movableSubManager;
        }
    }

    [Serializable]
    public class PlayerAttackInputSubManagerSetupData : SubManagerSetupData
    {
        public override void ApplyToPawnData(PawnData pawnData)
        {
            // PlayerAttackInput에 적용할 데이터 없음
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            return pawnObject.AddComponent<PlayerAttackInputSubManager>();
        }
    }

    [Serializable]
    public class ProjectileShooterSubManagerSetupData : SubManagerSetupData
    {
        public string projectileRecipeName;
        public float fireRate;

        public override void ApplyToPawnData(PawnData pawnData)
        {
            if (pawnData.combatData == null)
            {
                pawnData.combatData = new CombatData();
            }
            pawnData.combatData.projectileRecipeName = projectileRecipeName;
            pawnData.combatData.fireRate = fireRate;
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            ProjectileShooterSubManager subManager = pawnObject.AddComponent<ProjectileShooterSubManager>();
            
            // FirePoint가 없으면 생성
            Transform firePointTransform = pawnObject.transform.Find("FirePoint");
            if (firePointTransform == null)
            {
                GameObject firePoint = new GameObject("FirePoint");
                firePoint.transform.SetParent(pawnObject.transform);
                firePoint.transform.localPosition = Vector3.zero;
                firePointTransform = firePoint.transform;
            }
            subManager.firePoint = firePointTransform;
            return subManager;
        }
    }

    [Serializable]
    public class VisualSubManagerSetupData : SubManagerSetupData
    {
        public string visualSpriteName; // visualPrefabName에서 변경됨
        public Color visualColor = Color.white; // 선택 사항: 스프라이트 사용자 지정용
        
        [Tooltip("스프라이트 인덱스 (-1이면 이름 기반, 0 이상이면 인덱스 기반)")]
        public int visualSpriteIndex = -1;
        
        [Tooltip("그림자 프리셋 이름 (null이면 그림자 없음)")]
        public string shadowPreset = null;
        
        [Header("바운스 애니메이션")]
        [Tooltip("바운스 효과 활성화")]
        public bool enableBounce = true;
        
        [Tooltip("바운스 높이")]
        public float bounceHeight = 0.1f;
        
        [Tooltip("바운스 속도 (높을수록 빠름)")]
        public float bounceSpeed = 10f;
        
        [Tooltip("이동 시작으로 간주할 최소 속도")]
        public float movementThreshold = 0.1f;

        public override void ApplyToPawnData(PawnData pawnData)
        {
            // VisualData가 없으면 생성
            if (pawnData.visualData == null)
            {
                pawnData.visualData = new VisualData();
            }

            pawnData.visualData.visualSpriteName = visualSpriteName;
            pawnData.visualData.visualColor = visualColor;
            pawnData.visualData.visualSpriteIndex = visualSpriteIndex;
            pawnData.visualData.shadowPresetName = shadowPreset;
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            var visualSubManager = pawnObject.AddComponent<PawnCore.Presentation.SubManagers.Visual.VisualSubManager>();
            
            // 바운스 설정 적용
            visualSubManager.enableBounce = enableBounce;
            visualSubManager.bounceHeight = bounceHeight;
            visualSubManager.bounceSpeed = bounceSpeed;
            visualSubManager.movementThreshold = movementThreshold;
            
            return visualSubManager;
        }
    }

    [Serializable]
    public class PhysicsSubManagerSetupData : SubManagerSetupData
    {
        public PawnCore.Domain.ColliderType colliderType = PawnCore.Domain.ColliderType.None;
        public bool isTrigger = false;
        public PawnCore.Domain.RigidbodyType rigidbodyType = PawnCore.Domain.RigidbodyType.None;
        public float gravityScale = 1f;
        public string physicsLayerName = "Default";
        public string physicsTag = "Untagged";

        public override void ApplyToPawnData(PawnData pawnData)
        {
            // PhysicsData가 없으면 생성
            if (pawnData.physicsData == null)
            {
                pawnData.physicsData = new PhysicsData();
            }

            pawnData.physicsData.colliderType = colliderType;
            pawnData.physicsData.isTrigger = isTrigger;
            pawnData.physicsData.rigidbodyType = rigidbodyType;
            pawnData.physicsData.gravityScale = gravityScale;
            pawnData.physicsData.physicsLayerName = physicsLayerName;
            pawnData.physicsData.physicsTag = physicsTag;
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            return pawnObject.AddComponent<PawnCore.Presentation.SubManagers.Physics.PhysicsSubManager>();
        }
    }

    [Serializable]
    public abstract class MovementStrategySetupData
    {
        public bool isEnabledByDefault;
        public abstract void ApplyToMovableData(MovableData movableData);
        public abstract MonoBehaviour AddMovementStrategyComponent(GameObject pawnObject);
    }

    [Serializable]
    public class DirectionalStrategySetupData : MovementStrategySetupData
    {
        public float speed;
        public float lifetime;
        public Vector3 moveDirection;

        public override void ApplyToMovableData(MovableData movableData)
        {
            if (movableData.directionalMovement == null)
                movableData.directionalMovement = new DirectionalMovementData();
                
            movableData.directionalMovement.speed = speed;
            movableData.directionalMovement.lifetime = lifetime;
            movableData.directionalMovement.moveDirection = moveDirection;
        }

        public override MonoBehaviour AddMovementStrategyComponent(GameObject pawnObject)
        {
            DirectionalMovementStrategy strategy = pawnObject.AddComponent<DirectionalMovementStrategy>();
            strategy.SetInitialEnabledState(isEnabledByDefault);
            return strategy;
        }
    }

    [Serializable]
    public class HomingStrategySetupData : MovementStrategySetupData
    {
        public float speed;
        public string targetTag;
        public float detectionRange;
        public float retargetFrequency;

        public override void ApplyToMovableData(MovableData movableData)
        {
            if (movableData.homingMovement == null)
                movableData.homingMovement = new HomingMovementData();
                
            movableData.homingMovement.speed = speed;
            movableData.homingMovement.targetTag = targetTag;
            movableData.homingMovement.detectionRange = detectionRange;
            movableData.homingMovement.retargetFrequency = retargetFrequency;
        }

        public override MonoBehaviour AddMovementStrategyComponent(GameObject pawnObject)
        {
            HomingMovementStrategy strategy = pawnObject.AddComponent<HomingMovementStrategy>();
            strategy.SetInitialEnabledState(isEnabledByDefault);
            return strategy;
        }
    }

    [Serializable]
    public class KeyboardStrategySetupData : MovementStrategySetupData
    {
        public float moveSpeed;

        public override void ApplyToMovableData(MovableData movableData)
        {
            if (movableData.keyboardMovement == null)
                movableData.keyboardMovement = new KeyboardMovementData();
                
            movableData.keyboardMovement.moveSpeed = moveSpeed;
        }

        public override MonoBehaviour AddMovementStrategyComponent(GameObject pawnObject)
        {
            KeyboardMovementStrategy strategy = pawnObject.AddComponent<KeyboardMovementStrategy>();
            strategy.SetInitialEnabledState(isEnabledByDefault);
            return strategy;
        }
    }

    [Serializable]
    public class TargetStrategySetupData : MovementStrategySetupData
    {
        public float speed;

        public override void ApplyToMovableData(MovableData movableData)
        {
            if (movableData.targetMovement == null)
                movableData.targetMovement = new TargetMovementData();
                
            movableData.targetMovement.speed = speed;
        }

        public override MonoBehaviour AddMovementStrategyComponent(GameObject pawnObject)
        {
            TargetMovementStrategy strategy = pawnObject.AddComponent<TargetMovementStrategy>();
            strategy.SetInitialEnabledState(isEnabledByDefault);
            return strategy;
        }
    }

    [Serializable]
    public class InvincibilitySubManagerSetupData : SubManagerSetupData
    {
        public float invincibilityDuration = 1f;
        public float blinkSpeed = 5f;

        public override void ApplyToPawnData(PawnData pawnData)
        {
            // InvincibilitySubManager는 PawnData에 저장할 데이터가 없음
            // 모든 설정은 컴포넌트 자체에서 관리
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            InvincibilitySubManager subManager = pawnObject.AddComponent<InvincibilitySubManager>();
            subManager.invincibilityDuration = invincibilityDuration;
            subManager.blinkSpeed = blinkSpeed;
            return subManager;
        }
    }

    [Serializable]
    public class HitFlashSubManagerSetupData : SubManagerSetupData
    {
        public float flashDuration = 0.1f;

        public override void ApplyToPawnData(PawnData pawnData)
        {
            // HitFlashSubManager는 PawnData에 저장할 데이터가 없음
            // 모든 설정은 컴포넌트 자체에서 관리
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            PawnCore.Presentation.SubManagers.Visual.HitFlashSubManager subManager = 
                pawnObject.AddComponent<PawnCore.Presentation.SubManagers.Visual.HitFlashSubManager>();
            subManager.flashDuration = flashDuration;
            return subManager;
        }
    }

    [Serializable]
    public class SeparationSubManagerSetupData : SubManagerSetupData
    {
        [Tooltip("분리를 적용할 최소 거리")]
        public float separationRadius = 1.0f;
        
        [Tooltip("분리 힘의 강도")]
        public float separationStrength = 2.0f;
        
        [Tooltip("분리 체크 빈도 (초)")]
        public float checkFrequency = 0.1f;

        public override void ApplyToPawnData(PawnData pawnData)
        {
            // SeparationSubManager는 PawnData에 저장할 데이터가 없음
            // 모든 설정은 컴포넌트 자체에서 관리
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            PawnCore.Presentation.SubManagers.Movement.SeparationSubManager subManager = 
                pawnObject.AddComponent<PawnCore.Presentation.SubManagers.Movement.SeparationSubManager>();
            subManager.separationRadius = separationRadius;
            subManager.separationStrength = separationStrength;
            subManager.checkFrequency = checkFrequency;
            return subManager;
        }
    }
}
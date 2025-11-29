using System;
using System.Collections.Generic;
using UnityEngine;
using PawnSurvivors.Domain;
using PawnSurvivors.Presentation.SubManagers.Visual;
using PawnSurvivors.Presentation.SubManagers.Physics;
using PawnSurvivors.Presentation.SubManagers.Movement;

namespace PawnSurvivors.Data.Recipes
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
            // CombatData 가져오기 또는 생성
            var combatData = pawnData.GetOrCreateCombatData();
            combatData.damage = damage;
            combatData.destroyOnHit = destroyOnHit;
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
            // HealthData 가져오기 또는 생성
            var healthData = pawnData.GetOrCreateHealthData();
            healthData.maxHealth = maxHealth;
            healthData.currentHealth = maxHealth;
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
            // MovableData 가져오기 또는 생성
            pawnData.GetOrCreateMovableData();

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
        public float damage; // 발사하는 투사체의 데미지
        public string targetTag = null; // 타겟 태그 (null이면 자동 결정)
        public float projectileSpeed = 0f; // 투사체 속도 (0이면 레시피 기본값 사용)

        public override void ApplyToPawnData(PawnData pawnData)
        {
            var combatData = pawnData.GetOrCreateCombatData();
            combatData.projectileRecipeName = projectileRecipeName;
            combatData.fireRate = fireRate;
            combatData.damage = damage; // 발사자의 데미지 저장
            combatData.targetTag = targetTag; // 타겟 태그 설정
            combatData.projectileSpeed = projectileSpeed; // 투사체 속도 설정
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
        
        [Tooltip("시각적 스케일 (기본값: 1,1,1)")]
        public Vector3 visualScale = Vector3.one;
        
        [SerializeReference]
        public List<AnimationStrategySetupData> strategySetups;

        public override void ApplyToPawnData(PawnData pawnData)
        {
            // VisualData 가져오기 또는 생성
            var visualData = pawnData.GetOrCreateVisualData();
            visualData.visualSpriteName = visualSpriteName;
            visualData.visualColor = visualColor;
            visualData.visualSpriteIndex = visualSpriteIndex;
            visualData.shadowPresetName = shadowPreset;
            visualData.visualScale = visualScale;
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            var visualSubManager = pawnObject.AddComponent<VisualSubManager>();
            
            // 각 애니메이션 전략 컴포넌트 추가
            if (strategySetups != null)
            {
                foreach (var strategySetup in strategySetups)
                {
                    strategySetup.AddAnimationStrategyComponent(pawnObject);
                }
            }
            
            return visualSubManager;
        }
    }

    // ========================================
    // Animation Strategy Setup Data
    // ========================================

    [Serializable]
    public abstract class AnimationStrategySetupData
    {
        [Tooltip("레시피에서 기본적으로 활성화할지 여부")]
        public bool isEnabledByDefault = true;

        public abstract MonoBehaviour AddAnimationStrategyComponent(GameObject pawnObject);
    }

    [Serializable]
    public class BounceAnimationStrategySetupData : AnimationStrategySetupData
    {
        [Tooltip("바운스 높이")]
        public float bounceHeight = 0.1f;
        
        [Tooltip("바운스 속도 (높을수록 빠름)")]
        public float bounceSpeed = 10f;
        
        [Tooltip("이동 시작으로 간주할 최소 속도")]
        public float movementThreshold = 0.1f;

        public override MonoBehaviour AddAnimationStrategyComponent(GameObject pawnObject)
        {
            BounceAnimationStrategy strategy = pawnObject.AddComponent<BounceAnimationStrategy>();
            strategy.bounceHeight = bounceHeight;
            strategy.bounceSpeed = bounceSpeed;
            strategy.movementThreshold = movementThreshold;
            strategy.SetInitialEnabledState(isEnabledByDefault);
            return strategy;
        }
    }

    [Serializable]
    public class IdleAnimationStrategySetupData : AnimationStrategySetupData
    {
        public override MonoBehaviour AddAnimationStrategyComponent(GameObject pawnObject)
        {
            IdleAnimationStrategy strategy = pawnObject.AddComponent<IdleAnimationStrategy>();
            strategy.SetInitialEnabledState(isEnabledByDefault);
            return strategy;
        }
    }

    [Serializable]
    public class PhysicsSubManagerSetupData : SubManagerSetupData
    {
        public ColliderType colliderType = ColliderType.None;
        public bool isTrigger = false;
        public RigidbodyType rigidbodyType = RigidbodyType.None;
        public float gravityScale = 1f;
        public string physicsLayerName = "Default";
        public string physicsTag = "Untagged";

        public override void ApplyToPawnData(PawnData pawnData)
        {
            // PhysicsData 가져오기 또는 생성
            var physicsData = pawnData.GetOrCreatePhysicsData();
            physicsData.colliderType = colliderType;
            physicsData.isTrigger = isTrigger;
            physicsData.rigidbodyType = rigidbodyType;
            physicsData.gravityScale = gravityScale;
            physicsData.physicsLayerName = physicsLayerName;
            physicsData.physicsTag = physicsTag;
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            return pawnObject.AddComponent<PhysicsSubManager>();
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
            HitFlashSubManager subManager = 
                pawnObject.AddComponent<HitFlashSubManager>();
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
            SeparationSubManager subManager = 
                pawnObject.AddComponent<SeparationSubManager>();
            subManager.separationRadius = separationRadius;
            subManager.separationStrength = separationStrength;
            subManager.checkFrequency = checkFrequency;
            return subManager;
        }
    }

    [Serializable]
    public class LevelUpSubManagerSetupData : SubManagerSetupData
    {
        [SerializeReference]
        public List<LevelUpStrategySetupData> strategySetups;

        public override void ApplyToPawnData(PawnData pawnData)
        {
            // LevelUpSubManager는 PawnData에 저장할 데이터가 없음
            // 레벨업 보상이 PawnData를 직접 수정함
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            LevelUpSubManager levelUpSubManager = pawnObject.AddComponent<LevelUpSubManager>();
            
            // 각 전략 컴포넌트 추가
            if (strategySetups != null)
            {
                foreach (var strategySetup in strategySetups)
                {
                    strategySetup.AddLevelUpStrategyComponent(pawnObject);
                }
            }
            
            return levelUpSubManager;
        }
    }

    // ========================================
    // LevelUp Strategy Setup Data
    // ========================================

    [Serializable]
    public abstract class LevelUpStrategySetupData
    {
        public int targetLevel = 2;
        
        // UI 표시용 텍스트
        public string conditionDescription = "";
        public string rewardDescription = "";
        
        // 보상 데이터
        public float healthIncrease = 0f;
        public float damageMultiplier = 1f;
        public float speedIncrease = 0f;
        public float fireRateMultiplier = 1f;

        public abstract MonoBehaviour AddLevelUpStrategyComponent(GameObject pawnObject);
        
        protected void ApplyCommonSettings(LevelUpStrategyBase strategy)
        {
            strategy.targetLevel = targetLevel;
            strategy.conditionDescription = conditionDescription;
            strategy.rewardDescription = rewardDescription;
            strategy.healthIncrease = healthIncrease;
            strategy.damageMultiplier = damageMultiplier;
            strategy.speedIncrease = speedIncrease;
            strategy.fireRateMultiplier = fireRateMultiplier;
        }
    }

    [Serializable]
    public class KillCountStrategySetup : LevelUpStrategySetupData
    {
        public int requiredKills = 3;

        public override MonoBehaviour AddLevelUpStrategyComponent(GameObject pawnObject)
        {
            KillCountLevelUpStrategy strategy = pawnObject.AddComponent<KillCountLevelUpStrategy>();
            ApplyCommonSettings(strategy);
            strategy.requiredKills = requiredKills;
            return strategy;
        }
    }

    [Serializable]
    public class SurvivalTimeStrategySetup : LevelUpStrategySetupData
    {
        public float requiredSeconds = 30f;

        public override MonoBehaviour AddLevelUpStrategyComponent(GameObject pawnObject)
        {
            SurvivalTimeLevelUpStrategy strategy = pawnObject.AddComponent<SurvivalTimeLevelUpStrategy>();
            ApplyCommonSettings(strategy);
            strategy.requiredSeconds = requiredSeconds;
            return strategy;
        }
    }

    [Serializable]
    public class DamageDealtStrategySetup : LevelUpStrategySetupData
    {
        public float requiredDamage = 500f;

        public override MonoBehaviour AddLevelUpStrategyComponent(GameObject pawnObject)
        {
            DamageDealtLevelUpStrategy strategy = pawnObject.AddComponent<DamageDealtLevelUpStrategy>();
            ApplyCommonSettings(strategy);
            strategy.requiredDamage = requiredDamage;
            return strategy;
        }
    }

    [Serializable]
    public class EventTriggerStrategySetup : LevelUpStrategySetupData
    {
        public string eventName = "BossDiscoveredEvent";
        public int requiredCount = 1;

        public override MonoBehaviour AddLevelUpStrategyComponent(GameObject pawnObject)
        {
            EventTriggerLevelUpStrategy strategy = pawnObject.AddComponent<EventTriggerLevelUpStrategy>();
            ApplyCommonSettings(strategy);
            strategy.eventName = eventName;
            strategy.requiredCount = requiredCount;
            return strategy;
        }
    }

    // ========================================
    // SpawnOnDeath SubManager Setup Data
    // ========================================

    [Serializable]
    public class SpawnOnDeathSubManagerSetupData : SubManagerSetupData
    {
        /// <summary>죽을 때 생성할 Pawn의 레시피 이름들</summary>
        public string[] spawnRecipeNames = new string[0];
        
        /// <summary>각 Pawn의 생성 확률 (0.0 ~ 1.0)</summary>
        public float[] spawnChances = new float[0];
        
        /// <summary>각 Pawn의 생성 개수</summary>
        public int[] spawnCounts = new int[0];

        public override void ApplyToPawnData(PawnData pawnData)
        {
            // SpawnOnDeathSubManager는 PawnData에 저장할 데이터가 없음
            // 모든 설정은 컴포넌트 자체에서 관리
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            SpawnOnDeathSubManager subManager = pawnObject.AddComponent<SpawnOnDeathSubManager>();
            subManager.spawnRecipeNames = spawnRecipeNames ?? new string[0];
            subManager.spawnChances = spawnChances ?? new float[0];
            subManager.spawnCounts = spawnCounts ?? new int[0];
            return subManager;
        }
    }

    // ========================================
    // CoinPickup SubManager Setup Data
    // ========================================

    [Serializable]
    public class CoinPickupSubManagerSetupData : SubManagerSetupData
    {
        /// <summary>이 코인이 주는 골드 양</summary>
        public int goldAmount = 1;
        
        /// <summary>플레이어와의 거리가 이 값 이하일 때 자동 흡수 시작</summary>
        public float magnetRange = 3f;
        
        /// <summary>
        /// 주의: 이동 속도는 MovableSubManager의 HomingMovementStrategy에서 관리됩니다.
        /// magnetSpeed는 더 이상 사용되지 않습니다.
        /// </summary>

        public override void ApplyToPawnData(PawnData pawnData)
        {
            // CoinPickupSubManager는 PawnData에 저장할 데이터가 없음
            // 모든 설정은 컴포넌트 자체에서 관리
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            CoinPickupSubManager subManager = pawnObject.AddComponent<CoinPickupSubManager>();
            subManager.goldAmount = goldAmount;
            subManager.magnetRange = magnetRange;
            // magnetSpeed는 제거됨 - 이동은 MovableSubManager의 HomingMovementStrategy가 처리
            return subManager;
        }
    }
}
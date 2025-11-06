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

        public override void ApplyToPawnData(PawnData pawnData)
        {
            pawnData.damage = damage;
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
            pawnData.maxHealth = maxHealth;
            pawnData.currentHealth = maxHealth;
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
            // No data to apply for PlayerAttackInput
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
            pawnData.projectileRecipeName = projectileRecipeName;
            pawnData.fireRate = fireRate;
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            ProjectileShooterSubManager subManager = pawnObject.AddComponent<ProjectileShooterSubManager>();
            
            // Create FirePoint if it doesn't exist
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
        public string visualSpriteName; // Changed from visualPrefabName
        public Color visualColor = Color.white; // Optional: for sprite customization

        public override void ApplyToPawnData(PawnData pawnData)
        {
            pawnData.visualSpriteName = visualSpriteName;
            pawnData.visualColor = visualColor;
        }

        public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
        {
            return pawnObject.AddComponent<PawnCore.Presentation.SubManagers.Visual.VisualSubManager>();
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
            movableData.targetMovement.speed = speed;
        }

        public override MonoBehaviour AddMovementStrategyComponent(GameObject pawnObject)
        {
            TargetMovementStrategy strategy = pawnObject.AddComponent<TargetMovementStrategy>();
            strategy.SetInitialEnabledState(isEnabledByDefault);
            return strategy;
        }
    }
}
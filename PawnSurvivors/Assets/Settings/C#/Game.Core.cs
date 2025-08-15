//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;

//namespace Game.Core
//{
//    #region Base Components

//    public abstract class ManagerBase : SubComponentBase<GameEventType, GameEventContext> { }
//    public abstract class PawnBase : SubComponentBase<Acts, AbilityContext> { }



//    #endregion

//    #region Enums

//    public enum ComponentType
//    {
//        DamageableComponent,
//        DamageDealerComponent,
//        GraphicComponent,
//        MoveableComponent,
//        PawnMoverComponent,
//        PawnRegisterToGameManagerComponent,
//        PawnTargetFinderComponent,
//        PlayerMoverComponent,
//        SubComponentBase,
//    }

//    public enum Acts
//    {
//        OnMove,
//        OnAttack,
//        OnDamaged,
//        OnDeath,
//        OnHeal,
//        OnUpdateTarget,
//        OnStart,
//        OnUpdate,
//        OnDisable,
//        OnCollisionEnter,
//        OnTriggerEnter,
//    }

//    public enum UpdateActionTypes
//    {
//        Update,
//    }

//    public enum JsonPath
//    {
//        pawns,
//    }

//    public enum GameEventType
//    {
//        XmlLoaded,
//        PawnSpawn,
//        Update,
//        RegisterUpdateAction,
//        JsonLoading,
//        GetPawnData,
//        GetPawnDatas,
//    }

//    #endregion

//    #region Interfaces

//    public interface IConfigurable<TConfig>
//    {
//        void Initialize(TConfig config);
//    }

//    public interface IActionMapManager
//    {
//        IReadOnlyDictionary<Acts, Action<AbilityContext>> GetActions { get; }
//        void AddAction(Acts act, Action<AbilityContext> action);
//        void RemoveAction(Acts act, Action<AbilityContext> action = null);
//    }

//    public interface IBaseConfig { IBaseConfig Clone(); }

//    #endregion

//    #region Configs / Models



//    [Serializable]
//    public class PawnConfig : IBaseConfig
//    {
//        [SerializeField, JsonProperty("id")]
//        private int id = -1;

//        [SerializeField, JsonProperty("name")]
//        private string name = "Default Pawn";

//        [SerializeField, JsonProperty("description")]
//        private string description = "This is a default pawn.";

//        public int Id => id;
//        public string Name => name;
//        public string Description => description;

//        public PawnConfig(PawnConfig other)
//        {
//            this.id = other.id;
//            this.name = other.name;
//            this.description = other.description;
//        }

//        public PawnConfig() { }

//        public IBaseConfig Clone() => new PawnConfig(this);
//    }

//    [Serializable]
//    public class DamageableConfig : IBaseConfig
//    {
//        [SerializeField, JsonProperty("maxHealth")]
//        private float maxHealth = 100f;

//        [SerializeField, JsonProperty("currentHealth")]
//        private float currentHealth = 100f;

//        public float MaxHealth { get => maxHealth; set => maxHealth = value; }
//        public float CurrentHealth { get => currentHealth; set => currentHealth = value; }

//        public DamageableConfig(DamageableConfig other)
//        {
//            this.maxHealth = other.maxHealth;
//            this.currentHealth = other.currentHealth;
//        }
//        public DamageableConfig() { }

//        public IBaseConfig Clone() => new DamageableConfig(this);
//    }

//    [Serializable]
//    public class DamageDealerConfig : IBaseConfig
//    {
//        [SerializeField, JsonProperty("damageAmount")]
//        private float damageAmount = 10f;

//        public float DamageAmount => damageAmount;

//        public DamageDealerConfig(DamageDealerConfig other)
//        {
//            this.damageAmount = other.damageAmount;
//        }
//        public DamageDealerConfig() { }

//        public IBaseConfig Clone() => new DamageDealerConfig(this);
//    }

//    [Serializable]
//    public class MoveableConfig : IBaseConfig
//    {
//        [SerializeField, JsonProperty("moveSpeed")]
//        private float moveSpeed = 1f;

//        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

//        public MoveableConfig(MoveableConfig other)
//        {
//            this.moveSpeed = other.moveSpeed;
//        }
//        public MoveableConfig() { }

//        public IBaseConfig Clone() => new MoveableConfig(this);
//    }


//    public class PawnData
//    {
//        public PawnConfig PawnConfig { get; private set; }
//        public Dictionary<string, IBaseConfig> ComponentConfigs { get; private set; }

//        public PawnData(PawnConfig pawnConfig, Dictionary<string, IBaseConfig> componentConfigs)
//        {
//            PawnConfig = pawnConfig;
//            ComponentConfigs = componentConfigs;
//        }

//        public PawnData() { }

//        // 복사 생성자 (깊은 복사)
//        public PawnData(PawnData other)
//        {
//            // PawnConfig도 Clone() 호출
//            PawnConfig = (PawnConfig)other.PawnConfig.Clone();

//            // Dictionary 내부 값도 전부 Clone() 호출
//            ComponentConfigs = other.ComponentConfigs.ToDictionary(
//                kvp => kvp.Key,
//                kvp => kvp.Value.Clone()
//            );
//        }

//        // Clone 메서드
//        public PawnData Clone()
//        {
//            return new PawnData(this);
//        }
//    }


//    [Serializable]
//    public class PawnSerializationContainer
//    {
//        public PawnConfig pawnConfig;
//        public Dictionary<string, IBaseConfig> components;
//    }

//    public class EmptyConfig : IBaseConfig
//    {
//        public IBaseConfig Clone()
//        {
//            return new EmptyConfig();
//        }
//    }

//    #endregion

//    #region AbilityContext

//    public class AbilityContext
//    {
//        public Pawn SourcePawn { get; set; }
//        public Pawn TargetPawn { get; set; }
//        public Vector3? InputDirection { get; set; }
//        public float? DamageAmount { get; set; }
//    }

//    public class AbilityContextold
//    {
//        public Vector3 inputDirection;
//        public Pawn targetPawn;
//        public float damageAmount;

//        public void ResetTransientInfo() => damageAmount = 0;
//    }

//    #endregion

//    #region GameEventContext

//    public class GameEventContext
//    {
//        public bool stopUpdate = false;
//        public PawnData PawnData { get; set; }
//        public int PawnDataIndex { get; set; }
//        public List<PawnData> PawnDatas { get; set; }

//        public void LogCurrentState()
//        {
//            Debug.Log("--- GameEventContext 상태 ---");
//            Debug.Log($"stopUpdate: {stopUpdate}");
//            Debug.Log($"PawnData: {(PawnData != null ? "데이터 있음" : "null")}");
//            if (PawnData != null)
//            {
//                Debug.Log($"PawnData.PawnConfig.Name: {PawnData.PawnConfig?.Name ?? "null"}");
//                Debug.Log($"PawnData.ComponentConfigs.Count: {PawnData.ComponentConfigs?.Count ?? 0}");
//            }
//            Debug.Log("-----------------------------");
//        }
//    }

//    #endregion

//    #region Component Mapping

//    public static class ComponentMapping
//    {
//        public static readonly Dictionary<string, (Type ComponentType, Type ConfigType)> ComponentMap = new()
//        {
//            { "DamageableComponent", (typeof(DamageableComponent), typeof(DamageableConfig)) },
//            { "DamageDealerComponent", (typeof(DamageDealerComponent), typeof(DamageDealerConfig)) },
//            { "GraphicComponent", (typeof(GraphicComponent), typeof(EmptyConfig)) },
//            { "MoveableComponent", (typeof(MoveableComponent), typeof(MoveableConfig)) },
//            { "PawnMoverComponent", (typeof(PawnMoverComponent), typeof(EmptyConfig)) },
//            { "PawnRegisterToGameManagerComponent", (typeof(PawnRegisterToGameManagerComponent), typeof(EmptyConfig)) },
//            { "PawnTargetFinderComponent", (typeof(PawnTargetFinderComponent), typeof(EmptyConfig)) },
//            { "PlayerMoverComponent", (typeof(PlayerMoverComponent), typeof(EmptyConfig)) },
//        };
//    }

//    #endregion
//}

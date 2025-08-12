using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    #region 직렬화 관련 클래스와

    // Game.Core 네임스페이스 내 (별도의 파일 가능)
    public abstract class ManagerBase : SubComponentBase<GameEventType, GameEventContext> { }
    public abstract class PawnBase : SubComponentBase<Acts, AbilityContext> { }

    // 정적(static) 유틸리티 클래스
    public static class ComponentMapping
    {
        // 딕셔너리는 클래스 내부에 정의합니다.
        // public으로 선언하여 다른 클래스에서 접근 가능하게 합니다.
        public static readonly Dictionary<string, (Type ComponentType, Type ConfigType)> ComponentMap = new()
        {
            { "DamageableComponent", (typeof(DamageableComponent), typeof(DamageableConfig)) },
            { "DamageDealerComponent", (typeof(DamageDealerComponent), typeof(DamageDealerConfig)) },
            { "GraphicComponent", (typeof(GraphicComponent), typeof(EmptyConfig)) },
            { "MoveableComponent", (typeof(MoveableComponent), typeof(MoveableConfig)) },
            { "PawnMoverComponent", (typeof(PawnMoverComponent), typeof(EmptyConfig)) },
            { "PawnRegisterToGameManagerComponent", (typeof(PawnRegisterToGameManagerComponent), typeof(EmptyConfig)) },
            { "PawnTargetFinderComponent", (typeof(PawnTargetFinderComponent), typeof(EmptyConfig)) },
            { "PlayerMoverComponent", (typeof(PlayerMoverComponent), typeof(EmptyConfig)) },
        };
    }




    #endregion

    #region Enums

    public enum ComponentType
    {
        DamageableComponent,
        DamageDealerComponent,
        GraphicComponent,
        MoveableComponent,
        PawnMoverComponent,
        PawnRegisterToGameManagerComponent,
        PawnTargetFinderComponent,
        PlayerMoverComponent,
        SubComponentBase,
    }

    /// <summary>
    /// 게임 내에서 발생하는 핵심 행동들을 정의하는 Enum입니다.
    /// Pawn의 델리게이트 시스템에서 키로 사용됩니다.
    /// </summary>
    public enum Acts
    {
        // 폰의 기본 행동 및 생명 주기 관련 액트
        OnMove,         // 이동 요청
        OnAttack,       // 공격 요청 (어떤 방식이든 상관 없이 공격 행위 자체)
        OnDamaged,      // 피해를 받았을 때
        OnDeath,        // 사망했을 때
        OnHeal,         // 체력을 회복했을 때
        OnUpdateTarget,   // 타겟을 찾을 때 

        // 유니티 생명 주기 메서드에 연결될 액트 (전역 업데이트 요청용)
        OnStart,
        OnUpdate,       // MonoBehaviour.Update() 시점에 해당 델리게이트 등록 함수들 호출
        OnDisable,

        OnCollisionEnter,
        OnTriggerEnter,


    }

    public enum UpdateActionTypes
    {         

        Update,         // MonoBehaviour.Update() 시점

    }



    #endregion



    #region AbilityContext Class

    /// <summary>
    /// 델리게이트에 필요한 모든 맥락 정보를 담는 클래스입니다.
    /// 이 클래스의 인스턴스는 각 행위(Act) 발생 시 관련 데이터를 전달하는 데 사용됩니다.
    /// </summary>
    public class AbilityContextold
    {
        // --- 비단발성 (Persistent) 정보 ---
        public Vector3 inputDirection;
        public Pawn targetPawn; // 오타 수정: tartgetPawn -> targetPawn

        // --- 단발성 (Transient) 정보 ---
        // #region 으로 묶어두는 것은 코드 가독성 측면에서 좋습니다.
        #region TransientInfo
        public float damageAmount;
        // 여기에 hitPoint, hitNormal, isCriticalHit, sourceObject 등
        // 단발성 액트와 관련된 다른 필드들을 추가할 수 있습니다.
        #endregion

        /// <summary>
        /// 단발성(Transient) 정보를 초기화하는 메서드입니다.
        /// 액트 처리 후 또는 다음 액트 시작 전에 호출됩니다.
        /// </summary>
        public void ResetTransientInfo()
        {
            damageAmount = 0;
            // 여기에 다른 모든 단발성 필드들을 초기화하는 로직을 추가합니다.
            // 예: hitPoint = Vector3.zero; hitNormal = Vector3.zero; isCriticalHit = false;
            // sourceObject = null;
        }

        // 모든 필드를 초기화하는 Reset() 메서드는 비단발성 정보가 있다면 이제 신중하게 사용해야 합니다.
        // public void Reset() { ... }
    }

    #endregion

    #region 아이템스탯

    #endregion

    #region Interfaces

    /// <summary>
    /// Acts와 연결된 Action 델리게이트 딕셔너리를 관리하는 기능을 제공하는 인터페이스입니다.
    /// PawnAction 등의 컴포넌트들이 이 인터페이스를 구현하여 자신의 능력을 시스템에 등록하고 해제합니다.
    /// </summary>
    public interface IActionMapManager
    {
        /// <summary>
        /// 이 관리자가 소유한 Acts-Action 델리게이트 딕셔너리에 대한 읽기 전용 뷰를 가져옵니다.
        /// 외부에서 딕셔너리 내용을 직접 변경(추가/제거)할 수 없도록 IReadOnlyDictionary를 사용합니다.
        /// </summary>
        IReadOnlyDictionary<Acts, Action<AbilityContext>> GetActions { get; }

        /// <summary>
        /// 지정된 Acts에 대한 Action 델리게이트를 추가하거나, 이미 존재하는 경우 기존 델리게이트에 연결합니다.
        /// </summary>
        /// <param name="act">등록할 Acts Enum 값.</param>
        /// <param name="action">해당 Acts에 연결할 Action 델리게이트.</param>
        void AddAction(Acts act, Action<AbilityContext> action);

        /// <summary>
        /// 지정된 Acts에 대한 특정 Action 델리게이트를 제거하거나,
        /// action 매개변수가 null인 경우 해당 Acts에 연결된 모든 델리게이트를 제거합니다.
        /// </summary>
        /// <param name="act">제거할 대상 Acts Enum 값.</param>
        /// <param name="action">제거할 특정 Action 델리게이트. null이면 해당 Acts의 모든 델리게이트를 제거합니다.</param>
        void RemoveAction(Acts act, Action<AbilityContext> action = null);

        // 참고: InvokeAction 메서드는 델리게이트 맵 '관리' 기능이라기보다는 '사용' 기능에 가깝습니다.
        // 딕셔너리를 직접 노출하는 GetActions를 통해 외부에서 Invoke를 할 수 있으므로,
        // 이 인터페이스에 반드시 포함될 필요는 없습니다. 하지만 필요하다면 추가할 수 있습니다.
        // void InvokeAction(Acts act, AbilityContext context);
    }

    #endregion
}


#region Models

namespace Game.Core
{
    public enum JsonPath
    {
        pawns,
    }


    [System.Serializable]
    public class PawnSerializationContainer
    {
        public PawnConfig pawnConfig;

        // 컴포넌트 이름(string)을 키로, Config 객체(object)를 값으로 받습니다.
        // JSON의 null 값과 객체를 모두 처리할 수 있습니다.
        public Dictionary<string, IBaseConfig> components;
    }

    /// <summary>
    /// 이 클래스는 Pawn을 생성하기 위한 모든 가공된 데이터를 담고 있습니다.
    /// Pawn 생성 로직에서 직접적으로 사용되는, 타입 안전한 컨테이너입니다.
    /// </summary>
    public class PawnData
    {
        public PawnConfig PawnConfig { get; private set; }
        public Dictionary<string, IBaseConfig> ComponentConfigs { get; private set; }

        public PawnData(PawnConfig pawnConfig, Dictionary<string, IBaseConfig> componentConfigs)
        {
            PawnConfig = pawnConfig;
            ComponentConfigs = componentConfigs;
        }

        public PawnData()
        {
        }
    }

    [System.Serializable]
    public class PawnConfig
    {
        [SerializeField] private int _id = -1;
        [SerializeField] private string _name = "Default Pawn";
        [SerializeField] private string _description = "This is a default pawn.";

        // 필드의 값을 읽기 전용으로 노출하는 프로퍼티로 통일
        public int Id => _id;
        public string Name => _name;
        public string Description => _description;
    }
    public interface IBaseConfig
    { }

    public class EmptyConfig : IBaseConfig
    { }



    /// <summary>
    /// 체력 관리에 필요한 설정과 현재 상태를 담는 클래스입니다.
    /// 이 데이터는 DamageableComponent에서 사용되며 JSON 직렬화/역직렬화의 대상이 됩니다.
    /// </summary>
    [Serializable]
    public class MoveableConfig : IBaseConfig
    {
        [SerializeField]
        private float moveSpeed = 1f; // 이동 속도 (유니티 에디터에서 설정 가능)

        /// <summary>
        /// 현재 이동 속도 값을 외부에 노출합니다.
        /// </summary>
        public float MoveSpeed => moveSpeed;
    }

    /// <summary>
    /// 체력 관리에 필요한 설정과 현재 상태를 담는 클래스입니다.
    /// 이 데이터는 DamageableComponent에서 사용되며 JSON 직렬화/역직렬화의 대상이 됩니다.
    /// </summary>
    [Serializable]
    public class DamageableConfig : IBaseConfig
    {
        // private 필드로 선언하여 외부에서 직접적인 수정 방지
        [SerializeField]
        private float _maxHealth = 100f;
        [SerializeField]
        private float _currentHealth = 100f;

        public float MaxHealth
        {
            get { return _maxHealth; }
            // set을 private으로 설정하여 내부에서만 수정 가능하게 할 수 있습니다.
            set { _maxHealth = value; }
        }

        /// <summary>
        /// 현재 체력 프로퍼티. 외부에서 읽기/쓰기 가능 (get, set).
        /// </summary>
        public float CurrentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = value; }
        }
    }

    /// <summary>
    /// 체력 관리에 필요한 설정과 현재 상태를 담는 클래스입니다.
    /// 이 데이터는 DamageableComponent에서 사용되며 JSON 직렬화/역직렬화의 대상이 됩니다.
    /// </summary>
    [Serializable]
    public class DamageDealerConfig : IBaseConfig
    {
        [SerializeField]
        private float _damageAmount = 10f; // 이 DamageDealer가 입힐 기본 피해량

        public float DamageAmount => _damageAmount;       // 피해량을 외부에 노출 (읽기 전용)
    }
}


#endregion

#region AbilityContext

namespace Game.Core // 프로젝트 구조에 맞게 네임스페이스 조정
{

    /// <summary>
    /// 게임 내에서 발생하는 다양한 행동(Ability)의 맥락(Context) 정보를 담는 클래스입니다.
    /// 모든 델리게이트 시그니처에 Action<AbilityContext>를 유지하면서,
    /// 필요한 정보만 선택적으로 제공하여 정보 과다를 줄이고,
    /// SourcePawn과 TargetPawn으로 행동의 주체와 대상을 명확히 합니다.
    /// </summary>
    public class AbilityContext
    {
        // --- 핵심 정보 (대부분의 Acts에서 유용) ---
        // 누가 이 행동을 시작했는가? (주체/발신자)
        public Pawn SourcePawn { get; set; }

        // 누가 이 행동의 대상인가? (수신자)
        // Self-action의 경우 SourcePawn과 동일하거나, 해당 PawnAction이 부착된 Pawn을 의미.
        public Pawn TargetPawn { get; set; }



        // 이동 관련 입력 방향 (Acts.OnMove 등에서 사용, 없을 시 null)
        public Vector3? InputDirection { get; set; }

        // --- 단발성 이벤트 정보 (각 Acts에 따라 선택적으로 사용) ---
        // Acts.OnHit, OnDamage 등 피해 관련
        public float? DamageAmount { get; set; }
        //public float? DamageAmount { get; set; }
        //public float? DamageAmount { get; set; }



    }
}

#endregion

#region GameEventType

namespace Game.Core
{
    public enum GameEventType
    {
        XmlLoaded,
        PawnSpawn,
        Update,
        RegisterUpdateAction,
        JsonLoading,
        GetPawnData,
        // ...
    }

    public class GameEventContext
    {
        public bool stopUpdate = false;
        public PawnData PawnData { get; set; }

        /// <summary>
        /// GameEventContext의 현재 상태를 디버그 로그로 출력합니다.
        /// </summary>
        public void LogCurrentState()
        {
            Debug.Log("--- GameEventContext 상태 ---");
            Debug.Log($"stopUpdate: {stopUpdate}");
            Debug.Log($"PawnData: {(PawnData != null ? "데이터 있음" : "null")}");

            // PawnData가 null이 아닐 경우, 더 상세한 정보를 출력할 수 있습니다.
            if (PawnData != null)
            {
                Debug.Log($"PawnData.PawnConfig.Name: {PawnData.PawnConfig?.Name ?? "null"}");
                Debug.Log($"PawnData.ComponentConfigs.Count: {PawnData.ComponentConfigs?.Count ?? 0}");
            }
            Debug.Log("-----------------------------");
        }
    }
}

#endregion
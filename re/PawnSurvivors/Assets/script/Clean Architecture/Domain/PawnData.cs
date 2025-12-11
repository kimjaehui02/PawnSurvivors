
using System;
using System.Collections.Generic;
using UnityEngine;
using PawnSurvivors.Data.Recipes;

namespace PawnSurvivors.Domain
{
    [Serializable]
    public class PawnData
    {
        /// <summary>
        /// 플레이어 캐릭터 타입 (enum 기반)
        /// null이면 플레이어가 아닌 Pawn (Enemy, Projectile 등)
        /// </summary>
        public PlayerCharacter? characterType;
        
        // 플레이어 인덱스 (PlayerController의 playerPawns 리스트에서의 인덱스)
        // 경험치 추적 등에 사용됨 (같은 레시피로 생성된 여러 Pawn도 각각 독립적인 경험치를 가짐)
        public int playerIndex = -1;
        
        /// <summary>
        /// 캐릭터 강화 레벨 (중복 구매 강화 시스템)
        /// 같은 캐릭터를 여러 번 구매할 때마다 증가
        /// 0 = 기본, 1 = 1회 강화, 2 = 2회 강화...
        /// </summary>
        public int upgradeLevel = 0;
        
        /// <summary>
        /// 레시피 이름 (하위 호환성 및 비플레이어 Pawn용)
        /// characterType이 null이 아닌 경우 characterType.ToString()과 동일
        /// </summary>
        public string recipeName
        {
            get
            {
                if (characterType.HasValue)
                    return characterType.Value.ToString();
                return _recipeName;
            }
            set
            {
                _recipeName = value;
                // string에서 enum으로 자동 변환 시도
                if (!string.IsNullOrEmpty(value) && System.Enum.TryParse<PlayerCharacter>(value, true, out var character))
                {
                    characterType = character;
                }
            }
        }
        
        [SerializeField]
        private string _recipeName;
        
        // 모듈화된 데이터 (nullable로 필요한 것만 할당)
        public HealthData healthData;
        public VisualData visualData;
        public CombatData combatData;
        public MovableData movableData;
        public PhysicsData physicsData;
        public ExperienceData experienceData;

        /// <summary>
        /// HealthData를 가져오거나 없으면 생성합니다.
        /// </summary>
        public HealthData GetOrCreateHealthData()
        {
            if (healthData == null)
            {
                healthData = new HealthData();
            }
            return healthData;
        }

        /// <summary>
        /// VisualData를 가져오거나 없으면 생성합니다.
        /// </summary>
        public VisualData GetOrCreateVisualData()
        {
            if (visualData == null)
            {
                visualData = new VisualData();
            }
            return visualData;
        }

        /// <summary>
        /// CombatData를 가져오거나 없으면 생성합니다.
        /// </summary>
        public CombatData GetOrCreateCombatData()
        {
            if (combatData == null)
            {
                combatData = new CombatData();
            }
            return combatData;
        }

        /// <summary>
        /// MovableData를 가져오거나 없으면 생성합니다.
        /// </summary>
        public MovableData GetOrCreateMovableData()
        {
            if (movableData == null)
            {
                movableData = new MovableData();
            }
            return movableData;
        }

        /// <summary>
        /// PhysicsData를 가져오거나 없으면 생성합니다.
        /// </summary>
        public PhysicsData GetOrCreatePhysicsData()
        {
            if (physicsData == null)
            {
                physicsData = new PhysicsData();
            }
            return physicsData;
        }

        /// <summary>
        /// ExperienceData를 가져오거나 없으면 생성합니다.
        /// 주의: 레벨업 시스템이 있는 Pawn에만 사용해야 합니다.
        /// </summary>
        public ExperienceData GetOrCreateExperienceData()
        {
            if (experienceData == null)
            {
                experienceData = new ExperienceData();
            }
            return experienceData;
        }
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
        
        // 시각적 스케일 (1.0이 기본값, 작게 하려면 1.0보다 작게)
        public Vector3 visualScale = Vector3.one;
        
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
        
        /// <summary>
        /// 투사체 발사 시 타겟으로 할 태그입니다.
        /// null이거나 빈 문자열이면 자동으로 결정됩니다 (Player 태그면 Enemy, Enemy 태그면 Player).
        /// </summary>
        public string targetTag = null;
        
        /// <summary>
        /// 발사하는 투사체의 속도입니다.
        /// 0이면 투사체 레시피의 기본 속도를 사용합니다.
        /// </summary>
        public float projectileSpeed = 0f;
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

    [Serializable]
    public class ExperienceData
    {
        /// <summary>
        /// 경험치바에 표시될 현재 진행도입니다.
        /// 각 레벨업 전략이 자신의 조건에 맞게 이 값을 증가시킵니다.
        /// 예: DamageDealtLevelUpStrategy는 데미지를 쌓고, KillCountLevelUpStrategy는 킬을 쌓습니다.
        /// </summary>
        public float currentProgress = 0f;
    }

    /// <summary>
    /// 플레이어 Pawn의 영구 데이터입니다.
    /// 라운드 간 유지되어야 하는 정보만 저장합니다.
    /// (경험치, 레벨, 획득한 업그레이드 등)
    /// </summary>
    [Serializable]
    public class PawnPersistentData
    {
        /// <summary>플레이어 캐릭터 타입 (enum 기반)</summary>
        public PlayerCharacter characterType;
        
        /// <summary>플레이어 인덱스 (같은 레시피의 여러 Pawn 구분)</summary>
        public int playerIndex = -1;
        
        /// <summary>
        /// 레시피 이름 (하위 호환성용)
        /// characterType.ToString()과 동일
        /// </summary>
        public string recipeName
        {
            get => characterType.ToString();
            set
            {
                // string에서 enum으로 자동 변환 시도
                if (!string.IsNullOrEmpty(value) && System.Enum.TryParse<PlayerCharacter>(value, true, out var character))
                {
                    characterType = character;
                }
            }
        }
        
        /// <summary>경험치 진행도 (라운드 간 유지)</summary>
        public float experienceProgress = 0f;
        
        /// <summary>현재 레벨 (라운드 간 유지)</summary>
        public int currentLevel = 0;
        
        /// <summary>
        /// 캐릭터 강화 레벨 (중복 구매 강화 시스템, 라운드 간 유지)
        /// 0 = 기본, 1 = 1회 강화, 2 = 2회 강화...
        /// </summary>
        public int upgradeLevel = 0;
        
        /// <summary>
        /// 획득한 업그레이드 (업그레이드 이름 -> 레벨)
        /// 예: upgrades["AttackSpeed"] = 3
        /// </summary>
        public Dictionary<string, int> upgrades = new Dictionary<string, int>();
        
        /// <summary>
        /// 영구적으로 변경된 스탯 (레시피 기본값에서 변경된 값)
        /// 예: permanentStats["damage"] = 15.0f (기본 10에서 +5)
        /// </summary>
        public Dictionary<string, float> permanentStats = new Dictionary<string, float>();
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

    /// <summary>
    /// 현재 활성화된 Pawn의 상태 데이터입니다.
    /// 씬 전환 시 저장되어 다음 씬에서 Pawn을 복원하는 데 사용됩니다.
    /// </summary>
    [Serializable]
    public class ActivePawnData
    {
        /// <summary>플레이어 캐릭터 타입</summary>
        public PlayerCharacter characterType;

        /// <summary>플레이어 인덱스 (대열 위치)</summary>
        public int playerIndex;

        /// <summary>현재 체력</summary>
        public float currentHealth;

        /// <summary>최대 체력</summary>
        public float maxHealth;

        /// <summary>경험치 진행도</summary>
        public float experienceProgress;

        /// <summary>캐릭터 강화 레벨</summary>
        public int upgradeLevel;

        /// <summary>Pawn이 살아있는지 여부</summary>
        public bool isAlive;

        /// <summary>
        /// PawnData에서 ActivePawnData를 생성합니다.
        /// </summary>
        public static ActivePawnData FromPawnData(PawnData pawnData, bool isAlive = true)
        {
            if (pawnData == null || !pawnData.characterType.HasValue)
                return null;

            return new ActivePawnData
            {
                characterType = pawnData.characterType.Value,
                playerIndex = pawnData.playerIndex,
                currentHealth = pawnData.healthData?.currentHealth ?? 100f,
                maxHealth = pawnData.healthData?.maxHealth ?? 100f,
                experienceProgress = pawnData.experienceData?.currentProgress ?? 0f,
                upgradeLevel = pawnData.upgradeLevel,
                isAlive = isAlive
            };
        }

        /// <summary>
        /// ActivePawnData를 PawnData에 적용합니다.
        /// </summary>
        public void ApplyToPawnData(PawnData pawnData)
        {
            if (pawnData == null) return;

            pawnData.characterType = characterType;
            pawnData.playerIndex = playerIndex;
            pawnData.upgradeLevel = upgradeLevel;

            if (pawnData.healthData != null)
            {
                pawnData.healthData.currentHealth = currentHealth;
                pawnData.healthData.maxHealth = maxHealth;
            }

            if (pawnData.experienceData != null)
            {
                pawnData.experienceData.currentProgress = experienceProgress;
            }
        }
    }
}

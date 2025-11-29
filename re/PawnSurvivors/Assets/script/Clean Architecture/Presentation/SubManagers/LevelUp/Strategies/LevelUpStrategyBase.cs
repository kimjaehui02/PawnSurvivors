using UnityEngine;
using PawnSurvivors.Domain;

/// <summary>
/// 모든 레벨업 전략에 대한 추상 MonoBehaviour 기본 클래스입니다.
/// 각 구체적인 전략은 Pawn에 연결된 구성 요소여야 합니다.
/// </summary>
public abstract class LevelUpStrategyBase : MonoBehaviour
{
    /// <summary>이 전략을 달성했을 때 도달하는 레벨</summary>
    public int targetLevel = 2;

    protected PawnManager _pawnManager;

    // ========================================
    // UI 표시용 텍스트 (JSON 레시피에서 설정)
    // ========================================

    /// <summary>조건 설명 텍스트 (예: "적 3명 처치")</summary>
    public string conditionDescription = "";

    /// <summary>보상 설명 텍스트 (예: "체력 +20, 데미지 1.5배")</summary>
    public string rewardDescription = "";

    // ========================================
    // 레벨업 보상 (JSON 레시피에서 설정)
    // ========================================

    /// <summary>최대 체력 증가량</summary>
    public float healthIncrease = 0f;

    /// <summary>데미지 배율 (1.5 = 50% 증가)</summary>
    public float damageMultiplier = 1f;

    /// <summary>이동 속도 증가량</summary>
    public float speedIncrease = 0f;

    /// <summary>공격 속도 배율 (0.8 = 20% 빠르게)</summary>
    public float fireRateMultiplier = 1f;

    public virtual void Init(PawnManager pawnManager)
    {
        _pawnManager = pawnManager;
    }

    protected virtual void Awake()
    {
        if (_pawnManager == null)
        {
            _pawnManager = GetComponent<PawnManager>();
        }
        if (_pawnManager == null)
        {
            Debug.LogError("A LevelUpStrategy must be on a GameObject with a PawnManager.", this);
        }
    }

    /// <summary>
    /// 레벨업 조건이 만족되었는지 확인합니다.
    /// </summary>
    /// <returns>조건이 만족되면 true</returns>
    public abstract bool CheckCondition();

    /// <summary>
    /// 레벨업 진행도를 반환합니다. (UI 표시용)
    /// </summary>
    /// <returns>0.0 ~ 1.0 사이의 값</returns>
    public abstract float GetProgress();

    /// <summary>
    /// 레벨업 진행도를 텍스트로 반환합니다. (UI 표시용)
    /// </summary>
    /// <returns>예: "2/3 처치"</returns>
    public abstract string GetProgressText();

    /// <summary>
    /// 레벨업 보상을 적용합니다.
    /// 하위 클래스에서 오버라이드하여 커스텀 보상을 추가할 수 있습니다.
    /// </summary>
    public virtual void ApplyRewards(PawnManager pawnManager)
    {
        var pawnData = pawnManager.PawnData;

        // 체력 증가
        if (healthIncrease > 0 && pawnData.healthData != null)
        {
            pawnData.healthData.maxHealth += healthIncrease;
            pawnData.healthData.currentHealth += healthIncrease;
            Debug.Log($"  └─ 최대 체력 +{healthIncrease} (총: {pawnData.healthData.maxHealth})", this);
        }

        // 데미지 배율 증가
        if (damageMultiplier != 1f && pawnData.combatData != null)
        {
            float oldDamage = pawnData.combatData.damage;
            pawnData.combatData.damage *= damageMultiplier;
            Debug.Log($"  └─ 데미지 {oldDamage:F1} → {pawnData.combatData.damage:F1} (x{damageMultiplier})", this);
        }

        // 이동 속도 증가
        if (speedIncrease > 0 && pawnData.movableData != null)
        {
            if (pawnData.movableData.keyboardMovement != null)
            {
                pawnData.movableData.keyboardMovement.moveSpeed += speedIncrease;
                Debug.Log($"  └─ 이동 속도 +{speedIncrease} (총: {pawnData.movableData.keyboardMovement.moveSpeed})", this);
            }
        }

        // 공격 속도 배율 증가
        if (fireRateMultiplier != 1f && pawnData.combatData != null)
        {
            float oldFireRate = pawnData.combatData.fireRate;
            pawnData.combatData.fireRate *= fireRateMultiplier;
            Debug.Log($"  └─ 공격 속도 {oldFireRate:F2} → {pawnData.combatData.fireRate:F2} (x{fireRateMultiplier})", this);
        }
    }

    /// <summary>
    /// 레시피 설정에 따라 전략의 초기 활성화 상태를 설정합니다.
    /// </summary>
    /// <param name="enabledState">전략을 기본적으로 활성화해야 하는지 여부입니다.</param>
    public void SetInitialEnabledState(bool enabledState)
    {
        this.enabled = enabledState;
    }
}


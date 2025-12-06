using UnityEngine;
using PawnSurvivors.Domain;
using PawnSurvivors.Data.Recipes;

/// <summary>
/// Pawn의 중복 구매 강화를 관리하는 SubManager입니다.
/// CharacterUpgradeUseCase로부터 강화 단계를 받아서 스탯을 적용합니다.
/// </summary>
public class CharacterUpgradeSubManager : PawnSubManager
{
    private PawnData _pawnData;
    private int _currentUpgradeLevel = 0;

    public override void SubStart()
    {
        _pawnData = _pawnManager.PawnData;
        
        if (_pawnData == null)
        {
            Debug.LogWarning($"[CharacterUpgradeSubManager] {_pawnManager.name}: PawnData가 없습니다.", this);
            return;
        }

        // 현재 upgradeLevel 복원 (라운드 간 유지)
        _currentUpgradeLevel = _pawnData.upgradeLevel;
        
        // 라운드 시작 시 저장된 강화 레벨이 있으면 스탯 재적용
        if (_currentUpgradeLevel > 0 && _pawnData.characterType.HasValue)
        {
            RestoreUpgradeStats(_currentUpgradeLevel);
        }
    }

    public override void SubUpdate()
    {
        // 업데이트할 로직 없음 (강화는 UseCase에서 요청 시에만 적용)
    }

    /// <summary>
    /// 강화 단계를 적용합니다. (CharacterUpgradeUseCase에서 호출)
    /// </summary>
    /// <param name="stage">강화 단계 (1, 2, ...)</param>
    /// <param name="settings">강화 설정 (JSON에서 로드)</param>
    public void ApplyUpgradeStage(int stage, CharacterUpgradeSettings settings)
    {
        if (_pawnData == null || settings == null || settings.stageRewards == null)
        {
            Debug.LogWarning($"[CharacterUpgradeSubManager] {_pawnManager.name}: 강화 적용 실패 - 데이터 없음", this);
            return;
        }

        // 해당 단계의 보상 찾기
        var stageReward = System.Linq.Enumerable.FirstOrDefault(settings.stageRewards, r => r.stage == stage);
        if (stageReward == null)
        {
            Debug.LogWarning($"[CharacterUpgradeSubManager] {_pawnManager.name}: 강화 단계 {stage}의 보상을 찾을 수 없습니다.", this);
            return;
        }

        // 체력 증가
        if (stageReward.healthIncrease > 0)
        {
            var healthData = _pawnData.GetOrCreateHealthData();
            healthData.maxHealth += stageReward.healthIncrease;
            healthData.currentHealth += stageReward.healthIncrease;
            Debug.Log($"[CharacterUpgradeSubManager] {_pawnManager.name} 강화 단계 {stage}: 체력 +{stageReward.healthIncrease} (총: {healthData.maxHealth})", this);
        }

        // 데미지 배율 증가
        if (stageReward.damageMultiplier != 1f)
        {
            var combatData = _pawnData.GetOrCreateCombatData();
            float oldDamage = combatData.damage;
            combatData.damage *= stageReward.damageMultiplier;
            Debug.Log($"[CharacterUpgradeSubManager] {_pawnManager.name} 강화 단계 {stage}: 데미지 {oldDamage:F1} → {combatData.damage:F1} (x{stageReward.damageMultiplier})", this);
        }

        // 이동 속도 증가
        if (stageReward.speedIncrease > 0)
        {
            var movableData = _pawnData.GetOrCreateMovableData();
            if (movableData.keyboardMovement != null)
            {
                float oldSpeed = movableData.keyboardMovement.moveSpeed;
                movableData.keyboardMovement.moveSpeed += stageReward.speedIncrease;
                Debug.Log($"[CharacterUpgradeSubManager] {_pawnManager.name} 강화 단계 {stage}: 이동 속도 {oldSpeed:F1} → {movableData.keyboardMovement.moveSpeed:F1} (+{stageReward.speedIncrease})", this);
            }
        }

        // 공격 속도 배율 증가
        if (stageReward.fireRateMultiplier != 1f)
        {
            var combatData = _pawnData.GetOrCreateCombatData();
            float oldFireRate = combatData.fireRate;
            combatData.fireRate *= stageReward.fireRateMultiplier;
            Debug.Log($"[CharacterUpgradeSubManager] {_pawnManager.name} 강화 단계 {stage}: 공격 속도 {oldFireRate:F2} → {combatData.fireRate:F2} (x{stageReward.fireRateMultiplier})", this);
        }

        // upgradeLevel 업데이트
        _currentUpgradeLevel = stage;
        _pawnData.upgradeLevel = stage;
    }

    /// <summary>
    /// 저장된 강화 레벨에 따라 스탯을 복원합니다. (라운드 간 유지)
    /// </summary>
    private void RestoreUpgradeStats(int upgradeLevel)
    {
        if (!_pawnData.characterType.HasValue)
        {
            return;
        }

        // Recipe에서 강화 설정 가져오기
        var recipe = GameManager.Instance?.RecipeRepository?.GetRecipe(_pawnData.characterType.Value);
        if (recipe == null || recipe.upgradeSettings == null)
        {
            return;
        }

        // 각 강화 단계를 순차적으로 적용 (1단계부터 현재 단계까지)
        for (int stage = 1; stage <= upgradeLevel; stage++)
        {
            ApplyUpgradeStage(stage, recipe.upgradeSettings);
        }
    }

    /// <summary>
    /// 현재 강화 레벨을 반환합니다.
    /// </summary>
    public int GetCurrentUpgradeLevel()
    {
        return _currentUpgradeLevel;
    }
}


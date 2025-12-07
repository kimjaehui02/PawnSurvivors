using System.Linq;
using UnityEngine;

/// <summary>
/// Pawn의 레벨업을 관리하는 SubManager입니다.
/// 여러 레벨업 전략을 순차적으로 관리하며, 조건 달성 시 다음 레벨로 진행합니다.
/// </summary>
public class LevelUpSubManager : PawnSubManager
{
    private LevelUpStrategyBase[] _allStrategies;
    private int _currentStrategyIndex = 0;
    private int _currentLevel = 1;

    public override void SubStart()
    {
        // ExperienceData 생성 (레벨업 시스템이 있는 Pawn에만 필요)
        if (_pawnManager.PawnData != null)
        {
            _pawnManager.PawnData.GetOrCreateExperienceData();
        }

        _allStrategies = GetComponents<LevelUpStrategyBase>();

        // targetLevel 순으로 정렬
        _allStrategies = _allStrategies.OrderBy(s => s.targetLevel).ToArray();

        Debug.Log($"[LevelUp] {_pawnManager.name} - 총 {_allStrategies.Length}개의 레벨업 전략 발견", this);

        // 저장된 레벨과 경험치 복원
        RestoreLevelAndProgress();

        // 모든 전략 초기화
        foreach (var strategy in _allStrategies)
        {
            strategy.Init(_pawnManager);
            strategy.enabled = false;
            
            // 전략 타입과 설정값 로그
            string strategyType = strategy.GetType().Name;
            string strategyInfo = "";
            
            if (strategy is DamageDealtLevelUpStrategy damageStrategy)
            {
                strategyInfo = $"requiredDamage={damageStrategy.requiredDamage}";
            }
            else if (strategy is KillCountLevelUpStrategy killStrategy)
            {
                strategyInfo = $"requiredKills={killStrategy.requiredKills}";
            }
            else if (strategy is SurvivalTimeLevelUpStrategy timeStrategy)
            {
                strategyInfo = $"requiredSeconds={timeStrategy.requiredSeconds}";
            }
            
            Debug.Log($"[LevelUp] {_pawnManager.name} - 전략: {strategyType}, targetLevel={strategy.targetLevel}, {strategyInfo}", this);
        }

        // 저장된 레벨에 맞는 전략 활성화
        ActivateStrategyForCurrentLevel();
    }
    
    /// <summary>
    /// 저장된 레벨과 경험치를 복원합니다.
    /// </summary>
    private void RestoreLevelAndProgress()
    {
        if (_pawnManager?.PawnData == null) return;
        if (GameManager.Instance?.PawnPersistenceUseCase == null) return;
        
        // PawnPersistenceUseCase를 통해 저장된 영구 데이터 가져오기
        PawnSurvivors.Domain.PawnPersistentData persistentData = null;
        
        if (_pawnManager.PawnData.characterType.HasValue)
        {
            persistentData = GameManager.Instance.PawnPersistenceUseCase.GetPersistentData(
                _pawnManager.PawnData.characterType.Value, 
                _pawnManager.PawnData.playerIndex);
        }
        else if (!string.IsNullOrEmpty(_pawnManager.PawnData.recipeName))
        {
            persistentData = GameManager.Instance.PawnPersistenceUseCase.GetPersistentData(
                _pawnManager.PawnData.recipeName, 
                _pawnManager.PawnData.playerIndex);
        }
        
        if (persistentData == null) return;
        
        // 레벨 복원
        if (persistentData.currentLevel > 0)
        {
            _currentLevel = persistentData.currentLevel;
            Debug.Log($"[LevelUp] {_pawnManager.name} 레벨 복원: {_currentLevel}");
        }
        
        // 경험치는 이미 RestorePawnPersistentData에서 복원되었으므로 확인만
        if (_pawnManager.PawnData.experienceData != null)
        {
            Debug.Log($"[LevelUp] {_pawnManager.name} 경험치 복원됨: {_pawnManager.PawnData.experienceData.currentProgress}");
        }
    }
    
    /// <summary>
    /// 현재 레벨에 맞는 전략을 활성화합니다.
    /// </summary>
    private void ActivateStrategyForCurrentLevel()
    {
        if (_allStrategies == null || _allStrategies.Length == 0)
        {
            Debug.LogWarning($"[LevelUp] {_pawnManager.name}에 레벨업 전략이 없습니다.", this);
            return;
        }
        
        // 현재 레벨 이상의 첫 번째 전략 찾기
        _currentStrategyIndex = 0;
        for (int i = 0; i < _allStrategies.Length; i++)
        {
            if (_allStrategies[i].targetLevel > _currentLevel)
            {
                _currentStrategyIndex = i;
                break;
            }
            else if (_allStrategies[i].targetLevel == _currentLevel)
            {
                // 현재 레벨과 같은 전략이면 다음 전략으로
                _currentStrategyIndex = i + 1;
                break;
            }
        }
        
        // 마지막 전략을 넘어가면 최대 레벨
        if (_currentStrategyIndex >= _allStrategies.Length)
        {
            Debug.Log($"[LevelUp] {_pawnManager.name} 최대 레벨 도달: {_currentLevel}");
            return;
        }
        
        // 찾은 전략 활성화
        _allStrategies[_currentStrategyIndex].enabled = true;
        Debug.Log($"[LevelUp] {_pawnManager.name} 레벨 {_currentLevel} - 목표: 레벨 {_allStrategies[_currentStrategyIndex].targetLevel}, 진행도: {_allStrategies[_currentStrategyIndex].GetProgressText()}", this);
    }

    public override void SubUpdate()
    {
        // 모든 레벨업 완료
        if (_currentStrategyIndex >= _allStrategies.Length) return;

        var currentStrategy = _allStrategies[_currentStrategyIndex];

        // 조건 체크
        if (currentStrategy.CheckCondition())
        {
            // 레벨업 달성!
            _currentLevel = currentStrategy.targetLevel;
            currentStrategy.ApplyRewards(_pawnManager);
            currentStrategy.enabled = false;

            Debug.Log($"🎉 [LevelUp] {_pawnManager.name} 레벨 {_currentLevel} 달성! ({currentStrategy.GetProgressText()})", this);

            // 다음 전략으로 이동
            _currentStrategyIndex++;
            if (_currentStrategyIndex < _allStrategies.Length)
            {
                _allStrategies[_currentStrategyIndex].enabled = true;
                Debug.Log($"[LevelUp] {_pawnManager.name} 다음 목표: 레벨 {_allStrategies[_currentStrategyIndex].targetLevel}", this);
            }
            else
            {
                Debug.Log($"⭐ [LevelUp] {_pawnManager.name} 최대 레벨 도달!", this);
            }
        }
    }

    /// <summary>
    /// 현재 레벨을 반환합니다.
    /// </summary>
    public int GetCurrentLevel()
    {
        return _currentLevel;
    }

    /// <summary>
    /// 현재 진행 중인 레벨업 전략의 진행도를 반환합니다. (0~1)
    /// </summary>
    public float GetCurrentProgress()
    {
        if (_allStrategies == null || _allStrategies.Length == 0) return 0f;
        if (_currentStrategyIndex >= _allStrategies.Length) return 1f;
        if (_allStrategies[_currentStrategyIndex] == null) return 0f;
        return _allStrategies[_currentStrategyIndex].GetProgress();
    }

    /// <summary>
    /// 현재 진행 중인 레벨업 전략의 진행도 텍스트를 반환합니다.
    /// </summary>
    public string GetCurrentProgressText()
    {
        if (_allStrategies == null || _allStrategies.Length == 0) return "0/0";
        if (_currentStrategyIndex >= _allStrategies.Length) return "최대 레벨";
        if (_allStrategies[_currentStrategyIndex] == null) return "0/0";
        return _allStrategies[_currentStrategyIndex].GetProgressText();
    }

    /// <summary>
    /// 현재 레벨업 조건 설명을 반환합니다. (UI용)
    /// </summary>
    public string GetCurrentConditionDescription()
    {
        if (_currentStrategyIndex >= _allStrategies.Length) return "";
        return _allStrategies[_currentStrategyIndex].conditionDescription;
    }

    /// <summary>
    /// 현재 레벨업 보상 설명을 반환합니다. (UI용)
    /// </summary>
    public string GetCurrentRewardDescription()
    {
        if (_currentStrategyIndex >= _allStrategies.Length) return "";
        return _allStrategies[_currentStrategyIndex].rewardDescription;
    }
}


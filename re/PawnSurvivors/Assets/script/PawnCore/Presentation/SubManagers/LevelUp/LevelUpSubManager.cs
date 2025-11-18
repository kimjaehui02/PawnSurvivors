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
        _allStrategies = GetComponents<LevelUpStrategyBase>();

        // targetLevel 순으로 정렬
        _allStrategies = _allStrategies.OrderBy(s => s.targetLevel).ToArray();

        Debug.Log($"[LevelUp] {_pawnManager.name} - 총 {_allStrategies.Length}개의 레벨업 전략 발견", this);

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

        // 첫 번째 전략만 활성화
        if (_allStrategies.Length > 0)
        {
            _allStrategies[0].enabled = true;
            Debug.Log($"[LevelUp] {_pawnManager.name} 레벨 {_currentLevel} - 목표: 레벨 {_allStrategies[0].targetLevel}, 진행도: {_allStrategies[0].GetProgressText()}", this);
        }
        else
        {
            Debug.LogWarning($"[LevelUp] {_pawnManager.name}에 레벨업 전략이 없습니다.", this);
        }
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
        if (_currentStrategyIndex >= _allStrategies.Length) return 1f;
        return _allStrategies[_currentStrategyIndex].GetProgress();
    }

    /// <summary>
    /// 현재 진행 중인 레벨업 전략의 진행도 텍스트를 반환합니다.
    /// </summary>
    public string GetCurrentProgressText()
    {
        if (_currentStrategyIndex >= _allStrategies.Length) return "최대 레벨";
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


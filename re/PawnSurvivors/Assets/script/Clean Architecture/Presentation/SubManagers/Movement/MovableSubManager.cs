using System;
using System.Linq;
using UnityEngine;
using PawnSurvivors.Domain.Events;

/// <summary>
/// Pawn의 이동을 관리하는 SubManager입니다.
/// 여러 이동 전략을 관리하고, ChangeMovementStrategyEvent를 구독하여 전략을 변경합니다.
/// </summary>
public class MovableSubManager : PawnSubManager
{
    private MovementStrategyBase _currentStrategy;

    public override void SubStart()
    {
        var allStrategies = GetComponents<MovementStrategyBase>();

        // PawnManager에 대한 참조로 모든 전략 초기화
        foreach (var strategy in allStrategies)
        {
            strategy.Init(_pawnManager);
        }

        // 초기에 활성화된 전략 찾기 (JSON 레시피로 설정)
        _currentStrategy = allStrategies.FirstOrDefault(s => s.enabled);

        if (_currentStrategy == null && allStrategies.Length > 0)
        {
            // 활성화된 것이 없으면 첫 번째 것으로 기본 설정하고 활성화합니다.
            _currentStrategy = allStrategies[0];
            _currentStrategy.enabled = true;
            // 경고 제거: 코인처럼 의도적으로 기본 전략을 비활성화한 경우도 있으므로 경고가 불필요함
            // Debug.LogWarning($"MovableSubManager: No movement strategy was enabled by default by the recipe. Defaulting to and enabling ''{_currentStrategy.GetType().Name}'.", this);
        }
        
        // 현재 전략만 활성 상태인지 확인합니다. 실수로 여러 개가 활성화된 경우 중요합니다.
        foreach (var strategy in allStrategies)
        {
            if (strategy != _currentStrategy)
            {
                strategy.enabled = false;
            }
        }

        // 런타임 전략 변경을 처리하기 위해 이벤트 구독
        _pawnManager.Subscribe<ChangeMovementStrategyEvent>(HandleChangeStrategyEvent);
    }

    private void OnDestroy()
    {
        if (_pawnManager != null)
        {
            _pawnManager.Unsubscribe<ChangeMovementStrategyEvent>(HandleChangeStrategyEvent);
        }
    }

    public override void SubUpdate()
    {
        if (_currentStrategy != null)
        {
            _currentStrategy.Move();
        }
    }

    private void HandleChangeStrategyEvent(ChangeMovementStrategyEvent evt)
    {
        if (evt?.StrategyType == null) return;
        if (_currentStrategy != null && _currentStrategy.GetType() == evt.StrategyType) return; // 이미 활성 전략임

        // 이벤트가 처리되는 순간의 현재 구성 요소 목록을 가져옵니다.
        // 이렇게 하면 런타임에 구성 요소가 추가/제거될 때 시스템이 견고해집니다.
        var allStrategies = GetComponents<MovementStrategyBase>();
        var nextStrategy = allStrategies.FirstOrDefault(s => s.GetType() == evt.StrategyType);

        if (nextStrategy != null)
        {
            // 현재 전략 비활성화
            if (_currentStrategy != null)
            {
                _currentStrategy.enabled = false;
            }

            // 새 전략 활성화
            nextStrategy.enabled = true;
            _currentStrategy = nextStrategy;
            Debug.Log($"MovableSubManager: Movement strategy changed to ''{_currentStrategy.GetType().Name}'.");
        }
        else
        {
            Debug.LogWarning($"MovableSubManager: A request was made to switch to strategy ''{evt.StrategyType.Name}', but no such component is attached to this GameObject.", this);
        }
    }
}

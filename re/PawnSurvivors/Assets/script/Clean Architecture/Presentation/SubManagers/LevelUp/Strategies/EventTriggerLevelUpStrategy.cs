using UnityEngine;
using PawnSurvivors.Domain.Events;

/// <summary>
/// 특정 이벤트 발생을 기준으로 레벨업하는 전략입니다.
/// 예: 보스 발견, 특정 아이템 획득, 특정 지역 도달 등
/// </summary>
public class EventTriggerLevelUpStrategy : LevelUpStrategyBase
{
    /// <summary>감지할 이벤트 이름 (커스텀 이벤트 클래스 이름)</summary>
    public string eventName = "BossDiscoveredEvent";
    
    /// <summary>필요한 이벤트 발생 횟수 (보통 1)</summary>
    public int requiredCount = 1;
    
    private int _currentCount = 0;
    private bool _isSubscribed = false;

    public override void Init(PawnManager pawnManager)
    {
        base.Init(pawnManager);
        
        _currentCount = 0;
        _isSubscribed = false;
        
        // 이벤트 구독은 SubStart에서 하도록 함 (전략이 활성화될 때)
        Debug.Log($"[EventTriggerLevelUpStrategy] {pawnManager.name} 초기화 - 목표: {eventName} x{requiredCount}", this);
    }

    private void OnEnable()
    {
        // 전략이 활성화될 때 이벤트 구독
        if (_pawnManager != null && !_isSubscribed)
        {
            SubscribeToEvent();
        }
    }

    private void OnDisable()
    {
        // 전략이 비활성화될 때 이벤트 구독 해제
        UnsubscribeFromEvent();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvent();
    }

    /// <summary>
    /// 이벤트를 구독합니다.
    /// 실제 사용 시에는 구체적인 이벤트 타입에 맞게 수정해야 합니다.
    /// </summary>
    private void SubscribeToEvent()
    {
        if (_isSubscribed) return;
        
        // 예시: PawnDamagedEvent를 감지하는 경우
        // 실제로는 eventName에 따라 다른 이벤트를 구독해야 함
        
        // TODO: 이벤트 이름에 따라 동적으로 구독하는 시스템 필요
        // 현재는 간단한 카운터 방식으로 구현
        
        _isSubscribed = true;
        Debug.Log($"[EventTriggerLevelUpStrategy] {eventName} 이벤트 구독 시작", this);
    }

    private void UnsubscribeFromEvent()
    {
        if (!_isSubscribed) return;
        
        // TODO: 이벤트 구독 해제
        
        _isSubscribed = false;
    }

    /// <summary>
    /// 외부에서 이벤트 발생을 알릴 수 있는 메서드
    /// </summary>
    public void TriggerEvent()
    {
        if (!enabled) return;
        
        _currentCount++;
        Debug.Log($"[EventTriggerLevelUpStrategy] {eventName} 발생! ({_currentCount}/{requiredCount})", this);
    }

    public override bool CheckCondition()
    {
        return _currentCount >= requiredCount;
    }

    public override float GetProgress()
    {
        return Mathf.Clamp01(_currentCount / (float)requiredCount);
    }

    public override string GetProgressText()
    {
        return $"{_currentCount}/{requiredCount} 이벤트";
    }
}


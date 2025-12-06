using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PawnSurvivors.Managers;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// 게임오버 조건을 체크하고 처리하는 UseCase입니다.
    /// Strategy 패턴을 사용하여 다양한 게임오버 조건을 지원합니다.
    /// </summary>
    public class GameOverUseCase
    {
        private readonly List<IGameOverCondition> _conditions;
        private readonly StageFlowUseCase _stageFlowUseCase;
        
        public event Action OnGameOverTriggered;
        
        public GameOverUseCase(StageFlowUseCase stageFlowUseCase)
        {
            _stageFlowUseCase = stageFlowUseCase;
            _conditions = new List<IGameOverCondition>();
        }
        
        /// <summary>
        /// 게임오버 조건을 추가합니다.
        /// </summary>
        public void AddCondition(IGameOverCondition condition)
        {
            if (condition != null && !_conditions.Contains(condition))
            {
                _conditions.Add(condition);
                condition.OnConditionMet += HandleConditionMet;
            }
        }
        
        /// <summary>
        /// 게임오버 조건을 제거합니다.
        /// </summary>
        public void RemoveCondition(IGameOverCondition condition)
        {
            if (condition != null && _conditions.Contains(condition))
            {
                condition.OnConditionMet -= HandleConditionMet;
                _conditions.Remove(condition);
            }
        }
        
        /// <summary>
        /// 모든 게임오버 조건을 제거합니다.
        /// </summary>
        public void ClearConditions()
        {
            foreach (var condition in _conditions)
            {
                condition.OnConditionMet -= HandleConditionMet;
            }
            _conditions.Clear();
        }
        
        /// <summary>
        /// 게임오버 조건을 수동으로 체크합니다.
        /// </summary>
        public void CheckConditions()
        {
            foreach (var condition in _conditions)
            {
                // 조건 체크 후 만족되면 이벤트 발생 (이벤트 기반 처리)
                if (condition.Check())
                {
                    // 조건이 만족되면 이벤트를 발생시켜 HandleConditionMet가 호출되도록 함
                    // 하지만 현재 구조에서는 직접 호출이 더 효율적이므로 이벤트는 보조적으로 사용
                    HandleConditionMet(condition);
                    break; // 첫 번째 조건이 만족되면 중단
                }
            }
        }
        
        private void HandleConditionMet(IGameOverCondition condition)
        {
            LogManager.LogInfo(LogCategory.System, 
                $"[GameOverUseCase] 게임오버 조건 만족: {condition.GetType().Name}");
            
            OnGameOverTriggered?.Invoke();
            
            if (_stageFlowUseCase != null)
            {
                _stageFlowUseCase.FailStage();
            }
        }
    }
    
    /// <summary>
    /// 게임오버 조건 인터페이스입니다. (Strategy 패턴)
    /// </summary>
    public interface IGameOverCondition
    {
        event Action<IGameOverCondition> OnConditionMet;
        
        /// <summary>
        /// 게임오버 조건을 체크합니다.
        /// </summary>
        /// <returns>조건이 만족되면 true</returns>
        bool Check();
        
        /// <summary>
        /// 조건을 활성화합니다. (이벤트 구독 등)
        /// </summary>
        void Enable();
        
        /// <summary>
        /// 조건을 비활성화합니다. (이벤트 구독 해제 등)
        /// </summary>
        void Disable();
    }
    
    /// <summary>
    /// 모든 플레이어가 사망했을 때 게임오버가 되는 조건입니다.
    /// </summary>
    public class AllPlayersDeadCondition : IGameOverCondition
    {
        private readonly Func<List<GameObject>> _getPlayerPawns;
        private bool _isEnabled = false;
        
        public event Action<IGameOverCondition> OnConditionMet;
        
        public AllPlayersDeadCondition(Func<List<GameObject>> getPlayerPawns)
        {
            _getPlayerPawns = getPlayerPawns ?? throw new ArgumentNullException(nameof(getPlayerPawns));
        }
        
        public bool Check()
        {
            var playerPawns = _getPlayerPawns();
            if (playerPawns == null || playerPawns.Count == 0)
            {
                return false;
            }
            
            // 활성화된 플레이어가 있는지 확인
            int aliveCount = playerPawns.Count(pawn => pawn != null && pawn.activeInHierarchy);
            
            bool isConditionMet = aliveCount == 0;
            
            // 조건이 만족되면 이벤트 발생
            if (isConditionMet && _isEnabled)
            {
                OnConditionMet?.Invoke(this);
            }
            
            return isConditionMet;
        }
        
        public void Enable()
        {
            if (_isEnabled) return;
            
            _isEnabled = true;
            
            // PlayerController의 Pawn 사망 이벤트를 구독
            // 하지만 PlayerController에 직접 접근하는 것은 Clean Architecture에 맞지 않음
            // 대신 Check()를 주기적으로 호출하거나, 이벤트 기반으로 처리
            // 여기서는 Check()를 수동으로 호출하는 방식 사용
        }
        
        public void Disable()
        {
            if (!_isEnabled) return;
            
            _isEnabled = false;
        }
    }
}


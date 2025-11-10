using System;
using UnityEngine;

namespace PawnCore.Domain.Events
{
    /// <summary>
    /// 이동 전략을 변경해야 할 때 발행되는 이벤트입니다.
    /// </summary>
    public class ChangeMovementStrategyEvent
    {
        /// <summary>
        /// 변경할 이동 전략의 타입입니다.
        /// </summary>
        public Type StrategyType { get; }

        public ChangeMovementStrategyEvent(Type strategyType)
        {
            if (!typeof(MovementStrategyBase).IsAssignableFrom(strategyType))
            {
                Debug.LogError($"Type {strategyType.Name} is not a valid movement strategy.");
                return;
            }
            StrategyType = strategyType;
        }
    }
}

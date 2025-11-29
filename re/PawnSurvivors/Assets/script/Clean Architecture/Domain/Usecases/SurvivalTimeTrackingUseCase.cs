using UnityEngine;
using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// 생존 시간 추적을 담당하는 UseCase입니다.
    /// </summary>
    public class SurvivalTimeTrackingUseCase
    {
        private readonly ISessionDataRepository _repository;
        private readonly LifecycleManager _lifecycleManager;

        public SurvivalTimeTrackingUseCase(ISessionDataRepository repository, LifecycleManager lifecycleManager)
        {
            _repository = repository;
            _lifecycleManager = lifecycleManager;
        }

        /// <summary>
        /// 현재 생존 시간을 가져옵니다.
        /// </summary>
        /// <returns>생존 시간 (초)</returns>
        public float GetCurrentSurvivalTime()
        {
            if (_lifecycleManager != null)
            {
                return _lifecycleManager.GameTime;
            }
            return _repository.GetSurvivalTime();
        }

        /// <summary>
        /// 전략 활성화 시점 이후의 생존 시간을 가져옵니다.
        /// </summary>
        /// <param name="timeAtStart">전략 활성화 시점의 시간</param>
        /// <returns>활성화 이후 경과 시간</returns>
        public float GetElapsedTimeSinceStart(float timeAtStart)
        {
            float currentTime = GetCurrentSurvivalTime();
            return currentTime - timeAtStart;
        }
    }
}


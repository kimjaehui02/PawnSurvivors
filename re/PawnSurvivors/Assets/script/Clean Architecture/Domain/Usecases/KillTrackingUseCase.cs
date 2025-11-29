using PawnSurvivors.Domain.Repositories;
using PawnSurvivors.Domain;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// 적 처치 수 추적을 담당하는 UseCase입니다.
    /// </summary>
    public class KillTrackingUseCase
    {
        private readonly ISessionDataRepository _repository;

        public KillTrackingUseCase(ISessionDataRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 적 처치 수를 증가시킵니다.
        /// </summary>
        /// <param name="amount">증가량 (기본값: 1)</param>
        public void RecordKill(int amount = 1)
        {
            _repository.AddInt(SessionDataIntKey.EnemiesKilled, amount);
        }

        /// <summary>
        /// 전체 적 처치 수를 가져옵니다.
        /// </summary>
        /// <returns>전체 처치 수</returns>
        public int GetTotalKills()
        {
            return _repository.GetInt(SessionDataIntKey.EnemiesKilled, 0);
        }

        /// <summary>
        /// 전략 활성화 시점 이후의 처치 수를 가져옵니다.
        /// </summary>
        /// <param name="killsAtStart">전략 활성화 시점의 처치 수</param>
        /// <returns>활성화 이후 처치 수</returns>
        public int GetKillsSinceStart(int killsAtStart)
        {
            int currentKills = GetTotalKills();
            return currentKills - killsAtStart;
        }
    }
}


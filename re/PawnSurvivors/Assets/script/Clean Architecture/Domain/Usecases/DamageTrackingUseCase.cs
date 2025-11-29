using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// 데미지 추적을 담당하는 UseCase입니다.
    /// 플레이어가 입힌 데미지를 추적하고 저장합니다.
    /// </summary>
    public class DamageTrackingUseCase
    {
        private readonly ISessionDataRepository _repository;

        public DamageTrackingUseCase(ISessionDataRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 플레이어가 입힌 데미지를 기록합니다.
        /// </summary>
        /// <param name="playerId">플레이어 식별자 (PawnManager의 InstanceID 등)</param>
        /// <param name="damage">입힌 데미지 양</param>
        public void RecordDamage(int playerId, float damage)
        {
            string key = $"player_{playerId}_damageDealt";
            _repository.AddFloat(key, damage);
        }

        /// <summary>
        /// 플레이어가 입힌 총 데미지를 가져옵니다.
        /// </summary>
        /// <param name="playerId">플레이어 식별자</param>
        /// <returns>총 데미지</returns>
        public float GetTotalDamage(int playerId)
        {
            string key = $"player_{playerId}_damageDealt";
            return _repository.GetFloat(key, 0f);
        }

        /// <summary>
        /// 플레이어의 데미지를 초기화합니다.
        /// </summary>
        /// <param name="playerId">플레이어 식별자</param>
        public void ResetDamage(int playerId)
        {
            string key = $"player_{playerId}_damageDealt";
            _repository.SetFloat(key, 0f);
        }

        /// <summary>
        /// 전략 활성화 시점 이후의 데미지를 가져옵니다.
        /// </summary>
        /// <param name="playerId">플레이어 식별자</param>
        /// <param name="damageAtStart">전략 활성화 시점의 데미지</param>
        /// <returns>활성화 이후 입힌 데미지</returns>
        public float GetDamageSinceStart(int playerId, float damageAtStart)
        {
            float currentDamage = GetTotalDamage(playerId);
            return currentDamage - damageAtStart;
        }
    }
}


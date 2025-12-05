using PawnSurvivors.Domain.Repositories;
using PawnSurvivors.Domain;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// 재화(돈) 관리를 담당하는 UseCase입니다.
    /// 골드 획득, 소비, 잔액 확인 등을 처리합니다.
    /// </summary>
    public class CurrencyUseCase
    {
        private readonly ISessionDataRepository _repository;

        public CurrencyUseCase(ISessionDataRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 현재 골드를 가져옵니다.
        /// </summary>
        /// <returns>현재 골드 양</returns>
        public int GetGold()
        {
            return _repository.GetInt(SessionDataIntKey.Gold, 0);
        }

        /// <summary>
        /// 골드를 추가합니다.
        /// </summary>
        /// <param name="amount">추가할 골드 양</param>
        public void AddGold(int amount)
        {
            if (amount > 0)
            {
                _repository.AddInt(SessionDataIntKey.Gold, amount);
            }
        }

        /// <summary>
        /// 골드를 소비합니다.
        /// </summary>
        /// <param name="amount">소비할 골드 양</param>
        /// <returns>소비 성공 여부 (잔액 부족 시 false)</returns>
        public bool SpendGold(int amount)
        {
            if (amount <= 0) return false;

            int currentGold = GetGold();
            if (currentGold < amount)
            {
                return false; // 잔액 부족
            }

            _repository.AddInt(SessionDataIntKey.Gold, -amount);
            return true;
        }

        /// <summary>
        /// 골드를 설정합니다. (초기화 등)
        /// </summary>
        /// <param name="amount">설정할 골드 양</param>
        public void SetGold(int amount)
        {
            _repository.SetInt(SessionDataIntKey.Gold, amount);
        }

        /// <summary>
        /// 골드가 충분한지 확인합니다.
        /// </summary>
        /// <param name="amount">필요한 골드 양</param>
        /// <returns>충분한 골드를 가지고 있는지 여부</returns>
        public bool HasEnoughGold(int amount)
        {
            return GetGold() >= amount;
        }
    }
}


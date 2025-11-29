using System.Collections.Generic;
using PawnSurvivors.Data;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// 상점 관리를 담당하는 UseCase입니다.
    /// 상점 아이템 가져오기, 리롤 등의 로직을 처리합니다.
    /// </summary>
    public class ShopUseCase
    {
        private readonly ItemPoolUseCase _itemPoolUseCase;
        private readonly IItemRepository _itemRepository;
        private readonly CurrencyUseCase _currencyUseCase;

        public ShopUseCase(
            ItemPoolUseCase itemPoolUseCase,
            IItemRepository itemRepository,
            CurrencyUseCase currencyUseCase)
        {
            _itemPoolUseCase = itemPoolUseCase;
            _itemRepository = itemRepository;
            _currencyUseCase = currencyUseCase;
        }

        /// <summary>
        /// 상점에 표시할 아이템을 랜덤하게 가져옵니다.
        /// </summary>
        /// <param name="count">가져올 아이템 개수</param>
        /// <param name="excludeOwned">보유한 아이템 제외 여부</param>
        /// <param name="excludeItemIds">제외할 아이템 ID 목록 (잠긴 슬롯의 아이템 등)</param>
        /// <returns>선택된 아이템 목록</returns>
        public List<ItemData> GetShopItems(int count, bool excludeOwned = false, List<string> excludeItemIds = null)
        {
            return _itemPoolUseCase.GetRandomShopItems(count, excludeOwned, _itemRepository, excludeItemIds);
        }

        /// <summary>
        /// 상점을 리롤합니다. (골드를 소비하고 새 아이템을 가져옵니다)
        /// </summary>
        /// <param name="cost">리롤 비용</param>
        /// <param name="itemCount">가져올 아이템 개수</param>
        /// <param name="excludeOwned">보유한 아이템 제외 여부</param>
        /// <param name="excludeItemIds">제외할 아이템 ID 목록 (잠긴 슬롯의 아이템 등)</param>
        /// <returns>리롤 성공 여부 및 새 아이템 목록 (성공 시 아이템 목록, 실패 시 null)</returns>
        public List<ItemData> RerollShop(int cost, int itemCount, bool excludeOwned = false, List<string> excludeItemIds = null)
        {
            // 골드 확인 및 소비
            if (!_currencyUseCase.SpendGold(cost))
            {
                return null; // 골드 부족
            }

            // 새 아이템 가져오기
            return GetShopItems(itemCount, excludeOwned, excludeItemIds);
        }

        /// <summary>
        /// 리롤 비용을 지불할 수 있는지 확인합니다.
        /// </summary>
        /// <param name="cost">리롤 비용</param>
        /// <returns>리롤 가능 여부</returns>
        public bool CanReroll(int cost)
        {
            return _currencyUseCase.HasEnoughGold(cost);
        }
    }
}


using System.Collections.Generic;
using PawnSurvivors.Data;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// 아이템 관리를 담당하는 UseCase입니다.
    /// 아이템 구매, 장착, 전역 적용 등을 처리합니다.
    /// </summary>
    public class ItemManagementUseCase
    {
        private readonly IItemRepository _itemRepository;
        private readonly ISessionDataRepository _sessionRepository;
        private readonly CurrencyUseCase _currencyUseCase;

        public ItemManagementUseCase(
            IItemRepository itemRepository,
            ISessionDataRepository sessionRepository,
            CurrencyUseCase currencyUseCase)
        {
            _itemRepository = itemRepository;
            _sessionRepository = sessionRepository;
            _currencyUseCase = currencyUseCase;
        }

        /// <summary>
        /// 상점에서 아이템을 구매합니다.
        /// </summary>
        /// <param name="itemData">아이템 데이터</param>
        /// <returns>구매 성공 여부</returns>
        public bool BuyItem(ItemData itemData)
        {
            if (itemData == null || string.IsNullOrEmpty(itemData.itemId))
            {
                return false;
            }

            // 골드 확인 및 소비
            if (!_currencyUseCase.SpendGold(itemData.cost))
            {
                return false; // 골드 부족
            }

            // 아이템 저장
            _itemRepository.SaveItem(itemData);

            // 전역 아이템인 경우 즉시 적용
            if (itemData.itemType == ItemType.Global)
            {
                ApplyGlobalItem(itemData.itemId);
            }

            return true;
        }

        /// <summary>
        /// 특정 Pawn에 아이템을 장착합니다.
        /// </summary>
        /// <param name="itemId">아이템 ID</param>
        /// <param name="playerIndex">플레이어 인덱스</param>
        /// <returns>장착 성공 여부</returns>
        public bool EquipItemToPawn(string itemId, int playerIndex)
        {
            if (!_itemRepository.HasItem(itemId))
            {
                return false; // 아이템을 보유하지 않음
            }

            var itemData = _itemRepository.GetItemData(itemId);
            if (itemData == null || itemData.itemType != ItemType.Equipped)
            {
                return false; // 장착 가능한 아이템이 아님
            }

            _itemRepository.EquipItemToPawn(itemId, playerIndex);
            ApplyItemEffectToPawn(itemId, playerIndex);
            return true;
        }

        /// <summary>
        /// 특정 Pawn에서 아이템을 해제합니다.
        /// </summary>
        /// <param name="itemId">아이템 ID</param>
        /// <param name="playerIndex">플레이어 인덱스</param>
        /// <returns>해제 성공 여부</returns>
        public bool UnequipItemFromPawn(string itemId, int playerIndex)
        {
            if (!_itemRepository.IsItemEquippedToPawn(itemId, playerIndex))
            {
                return false; // 장착되어 있지 않음
            }

            RemoveItemEffectFromPawn(itemId, playerIndex);
            _itemRepository.UnequipItemFromPawn(itemId, playerIndex);
            return true;
        }

        /// <summary>
        /// 전역 아이템 목록을 가져옵니다.
        /// </summary>
        public List<ItemData> GetGlobalItems()
        {
            return _itemRepository.GetGlobalItems();
        }

        /// <summary>
        /// 특정 Pawn에 장착된 아이템 목록을 가져옵니다.
        /// </summary>
        public List<ItemData> GetEquippedItems(int playerIndex)
        {
            return _itemRepository.GetEquippedItems(playerIndex);
        }

        /// <summary>
        /// 아이템을 보유하고 있는지 확인합니다.
        /// </summary>
        public bool HasItem(string itemId)
        {
            return _itemRepository.HasItem(itemId);
        }

        /// <summary>
        /// 특정 Pawn에 아이템이 장착되어 있는지 확인합니다.
        /// </summary>
        public bool IsItemEquippedToPawn(string itemId, int playerIndex)
        {
            return _itemRepository.IsItemEquippedToPawn(itemId, playerIndex);
        }

        /// <summary>
        /// 아이템을 구매하고 특정 Pawn에 장착합니다. (원자적 작업)
        /// 구매와 장착이 모두 성공해야 하며, 장착 실패 시 구매도 롤백됩니다.
        /// </summary>
        /// <param name="itemData">아이템 데이터</param>
        /// <param name="playerIndex">플레이어 인덱스</param>
        /// <returns>구매 및 장착 성공 여부</returns>
        public bool BuyAndEquipItem(ItemData itemData, int playerIndex)
        {
            // 1. 입력 검증
            if (itemData == null || string.IsNullOrEmpty(itemData.itemId))
            {
                return false;
            }

            // 2. 장착 가능한 아이템인지 확인
            if (itemData.itemType != ItemType.Equipped)
            {
                return false; // 장착 가능한 아이템이 아님
            }

            // 3. 골드 확인 (차감 전)
            if (!_currencyUseCase.HasEnoughGold(itemData.cost))
            {
                return false; // 골드 부족
            }

            // 4. 골드 차감
            if (!_currencyUseCase.SpendGold(itemData.cost))
            {
                return false; // 골드 소비 실패 (이미 확인했지만 안전장치)
            }

            // 5. 아이템 저장
            _itemRepository.SaveItem(itemData);

            // 6. 장착 시도
            bool equipSuccess = EquipItemToPawn(itemData.itemId, playerIndex);
            
            if (!equipSuccess)
            {
                // 장착 실패 시 롤백: 아이템 제거 및 골드 환불
                _itemRepository.RemoveItemStack(itemData.itemId, 1);
                _currencyUseCase.AddGold(itemData.cost);
                return false;
            }

            // 7. 성공
            return true;
        }

        // ========================================
        // 내부 메서드 (효과 적용)
        // ========================================

        /// <summary>
        /// 전역 아이템 효과를 적용합니다.
        /// 런타임 계산 방식을 사용하므로 실제로는 PawnStatCalculator가 처리합니다.
        /// </summary>
        private void ApplyGlobalItem(string itemId)
        {
            // 런타임 계산 방식이므로 여기서는 로그만 남깁니다.
            // 실제 효과는 PawnStatCalculator에서 계산됩니다.
            UnityEngine.Debug.Log($"[ItemManagementUseCase] 전역 아이템 적용: {itemId}");
        }

        /// <summary>
        /// 특정 Pawn에 아이템 효과를 적용합니다.
        /// 런타임 계산 방식을 사용하므로 실제로는 PawnStatCalculator가 처리합니다.
        /// </summary>
        private void ApplyItemEffectToPawn(string itemId, int playerIndex)
        {
            // 런타임 계산 방식이므로 여기서는 로그만 남깁니다.
            // 실제 효과는 PawnStatCalculator에서 계산됩니다.
            UnityEngine.Debug.Log($"[ItemManagementUseCase] 아이템 효과 적용: {itemId} → Pawn {playerIndex}");
        }

        /// <summary>
        /// 특정 Pawn에서 아이템 효과를 제거합니다.
        /// 런타임 계산 방식을 사용하므로 실제로는 PawnStatCalculator가 처리합니다.
        /// </summary>
        private void RemoveItemEffectFromPawn(string itemId, int playerIndex)
        {
            // 런타임 계산 방식이므로 여기서는 로그만 남깁니다.
            // 실제 효과는 PawnStatCalculator에서 계산됩니다.
            UnityEngine.Debug.Log($"[ItemManagementUseCase] 아이템 효과 제거: {itemId} → Pawn {playerIndex}");
        }
    }
}


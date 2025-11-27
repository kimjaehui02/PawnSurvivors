using System.Collections.Generic;
using System.Linq;
using PawnCore.Domain;
using PawnSurvivors.Data;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// Pawn의 실제 스탯을 계산하는 UseCase입니다.
    /// 기본값 + 전역 아이템 효과 + 장착 아이템 효과를 합산하여 반환합니다.
    /// </summary>
    public class PawnStatCalculator
    {
        private readonly IItemRepository _itemRepository;

        public PawnStatCalculator(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        // ========================================
        // 주요 스탯 계산 메서드
        // ========================================

        /// <summary>
        /// 실제 데미지를 계산합니다. (기본값 + 아이템 효과)
        /// </summary>
        public float GetEffectiveDamage(PawnData pawnData)
        {
            if (pawnData?.combatData == null) return 0f;
            
            float baseDamage = pawnData.combatData.damage;
            float itemBonus = CalculateItemStatBonus(pawnData, StatKey.Damage);
            
            return baseDamage + itemBonus;
        }

        /// <summary>
        /// 실제 최대 체력을 계산합니다. (기본값 + 아이템 효과)
        /// </summary>
        public float GetEffectiveMaxHealth(PawnData pawnData)
        {
            if (pawnData?.healthData == null) return 0f;
            
            float baseHealth = pawnData.healthData.maxHealth;
            float itemBonus = CalculateItemStatBonus(pawnData, StatKey.MaxHealth);
            
            return baseHealth + itemBonus;
        }

        /// <summary>
        /// 실제 이동 속도를 계산합니다. (기본값 + 아이템 효과)
        /// </summary>
        public float GetEffectiveMoveSpeed(PawnData pawnData)
        {
            if (pawnData?.movableData?.keyboardMovement == null) return 0f;
            
            float baseSpeed = pawnData.movableData.keyboardMovement.moveSpeed;
            float itemBonus = CalculateItemStatBonus(pawnData, StatKey.MoveSpeed);
            
            return baseSpeed + itemBonus;
        }

        /// <summary>
        /// 실제 공격 속도를 계산합니다. (기본값 + 아이템 효과)
        /// </summary>
        public float GetEffectiveFireRate(PawnData pawnData)
        {
            if (pawnData?.combatData == null) return 0f;
            
            float baseFireRate = pawnData.combatData.fireRate;
            float itemBonus = CalculateItemStatBonus(pawnData, StatKey.FireRate);
            
            return baseFireRate + itemBonus;
        }

        /// <summary>
        /// 실제 투사체 속도를 계산합니다. (기본값 + 아이템 효과)
        /// </summary>
        public float GetEffectiveProjectileSpeed(PawnData pawnData)
        {
            if (pawnData?.combatData == null) return 0f;
            
            float baseSpeed = pawnData.combatData.projectileSpeed;
            float itemBonus = CalculateItemStatBonus(pawnData, StatKey.ProjectileSpeed);
            
            return baseSpeed + itemBonus;
        }

        // ========================================
        // 내부 헬퍼 메서드
        // ========================================

        /// <summary>
        /// 특정 스탯에 대한 아이템 보너스를 계산합니다.
        /// 전역 아이템 + 장착 아이템의 효과를 합산합니다.
        /// 스택 개수만큼 효과가 곱해집니다.
        /// </summary>
        private float CalculateItemStatBonus(PawnData pawnData, StatKey statKey)
        {
            float totalBonus = 0f;

            // 전역 아이템 효과 (스택 반영)
            var globalItems = _itemRepository.GetGlobalItems();
            foreach (var item in globalItems)
            {
                int stackCount = _itemRepository.GetItemStackCount(item.itemId);
                totalBonus += item.GetStatModifier(statKey) * stackCount;
            }

            // 장착 아이템 효과 (스택 반영)
            if (pawnData.playerIndex >= 0)
            {
                var equippedItems = _itemRepository.GetEquippedItems(pawnData.playerIndex);
                foreach (var item in equippedItems)
                {
                    int stackCount = _itemRepository.GetItemStackCount(item.itemId);
                    totalBonus += item.GetStatModifier(statKey) * stackCount;
                }
            }

            return totalBonus;
        }

        /// <summary>
        /// 특정 업그레이드에 대한 아이템 보너스를 계산합니다.
        /// 전역 아이템 + 장착 아이템의 효과를 합산합니다.
        /// 스택 개수만큼 효과가 곱해집니다.
        /// </summary>
        private int CalculateItemUpgradeBonus(PawnData pawnData, UpgradeKey upgradeKey)
        {
            int totalBonus = 0;

            // 전역 아이템 효과 (스택 반영)
            var globalItems = _itemRepository.GetGlobalItems();
            foreach (var item in globalItems)
            {
                int stackCount = _itemRepository.GetItemStackCount(item.itemId);
                totalBonus += item.GetUpgradeModifier(upgradeKey) * stackCount;
            }

            // 장착 아이템 효과 (스택 반영)
            if (pawnData.playerIndex >= 0)
            {
                var equippedItems = _itemRepository.GetEquippedItems(pawnData.playerIndex);
                foreach (var item in equippedItems)
                {
                    int stackCount = _itemRepository.GetItemStackCount(item.itemId);
                    totalBonus += item.GetUpgradeModifier(upgradeKey) * stackCount;
                }
            }

            return totalBonus;
        }
    }
}


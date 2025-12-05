using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PawnSurvivors.Domain;
using PawnSurvivors.Data;
using PawnSurvivors.Domain.Repositories;
using PawnSurvivors.Managers;

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
        /// 실제 데미지를 계산합니다. (기본값 + 아이템 효과) * 아이템 곱셈 효과
        /// </summary>
        public float GetEffectiveDamage(PawnData pawnData)
        {
            if (pawnData?.combatData == null) return 0f;
            
            float baseDamage = pawnData.combatData.damage;
            float itemBonus = CalculateItemStatBonus(pawnData, StatKey.Damage);
            float itemMultiplier = CalculateItemStatMultiplier(pawnData, StatKey.Damage);
            
            float result = (baseDamage + itemBonus) * itemMultiplier;
            
            // 임시 디버그 로그 (테스트용 - 문제 해결 후 제거)
            if (Time.frameCount % 60 == 0) // 1초마다 한 번씩만 로그
            {
                LogManager.LogDebug(LogCategory.System, $"GetEffectiveDamage: base={baseDamage}, bonus={itemBonus}, multiplier={itemMultiplier}, result={result}, playerIndex={pawnData.playerIndex}");
            }
            
            return result;
        }

        /// <summary>
        /// 실제 최대 체력을 계산합니다. (기본값 + 아이템 효과) * 아이템 곱셈 효과
        /// </summary>
        public float GetEffectiveMaxHealth(PawnData pawnData)
        {
            if (pawnData?.healthData == null) return 0f;
            
            float baseHealth = pawnData.healthData.maxHealth;
            float itemBonus = CalculateItemStatBonus(pawnData, StatKey.MaxHealth);
            float itemMultiplier = CalculateItemStatMultiplier(pawnData, StatKey.MaxHealth);
            
            return (baseHealth + itemBonus) * itemMultiplier;
        }

        /// <summary>
        /// 실제 이동 속도를 계산합니다. (기본값 + 아이템 효과) * 아이템 곱셈 효과
        /// </summary>
        public float GetEffectiveMoveSpeed(PawnData pawnData)
        {
            if (pawnData?.movableData?.keyboardMovement == null) return 0f;
            
            float baseSpeed = pawnData.movableData.keyboardMovement.moveSpeed;
            float itemBonus = CalculateItemStatBonus(pawnData, StatKey.MoveSpeed);
            float itemMultiplier = CalculateItemStatMultiplier(pawnData, StatKey.MoveSpeed);
            
            return (baseSpeed + itemBonus) * itemMultiplier;
        }

        /// <summary>
        /// 실제 공격 속도를 계산합니다. (기본값 + 아이템 효과) * 아이템 곱셈 효과
        /// </summary>
        public float GetEffectiveFireRate(PawnData pawnData)
        {
            if (pawnData?.combatData == null) return 0f;
            
            float baseFireRate = pawnData.combatData.fireRate;
            float itemBonus = CalculateItemStatBonus(pawnData, StatKey.FireRate);
            float itemMultiplier = CalculateItemStatMultiplier(pawnData, StatKey.FireRate);
            
            float result = (baseFireRate + itemBonus) * itemMultiplier;
            
            // 디버그 로그 (문제 해결용)
            if (Time.frameCount % 60 == 0)
            {
                LogManager.LogDebug(LogCategory.System, 
                    $"[공격속도] playerIndex={pawnData.playerIndex}, base={baseFireRate}, bonus={itemBonus}, multiplier={itemMultiplier}, result={result}");
            }
            
            return result;
        }

        /// <summary>
        /// 실제 투사체 속도를 계산합니다. (기본값 + 아이템 효과) * 아이템 곱셈 효과
        /// </summary>
        public float GetEffectiveProjectileSpeed(PawnData pawnData)
        {
            if (pawnData?.combatData == null) return 0f;
            
            float baseSpeed = pawnData.combatData.projectileSpeed;
            float itemBonus = CalculateItemStatBonus(pawnData, StatKey.ProjectileSpeed);
            float itemMultiplier = CalculateItemStatMultiplier(pawnData, StatKey.ProjectileSpeed);
            
            return (baseSpeed + itemBonus) * itemMultiplier;
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
            // 임시 디버그 로그 (테스트용)
            if (Time.frameCount % 60 == 0 && pawnData.playerIndex == 0)
            {
                LogManager.LogDebug(LogCategory.System, $"CalculateItemStatBonus: globalItems count={globalItems.Count}, playerIndex={pawnData.playerIndex}, statKey={statKey}");
            }
            foreach (var item in globalItems)
            {
                int stackCount = _itemRepository.GetItemStackCount(item.itemId);
                float modifier = item.GetStatModifier(statKey);
                totalBonus += modifier * stackCount;
                if (Time.frameCount % 60 == 0 && pawnData.playerIndex == 0)
                {
                    LogManager.LogDebug(LogCategory.System, $"Global item: {item.itemId}, modifier={modifier}, stack={stackCount}, totalBonus={totalBonus}");
                }
            }

            // 장착 아이템 효과 (스택 반영)
            // playerIndex가 -1인 경우 (투사체 등), Owner의 playerIndex를 사용
            int effectivePlayerIndex = pawnData.playerIndex;
            if (effectivePlayerIndex < 0)
            {
                // 투사체의 경우 Owner의 playerIndex를 사용
                // 이는 PawnManager에서 Owner를 통해 접근할 수 없으므로,
                // CreationManager에서 이미 playerIndex를 설정해야 함
                // 여기서는 playerIndex < 0이면 장착 아이템을 적용하지 않음
                return totalBonus; // 장착 아이템 적용 안 함
            }
            
            var equippedItems = _itemRepository.GetEquippedItems(effectivePlayerIndex);
            if (Time.frameCount % 60 == 0)
            {
                LogManager.LogDebug(LogCategory.System, $"[장착아이템] playerIndex={effectivePlayerIndex}, statKey={statKey}, 아이템개수={equippedItems.Count}");
            }
            // 같은 아이템이 여러 번 장착된 경우를 처리하기 위해 그룹화
            var itemGroups = equippedItems.GroupBy(item => item.itemId);
            foreach (var group in itemGroups)
            {
                var item = group.First();
                // 해당 캐릭터에 장착된 같은 아이템의 개수 (스택)
                int stackCount = group.Count();
                float modifier = item.GetStatModifier(statKey);
                totalBonus += modifier * stackCount;
                if (Time.frameCount % 60 == 0)
                {
                    LogManager.LogDebug(LogCategory.System, $"[장착아이템] playerIndex={effectivePlayerIndex}, 아이템={item.itemId}, modifier={modifier}, stack={stackCount}, totalBonus={totalBonus}");
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
                // 같은 아이템이 여러 번 장착된 경우를 처리하기 위해 그룹화
                var itemGroups = equippedItems.GroupBy(item => item.itemId);
                foreach (var group in itemGroups)
                {
                    var item = group.First();
                    // 해당 캐릭터에 장착된 같은 아이템의 개수 (스택)
                    int stackCount = group.Count();
                    totalBonus += item.GetUpgradeModifier(upgradeKey) * stackCount;
                }
            }

            return totalBonus;
        }

        /// <summary>
        /// 특정 스탯에 대한 아이템 곱셈 효과를 계산합니다.
        /// 전역 아이템 + 장착 아이템의 곱셈 효과를 누적 곱합니다.
        /// 스택 개수만큼 효과가 곱해집니다.
        /// </summary>
        private float CalculateItemStatMultiplier(PawnData pawnData, StatKey statKey)
        {
            float totalMultiplier = 1f;

            // 전역 아이템 곱셈 효과 (스택 반영)
            var globalItems = _itemRepository.GetGlobalItems();
            foreach (var item in globalItems)
            {
                float multiplier = item.GetStatMultiplier(statKey);
                if (multiplier != 1f) // 1이 아닌 경우만 곱하기
                {
                    int stackCount = _itemRepository.GetItemStackCount(item.itemId);
                    // 곱셈은 스택마다 곱하기 (예: 2배 아이템 2개 = 2 * 2 = 4배)
                    for (int i = 0; i < stackCount; i++)
                    {
                        totalMultiplier *= multiplier;
                    }
                    if (Time.frameCount % 60 == 0 && pawnData.playerIndex == 0)
                    {
                        LogManager.LogDebug(LogCategory.System, $"Global multiplier: {item.itemId}, multiplier={multiplier}, stack={stackCount}, totalMultiplier={totalMultiplier}");
                    }
                }
            }

            // 장착 아이템 곱셈 효과 (스택 반영)
            int effectivePlayerIndex = pawnData.playerIndex;
            if (effectivePlayerIndex < 0)
            {
                // 투사체 등은 장착 아이템 적용 안 함
                return totalMultiplier;
            }
            
            var equippedItems = _itemRepository.GetEquippedItems(effectivePlayerIndex);
            if (Time.frameCount % 60 == 0)
            {
                LogManager.LogDebug(LogCategory.System, $"[장착아이템곱셈] playerIndex={effectivePlayerIndex}, statKey={statKey}, 아이템개수={equippedItems.Count}");
            }
            // 같은 아이템이 여러 번 장착된 경우를 처리하기 위해 그룹화
            var itemGroups = equippedItems.GroupBy(item => item.itemId);
            foreach (var group in itemGroups)
            {
                var item = group.First();
                float multiplier = item.GetStatMultiplier(statKey);
                if (multiplier != 1f) // 1이 아닌 경우만 곱하기
                {
                    // 해당 캐릭터에 장착된 같은 아이템의 개수 (스택)
                    int stackCount = group.Count();
                    // 곱셈은 스택마다 곱하기 (예: 1.2배 아이템 2개 = 1.2 * 1.2 = 1.44배)
                    for (int i = 0; i < stackCount; i++)
                    {
                        totalMultiplier *= multiplier;
                    }
                    if (Time.frameCount % 60 == 0)
                    {
                        LogManager.LogDebug(LogCategory.System, $"[장착아이템곱셈] playerIndex={effectivePlayerIndex}, 아이템={item.itemId}, multiplier={multiplier}, stack={stackCount}, totalMultiplier={totalMultiplier}");
                    }
                }
            }

            return totalMultiplier;
        }
    }
}


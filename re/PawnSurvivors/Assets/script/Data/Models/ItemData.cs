using System;
using System.Collections.Generic;
using UnityEngine;
using PawnSurvivors.Domain;

namespace PawnSurvivors.Data
{
    /// <summary>
    /// 아이템의 데이터 모델입니다.
    /// 아이템의 기본 정보와 효과를 저장합니다.
    /// </summary>
    [Serializable]
    public class ItemData
    {
        /// <summary>아이템 고유 ID</summary>
        public string itemId;
        
        /// <summary>아이템 이름</summary>
        public string itemName;
        
        /// <summary>아이템 타입 (Global 또는 Equipped)</summary>
        public ItemType itemType;
        
        /// <summary>아이템 설명</summary>
        public string description;
        
        /// <summary>아이템 가격 (상점에서 구매 시)</summary>
        public int cost;
        
        /// <summary>아이템 아이콘 경로 (Resources 경로)</summary>
        public string iconPath;
        
        /// <summary>
        /// 스탯 수정자 (enum 기반, int로 직렬화)
        /// 예: statModifiers[StatKey.Damage] = 10.0f (데미지 +10)
        /// </summary>
        public Dictionary<int, float> statModifiers = new Dictionary<int, float>();
        
        /// <summary>
        /// 업그레이드 수정자 (enum 기반, int로 직렬화)
        /// 예: upgradeModifiers[UpgradeKey.AttackSpeed] = 1 (공격 속도 업그레이드 +1 레벨)
        /// </summary>
        public Dictionary<int, int> upgradeModifiers = new Dictionary<int, int>();
        
        /// <summary>
        /// 스탯 곱셈 수정자 (enum 기반, int로 직렬화)
        /// 예: statMultipliers[StatKey.Damage] = 2.0f (데미지 2배)
        /// </summary>
        public Dictionary<int, float> statMultipliers = new Dictionary<int, float>();
        
        /// <summary>
        /// 아이템 기능 타입 (기능이 있는 아이템인 경우)
        /// 예: "OnKill", "OnHit", "OnDamageTaken" 등
        /// </summary>
        public string itemFunctionType;
        
        /// <summary>
        /// 아이템 기능 파라미터 (기능이 있는 아이템의 추가 데이터)
        /// </summary>
        public Dictionary<string, float> functionParameters = new Dictionary<string, float>();
        
        // ========================================
        // 헬퍼 메서드 (enum 변환)
        // ========================================
        
        /// <summary>스탯 수정자 가져오기</summary>
        public float GetStatModifier(StatKey key)
        {
            return statModifiers.ContainsKey((int)key) ? statModifiers[(int)key] : 0f;
        }
        
        /// <summary>스탯 수정자 설정</summary>
        public void SetStatModifier(StatKey key, float value)
        {
            statModifiers[(int)key] = value;
        }
        
        /// <summary>업그레이드 수정자 가져오기</summary>
        public int GetUpgradeModifier(UpgradeKey key)
        {
            return upgradeModifiers.ContainsKey((int)key) ? upgradeModifiers[(int)key] : 0;
        }
        
        /// <summary>업그레이드 수정자 설정</summary>
        public void SetUpgradeModifier(UpgradeKey key, int value)
        {
            upgradeModifiers[(int)key] = value;
        }
        
        /// <summary>스탯 곱셈 수정자 가져오기</summary>
        public float GetStatMultiplier(StatKey key)
        {
            return statMultipliers.ContainsKey((int)key) ? statMultipliers[(int)key] : 1f;
        }
        
        /// <summary>스탯 곱셈 수정자 설정</summary>
        public void SetStatMultiplier(StatKey key, float value)
        {
            statMultipliers[(int)key] = value;
        }
        
        /// <summary>
        /// 기능이 있는 아이템인지 확인합니다.
        /// </summary>
        public bool HasFunction()
        {
            return !string.IsNullOrEmpty(itemFunctionType);
        }
        
        /// <summary>
        /// 기능 파라미터를 가져옵니다.
        /// </summary>
        public float GetFunctionParameter(string key, float defaultValue = 0f)
        {
            return functionParameters.ContainsKey(key) ? functionParameters[key] : defaultValue;
        }
    }
}


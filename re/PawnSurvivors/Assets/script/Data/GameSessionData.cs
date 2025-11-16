using System;
using System.Collections.Generic;
using UnityEngine;

namespace PawnSurvivors.Data
{
    /// <summary>
    /// 현재 게임 세션의 모든 런타임 데이터를 관리합니다.
    /// 게임 시작 시 초기화되고, 게임 종료 시 리셋됩니다.
    /// 
    /// 유연한 Dictionary 기반 구조로 나중에 필요한 데이터를 쉽게 추가할 수 있습니다.
    /// </summary>
    [Serializable]
    public class GameSessionData
    {
        // ========================================
        // 핵심 스테이지 정보 (고정)
        // ========================================
        
        /// <summary>현재 스테이지 이름</summary>
        public string currentStageName = "";
        
        /// <summary>게임 시작 시간 (Time.time)</summary>
        public float gameStartTime = 0f;
        
        // ========================================
        // 유연한 데이터 저장소 (확장 가능)
        // ========================================
        
        /// <summary>
        /// 정수 값 데이터 (처치 수, 레벨, 골드 등)
        /// 예: "enemiesKilled", "level", "gold", "wave"
        /// </summary>
        public Dictionary<string, int> intValues = new Dictionary<string, int>();
        
        /// <summary>
        /// 실수 값 데이터 (경과 시간, 데미지 등)
        /// 예: "damageDealt", "damageTaken", "survivalTime"
        /// </summary>
        public Dictionary<string, float> floatValues = new Dictionary<string, float>();
        
        /// <summary>
        /// 문자열 값 데이터 (상태, 설정 등)
        /// 예: "lastUpgrade", "difficulty"
        /// </summary>
        public Dictionary<string, string> stringValues = new Dictionary<string, string>();
        
        /// <summary>
        /// 카운터 맵 (업그레이드 레벨, 아이템 개수 등)
        /// 예: upgrades["AttackSpeed"] = 3, items["HealthPotion"] = 5
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> counterMaps = new Dictionary<string, Dictionary<string, int>>();
        
        /// <summary>
        /// 플래그 세트 (획득한 아이템, 달성한 업적 등)
        /// 예: flags["collectedItems"].Contains("Sword"), flags["achievements"].Contains("FirstKill")
        /// </summary>
        public Dictionary<string, HashSet<string>> flagSets = new Dictionary<string, HashSet<string>>();
        
        // ========================================
        // 기본 메서드
        // ========================================
        
        /// <summary>
        /// 세션 데이터를 초기화합니다. (새 게임 시작 시)
        /// </summary>
        public void Reset()
        {
            currentStageName = "";
            gameStartTime = Time.time;
            
            intValues.Clear();
            floatValues.Clear();
            stringValues.Clear();
            counterMaps.Clear();
            flagSets.Clear();
        }
        
        // ========================================
        // 정수 값 헬퍼
        // ========================================
        
        /// <summary>정수 값 가져오기 (없으면 기본값 0)</summary>
        public int GetInt(string key, int defaultValue = 0)
        {
            return intValues.ContainsKey(key) ? intValues[key] : defaultValue;
        }
        
        /// <summary>정수 값 설정</summary>
        public void SetInt(string key, int value)
        {
            intValues[key] = value;
        }
        
        /// <summary>정수 값 증가</summary>
        public void AddInt(string key, int amount = 1)
        {
            intValues[key] = GetInt(key) + amount;
        }
        
        // ========================================
        // 실수 값 헬퍼
        // ========================================
        
        /// <summary>실수 값 가져오기 (없으면 기본값 0)</summary>
        public float GetFloat(string key, float defaultValue = 0f)
        {
            return floatValues.ContainsKey(key) ? floatValues[key] : defaultValue;
        }
        
        /// <summary>실수 값 설정</summary>
        public void SetFloat(string key, float value)
        {
            floatValues[key] = value;
        }
        
        /// <summary>실수 값 증가</summary>
        public void AddFloat(string key, float amount)
        {
            floatValues[key] = GetFloat(key) + amount;
        }
        
        // ========================================
        // 문자열 값 헬퍼
        // ========================================
        
        /// <summary>문자열 값 가져오기</summary>
        public string GetString(string key, string defaultValue = "")
        {
            return stringValues.ContainsKey(key) ? stringValues[key] : defaultValue;
        }
        
        /// <summary>문자열 값 설정</summary>
        public void SetString(string key, string value)
        {
            stringValues[key] = value;
        }
        
        // ========================================
        // 카운터 맵 헬퍼
        // ========================================
        
        /// <summary>카운터 맵의 값 가져오기</summary>
        public int GetCounter(string mapName, string key, int defaultValue = 0)
        {
            if (!counterMaps.ContainsKey(mapName))
                return defaultValue;
            
            var map = counterMaps[mapName];
            return map.ContainsKey(key) ? map[key] : defaultValue;
        }
        
        /// <summary>카운터 맵의 값 설정</summary>
        public void SetCounter(string mapName, string key, int value)
        {
            if (!counterMaps.ContainsKey(mapName))
                counterMaps[mapName] = new Dictionary<string, int>();
            
            counterMaps[mapName][key] = value;
        }
        
        /// <summary>카운터 맵의 값 증가</summary>
        public void AddCounter(string mapName, string key, int amount = 1)
        {
            SetCounter(mapName, key, GetCounter(mapName, key) + amount);
        }
        
        // ========================================
        // 플래그 세트 헬퍼
        // ========================================
        
        /// <summary>플래그 존재 여부 확인</summary>
        public bool HasFlag(string setName, string flag)
        {
            if (!flagSets.ContainsKey(setName))
                return false;
            
            return flagSets[setName].Contains(flag);
        }
        
        /// <summary>플래그 추가</summary>
        public void AddFlag(string setName, string flag)
        {
            if (!flagSets.ContainsKey(setName))
                flagSets[setName] = new HashSet<string>();
            
            flagSets[setName].Add(flag);
        }
        
        /// <summary>플래그 제거</summary>
        public void RemoveFlag(string setName, string flag)
        {
            if (flagSets.ContainsKey(setName))
            {
                flagSets[setName].Remove(flag);
            }
        }
        
        /// <summary>플래그 세트의 모든 플래그 가져오기</summary>
        public HashSet<string> GetFlags(string setName)
        {
            if (!flagSets.ContainsKey(setName))
                flagSets[setName] = new HashSet<string>();
            
            return flagSets[setName];
        }
        
        // ========================================
        // 유틸리티
        // ========================================
        
        /// <summary>생존 시간 계산 (초)</summary>
        public float GetSurvivalTime()
        {
            return Time.time - gameStartTime;
        }
        
        /// <summary>디버그용 출력</summary>
        public override string ToString()
        {
            return $"[GameSession] Stage: {currentStageName}, " +
                   $"IntValues: {intValues.Count}, FloatValues: {floatValues.Count}, " +
                   $"CounterMaps: {counterMaps.Count}, FlagSets: {flagSets.Count}";
        }
    }
}


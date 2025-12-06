using System.Collections.Generic;
using UnityEngine;
using PawnSurvivors.Data;
using PawnSurvivors.Domain.Repositories;
using PawnSurvivors.Domain;

namespace PawnSurvivors.Data.Repositories
{
    /// <summary>
    /// ISessionDataRepository의 구현체입니다.
    /// GameSessionData를 래핑하여 Domain 계층에 데이터를 제공합니다.
    /// </summary>
    public class SessionDataRepository : ISessionDataRepository
    {
        private GameSessionData _sessionData;

        public SessionDataRepository(GameSessionData sessionData)
        {
            _sessionData = sessionData;
        }

        // ========================================
        // 스테이지 정보
        // ========================================
        
        public string GetCurrentStageName()
        {
            return _sessionData.currentStageName;
        }

        public void SetCurrentStageName(string stageName)
        {
            _sessionData.currentStageName = stageName;
        }

        public string GetCurrentStageState()
        {
            return _sessionData.currentStageState;
        }

        public void SetCurrentStageState(string stageState)
        {
            _sessionData.currentStageState = stageState;
        }

        public float GetGameStartTime()
        {
            return _sessionData.gameStartTime;
        }

        public void SetGameStartTime(float time)
        {
            _sessionData.gameStartTime = time;
        }

        public float GetSurvivalTime()
        {
            return _sessionData.GetSurvivalTime();
        }

        // ========================================
        // 정수 값 (enum 기반 - 권장)
        // ========================================
        
        public int GetInt(SessionDataIntKey key, int defaultValue = 0)
        {
            return _sessionData.GetInt(EnumToString(key), defaultValue);
        }

        public void SetInt(SessionDataIntKey key, int value)
        {
            _sessionData.SetInt(EnumToString(key), value);
        }

        public void AddInt(SessionDataIntKey key, int amount = 1)
        {
            _sessionData.AddInt(EnumToString(key), amount);
        }

        // ========================================
        // 실수 값 (enum 기반 - 권장)
        // ========================================
        
        public float GetFloat(SessionDataFloatKey key, float defaultValue = 0f)
        {
            return _sessionData.GetFloat(EnumToString(key), defaultValue);
        }

        public void SetFloat(SessionDataFloatKey key, float value)
        {
            _sessionData.SetFloat(EnumToString(key), value);
        }

        public void AddFloat(SessionDataFloatKey key, float amount)
        {
            _sessionData.AddFloat(EnumToString(key), amount);
        }

        // ========================================
        // 정수 값 (string 기반 - 하위 호환성)
        // ========================================
        
        public int GetInt(string key, int defaultValue = 0)
        {
            return _sessionData.GetInt(key, defaultValue);
        }

        public void SetInt(string key, int value)
        {
            _sessionData.SetInt(key, value);
        }

        public void AddInt(string key, int amount = 1)
        {
            _sessionData.AddInt(key, amount);
        }

        // ========================================
        // 실수 값 (string 기반 - 하위 호환성)
        // ========================================
        
        public float GetFloat(string key, float defaultValue = 0f)
        {
            return _sessionData.GetFloat(key, defaultValue);
        }

        public void SetFloat(string key, float value)
        {
            _sessionData.SetFloat(key, value);
        }

        public void AddFloat(string key, float amount)
        {
            _sessionData.AddFloat(key, amount);
        }

        // ========================================
        // Enum 변환 헬퍼
        // ========================================
        
        /// <summary>
        /// Enum을 문자열로 변환합니다. (camelCase)
        /// 예: EnemiesKilled → "enemiesKilled", Gold → "gold"
        /// </summary>
        private static string EnumToString(SessionDataIntKey key)
        {
            // Enum 이름을 camelCase로 변환
            string enumName = key.ToString();
            if (enumName.Length > 0)
            {
                return char.ToLowerInvariant(enumName[0]) + enumName.Substring(1);
            }
            return enumName;
        }

        /// <summary>
        /// Enum을 문자열로 변환합니다. (camelCase)
        /// 예: TotalDamageDealt → "totalDamageDealt"
        /// </summary>
        private static string EnumToString(SessionDataFloatKey key)
        {
            // Enum 이름을 camelCase로 변환
            string enumName = key.ToString();
            if (enumName.Length > 0)
            {
                return char.ToLowerInvariant(enumName[0]) + enumName.Substring(1);
            }
            return enumName;
        }

        // ========================================
        // 문자열 값
        // ========================================
        
        public string GetString(string key, string defaultValue = "")
        {
            return _sessionData.GetString(key, defaultValue);
        }

        public void SetString(string key, string value)
        {
            _sessionData.SetString(key, value);
        }

        // ========================================
        // 카운터 맵
        // ========================================
        
        public int GetCounter(string mapName, string key, int defaultValue = 0)
        {
            return _sessionData.GetCounter(mapName, key, defaultValue);
        }

        public void SetCounter(string mapName, string key, int value)
        {
            _sessionData.SetCounter(mapName, key, value);
        }

        public void AddCounter(string mapName, string key, int amount = 1)
        {
            _sessionData.AddCounter(mapName, key, amount);
        }

        // ========================================
        // 플래그 세트
        // ========================================
        
        public bool HasFlag(string setName, string flag)
        {
            return _sessionData.HasFlag(setName, flag);
        }

        public void AddFlag(string setName, string flag)
        {
            _sessionData.AddFlag(setName, flag);
        }

        public void RemoveFlag(string setName, string flag)
        {
            _sessionData.RemoveFlag(setName, flag);
        }

        public HashSet<string> GetFlags(string setName)
        {
            return _sessionData.GetFlags(setName);
        }

        // ========================================
        // 플래그 세트 (enum 기반)
        // ========================================
        
        public void AddFlag<T>(string setName, T flag) where T : struct, System.Enum
        {
            _sessionData.AddFlag(setName, flag.ToString());
        }

        public void RemoveFlag<T>(string setName, T flag) where T : struct, System.Enum
        {
            _sessionData.RemoveFlag(setName, flag.ToString());
        }

        public bool HasFlag<T>(string setName, T flag) where T : struct, System.Enum
        {
            return _sessionData.HasFlag(setName, flag.ToString());
        }

        public HashSet<T> GetFlags<T>(string setName) where T : struct, System.Enum
        {
            var stringFlags = _sessionData.GetFlags(setName);
            var enumFlags = new HashSet<T>();
            
            foreach (var flagString in stringFlags)
            {
                if (System.Enum.TryParse<T>(flagString, out T enumValue))
                {
                    enumFlags.Add(enumValue);
                }
            }
            
            return enumFlags;
        }
        
        // ========================================
        // 세션 관리
        // ========================================
        
        public void Reset()
        {
            _sessionData.Reset();
        }
    }
}


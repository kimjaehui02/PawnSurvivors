using System.Collections.Generic;
using PawnSurvivors.Domain;

namespace PawnSurvivors.Domain.Repositories
{
    /// <summary>
    /// 게임 세션 데이터에 접근하기 위한 Repository 인터페이스입니다.
    /// Domain 계층에서 사용되며, Data 계층에서 구현됩니다.
    /// </summary>
    public interface ISessionDataRepository
    {
        // ========================================
        // 스테이지 정보
        // ========================================
        
        string GetCurrentStageName();
        void SetCurrentStageName(string stageName);
        string GetCurrentStageState();
        void SetCurrentStageState(string stageState);
        float GetGameStartTime();
        void SetGameStartTime(float time);
        float GetSurvivalTime();
        
        // ========================================
        // 정수 값 (enum 기반 - 권장)
        // ========================================
        
        int GetInt(SessionDataIntKey key, int defaultValue = 0);
        void SetInt(SessionDataIntKey key, int value);
        void AddInt(SessionDataIntKey key, int amount = 1);
        
        // ========================================
        // 실수 값 (enum 기반 - 권장)
        // ========================================
        
        float GetFloat(SessionDataFloatKey key, float defaultValue = 0f);
        void SetFloat(SessionDataFloatKey key, float value);
        void AddFloat(SessionDataFloatKey key, float amount);
        
        // ========================================
        // 정수 값 (string 기반 - 하위 호환성)
        // ========================================
        
        int GetInt(string key, int defaultValue = 0);
        void SetInt(string key, int value);
        void AddInt(string key, int amount = 1);
        
        // ========================================
        // 실수 값 (string 기반 - 하위 호환성)
        // ========================================
        
        float GetFloat(string key, float defaultValue = 0f);
        void SetFloat(string key, float value);
        void AddFloat(string key, float amount);
        
        // ========================================
        // 문자열 값
        // ========================================
        
        string GetString(string key, string defaultValue = "");
        void SetString(string key, string value);
        
        // ========================================
        // 카운터 맵
        // ========================================
        
        int GetCounter(string mapName, string key, int defaultValue = 0);
        void SetCounter(string mapName, string key, int value);
        void AddCounter(string mapName, string key, int amount = 1);
        
        // ========================================
        // 플래그 세트
        // ========================================
        
        bool HasFlag(string setName, string flag);
        void AddFlag(string setName, string flag);
        void RemoveFlag(string setName, string flag);
        HashSet<string> GetFlags(string setName);
        
        // ========================================
        // 플래그 세트 (enum 기반)
        // ========================================
        
        /// <summary>
        /// enum 기반 플래그를 추가합니다.
        /// </summary>
        void AddFlag<T>(string setName, T flag) where T : struct, System.Enum;
        
        /// <summary>
        /// enum 기반 플래그를 제거합니다.
        /// </summary>
        void RemoveFlag<T>(string setName, T flag) where T : struct, System.Enum;
        
        /// <summary>
        /// enum 기반 플래그 존재 여부를 확인합니다.
        /// </summary>
        bool HasFlag<T>(string setName, T flag) where T : struct, System.Enum;
        
        /// <summary>
        /// enum 기반 플래그 세트를 가져옵니다.
        /// </summary>
        HashSet<T> GetFlags<T>(string setName) where T : struct, System.Enum;
        
        // ========================================
        // 세션 관리
        // ========================================
        
        void Reset();
    }
}


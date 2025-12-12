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

        /// <summary>
        /// 스테이지 간 세션 데이터를 초기화합니다.
        /// 플레이어 데이터, 아이템 등은 유지됩니다.
        /// </summary>
        void Reset();

        /// <summary>
        /// 완전히 새 게임을 시작할 때 호출합니다.
        /// 캐릭터 선택으로 돌아갈 때 사용합니다.
        /// 모든 데이터가 초기화됩니다 (저장된 캐릭터 선택 포함).
        /// </summary>
        void ResetForNewGame();

        /// <summary>
        /// 게임오버 후 같은 캐릭터로 재시작할 때 호출합니다.
        /// Pawn, 아이템 등 게임 진행 데이터는 초기화하지만
        /// 저장된 캐릭터 선택은 유지합니다.
        /// </summary>
        void ResetForRetry();
    }
}


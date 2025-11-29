using System.Collections.Generic;
using UnityEngine;

namespace PawnSurvivors.Managers
{
    /// <summary>
    /// 로그 카테고리 정의
    /// </summary>
    public enum LogCategory
    {
        Recipe,      // RecipeLoader, ItemPoolLoader 등
        Item,        // 아이템 관련
        Combat,      // 전투, 데미지 등
        UI,          // UI 관련
        Debug,       // 디버깅용 (ItemPoolTestManager 등)
        System,      // GameManager, StageManager 등
        Pawn,        // Pawn 관련
        Data,        // 데이터 로딩/저장
        Stage        // 스테이지 관련
    }

    /// <summary>
    /// 로그 레벨 정의
    /// </summary>
    public enum LogLevel
    {
        Debug = 0,   // 가장 상세한 로그 (디버깅용)
        Info = 1,    // 일반 정보
        Warning = 2, // 경고
        Error = 3    // 에러 (항상 표시)
    }

    /// <summary>
    /// 카테고리별 필터 설정
    /// </summary>
    [System.Serializable]
    public class CategoryFilter
    {
        public LogCategory category;
        public bool enabled = true;
        public LogLevel minLevel = LogLevel.Info; // 이 레벨 이상만 표시
    }

    /// <summary>
    /// 중앙집중형 디버그 로그 매니저
    /// 카테고리별, 레벨별 필터링으로 로그 스팸 제어
    /// </summary>
    public class LogManager : MonoBehaviour
    {
        private static LogManager _instance;
        public static LogManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 씬에 LogManager가 없으면 null 반환 (자동 생성 안 함)
                    _instance = FindFirstObjectByType<LogManager>();
                }
                return _instance;
            }
        }

        [Header("전체 설정")]
        [Tooltip("전체 로그 활성화/비활성화")]
        public bool enableAll = true;

        [Header("카테고리별 필터")]
        [Tooltip("각 카테고리별 로그 필터 설정")]
        public List<CategoryFilter> categoryFilters = new List<CategoryFilter>();

        // Dictionary로 필터 조회 최적화 (O(N) → O(1))
        private Dictionary<LogCategory, CategoryFilter> _filterMap;

        // 초기화 안전성을 위한 플래그
        private bool _isInitialized = false;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                // Editor 모드가 아닐 때만 DontDestroyOnLoad 사용 (Editor 저장 에러 방지)
                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            InitializeFilters();
        }

        private void OnDestroy()
        {
            // Editor 모드 종료 시 인스턴스 정리
            if (_instance == this)
            {
                _instance = null;
            }
        }

        /// <summary>
        /// 필터 Dictionary 초기화
        /// </summary>
        private void InitializeFilters()
        {
            _filterMap = new Dictionary<LogCategory, CategoryFilter>();

            // 기본 필터 설정 (모든 카테고리 포함)
            if (categoryFilters.Count == 0)
            {
                foreach (LogCategory category in System.Enum.GetValues(typeof(LogCategory)))
                {
                    categoryFilters.Add(new CategoryFilter
                    {
                        category = category,
                        enabled = true,
                        minLevel = category == LogCategory.Debug ? LogLevel.Debug : LogLevel.Info
                    });
                }
            }

            // Dictionary 빌드
            foreach (var filter in categoryFilters)
            {
                _filterMap[filter.category] = filter;
            }

            _isInitialized = true;
        }

        /// <summary>
        /// 로그 출력 여부 확인 (Early-out 최적화)
        /// </summary>
        private bool ShouldPrint(LogCategory category, LogLevel level)
        {
            if (!_isInitialized)
            {
                InitializeFilters();
            }

            // 전체 비활성화 (Error 포함 모든 로그 차단)
            if (!enableAll)
            {
                return false;
            }

            // 필터 조회 (Dictionary 사용으로 O(1))
            if (!_filterMap.TryGetValue(category, out CategoryFilter filter))
            {
                // 필터가 없으면 기본값으로 허용 (Info 이상)
                return level >= LogLevel.Info;
            }

            // 카테고리 비활성화
            if (!filter.enabled)
            {
                return false;
            }

            // 레벨 필터링
            return level >= filter.minLevel;
        }

        /// <summary>
        /// 로그 출력 (정적 메서드)
        /// </summary>
        public static void Log(LogCategory category, LogLevel level, string message, Object context = null)
        {
            // LogManager가 씬에 없으면 로그 출력 안 함
            if (Instance == null)
            {
                return;
            }

            // Early-out: 문자열 조합 전에 필터링 확인
            if (!Instance.ShouldPrint(category, level))
            {
                return;
            }

            // 실제 로그 출력
            string formattedMessage = $"[{category}] {message}";

            switch (level)
            {
                case LogLevel.Debug:
                case LogLevel.Info:
                    UnityEngine.Debug.Log(formattedMessage, context);
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(formattedMessage, context);
                    break;
                case LogLevel.Error:
                    UnityEngine.Debug.LogError(formattedMessage, context);
                    break;
            }
        }

        // 편의 메서드들
        public static void LogDebug(LogCategory category, string message, Object context = null)
        {
            Log(category, LogLevel.Debug, message, context);
        }

        public static void LogInfo(LogCategory category, string message, Object context = null)
        {
            Log(category, LogLevel.Info, message, context);
        }

        public static void LogWarning(LogCategory category, string message, Object context = null)
        {
            Log(category, LogLevel.Warning, message, context);
        }

        public static void LogError(LogCategory category, string message, Object context = null)
        {
            Log(category, LogLevel.Error, message, context);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor에서 필터 변경 시 Dictionary 재빌드
        /// </summary>
        private void OnValidate()
        {
            if (_filterMap != null)
            {
                InitializeFilters();
            }
        }
#endif
    }
}


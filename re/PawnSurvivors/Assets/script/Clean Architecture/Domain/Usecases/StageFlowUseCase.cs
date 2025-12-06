using System;
using System.Linq;
using PawnSurvivors.Domain.Repositories;
using PawnSurvivors.Data.DataSources;

namespace PawnSurvivors.Domain.Usecases
{
    public class StageFlowUseCase
    {
        private readonly ISessionDataRepository _sessionRepository;
        private readonly StageListDataSource _stageListDataSource;
        
        // 현재 선택된 캠페인의 스테이지 목록
        private string[] _campaignStages = null;
        
        public enum StageState
        {
            NotStarted,
            InProgress,
            Completed,
            Failed,
            AllCleared
        }
        
        public StageState CurrentState
        {
            get
            {
                var flags = _sessionRepository.GetFlags<StageState>("stageState");
                return flags.Count > 0 ? flags.First() : StageState.NotStarted;
            }
            private set
            {
                // 기존 플래그 제거
                var existingFlags = _sessionRepository.GetFlags<StageState>("stageState");
                foreach (var flag in existingFlags)
                {
                    _sessionRepository.RemoveFlag("stageState", flag);
                }
                // 새 상태 추가
                _sessionRepository.AddFlag("stageState", value);
            }
        }
        
        public event Action<string> OnStageStartRequested;
        public event Action OnStageCompletedToShop;
        public event Action OnGameOver;
        public event Action OnAllStagesCleared;

        public StageFlowUseCase(ISessionDataRepository sessionRepository, StageListDataSource stageListDataSource)
        {
            _sessionRepository = sessionRepository;
            _stageListDataSource = stageListDataSource;
        }

        public void StartStage(string stageName, bool resetSession = true)
        {
            if (resetSession)
            {
                _sessionRepository.Reset();
            }
            
            _sessionRepository.SetCurrentStageName(stageName);
            CurrentState = StageState.InProgress;
            
            OnStageStartRequested?.Invoke(stageName);
        }

        public void CompleteStage()
        {
            if (CurrentState != StageState.InProgress)
            {
                // 이미 완료되었거나 다른 상태인 경우 로그 출력
                UnityEngine.Debug.LogWarning($"[StageFlowUseCase] CompleteStage() 호출 실패: CurrentState = {CurrentState} (InProgress가 아님)");
                return;
            }
            
            CurrentState = StageState.Completed;
            
            string nextStageName = GetNextStageName();
            UnityEngine.Debug.Log($"[StageFlowUseCase] CompleteStage: nextStageName = {nextStageName ?? "null"}");
            
            if (string.IsNullOrEmpty(nextStageName))
            {
                CurrentState = StageState.AllCleared;
                UnityEngine.Debug.Log("[StageFlowUseCase] 모든 스테이지 완료 - OnAllStagesCleared 이벤트 발생");
                OnAllStagesCleared?.Invoke();
            }
            else
            {
                UnityEngine.Debug.Log("[StageFlowUseCase] 다음 스테이지 있음 - OnStageCompletedToShop 이벤트 발생");
                OnStageCompletedToShop?.Invoke();
            }
        }

        public void FailStage()
        {
            // 플레이어 사망 시 즉시 게임오버 처리 (InProgress가 아니어도 처리)
            // 예: 스테이지가 완료된 후에도 플레이어가 사망할 수 있음
            if (CurrentState == StageState.Failed || CurrentState == StageState.AllCleared)
            {
                // 이미 게임오버 상태이면 중복 처리 방지
                return;
            }
            
            CurrentState = StageState.Failed;
            UnityEngine.Debug.Log($"[StageFlowUseCase] FailStage() 호출됨. OnGameOver 이벤트 발생");
            OnGameOver?.Invoke();
        }

        public void ProceedToNextStage()
        {
            string nextStageName = GetNextStageName();
            
            if (string.IsNullOrEmpty(nextStageName))
            {
                CurrentState = StageState.AllCleared;
                OnAllStagesCleared?.Invoke();
                return;
            }
            
            StartStage(nextStageName, resetSession: false);
        }

        public string GetCurrentStageName()
        {
            string stageName = _sessionRepository.GetCurrentStageName();
            return string.IsNullOrEmpty(stageName) ? "DebugStage" : stageName;
        }

        public string GetNextStageName()
        {
            string currentStageName = GetCurrentStageName();
            
            // 캠페인 스테이지가 설정되어 있으면 그것 사용
            if (_campaignStages != null && _campaignStages.Length > 0)
            {
                UnityEngine.Debug.Log($"[StageFlowUseCase] GetNextStageName: 캠페인 스테이지 사용. 현재: {currentStageName}, 캠페인 스테이지 수: {_campaignStages.Length}");
                UnityEngine.Debug.Log($"[StageFlowUseCase] 캠페인 스테이지 목록: [{string.Join(", ", _campaignStages)}]");
                
                for (int i = 0; i < _campaignStages.Length; i++)
                {
                    if (_campaignStages[i] == currentStageName)
                    {
                        // 현재 스테이지가 마지막 스테이지인지 확인
                        if (i >= _campaignStages.Length - 1)
                        {
                            UnityEngine.Debug.Log($"[StageFlowUseCase] 현재 스테이지가 마지막 스테이지입니다. (인덱스: {i}/{_campaignStages.Length - 1})");
                            return null; // 마지막 스테이지
                        }
                        
                        string nextStage = _campaignStages[i + 1];
                        UnityEngine.Debug.Log($"[StageFlowUseCase] 다음 스테이지 찾음: {nextStage} (인덱스: {i + 1})");
                        return nextStage;
                    }
                }
                
                // 현재 스테이지가 캠페인 목록에 없으면 null 반환 (에러 상황)
                UnityEngine.Debug.LogWarning($"[StageFlowUseCase] 현재 스테이지 '{currentStageName}'가 캠페인 목록에 없습니다. 마지막 스테이지로 간주합니다.");
                return null;
            }
            
            // 기존 방식 (StageListDataSource 사용)
            if (_stageListDataSource == null)
            {
                UnityEngine.Debug.LogWarning("[StageFlowUseCase] 캠페인 스테이지도 없고 StageListDataSource도 null입니다.");
                return null;
            }

            string nextStageFromDataSource = _stageListDataSource.GetNextStageName(currentStageName);
            UnityEngine.Debug.Log($"[StageFlowUseCase] StageListDataSource 사용. 현재: {currentStageName}, 다음: {nextStageFromDataSource ?? "null"}");
            return nextStageFromDataSource;
        }
        
        /// <summary>
        /// 캠페인의 스테이지 목록을 설정합니다.
        /// </summary>
        public void SetCampaignStages(string[] stages)
        {
            _campaignStages = stages;
        }

        public void RestartStage()
        {
            // 재시작 시 세션 리셋 (캐릭터 선택 상태는 GameOverScreen에서 복원)
            string currentStageName = GetCurrentStageName();
            
            // 스테이지 시작 (세션 리셋 포함)
            StartStage(currentStageName, resetSession: true);
        }

        public void StartNewGame(string firstStageName = null)
        {
            if (string.IsNullOrEmpty(firstStageName) && _stageListDataSource != null)
            {
                var stageList = _stageListDataSource.GetStageList();
                if (stageList?.stages != null && stageList.stages.Count > 0)
                {
                    firstStageName = stageList.stages[0];
                }
            }
            
            if (string.IsNullOrEmpty(firstStageName))
            {
                firstStageName = "DebugStage";
            }
            
            StartStage(firstStageName, resetSession: true);
        }

        public void ResetState()
        {
            CurrentState = StageState.NotStarted;
        }
    }
}

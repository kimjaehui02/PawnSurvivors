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
                return;
            }
            
            CurrentState = StageState.Completed;
            
            string nextStageName = GetNextStageName();
            
            if (string.IsNullOrEmpty(nextStageName))
            {
                CurrentState = StageState.AllCleared;
                OnAllStagesCleared?.Invoke();
            }
            else
            {
                OnStageCompletedToShop?.Invoke();
            }
        }

        public void FailStage()
        {
            if (CurrentState != StageState.InProgress)
            {
                return;
            }
            
            CurrentState = StageState.Failed;
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
                for (int i = 0; i < _campaignStages.Length - 1; i++)
                {
                    if (_campaignStages[i] == currentStageName)
                    {
                        return _campaignStages[i + 1];
                    }
                }
                return null; // 마지막 스테이지
            }
            
            // 기존 방식 (StageListDataSource 사용)
            if (_stageListDataSource == null)
            {
                return null;
            }

            return _stageListDataSource.GetNextStageName(currentStageName);
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

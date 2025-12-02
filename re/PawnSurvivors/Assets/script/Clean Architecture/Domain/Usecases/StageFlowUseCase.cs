using System;
using PawnSurvivors.Domain.Repositories;
using PawnSurvivors.Data.DataSources;

namespace PawnSurvivors.Domain.Usecases
{
    public class StageFlowUseCase
    {
        private readonly ISessionDataRepository _sessionRepository;
        private readonly StageListDataSource _stageListDataSource;
        
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
                string stateString = _sessionRepository.GetCurrentStageState();
                return Enum.TryParse<StageState>(stateString, out var state) ? state : StageState.NotStarted;
            }
            private set
            {
                _sessionRepository.SetCurrentStageState(value.ToString());
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
            if (_stageListDataSource == null)
            {
                return null;
            }

            string currentStageName = GetCurrentStageName();
            return _stageListDataSource.GetNextStageName(currentStageName);
        }

        public void RestartStage()
        {
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

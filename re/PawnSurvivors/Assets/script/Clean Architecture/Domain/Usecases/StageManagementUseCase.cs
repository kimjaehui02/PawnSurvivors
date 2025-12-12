using PawnSurvivors.Domain.Repositories;
using PawnSurvivors.Data.DataSources;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// 스테이지 시작/종료 관리를 담당하는 UseCase입니다.
    /// 스테이지 라이프사이클의 비즈니스 로직을 처리합니다.
    /// </summary>
    public class StageManagementUseCase
    {
        private readonly ISessionDataRepository _sessionRepository;
        private readonly StageListDataSource _stageListDataSource;

        public StageManagementUseCase(ISessionDataRepository sessionRepository, StageListDataSource stageListDataSource = null)
        {
            _sessionRepository = sessionRepository;
            _stageListDataSource = stageListDataSource;
        }

        /// <summary>
        /// 스테이지 시작을 준비합니다.
        /// 세션 리셋 여부를 결정하고 스테이지 이름을 설정합니다.
        /// </summary>
        /// <param name="stageName">시작할 스테이지 이름</param>
        /// <param name="shouldResetSession">세션을 리셋할지 여부</param>
        public void PrepareStageStart(string stageName, bool shouldResetSession)
        {
            if (shouldResetSession)
            {
                _sessionRepository.Reset();
            }
            _sessionRepository.SetCurrentStageName(stageName);
        }

        /// <summary>
        /// 현재 스테이지 이름을 가져옵니다.
        /// </summary>
        /// <returns>현재 스테이지 이름 (없으면 "Stage1")</returns>
        public string GetCurrentStageName()
        {
            string stageName = _sessionRepository.GetCurrentStageName();
            return string.IsNullOrEmpty(stageName) ? "Stage1" : stageName;
        }
        
        /// <summary>
        /// 세션 데이터에서 현재 스테이지 이름을 가져옵니다. (기본값 없이)
        /// </summary>
        /// <returns>현재 스테이지 이름 (없으면 null 또는 빈 문자열)</returns>
        public string GetCurrentStageNameRaw()
        {
            return _sessionRepository.GetCurrentStageName();
        }

        /// <summary>
        /// 스테이지 종료를 준비합니다.
        /// (필요한 경우 세션 데이터 정리 등)
        /// </summary>
        public void PrepareStageEnd()
        {
            // 스테이지 종료 시 필요한 데이터 정리 로직
            // 현재는 세션 데이터는 유지 (상점에서 돌아올 수 있으므로)
        }

        /// <summary>
        /// 현재 스테이지의 다음 스테이지 이름을 가져옵니다.
        /// </summary>
        /// <returns>다음 스테이지 이름, 없으면 null (마지막 스테이지)</returns>
        public string GetNextStageName()
        {
            if (_stageListDataSource == null)
            {
                return null;
            }

            string currentStageName = GetCurrentStageName();
            return _stageListDataSource.GetNextStageName(currentStageName);
        }

        /// <summary>
        /// 현재 라운드(스테이지) 번호를 가져옵니다. (1부터 시작)
        /// </summary>
        /// <returns>현재 라운드 번호, 없으면 1</returns>
        public int GetCurrentRound()
        {
            if (_stageListDataSource == null)
            {
                return 1;
            }

            string currentStageName = GetCurrentStageName();
            int round = _stageListDataSource.GetStageNumber(currentStageName);
            return round > 0 ? round : 1;
        }

        /// <summary>
        /// 전체 라운드(스테이지) 수를 가져옵니다.
        /// </summary>
        /// <returns>전체 라운드 수, 없으면 0</returns>
        public int GetTotalRounds()
        {
            if (_stageListDataSource == null)
            {
                return 0;
            }

            return _stageListDataSource.GetTotalStageCount();
        }
    }
}


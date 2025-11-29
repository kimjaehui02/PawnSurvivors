using PawnSurvivors.Domain.Repositories;

namespace PawnSurvivors.Domain.Usecases
{
    /// <summary>
    /// 세션 데이터 관리를 담당하는 UseCase입니다.
    /// 스테이지 정보, 세션 리셋 등을 처리합니다.
    /// </summary>
    public class SessionManagementUseCase
    {
        private readonly ISessionDataRepository _repository;

        public SessionManagementUseCase(ISessionDataRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 세션 데이터를 초기화합니다.
        /// </summary>
        public void ResetSession()
        {
            _repository.Reset();
        }

        /// <summary>
        /// 현재 스테이지 이름을 설정합니다.
        /// </summary>
        /// <param name="stageName">스테이지 이름</param>
        public void SetCurrentStageName(string stageName)
        {
            _repository.SetCurrentStageName(stageName);
        }

        /// <summary>
        /// 현재 스테이지 이름을 가져옵니다.
        /// </summary>
        /// <returns>현재 스테이지 이름 (없으면 "Stage1")</returns>
        public string GetCurrentStageName()
        {
            string stageName = _repository.GetCurrentStageName();
            return string.IsNullOrEmpty(stageName) ? "Stage1" : stageName;
        }

        /// <summary>
        /// 게임 시작 시간을 설정합니다.
        /// </summary>
        /// <param name="time">게임 시작 시간</param>
        public void SetGameStartTime(float time)
        {
            _repository.SetGameStartTime(time);
        }

        /// <summary>
        /// 게임 시작 시간을 가져옵니다.
        /// </summary>
        /// <returns>게임 시작 시간</returns>
        public float GetGameStartTime()
        {
            return _repository.GetGameStartTime();
        }
    }
}


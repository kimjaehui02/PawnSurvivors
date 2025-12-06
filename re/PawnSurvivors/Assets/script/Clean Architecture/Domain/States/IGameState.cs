namespace PawnSurvivors.Domain.States
{
    /// <summary>
    /// 게임 상태를 나타내는 인터페이스입니다.
    /// State 패턴을 사용하여 각 게임 단계를 독립적으로 관리합니다.
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// 상태 진입 시 호출됩니다.
        /// </summary>
        void OnEnter();
        
        /// <summary>
        /// 상태 종료 시 호출됩니다.
        /// </summary>
        void OnExit();
        
        /// <summary>
        /// 상태 업데이트 (필요시 사용)
        /// </summary>
        void OnUpdate();
        
        /// <summary>
        /// 특정 상태로 전환 가능한지 확인합니다.
        /// </summary>
        bool CanTransitionTo(IGameState nextState);
        
        /// <summary>
        /// 상태 이름 (디버깅용)
        /// </summary>
        string StateName { get; }
    }
}


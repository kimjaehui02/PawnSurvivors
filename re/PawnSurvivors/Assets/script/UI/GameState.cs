namespace PawnSurvivors.UI
{
    /// <summary>
    /// 게임의 전체 상태를 나타내는 Enum입니다.
    /// </summary>
    public enum GameState
    {
        /// <summary>타이틀 화면</summary>
        Title,
        
        /// <summary>메인 메뉴 (스테이지 선택 등)</summary>
        MainMenu,
        
        /// <summary>게임 플레이 중</summary>
        Playing,
        
        /// <summary>일시정지</summary>
        Paused,
        
        /// <summary>게임 오버</summary>
        GameOver,
        
        /// <summary>스테이지 클리어</summary>
        StageClear
    }
}


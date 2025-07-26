using UnityEngine;

namespace Game.Core
{
    public enum GameEventType
    {
        None,
        SoundPlay,
        UIUpdateScore,
        PlayerDied,
        LevelLoaded,
        // ...
    }

    public interface IGameEventContext
    {
        // 모든 컨텍스트가 가져야 할 공통 속성이나 메서드가 있다면 여기에 정의
        // 예: GameEventType EventType { get; }
        // 예: DateTime Timestamp { get; }
    }
}

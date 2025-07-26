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

    public class ManagerContext
    {
        // 이 클래스는 GameEventType에 대한 컨텍스트 정보를 담을 수 있습니다.
        // 예를 들어, 이벤트 발생 시 필요한 추가 정보나 상태 등을 여기에 정의할 수 있습니다.
        // 현재는 비워두지만, 필요에 따라 확장 가능합니다.
    }
}

using Game.Core;
using Game.Core.Contexts;
using Game.Core.Enums;

namespace Game.Core.Base
{
    public abstract class ManagerBase : SubComponentBase<GameEventType, GameEventContext> { }
    public abstract class PawnBase : SubComponentBase<Acts, AbilityContext> { }
}

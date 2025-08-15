using Game.Core.Contexts;
using Game.Core.Enums;
using System;
using System.Collections.Generic;

namespace Game.Core.Base
{
    public interface IConfigurable<TConfig>
    {
        void Initialize(TConfig config);
    }

    public interface IActionMapManager
    {
        IReadOnlyDictionary<Acts, Action<AbilityContext>> GetActions { get; }
        void AddAction(Acts act, Action<AbilityContext> action);
        void RemoveAction(Acts act, Action<AbilityContext> action = null);
    }

    public interface IBaseConfig
    {
        IBaseConfig Clone();
    }
}

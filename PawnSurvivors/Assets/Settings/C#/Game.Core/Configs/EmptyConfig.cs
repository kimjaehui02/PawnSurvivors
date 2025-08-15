using Game.Core.Base;

namespace Game.Core.Configs
{
    public class EmptyConfig : IBaseConfig
    {
        public IBaseConfig Clone() => new EmptyConfig();
    }
}

using System.Collections.Generic;
using System.Linq;
using Game.Core.Base;
using Game.Core.Configs;

namespace Game.Core.Models
{
    public class PawnData
    {
        public PawnConfig PawnConfig { get; private set; }
        public Dictionary<string, IBaseConfig> ComponentConfigs { get; private set; }

        public PawnData(PawnConfig pawnConfig, Dictionary<string, IBaseConfig> componentConfigs)
        {
            PawnConfig = pawnConfig;
            ComponentConfigs = componentConfigs;
        }

        public PawnData() { }

        public PawnData(PawnData other)
        {
            PawnConfig = (PawnConfig)other.PawnConfig.Clone();
            ComponentConfigs = other.ComponentConfigs.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Clone()
            );
        }

        public PawnData Clone() => new(this);
    }
}

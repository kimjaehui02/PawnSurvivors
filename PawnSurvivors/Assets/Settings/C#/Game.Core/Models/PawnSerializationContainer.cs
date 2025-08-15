using System;
using System.Collections.Generic;
using Game.Core.Base;
using Game.Core.Configs;

namespace Game.Core.Models
{
    [Serializable]
    public class PawnSerializationContainer
    {
        public PawnConfig pawnConfig;
        public Dictionary<string, IBaseConfig> components;
    }
}

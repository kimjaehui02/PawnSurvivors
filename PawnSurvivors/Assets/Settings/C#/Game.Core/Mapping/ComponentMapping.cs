using System;
using System.Collections.Generic;
using Game.Core.Configs;

namespace Game.Core.Mapping
{
    public static class ComponentMapping
    {
        public static readonly Dictionary<string, (Type ComponentType, Type ConfigType)> ComponentMap = new()
        {
            { "DamageableComponent", (typeof(DamageableComponent), typeof(DamageableConfig)) },
            { "DamageDealerComponent", (typeof(DamageDealerComponent), typeof(DamageDealerConfig)) },
            { "GraphicComponent", (typeof(GraphicComponent), typeof(EmptyConfig)) },
            { "MoveableComponent", (typeof(MoveableComponent), typeof(MoveableConfig)) },
            { "PawnMoverComponent", (typeof(PawnMoverComponent), typeof(EmptyConfig)) },
            { "PawnRegisterToGameManagerComponent", (typeof(PawnRegisterToGameManagerComponent), typeof(EmptyConfig)) },
            { "PawnTargetFinderComponent", (typeof(PawnTargetFinderComponent), typeof(EmptyConfig)) },
            { "PlayerMoverComponent", (typeof(PlayerMoverComponent), typeof(EmptyConfig)) },
        };
    }
}

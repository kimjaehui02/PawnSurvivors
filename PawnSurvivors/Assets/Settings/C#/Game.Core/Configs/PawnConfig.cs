using System;
using Newtonsoft.Json;
using UnityEngine;
using Game.Core.Base;

namespace Game.Core.Configs
{
    [Serializable]
    public class PawnConfig : IBaseConfig
    {
        [SerializeField, JsonProperty("id")] private int id = -1;
        [SerializeField, JsonProperty("name")] private string name = "Default Pawn";
        [SerializeField, JsonProperty("description")] private string description = "This is a default pawn.";

        public int Id => id;
        public string Name => name;
        public string Description => description;

        public PawnConfig(PawnConfig other)
        {
            id = other.id;
            name = other.name;
            description = other.description;
        }

        public PawnConfig() { }
        public IBaseConfig Clone() => new PawnConfig(this);
    }
}

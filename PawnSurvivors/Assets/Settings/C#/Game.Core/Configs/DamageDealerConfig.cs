using System;
using Newtonsoft.Json;
using UnityEngine;
using Game.Core.Base;

namespace Game.Core.Configs
{
    [Serializable]
    public class DamageDealerConfig : IBaseConfig
    {
        [SerializeField, JsonProperty("damageAmount")] private float damageAmount = 10f;
        public float DamageAmount => damageAmount;

        public DamageDealerConfig(DamageDealerConfig other)
        {
            damageAmount = other.damageAmount;
        }

        public DamageDealerConfig() { }
        public IBaseConfig Clone() => new DamageDealerConfig(this);
    }
}

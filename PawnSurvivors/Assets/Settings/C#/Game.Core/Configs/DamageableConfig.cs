using System;
using Newtonsoft.Json;
using UnityEngine;
using Game.Core.Base;

namespace Game.Core.Configs
{
    [Serializable]
    public class DamageableConfig : IBaseConfig
    {
        [SerializeField, JsonProperty("maxHealth")] private float maxHealth = 100f;
        [SerializeField, JsonProperty("currentHealth")] private float currentHealth = 100f;

        public float MaxHealth { get => maxHealth; set => maxHealth = value; }
        public float CurrentHealth { get => currentHealth; set => currentHealth = value; }

        public DamageableConfig(DamageableConfig other)
        {
            maxHealth = other.maxHealth;
            currentHealth = other.currentHealth;
        }

        public DamageableConfig() { }
        public IBaseConfig Clone() => new DamageableConfig(this);
    }
}

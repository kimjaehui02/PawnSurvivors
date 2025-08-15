using System;
using Newtonsoft.Json;
using UnityEngine;
using Game.Core.Base;

namespace Game.Core.Configs
{
    [Serializable]
    public class MoveableConfig : IBaseConfig
    {
        [SerializeField, JsonProperty("moveSpeed")] private float moveSpeed = 1f;
        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

        public MoveableConfig(MoveableConfig other)
        {
            moveSpeed = other.moveSpeed;
        }

        public MoveableConfig() { }
        public IBaseConfig Clone() => new MoveableConfig(this);
    }
}

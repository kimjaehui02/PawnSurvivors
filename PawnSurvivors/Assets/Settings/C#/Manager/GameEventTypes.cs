using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public enum GameEventType
    {
        xmlLoaded,
        pawnSpawn,
        Update,
        RegisterUpdateAction,
        JsonLoading,
        // ...
    }

    public class GameEventContext
    {
        public bool stopUpdate = false;
    }


}


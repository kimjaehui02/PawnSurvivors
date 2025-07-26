using Game.Core;
using UnityEngine;

public abstract class ManagerBase : MonoBehaviour
{
    EnumDelegateMap<GameEventType, IGameEventContext> _delegateMap = new();


}

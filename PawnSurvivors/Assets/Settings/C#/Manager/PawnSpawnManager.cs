using Game.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PawnSpawnManager : ManagerBase
{

    private readonly Dictionary<string, Type> _componentMap = new()
    {
        { "MoveableComponent", typeof(MoveableConfig) },
        { "DamageableComponent", typeof(DamageableConfig) },
        { "DamageDealerComponent", typeof(DamageDealerConfig) },

    };

    //public List<GameObject> GameObjects;

    public GameObject BasePawn;

    public override void RegisterAbilities()
    {
        AddAction(GameEventType.PawnSpawn, SpawnPawn); // 또는 CustomLifecycleManager가 호출할 이벤트

    }

    public void SpawnPawn(GameEventContext context)
    {
        GameObject spawning = Instantiate(BasePawn);

        context.LogCurrentState();

        spawning.GetComponent<Pawn>().PawnConfig = context.PawnData.PawnConfig;
        
        spawning.AddComponent<MoveableComponent>(); // PawnBase 컴포넌트 추가
        spawning.AddComponent<PawnTargetFinderComponent>(); // PawnBase 컴포넌트 추가
        spawning.AddComponent<PawnMoverComponent>(); // PawnBase 컴포넌트 추가


    }




}

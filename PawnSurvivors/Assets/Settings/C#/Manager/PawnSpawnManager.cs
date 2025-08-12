using Game.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PawnSpawnManager : ManagerBase
{



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

        foreach (var component in context.PawnData.ComponentConfigs)
        {
            string componentName = component.Key;
            IBaseConfig config = component.Value;

            // _componentTypeMap에 키가 존재하는지 먼저 확인합니다.
            if (Game.Core.ComponentMapping.ComponentMap.TryGetValue(componentName, out (Type ComponentType, Type ConfigType) mapping))
            {
                // 키가 존재하면 컴포넌트를 추가합니다.
                spawning.AddComponent(mapping.ComponentType);
                Debug.Log($"Component Name: {componentName}, Config Type: {config.GetType().Name}를 추가했습니다.");
            }
            else
            {
                // 키가 존재하지 않으면 경고 메시지를 출력합니다.
                Debug.LogWarning($"경고: '{componentName}'에 해당하는 컴포넌트 타입을 _componentTypeMap에서 찾을 수 없습니다.");
            }
        }

        //spawning.AddComponent<MoveableComponent>(); // PawnBase 컴포넌트 추가
        //spawning.AddComponent<PawnTargetFinderComponent>(); // PawnBase 컴포넌트 추가
        //spawning.AddComponent<PawnMoverComponent>(); // PawnBase 컴포넌트 추가


    }




}

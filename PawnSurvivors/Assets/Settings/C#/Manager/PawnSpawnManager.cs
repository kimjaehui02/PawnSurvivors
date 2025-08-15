using Game.Core;
using Game.Core.Base;
using Game.Core.Contexts;
using Game.Core.Enums;
using Game.Core.Mapping;
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
        if (BasePawn == null)
        {
            Debug.LogError("PawnSpawnManager: BasePawn이 설정되지 않았습니다.");
            return;
        }

        if (context.PawnData == null)
        {
            Debug.LogError("PawnSpawnManager: PawnData가 null입니다. 스폰을 중단합니다.");
            return;
        }

        GameObject spawning = Instantiate(BasePawn);
        context.LogCurrentState();

        if (spawning.TryGetComponent<Pawn>(out var pawnComponent))
            pawnComponent.PawnConfig = context.PawnData.PawnConfig;

        foreach (var component in context.PawnData.ComponentConfigs)
        {
            string componentName = component.Key;
            IBaseConfig config = component.Value;

            if (ComponentMapping.ComponentMap.TryGetValue(componentName, out (Type ComponentType, Type ConfigType) mapping))
            {
                if (spawning.GetComponent(mapping.ComponentType) == null)
                {
                    var newComp = spawning.AddComponent(mapping.ComponentType);
                    Debug.Log($"Component Name: {componentName}, Config Type: {config.GetType().Name}를 추가했습니다.");

                    // Config 내부 내용물 출력
                    try
                    {
                        string configJson = JsonUtility.ToJson(config);
                        Debug.Log($"{componentName} Config 내용: {configJson}");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Config 로그 직렬화 실패 ({componentName}): {ex.Message}");
                    }

                    if (newComp is PawnBase configurable)
                    {

                        configurable.Initialize(config);
                        Debug.Log($"{componentName} 컴포넌트가 초기화되었습니다.");
                    }
                }
                else
                {
                    Debug.LogWarning($"{componentName} 컴포넌트가 이미 존재합니다. 추가하지 않습니다.");
                }
            }
            else
            {
                Debug.LogWarning($"경고: '{componentName}'에 해당하는 컴포넌트 타입을 찾을 수 없습니다.");
            }
        }
    }






}

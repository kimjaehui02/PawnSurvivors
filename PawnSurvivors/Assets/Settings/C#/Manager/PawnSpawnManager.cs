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
        if (context.PawnData == null || BasePawn == null)
        {
            Debug.LogError("PawnData 또는 BasePawn이 설정되지 않아 스폰을 중단합니다.");
            return;
        }

        // 정보전달용 객체가 값을 받아와야 합니다
        PawnSerializationContainer pawnSerializationContainer = context.PawnData;

        // 베이스폰을 생성합니다
        GameObject newPawnObject = Instantiate(BasePawn);
        if (newPawnObject == null)
        {
            Debug.LogError("BasePawn 오브젝트 생성에 실패했습니다. Pawn 스폰을 중단합니다.");
            return;
        }

        // 생성된 오브젝트에 Pawn 컴포넌트를 가져옵니다
        if (!newPawnObject.TryGetComponent<Pawn>(out var newPawn))
        {
            Debug.LogError("BasePawn에 Pawn 컴포넌트가 없습니다. Pawn 스폰을 중단합니다.");
            Destroy(newPawnObject);
            return;
        }
        // Pawn 컴포넌트에 ID 설정

        // Pawn 컴포넌트에 기본 설정 적용
        newPawn.PawnConfig = pawnSerializationContainer.pawnConfig;

        // 보조 컴포넌트 데이터 적용 (Newtonsoft.Json 사용)
        // components 딕셔너리를 직접 순회합니다.
        foreach (var componentData in pawnSerializationContainer.components)
        {
            string configTypeName = componentData.Key;
            string configJson = componentData.Value;

            try
            {
                // 1. Config 클래스 이름으로 Type 객체를 가져옵니다.
                Type configType = Type.GetType($"Game.Core.{configTypeName}");
                if (configType == null)
                {
                    Debug.LogWarning($"Config 타입 '{configTypeName}'을 찾을 수 없습니다. 어셈블리를 확인하세요.");
                    continue;
                }

                // 2. JSON 문자열을 해당 Config 타입으로 역직렬화합니다.
                // JsonUtility 대신 Newtonsoft.Json을 사용합니다.
                object configObject = JsonConvert.DeserializeObject(configJson, configType);
                if (configObject == null) continue;

                // 3. Config 이름에서 "Config"를 "Component"로 교체하여 컴포넌트 타입을 추론합니다.
                string componentTypeName = configTypeName.Replace("Config", "Component");

                // 4. 추론한 컴포넌트 타입 이름으로 실제 컴포넌트를 가져옵니다.
                Type componentType = Type.GetType($"Game.Core.{componentTypeName}");
                if (componentType == null)
                {
                    Debug.LogWarning($"컴포넌트 타입 '{componentTypeName}'을 찾을 수 없습니다. 어셈블리를 확인하세요.");
                    continue;
                }

                // 5. 게임 오브젝트에 해당 컴포넌트가 없다면 추가합니다.
                Component targetComponent = newPawnObject.GetComponent(componentType);
                if (targetComponent == null)
                {
                    targetComponent = newPawnObject.AddComponent(componentType);
                }

                if (targetComponent != null)
                {
                    // 6. 컴포넌트에서 "Initialize" 메서드를 찾습니다.
                    MethodInfo applyMethod = targetComponent.GetType().GetMethod("Initialize");
                    if (applyMethod != null)
                    {
                        // 7. 찾은 메서드를 리플렉션을 통해 동적으로 호출합니다.
                        applyMethod.Invoke(targetComponent, new object[] { configObject });
                    }
                    else
                    {
                        Debug.LogWarning($"컴포넌트 '{componentTypeName}'에 'Initialize' 메서드가 없습니다.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"컴포넌트 데이터 적용 중 오류 발생 ({configTypeName}): {ex.Message}");
            }
        }

        Debug.Log($"ID {newPawn.PawnConfig.Id} Pawn 스폰 및 설정 완료!");
    }




}

using Game.Core; // ManagerBase, PawnTypeData, PawnTypesWrapperData 등이 여기에 있다고 가정
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PawnType 데이터를 로드하고 관리하며, 다른 시스템에서 조회할 수 있도록 제공하는 클래스입니다.
/// 실제 JSON 파싱은 JsonLoader에 위임합니다.
/// 이 컴포넌트는 Unity 씬에 존재해야 합니다.
/// </summary>
public class JsonDataManager : ManagerBase
{
    #region

    /// <summary>
    /// 0. PawnType 데이터의 경로를 저장하는 리스트입니다.
    /// </summary>
    private readonly List<string> paths = new()
    {
        "Data/PawnTypes",
    };



    enum JsonDataType
    {
        PawnData, // PawnType 데이터

    }



    private readonly List<PawnData> pawns = new();


    #endregion


    public override void RegisterAbilities()
    {
        // GameManager에서 이 클래스가 초기 로딩을 시작해야 할 시점을 이벤트로 받을 수 있습니다.
        // 예를 들어, 게임 초기화 이벤트에 반응하여 데이터를 로드합니다.
        AddAction(GameEventType.JsonLoading, OnJsonLoading); // 또는 CustomLifecycleManager가 호출할 이벤트
        AddAction(GameEventType.GetPawnData, GetPawnData); // 또는 CustomLifecycleManager가 호출할 이벤트

    }


    private void OnJsonLoading(GameEventContext gameEventContext)
    {
        pawns.Add(LoadPawnDataList(paths[0])[0]);
    }

    private void GetPawnData(GameEventContext gameEventContext)
    {
        gameEventContext.PawnData = pawns[0];
    }



    public List<PawnData> LoadPawnDataList(string filePath)
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(filePath);

        if (jsonTextAsset == null)
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다: {filePath}");
            return null;
        }

        try
        {
            // 1. JSON을 List<JObject> 형태로 먼저 역직렬화합니다.
            // 이렇게 하면 IBaseConfig를 직접 인스턴스화하려 하지 않습니다.
            List<JObject> rawDataList = JsonConvert.DeserializeObject<List<JObject>>(jsonTextAsset.text);

            if (rawDataList == null)
            {
                Debug.LogWarning($"JSON 파일 '{filePath}'에 데이터가 없습니다.");
                return new List<PawnData>();
            }

            var pawnDataList = new List<PawnData>();
            foreach (var rawData in rawDataList)
            {
                // 2. JObject에서 pawnConfig 부분을 역직렬화합니다.
                PawnConfig pawnConfig = rawData["pawnConfig"]?.ToObject<PawnConfig>();

                if (pawnConfig == null)
                {
                    Debug.LogError("PawnConfig 데이터를 찾을 수 없습니다.");
                    continue;
                }

                // 3. components 부분을 수동으로 역직렬화합니다.
                var processedConfigs = new Dictionary<string, IBaseConfig>();
                if (rawData.TryGetValue("components", out JToken componentsToken))
                {
                    foreach (var componentPair in (JObject)componentsToken)
                    {
                        string componentName = componentPair.Key;
                        if (ComponentMapping.ComponentMap.TryGetValue(componentName, out (Type ComponentType, Type ConfigType) mapping))
                        {
                            try
                            {
                                // 4. 매핑된 타입으로 역직렬화합니다.
                                IBaseConfig configObject = (IBaseConfig)componentPair.Value.ToObject(mapping.ConfigType);
                                if (configObject != null)
                                {
                                    processedConfigs.Add(componentName, configObject);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError($"'{componentName}' Config 변환 중 오류 발생: {ex.Message}");
                            }
                        }
                    }
                }

                pawnDataList.Add(new PawnData(pawnConfig, processedConfigs));
            }

            return pawnDataList;
        }
        catch (JsonException ex)
        {
            Debug.LogError($"JSON 역직렬화 중 오류 발생: {ex.Message}");
            return null;
        }
    }

    // ProcessAndCreatePawnData 메서드는 더 이상 필요하지 않습니다.
    // 모든 로직이 LoadPawnDataList로 통합되었기 때문입니다.

    //private PawnData ProcessAndCreatePawnData(PawnSerializationContainer container)
    //{
    //    if (container?.pawnConfig == null)
    //    {
    //        Debug.LogError("PawnSerializationContainer가 유효하지 않습니다.");
    //        return null;
    //    }
    //    var processedConfigs = new Dictionary<string, IBaseConfig>();
    //    if (container.components != null)
    //    {
    //        foreach (var componentEntry in container.components)
    //        {
    //            string componentName = componentEntry.Key;
    //            object rawConfigData = componentEntry.Value;
    //            if (_componentConfigMap.TryGetValue(componentName, out Type configType) && configType != null)
    //            {
    //                try
    //                {
    //                    IBaseConfig configObject = null;
    //                    if (rawConfigData != null)
    //                    {
    //                        JObject jObject = (JObject)rawConfigData;
    //                        configObject = (IBaseConfig)jObject.ToObject(configType);
    //                    }
    //                    else
    //                    {
    //                        if (configType == typeof(EmptyConfig))
    //                        {
    //                            configObject = new EmptyConfig();
    //                        }
    //                    }
    //                    if (configObject != null)
    //                    {
    //                        processedConfigs.Add(componentName, configObject);
    //                    }
    //                }
    //                catch (Exception ex)
    //                {
    //                    Debug.LogError($"'{componentName}' Config 변환 중 오류 발생: {ex.Message}");
    //                }
    //            }
    //        }
    //    }
    //    return new PawnData(container.pawnConfig, processedConfigs);
    ////}


}
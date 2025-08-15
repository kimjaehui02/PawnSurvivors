using Game.Core;
using Game.Core.Base;
using Game.Core.Configs;
using Game.Core.Contexts;
using Game.Core.Enums;
using Game.Core.Mapping;
using Game.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JsonDataManager : ManagerBase
{
    private readonly List<string> paths = new()
    {
        "Data/PawnTypes",
    };

    private readonly List<PawnData> pawns = new();



    public override void RegisterAbilities()
    {
        AddAction(GameEventType.JsonLoading, OnJsonLoading);
        AddAction(GameEventType.GetPawnData, GetPawnData);
        AddAction(GameEventType.GetPawnDatas, GetPawnDatas);
    }

    private void OnJsonLoading(GameEventContext gameEventContext)
    {
        var loaded = LoadDataList(paths[0], ConvertToPawnData);
        if (loaded.Count > 0)
            pawns.AddRange(loaded);
        else
            Debug.LogWarning("PawnData 로딩 결과가 비어있습니다.");
    }

    private void GetPawnData(GameEventContext gameEventContext)
    {
        // 인덱스 범위 검사
        if (gameEventContext.PawnDataIndex < 0 ||
            gameEventContext.PawnDataIndex >= pawns.Count)
        {
            Debug.LogWarning("유효하지 않은 PawnDataIndex");
            return;
        }

        var originalPawnData = pawns[gameEventContext.PawnDataIndex];
        if (originalPawnData == null)
        {
            Debug.LogWarning("해당 인덱스의 Pawn 데이터가 null");
            return;
        }

        // 깊은 복사
        gameEventContext.PawnData = originalPawnData.Clone();
    }

    private void GetPawnDatas(GameEventContext gameEventContext)
    {
        if (pawns == null || pawns.Count == 0)
        {
            Debug.LogWarning("Pawn 데이터 목록이 비어있음");
            return;
        }

        // 모든 PawnData를 깊은 복사
        gameEventContext.PawnDatas = pawns
            .Where(p => p != null) // null 방지
            .Select(p => p.Clone())
            .ToList();
    }


    // ====== 범용 JSON 로딩 + 변환 ======
    private string LoadJsonText(string filePath)
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(filePath);
        if (jsonTextAsset == null)
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다: {filePath}");
            return null;
        }
        return jsonTextAsset.text;
    }

    private List<JObject> ParseToJObjectList(string jsonText)
    {
        try
        {
            return JsonConvert.DeserializeObject<List<JObject>>(jsonText);
        }
        catch (JsonException ex)
        {
            Debug.LogError($"JSON 파싱 오류: {ex.Message}");
            return null;
        }
    }

    private List<T> LoadDataList<T>(string filePath, Func<JObject, T> converter)
    {
        var result = new List<T>();

        string jsonText = LoadJsonText(filePath);
        if (string.IsNullOrEmpty(jsonText))
            return result;

        List<JObject> rawDataList = ParseToJObjectList(jsonText);
        if (rawDataList == null || rawDataList.Count == 0)
        {
            Debug.LogWarning($"{typeof(T).Name} JSON 데이터가 비어있습니다.");
            return result;
        }

        foreach (var rawData in rawDataList)
        {
            try
            {
                var item = converter(rawData);
                if (item != null)
                    result.Add(item);
            }
            catch (Exception ex)
            {
                Debug.LogError($"{typeof(T).Name} 변환 중 오류 발생: {ex.Message}");
            }
        }

        return result;
    }

    // ====== PawnData 변환 로직 ======
    private PawnData ConvertToPawnData(JObject rawData)
    {
        PawnConfig pawnConfig = rawData["pawnConfig"]?.ToObject<PawnConfig>();
        if (pawnConfig == null)
        {
            Debug.LogError("PawnConfig 데이터를 찾을 수 없습니다.");
            return null;
        }

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
                        IBaseConfig configObject = (IBaseConfig)componentPair.Value.ToObject(mapping.ConfigType);
                        if (configObject != null)
                            processedConfigs.Add(componentName, configObject);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"'{componentName}' Config 변환 중 오류 발생: {ex.Message}");
                    }
                }
            }
        }

        return new PawnData(pawnConfig, processedConfigs);
    }
}

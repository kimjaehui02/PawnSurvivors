using Game.Core; // ManagerBase, PawnTypeData, PawnTypesWrapperData 등이 여기에 있다고 가정
using Newtonsoft.Json;
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


    public Dictionary<JsonPath, string> JsonPaths { get; set; } = new Dictionary<JsonPath, string>
    {
        { JsonPath.pawns, "Data/PawnTypes" } // JSON 파일 경로를 여기에 추가
    };



    public override void RegisterAbilities()
    {
        // GameManager에서 이 클래스가 초기 로딩을 시작해야 할 시점을 이벤트로 받을 수 있습니다.
        // 예를 들어, 게임 초기화 이벤트에 반응하여 데이터를 로드합니다.
        AddAction(GameEventType.JsonLoading, OnJsonLoading); // 또는 CustomLifecycleManager가 호출할 이벤트
    }

    // Awake에서 JsonLoader 참조를 확인하고 초기화 로직을 시작할 수 있습니다.


    // 게임 초기화 이벤트에 반응하여 Pawn 데이터를 로드하는 메서드
    private void OnJsonLoading(GameEventContext context)
    {
        context.PawnData = LoadJsonFile(context.JsonPath);

    }

    // 기존 코드에서 기본 매개변수 값으로 인스턴스 필드(_pawnDataJsonPath)를 사용할 수 없으므로
    // 기본값을 null로 지정하고, 내부에서 null일 경우 _pawnDataJsonPath를 사용하도록 변경합니다.

    // 이 메서드를 수정해야 합니다.
    public PawnSerializationContainer LoadJsonFile(JsonPath input)
    {
        if (!JsonPaths.TryGetValue(input, out string filePath))
        {
            Debug.LogError($"Enum '{input}'에 대한 경로가 딕셔너리에 없습니다.");
            return null;
        }

        TextAsset jsonTextAsset = Resources.Load<TextAsset>(filePath);
        if (jsonTextAsset == null)
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다: {filePath}");
            return null;
        }

        try
        {
            // JsonUtility 대신 JsonConvert를 사용합니다.
            PawnSerializationContainer container = JsonConvert.DeserializeObject<PawnSerializationContainer>(jsonTextAsset.text);

            // JsonConvert는 null을 반환하지 않고 예외를 발생시키므로, 예외 처리로 충분합니다.
            return container;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"JSON 역직렬화 실패 ({filePath}): {ex.Message}");
            return null;
        }
    }


}
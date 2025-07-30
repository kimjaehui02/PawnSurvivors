// PawnDataLoader.cs
using UnityEngine;
using System;
using System.Collections.Generic;
using Game.Core; // ManagerBase, PawnTypeData, PawnTypesWrapperData 등이 여기에 있다고 가정

/// <summary>
/// PawnType 데이터를 로드하고 관리하며, 다른 시스템에서 조회할 수 있도록 제공하는 클래스입니다.
/// 실제 JSON 파싱은 JsonLoader에 위임합니다.
/// 이 컴포넌트는 Unity 씬에 존재해야 합니다.
/// </summary>
public class JsonDataManager : ManagerBase
{
    // JsonLoader 인스턴스에 대한 참조 (Unity 에디터에서 할당 또는 코드로 찾음)
    [SerializeField]
    private JsonLoader _jsonLoader; // JsonLoader 참조!

    // PawnType 데이터 JSON 파일의 Resources 경로 (에디터에서 설정)
    [SerializeField]
    private string _pawnDataJsonPath = "Data/PawnTypes";

    // 로드된 Pawn 타입 데이터를 저장할 딕셔너리
    private readonly Dictionary<string, PawnTypeData> _pawnTypeDefinitions = new();

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
        if (_jsonLoader == null) // Awake에서 찾지 못했을 경우 대비
        {
            Debug.LogError("PawnDataLoader: JsonLoader가 없어 Pawn 데이터 로딩을 시작할 수 없습니다.");
            return;
        }
        LoadAndStorePawnData(); // 데이터 로딩 및 저장 시작
    }

    /// <summary>
    /// JsonLoader를 사용하여 PawnType 정의 JSON 파일을 로드하고 파싱하여 내부 딕셔너리에 저장합니다.
    /// </summary>
    private void LoadAndStorePawnData() // 이름 변경 제안: LoadAndStorePawnData (로드하고 저장)
    {
        // JsonLoader의 범용 LoadJson<T> 메서드를 사용하여 PawnTypesWrapperData를 로드
        PawnTypesWrapperData wrapper = _jsonLoader.LoadJson<PawnTypesWrapperData>(_pawnDataJsonPath);

        if (wrapper != null && wrapper.PawnTypes != null)
        {
            foreach (PawnTypeData pawnData in wrapper.PawnTypes)
            {
                if (string.IsNullOrEmpty(pawnData.name))
                {
                    Debug.LogWarning("PawnDataLoader: 이름이 없는 PawnType 정의를 건너뜝니다.");
                    continue;
                }
                if (_pawnTypeDefinitions.ContainsKey(pawnData.name))
                {
                    Debug.LogWarning($"PawnDataLoader: 중복된 PawnType 이름 '{pawnData.name}'이(가) 발견되었습니다. 첫 번째 정의를 사용합니다.");
                    continue;
                }
                _pawnTypeDefinitions.Add(pawnData.name, pawnData);
                Debug.Log($"PawnDataLoader: PawnType '{pawnData.name}' 로드 완료.");
            }
            Debug.Log($"PawnDataLoader: 총 {_pawnTypeDefinitions.Count}개의 PawnType 로드 완료.");
        }
        else
        {
            Debug.LogError("PawnDataLoader: JSON 파일 파싱 실패 또는 PawnTypes 데이터가 유효하지 않습니다. JSON 구조를 확인하세요.");
        }
    }

    /// <summary>
    /// 로드된 PawnType 데이터를 이름으로 조회합니다.
    /// </summary>
    /// <param name="pawnTypeName">조회할 PawnType의 이름입니다.</param>
    /// <returns>해당 PawnTypeData 객체입니다. 이름에 해당하는 PawnType이 없으면 null을 반환합니다.</returns>
    public PawnTypeData GetPawnTypeData(string pawnTypeName)
    {
        _pawnTypeDefinitions.TryGetValue(pawnTypeName, out PawnTypeData pawnData);
        return pawnData;
    }
}
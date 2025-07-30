using UnityEngine;
using System;
using System.Collections.Generic;
using Game.Core; // ManagerBase, 그리고 PawnTypeData와 PawnTypesWrapperData가 있는 네임스페이스

/// <summary>
/// PawnType 데이터를 로드하고 관리하며, 다른 시스템에서 조회할 수 있도록 제공하는 클래스입니다.
/// 실제 JSON 파싱은 JsonDataManager에 위임합니다.
/// </summary>
public class PawnDataLoader : ManagerBase
{
    // JsonDataManager 인스턴스에 대한 참조입니다. Unity 에디터에서 할당하거나 코드로 찾을 수 있습니다.
    [SerializeField]
    private JsonDataManager _JsonDataManager;

    // PawnType 데이터 JSON 파일의 Resources 경로입니다. 에디터에서 설정 가능합니다.
    //[SerializeField]
    //private string _pawnDataJsonPath = "Data/PawnTypes"; // 기본값 제공

    // 로드된 Pawn 타입 데이터를 저장할 딕셔너리입니다.
    private readonly Dictionary<string, PawnTypeData> _pawnTypeDefinitions = new();

    public override void RegisterAbilities()
    {
        // GameManager와 같은 초기화 관리자로부터 로딩 시작 이벤트를 받을 수 있습니다.
        // 예: AddAction(GameEventType.GameInitialization, OnGameInitialization);
    }

    void Awake()
    {

        LoadAllPawnData(); // 게임 시작 시 Pawn 데이터 로딩 시작
    }

    /// <summary>
    /// JsonDataManager를 사용하여 PawnType 정의 JSON 파일을 로드하고 파싱합니다.
    /// 로드된 데이터는 내부 딕셔너리에 저장됩니다.
    /// </summary>
    private void LoadAllPawnData()
    {
        //// JsonDataManager의 범용 LoadJson<T> 메서드를 사용하여 PawnTypesWrapperData를 로드합니다.
        //// 여기서 T는 PawnTypesWrapperData이며, 이는 Game.Core 네임스페이스에 정의되어 있습니다.
        //PawnTypesWrapperData wrapper = _JsonDataManager.LoadJson<JsonDataManager>(_pawnDataJsonPath);

        //if (wrapper != null && wrapper.PawnTypes != null)
        //{
        //    foreach (PawnTypeData pawnData in wrapper.PawnTypes)
        //    {
        //        if (string.IsNullOrEmpty(pawnData.name))
        //        {
        //            Debug.LogWarning("PawnDataLoader: 이름이 없는 PawnType 정의를 건너뜝니다.");
        //            continue;
        //        }
        //        if (_pawnTypeDefinitions.ContainsKey(pawnData.name))
        //        {
        //            Debug.LogWarning($"PawnDataLoader: 중복된 PawnType 이름 '{pawnData.name}'이(가) 발견되었습니다. 첫 번째 정의를 사용합니다.");
        //            continue;
        //        }
        //        _pawnTypeDefinitions.Add(pawnData.name, pawnData);
        //        Debug.Log($"PawnDataLoader: PawnType '{pawnData.name}' 로드 완료.");
        //    }
        //    Debug.Log($"PawnDataLoader: 총 {_pawnTypeDefinitions.Count}개의 PawnType 로드 완료.");
        //}
        //else
        //{
        //    Debug.LogError("PawnDataLoader: JSON 파일 파싱 실패 또는 PawnTypes 데이터가 유효하지 않습니다. JSON 구조를 확인하세요.");
        //}
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
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection; // Reflection을 위해 필요

/// <summary>
/// JSON에서 Pawn 타입 정의를 로드하고, 해당 정의에 따라 Pawn GameObject를 설정하는 클래스입니다.
/// MonoBehaviour를 상속받아 Unity 씬에 배치할 수 있으며, Awake()에서 자동으로 데이터를 로드합니다.
/// 싱글톤 패턴을 사용하여 어디서든 접근 가능하도록 구현했습니다.
/// </summary>
public class PawnDataLoader : MonoBehaviour
{
    // 싱2

    // 모든 Pawn 타입 데이터를 저장할 딕셔너리
    private Dictionary<string, PawnTypeData> _pawnTypeDefinitions = new Dictionary<string, PawnTypeData>();

    // 로드할 JSON 파일의 Resources 경로
    [SerializeField] // Unity 에디터에서 설정 가능하도록 노출
    private string _pawnTypesJsonPath = "Data/PawnTypes"; // 기본값 제공

    // --- Unity 생명 주기 메서드 ---


    // --- 데이터 로딩 메서드 ---

    /// <summary>
    /// Resources 폴더에서 PawnType 정의 JSON 파일을 로드하고 파싱합니다.
    /// </summary>
    private void LoadPawnTypesFromJson()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(_pawnTypesJsonPath);
        if (jsonTextAsset == null)
        {
            Debug.LogError($"PawnDataLoader: JSON 파일 '{_pawnTypesJsonPath}.json'을 Resources에서 찾을 수 없습니다.");
            return;
        }

        try
        {
            // JsonUtility는 최상위 객체만 직접 파싱할 수 있으므로, PawnTypesData 래퍼 클래스를 사용
            PawnTypesWrapperData wrapper = JsonUtility.FromJson<PawnTypesWrapperData>(jsonTextAsset.text);

            if (wrapper != null && wrapper.PawnTypes != null)
            {
                foreach (PawnTypeData pawnData in wrapper.PawnTypes)
                {
                    if (string.IsNullOrEmpty(pawnData.name))
                    {
                        Debug.LogWarning("PawnDataLoader: 이름이 없는 PawnType 정의를 건너뜁니다.");
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
                Debug.LogError("PawnDataLoader: JSON 파일 파싱 실패 또는 PawnTypes 데이터가 유효하지 않습니다.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"PawnDataLoader: JSON 파싱 오류 발생: {e.Message}\n{e.StackTrace}");
        }
    }

    // --- Pawn 설정 메서드 ---

    /// <summary>
    /// 지정된 Pawn 타입에 따라 GameObject에 컴포넌트들을 추가하고 설정합니다.
    /// </summary>
    /// <param name="pawnGameObject">컴포넌트들을 추가할 GameObject (기본 Pawn 프리랩 인스턴스).</param>
    /// <param name="pawnTypeName">생성할 Pawn 타입의 이름 (예: "Goblin").</param>
    public void ConfigurePawnFromType(GameObject pawnGameObject, string pawnTypeName)
    {
        if (!_pawnTypeDefinitions.TryGetValue(pawnTypeName, out PawnTypeData pawnData))
        {
            Debug.LogError($"PawnDataLoader: '{pawnTypeName}' 타입의 Pawn 정의를 찾을 수 없습니다.");
            return;
        }

        Debug.Log($"PawnDataLoader: '{pawnTypeName}' 타입으로 '{pawnGameObject.name}' 설정 시작.");

        foreach (ComponentData compData in pawnData.components)
        {
            Type componentType = Type.GetType(compData.type);

            if (componentType == null || !typeof(MonoBehaviour).IsAssignableFrom(componentType))
            {
                Debug.LogError($"PawnDataLoader: 알 수 없는 컴포넌트 타입 '{compData.type}' for '{pawnTypeName}'. 어셈블리 이름이 필요한 경우 System.Type.GetType(\"Namespace.ClassName, AssemblyName\") 형식을 사용하거나, 타입이 MonoBehaviour가 아닙니다.");
                continue;
            }

            // 이미 해당 컴포넌트가 GameObject에 있는지 확인하고, 없으면 추가
            MonoBehaviour componentInstance = pawnGameObject.GetComponent(componentType) as MonoBehaviour;
            if (componentInstance == null)
            {
                componentInstance = pawnGameObject.AddComponent(componentType) as MonoBehaviour;
                if (componentInstance == null)
                {
                    Debug.LogError($"PawnDataLoader: '{compData.type}' 컴포넌트를 '{pawnGameObject.name}'에 추가하지 못했습니다.");
                    continue;
                }
            }

            // properties가 존재하는 경우에만 처리
            // JsonUtility는 딕셔너리를 직접 지원하지 않으므로, properties 객체를 raw string으로 가져와서
            // 다시 JsonUtility.FromJsonOverwrite()를 사용하거나, Newtonsoft.Json을 사용하는 것이 가장 좋습니다.
            // 여기서는 JsonUtility만 사용하는 경우를 가정하여 각 컴포넌트의 properties를 해당 컴포넌트 타입으로 직접 파싱합니다.
            // 이렇게 하려면 각 컴포넌트의 public/SerializeField 필드가 JSON key와 일치해야 합니다.
            if (compData.properties != null && compData.properties.Length > 0 && compData.properties != "{}") // 빈 객체 {}도 확인
            {
                try
                {
                    // JSON string을 기존 컴포넌트 인스턴스에 덮어씌웁니다.
                    // 이 방식은 componentInstance의 public/SerializeField 필드 이름이 JSON의 properties 키와 정확히 일치해야 합니다.
                    JsonUtility.FromJsonOverwrite(compData.properties, componentInstance);
                }
                catch (Exception e)
                {
                    Debug.LogError($"PawnDataLoader: '{pawnTypeName}' - '{compData.type}' 컴포넌트 속성 설정 오류: {e.Message}. Properties JSON: '{compData.properties}'");
                }
            }
        }
        Debug.Log($"PawnDataLoader: '{pawnTypeName}' 타입 설정 완료.");
    }
}

// --- JSON 데이터를 저장하기 위한 도우미 클래스들 ---
// 이 클래스들은 [Serializable] 어트리뷰트가 필요하며, public 필드로 선언해야 JsonUtility가 인식합니다.
// 이 파일 내부에 중첩 클래스로 두거나, 별도의 파일 (예: PawnTypeData.cs)로 분리할 수 있습니다.

// JSON의 최상위 PawnTypes 배열을 담는 래퍼 클래스 (JsonUtility의 요구사항)
[Serializable]
public class PawnTypesWrapperData
{
    public PawnTypeData[] PawnTypes; // JSON의 "PawnTypes": [] 배열과 매핑
}

// 각 PawnType 정의
[Serializable]
public class PawnTypeData
{
    public string name;
    public string description;
    public ComponentData[] components; // JSON의 "components": [] 배열과 매핑
}

// 각 Component 정의
[Serializable]
public class ComponentData
{
    public string type; // 컴포넌트 클래스 이름 (예: "PawnHealthComponent")

    // properties 객체를 JSON 문자열 그대로 저장합니다.
    // JsonUtility는 Dictionary<string, object>를 직접 지원하지 않으므로,
    // 이 문자열을 받은 후 다시 FromJsonOverwrite로 해당 컴포넌트 인스턴스에 적용합니다.
    public string properties;
}
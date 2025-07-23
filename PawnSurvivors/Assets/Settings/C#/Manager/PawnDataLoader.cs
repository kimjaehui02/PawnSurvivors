using System;
using System.Collections.Generic;
using System.Xml.Linq; // LINQ to XML 사용
using UnityEngine; // TextAsset, Debug, GameObject, MonoBehaviour, Resources 등을 위해 필요

/// <summary>
/// XML에서 Pawn 타입 정의를 로드하고, 해당 정의에 따라 Pawn GameObject를 설정하는 클래스입니다.
/// 싱글톤이 아니므로, 사용하는 곳에서 인스턴스화해야 합니다.
/// </summary>
public class PawnDataLoader
{
    // 모든 Pawn 타입 데이터를 저장할 딕셔너리
    private Dictionary<string, PawnTypeData> _pawnTypeDefinitions = new Dictionary<string, PawnTypeData>();

    // 로드할 XML 파일의 Resources 경로 (생성자 또는 메서드 인자로 받을 수 있음)
    private string _pawnTypesXmlPath;

    /// <summary>
    /// PawnDataLoader를 초기화하고 XML에서 Pawn 타입 정의를 로드합니다.
    /// </summary>
    /// <param name="xmlPath">Assets/Resources/ 내의 XML 파일 경로 (예: "Data/PawnTypes")</param>
    public PawnDataLoader(string xmlPath = "Data/PawnTypes") // 기본값 제공
    {
        _pawnTypesXmlPath = xmlPath;
        LoadPawnTypesFromXml(); // 생성 시 XML 데이터 로드
    }

    /// <summary>
    /// Resources 폴더에서 PawnType 정의 XML 파일을 로드하고 파싱합니다.
    /// </summary>
    private void LoadPawnTypesFromXml()
    {
        TextAsset xmlTextAsset = Resources.Load<TextAsset>(_pawnTypesXmlPath);
        if (xmlTextAsset == null)
        {
            Debug.LogError($"PawnDataLoader: XML 파일 '{_pawnTypesXmlPath}.xml'을 Resources에서 찾을 수 없습니다.");
            return;
        }

        try
        {
            XDocument doc = XDocument.Parse(xmlTextAsset.text);
            foreach (XElement pawnTypeElement in doc.Root.Elements("PawnType"))
            {
                PawnTypeData pawnData = new PawnTypeData();
                pawnData.Name = pawnTypeElement.Attribute("name")?.Value;
                pawnData.Description = pawnTypeElement.Attribute("description")?.Value;

                if (string.IsNullOrEmpty(pawnData.Name))
                {
                    Debug.LogWarning("PawnDataLoader: 이름이 없는 PawnType 정의를 건너킵니다.");
                    continue;
                }

                XElement componentsElement = pawnTypeElement.Element("Components");
                if (componentsElement != null)
                {
                    foreach (XElement componentElement in componentsElement.Elements("Component"))
                    {
                        ComponentData compData = new ComponentData();
                        compData.Type = componentElement.Attribute("type")?.Value;

                        if (string.IsNullOrEmpty(compData.Type))
                        {
                            Debug.LogWarning($"PawnType '{pawnData.Name}': 타입이 없는 Component 정의를 건너킵니다.");
                            continue;
                        }

                        foreach (XElement propElement in componentElement.Elements("Property"))
                        {
                            PropertyData propData = new PropertyData();
                            propData.Name = propElement.Attribute("name")?.Value;
                            propData.Value = propElement.Attribute("value")?.Value;
                            if (!string.IsNullOrEmpty(propData.Name))
                            {
                                compData.Properties.Add(propData);
                            }
                        }
                        pawnData.Components.Add(compData);
                    }
                }
                _pawnTypeDefinitions.Add(pawnData.Name, pawnData);
                Debug.Log($"PawnDataLoader: PawnType '{pawnData.Name}' 로드 완료.");
            }
            Debug.Log($"PawnDataLoader: 총 {_pawnTypeDefinitions.Count}개의 PawnType 로드 완료.");
        }
        catch (Exception e)
        {
            Debug.LogError($"PawnDataLoader: XML 파싱 오류 발생: {e.Message}");
        }
    }

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

        foreach (ComponentData compData in pawnData.Components)
        {
            Type componentType = Type.GetType(compData.Type);

            if (componentType == null)
            {
                Debug.LogError($"PawnDataLoader: 알 수 없는 컴포넌트 타입 '{compData.Type}' for '{pawnTypeName}'. 어셈블리 이름이 필요한 경우 System.Type.GetType(\"Namespace.ClassName, AssemblyName\") 형식을 사용하세요.");
                continue;
            }

            MonoBehaviour componentInstance = pawnGameObject.GetComponent(componentType) as MonoBehaviour;
            if (componentInstance == null)
            {
                componentInstance = pawnGameObject.AddComponent(componentType) as MonoBehaviour;
                if (componentInstance == null)
                {
                    Debug.LogError($"PawnDataLoader: '{compData.Type}' 컴포넌트를 '{pawnGameObject.name}'에 추가하지 못했습니다.");
                    continue;
                }
            }

            foreach (PropertyData propData in compData.Properties)
            {
                System.Reflection.FieldInfo field = componentType.GetField(propData.Name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    try
                    {
                        field.SetValue(componentInstance, Convert.ChangeType(propData.Value, field.FieldType));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"PawnDataLoader: '{pawnTypeName}' - '{compData.Type}.{propData.Name}' 필드 값 설정 오류: {e.Message}. 값: '{propData.Value}', 예상 타입: '{field.FieldType}'");
                    }
                    continue;
                }

                System.Reflection.PropertyInfo property = componentType.GetProperty(propData.Name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (property != null && property.CanWrite)
                {
                    try
                    {
                        property.SetValue(componentInstance, Convert.ChangeType(propData.Value, property.PropertyType));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"PawnDataLoader: '{pawnTypeName}' - '{compData.Type}.{propData.Name}' 프로퍼티 값 설정 오류: {e.Message}. 값: '{propData.Value}', 예상 타입: '{property.PropertyType}'");
                    }
                    continue;
                }

                Debug.LogWarning($"PawnDataLoader: '{pawnTypeName}' - '{compData.Type}'에 '{propData.Name}' 필드/프로퍼티를 찾을 수 없거나 설정할 수 없습니다. Public 또는 [SerializeField]로 선언되었는지 확인하세요.");
            }
        }
        Debug.Log($"PawnDataLoader: '{pawnTypeName}' 타입 설정 완료.");
    }
}

// XML 데이터를 저장하기 위한 도우미 클래스들
// 이 클래스들은 PawnDataLoader.cs 파일 내부에 중첩 클래스로 두거나,
// 별도의 파일 (예: PawnTypeData.cs)로 분리할 수 있습니다.
[Serializable]
public class PawnTypeData
{
    public string Name;
    public string Description;
    public List<ComponentData> Components = new List<ComponentData>();
}

[Serializable]
public class ComponentData
{
    public string Type; // 컴포넌트 클래스 이름 (예: "PawnHealthComponent")
    public List<PropertyData> Properties = new List<PropertyData>();
}

[Serializable]
public class PropertyData
{
    public string Name;  // 속성 이름 (예: "MaxHealth")
    public string Value; // 속성 값 (문자열, 파싱 필요)
}
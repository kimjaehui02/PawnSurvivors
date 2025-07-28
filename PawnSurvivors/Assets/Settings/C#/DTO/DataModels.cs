// PawnTypeData.cs (또는 DataModels.cs)
using System;
using System.Xml.Serialization; // XML을 사용하므로 XmlSerializer 관련 어트리뷰트 필요

namespace Game.Core // 여기에 네임스페이스를 추가합니다.
{
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
}
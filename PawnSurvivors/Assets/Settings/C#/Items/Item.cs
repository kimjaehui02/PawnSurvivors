using UnityEngine;


// 아이템이 영향을 주는것
// 체력이나 이동속도 등 다른 보조기능 컴포넌트에 증감을 해줄수도 있고
// 보조기능컴포넌트처럼 새로운 델리게이트 함수가 존재할 수도 있다
// 보조기능 컴포넌트의 종류도 enum형으로 정리한뒤
// xml작성같은걸 할때 보조기능 컴포넌트이름 하고 그 아래에 보조값 넣기 가능할수도 있다

// 

/// <summary>
/// 아이템의 기본형 값이다
/// 일단 무기가 될수도있고
/// 패시브아이템이 될수도잇고
/// 유니티객체가 될수도 아닐수도 있다
/// </summary>

public class Item
{
    // Item의 고유 ID입니다. 내부에서만 변경 가능합니다.
    public string Id { get; private set; }

    public string Name { get; private set; }


}

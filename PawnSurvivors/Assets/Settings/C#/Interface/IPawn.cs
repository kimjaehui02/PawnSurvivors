using UnityEngine;

public interface IPawn
{
    // 폰의 고유 식별자 (모든 폰은 ID를 가져야 함)
    string Id { get; }

    // 폰의 초기화 메서드 (PawnData 등)
    //void Initialize(PawnData data);

    // 기타 폰의 공통적인 고수준 메서드 (예: OnPawnFullyDied 이벤트 등)
}

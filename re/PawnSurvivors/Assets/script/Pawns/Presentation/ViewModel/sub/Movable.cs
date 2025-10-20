using UnityEngine;

// Movable 클래스
// 이 클래스는 PawnSub를 상속받아 게임 내에서 이동 가능한 오브젝트의 동작 및 이동 로직을 구현하는 데 사용됩니다.

public class Movable : PawnSub
{


    public PawnUseCase pawnUseCase;
    public float MoveSpeed;


    public void Start()
    {


        // 이동 관련 액션을 델리게이트 맵에 추가

        myMap.Add(EnumActions.Move, context =>
        {
            // Movable 변수 적용
            context.MoveSpeed = this.MoveSpeed;
        });
    }


}


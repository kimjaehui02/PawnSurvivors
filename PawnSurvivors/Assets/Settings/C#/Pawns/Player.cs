using UnityEngine;

public class Player : MonoBehaviour
{

    // 반대로 생각해보자
    // 가질 컴포넌트가 아니라
    // 할 수 있는 행동부터 정의하자

    // 플레이어는 
    // 1. 이동하기
    // 2. 피해입기
    // 3. 조작받기


    delegate void PlayerDelegate();

    event PlayerDelegate OnMove;
    event PlayerDelegate OnDamaged;
    event PlayerDelegate OnControl;




}

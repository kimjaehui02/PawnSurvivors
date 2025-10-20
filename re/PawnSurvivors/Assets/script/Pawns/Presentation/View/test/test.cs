using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public B bPrefab;
    public C cPrefab;

    int counter = 0;

    private void Start()
    {


        // B 액션 등록
        bPrefab.RegisterTestActions();

        // B의 액션을 C로 병합
        bPrefab.MergeToC(cPrefab);

        // C에서 호출
        cPrefab.TestInvoke(MyEnum.Test1, 10); // B Action1: 10
        cPrefab.TestInvoke(MyEnum.Test2, 20); // B Action2: 20

        // B 제거
        Destroy(bPrefab);

        // C Map은 정상, B 액션만 제거됨
        cPrefab.TestInvoke(MyEnum.Test1, 30); // 실행 안 됨
        cPrefab.TestInvoke(MyEnum.Test2, 40); // 실행 안 됨
        print("start end");
    }

    private void Update()
    {
        if(counter !=4)
        {             
            print(counter);
            counter++;
            cPrefab.TestInvoke(MyEnum.Test1, 30); // 실행 안 됨
            cPrefab.TestInvoke(MyEnum.Test2, 40); // 실행 안 됨
        }
    }

}

using UnityEngine;

public class NewMonoBehaviourScript12 : MonoBehaviour
{
    // MonoBehaviour가 생성된 후 Update의 첫 실행 전에 한 번 호출됩니다.
    void Start()
    {
        GameObject go = new GameObject("TestObject");

        Debug.Log("Before AddComponent");
        var comp = go.AddComponent<NewMonoBehaviourScript>();
        Debug.Log("After AddComponent");

        // 추가 코드
        comp.enabled = true;
        Debug.Log("Component enabled set");
    }

    // 프레임당 한 번 호출됩니다.
    void Update()
    {
        
    }
}

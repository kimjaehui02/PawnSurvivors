using UnityEngine;

public class NewMonoBehaviourScript12 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    // Update is called once per frame
    void Update()
    {
        
    }
}

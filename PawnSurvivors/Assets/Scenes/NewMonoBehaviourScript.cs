using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    bool isActive = true;

    private void Awake()
    {
        Debug.Log("NewMonoBehaviourScript has awoken.");
    }

    // MonoBehaviour가 생성된 후 Update의 첫 실행 전에 한 번 호출됩니다.
    void Start()
    {
        Debug.Log("NewMonoBehaviourScript has started.");
    }

    // 프레임당 한 번 호출됩니다.
    void Update()
    {
        if (isActive)
        {
            Debug.Log("NewMonoBehaviourScript is updating.");
            isActive = false; // 상태 변경을 보여주기 위함
        }
    }
}

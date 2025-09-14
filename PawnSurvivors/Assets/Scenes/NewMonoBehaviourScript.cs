using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    bool isActive = true;

    private void Awake()
    {
        Debug.Log("NewMonoBehaviourScript has awoken.");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("NewMonoBehaviourScript has started.");
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            Debug.Log("NewMonoBehaviourScript is updating.");
            isActive = false; // Just to demonstrate a change in state
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pawn : MonoBehaviour
{

    public UnityEvent myEvent; // 에디터에서 함수 연결 가능

    public List<Action> GetList;

    private void Awake()
    {
        myEvent.AddListener(Test);
        myEvent.AddListener(() => Debug.Log("lambda test"));
        myEvent.AddListener(() => Debug.Log(Testint()));
        myEvent.AddListener(() => Debug.Log(Testinput("input test")));
    }

    private void Start()
    {
        TriggerEvent();
    }

    public void TriggerEvent()
    {
        myEvent?.Invoke();
    }


    public void Test()
    {
        Debug.Log("test");
    }

    public int Testint()
    {
        return 1111;
    }

    public string Testinput(string input)
    {
        Debug.Log(input);
        return input;
    }

}

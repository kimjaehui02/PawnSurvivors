using System;
using System.Collections.Generic;
using UnityEngine;

public class B : MonoBehaviour
{
    public EnumDelegateMap<MyEnum, int> myMap = new();

    // C별로 내가 추가한 액션 기록
    private Dictionary<C, List<(MyEnum key, Action<int> action)>> addedToC = new();

    // B -> C 병합
    public void MergeToC(C c)
    {
        if (!addedToC.ContainsKey(c))
            addedToC[c] = new List<(MyEnum, Action<int>)>();

        foreach (var kv in myMap.GetAll())
        {
            var key = kv.Key;
            foreach (var action in kv.Value)
            {
                c.myMap.Add(key, action);
                addedToC[c].Add((key, action));
            }
        }
    }

    // 제거 시, 자신이 추가한 것만 제거
    private void OnDestroy()
    {
        foreach (var pair in addedToC)
        {
            C c = pair.Key;
            foreach (var (key, action) in pair.Value)
            {
                c.myMap.Remove(key, action);
            }
        }
    }

    // 테스트용 액션 등록
    // 일반 메서드로 변경
    public void Action1(int x)
    {
        Debug.Log($"{name} : B Action1: {x}");
    }

    public void Action2(int x)
    {
        Debug.Log($"{name} : B Action2: {x}");

    }

    // 등록 메서드
    public void RegisterTestActions()
    {
        myMap.Add(MyEnum.Test1, Action1); // 메서드 그룹 사용
        myMap.Add(MyEnum.Test2, Action2);
    }


}

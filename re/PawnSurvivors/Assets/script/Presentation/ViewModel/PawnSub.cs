using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PawnSub : MonoBehaviour
{
    // 1. 델리게이트 맵을 가지고 있다
    // 2. 

    public EnumDelegateMap<Actions, DataContext> myMap = new();

    // Pawn별로 내가 추가한 액션 기록
    private Dictionary<Pawn, List<(Actions key, Action<DataContext> action)>> addedToMain = new();

    // PawnSub -> Pawn 병합
    public void MergeToMain(Pawn pawn)
    {
        if (!addedToMain.ContainsKey(pawn))
            addedToMain[pawn] = new List<(Actions, Action<DataContext>)>();

        foreach (KeyValuePair<Actions, List<Action<DataContext>>> kv in myMap.GetAll())
        {
            Actions key = kv.Key;
            foreach (Action<DataContext> action in kv.Value)
            {
                pawn.myMap.Add(key, action);
                addedToMain[pawn].Add((key, action));
            }
        }
    }

    // 제거 시, 자신이 추가한 것만 제거
    private void OnDestroy()
    {
        foreach (KeyValuePair<Pawn, List<(Actions, Action<DataContext>)>> pair in addedToMain)
        {
            Pawn pawn = pair.Key;
            foreach ((Actions key, Action<DataContext> action) in pair.Value)
            {
                pawn.myMap.Remove(key, action);
            }
        }
    }



    // 등록 메서드
    public virtual void RegisterTestActions()
    {

    }

    public void Start()
    {
        
    }

    public void Update()
    {

    }
}

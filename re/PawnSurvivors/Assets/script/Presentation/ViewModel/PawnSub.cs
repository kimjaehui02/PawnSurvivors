using System;
using System.Collections.Generic;
using UnityEngine;

public class PawnSub : MonoBehaviour
{
    public EnumDelegateMap<Actions, DataContext> myMap = new();

    // Pawn별로 내가 추가한 액션 기록
    private Dictionary<Pawn, List<(Actions key, Action<DataContext> action)>> addedToMain = new();

    // PawnSub -> Pawn 병합
    public void MergeToMain(Pawn pawn)
    {
        if (!addedToMain.ContainsKey(pawn))
            addedToMain[pawn] = new List<(Actions, Action<DataContext>)>();

        foreach (var kv in myMap.GetAll())
        {
            var key = kv.Key;
            foreach (var action in kv.Value)
            {
                pawn.myMap.Add(key, action);
                addedToMain[pawn].Add((key, action));
            }
        }
    }

    // 제거 시, 자신이 추가한 것만 제거
    private void OnDestroy()
    {
        foreach (var pair in addedToMain)
        {
            Pawn pawn = pair.Key;
            foreach (var (key, action) in pair.Value)
            {
                pawn.myMap.Remove(key, action);
            }
        }
    }



    // 등록 메서드
    public virtual void RegisterTestActions()
    {

    }
}

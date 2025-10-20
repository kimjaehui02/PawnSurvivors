using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Pawn))]
public class PawnSub : MonoBehaviour
{
    // 1. 델리게이트 맵을 가지고 있다
    // 2. 

    public EnumDelegateMap<EnumActions, DataContext> myMap = new();

    #region 자신의 함수를 등록하는걸 기록하는 시스템

    // Pawn별로 내가 추가한 액션 기록
    private readonly Dictionary<Pawn, List<(EnumActions key, Action<DataContext> action)>> addedToMain = new();

    // PawnSub -> Pawn 병합
    public void MergeToMain(Pawn pawn)
    {
        if (!addedToMain.ContainsKey(pawn))
            addedToMain[pawn] = new List<(EnumActions, Action<DataContext>)>();

        foreach (KeyValuePair<EnumActions, List<Action<DataContext>>> kv in myMap.GetAll())
        {
            EnumActions key = kv.Key;
            foreach (Action<DataContext> action in kv.Value)
            {
                pawn.myMap.Add(key, action);
                addedToMain[pawn].Add((key, action));
            }
        }
    }
    #endregion

    #region 기본 라이프사이클

    void Awake()
    {
        
        MergeToMain(GetComponent<Pawn>());
    }

    // 제거 시, 자신이 추가한 것만 제거
    private void OnDestroy()
    {
        foreach (KeyValuePair<Pawn, List<(EnumActions, Action<DataContext>)>> pair in addedToMain)
        {
            Pawn pawn = pair.Key;
            foreach ((EnumActions key, Action<DataContext> action) in pair.Value)
            {
                pawn.myMap.Remove(key, action);
            }
        }
    }
    #endregion

    

}

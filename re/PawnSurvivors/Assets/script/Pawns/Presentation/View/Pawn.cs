using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pawn : MonoBehaviour
{
    public EnumDelegateMap<EnumActions, DataContext> myMap = new();

    public EnumValueMap<LifeCycle, ActionCollection> lifeCycleActions = new();

    //private void Start()
    //{
        
    //}

    private void Update()
    {
        DataContext context = new() { };
        myMap.Invoke(EnumActions.GetPlayerMovementInput, context);
        myMap.Invoke(EnumActions.Move, context);
    }

}


public enum LifeCycle
{
    Awake,
    Start,
    Update,
    FixedUpdate,
    LateUpdate,
    OnEnable,
    OnDisable,
    OnDestroy
}
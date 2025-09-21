using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pawn : MonoBehaviour
{
    public EnumDelegateMap<Actions, DataContext> myMap = new();
}

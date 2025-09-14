using System.Collections.Generic;
using UnityEngine;

public class C : MonoBehaviour
{
    public EnumDelegateMap<MyEnum, int> myMap = new();

    public void TestInvoke(MyEnum key, int value)
    {
        myMap.Invoke(key, value);
    }
}

public enum MyEnum
{
    Test1,
    Test2
}

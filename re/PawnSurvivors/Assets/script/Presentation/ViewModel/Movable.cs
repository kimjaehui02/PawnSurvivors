using UnityEngine;

public class Movable : PawnSub
{
    public NewMonoBehaviourScript newMonoBehaviourScript;
    public override void RegisterTestActions()
    {

        if (newMonoBehaviourScript == null)
        {
            Debug.LogError("NewMonoBehaviourScript 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        //myMap.Add(Actions.Move, newMonoBehaviourScript.Move);

    }
}

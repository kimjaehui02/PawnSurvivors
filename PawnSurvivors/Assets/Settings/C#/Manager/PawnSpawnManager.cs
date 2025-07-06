using System.Collections.Generic;
using UnityEngine;

public class PawnSpawnManager : MonoBehaviour
{



    //public List<GameObject> GameObjects;

    public GameObject PlayerSpawn;
    public GameObject AiPawnSpawn;

    public List<PawnAction> PawnActions;

    public void SpawnPawn()
    {
        //PawnActions = PlayerSpawn.GetComponentsInChildren<PawnAction>();
        // 씬에 있는 PlayerPawnObject와 그 자식들에서 모든 PawnAction 컴포넌트를 찾습니다.
        PawnAction[] foundActions = PlayerSpawn.GetComponentsInChildren<PawnAction>(true);

        // 기존 리스트를 비우고, 찾은 배열의 모든 요소를 한 번에 추가합니다.
        PawnActions.Clear();
        PawnActions.AddRange(foundActions);
    }

    private void Start()
    {
        SpawnPawn();
    }


}

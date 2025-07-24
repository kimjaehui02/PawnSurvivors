using System.Collections.Generic;
using UnityEngine;

public class PawnSpawnManager : MonoBehaviour
{



    //public List<GameObject> GameObjects;

    public GameObject BasePawn;

    public void SpawnPawn(string type)
    {
        Debug.Log("PawnSpawnManager: SpawnPawn 메서드 호출됨.");
        if (BasePawn == null)
        {
            return; // BasePawn이 설정되지 않은 경우, 스폰하지 않음
        }

        GameObject s = Instantiate(BasePawn);
        GameManager.Instance.PawnDataLoader.ConfigurePawnFromType(s, type);

    }

    private void Start()
    {
        SpawnPawn("Goblin");
        SpawnPawn("Golem");
    }


}

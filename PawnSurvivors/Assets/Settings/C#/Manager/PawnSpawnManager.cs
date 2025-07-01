using System.Collections.Generic;
using UnityEngine;

public class PawnSpawnManager : MonoBehaviour
{
    // 폰 스폰 매니저
    // 폰을 소환하는 행위 전반을 담당하고
    // 일단 생성과정은

    // 1. 폰을 생성한다
    // 2. 폰에게서 awake와 같은 처리를 하는데
    //      이 경우 델리게이트 등을 채워준다

    // 3. 폰을 생성완료한다?

    // 잘모르겟음


    //public List<GameObject> GameObjects;

    public GameObject PlayerSpawn;
    public GameObject AiPawnSpawn;

    public void SpawnPawn()
    {

    }

    public void PlayerSpawnPawn()
    {
        GameObject player = Instantiate(PlayerSpawn);
        Pawn playerpawn = player.GetComponent<Pawn>();



    }

    public void AiPawnSpawnPawn()
    {

    }



}

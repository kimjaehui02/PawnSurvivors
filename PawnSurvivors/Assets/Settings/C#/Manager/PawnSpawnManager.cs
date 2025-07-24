using System.Collections.Generic;
using UnityEngine;

public class PawnSpawnManager : MonoBehaviour
{



    //public List<GameObject> GameObjects;

    public GameObject BasePawn;

    public void SpawnPawn()
    {
        Debug.Log("PawnSpawnManager: SpawnPawn 메서드 호출됨.");
        if (BasePawn == null)
        {
            return; // BasePawn이 설정되지 않은 경우, 스폰하지 않음
        }

        GameObject s = Instantiate(BasePawn);
        s.AddComponent<MoveableComponent>(); // Pawn 컴포넌트 추가
        s.AddComponent<PawnTargetFinderComponent>(); // Pawn 컴포넌트 추가
        s.AddComponent<PawnMoverComponent>(); // Pawn 컴포넌트 추가
        s.AddComponent<DamageableComponent>(); // Pawn 컴포넌트 추가
        s.AddComponent<DamageDealerComponent>(); // Pawn 컴포넌트 추가
    }

    private void Start()
    {
        SpawnPawn();
    }


}

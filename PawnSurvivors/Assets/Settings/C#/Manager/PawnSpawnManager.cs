using Game.Core;
using System.Collections.Generic;
using UnityEngine;

public class PawnSpawnManager : MonoBehaviour
{



    //public List<GameObject> GameObjects;

    public GameObject BasePawn;

    public void SpawnPawn(string type)
    {
        Debug.Log($"PawnSpawnManager: SpawnPawn {type}메서드 호출됨.");
        if (BasePawn == null)
        {
            return; // BasePawn이 설정되지 않은 경우, 스폰하지 않음
        }

        GameObject s = Instantiate(BasePawn);
        //GameManager.Instance.PawnDataLoader.ConfigurePawnFromType(s, type);
        MoveableComponent moveableComponent = s.AddComponent<MoveableComponent>(); // PawnActionComponent를 추가합니다.
        PawnTargetFinderComponent pawnTargetFinderComponent = s.AddComponent<PawnTargetFinderComponent>(); // PawnActionComponent를 추가합니다.
        PawnMoverComponent pawnMoverComponent = s.AddComponent<PawnMoverComponent>(); // PawnActionComponent를 추가합니다.

        // Pawn의 타입에 따라 설정을 적용합니다.
        moveableComponent.SetBaseConfig(new MoveableConfig());

    }

    private void Start()
    {
        SpawnPawn("Goblin");
        SpawnPawn("Golem");
    }


}

// PawnRegisterToGameManagerComponent.cs
using Game.Core;
using UnityEngine;

public class PawnRegisterToGameManagerComponent : PawnAction
{
    public override void RegisterAbilities()
    {
        AddAction(Acts.OnStart, RegisterSelf); // 또는 AddAction(Acts.OnPawnSpawned, RegisterSelf);
    }

    public void RegisterSelf(AbilityContext context)
    {
        // 이 컴포넌트가 붙은 Pawn 인스턴스를 가져옵니다.
        Pawn ownerPawn = GetComponent<Pawn>();

        if (ownerPawn != null && GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPlayer(ownerPawn);
        }
    }
}
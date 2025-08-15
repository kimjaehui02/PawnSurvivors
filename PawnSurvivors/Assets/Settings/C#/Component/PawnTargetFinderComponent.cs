using Game.Core;
using Game.Core.Base;
using Game.Core.Contexts;
using Game.Core.Enums;
using UnityEngine;

public class PawnTargetFinderComponent : PawnBase
{
    public Pawn _targetPawn;

    public override void RegisterAbilities()
    {
        AddAction(Acts.OnStart, GetTarget);
        AddAction(Acts.OnUpdateTarget, UpdateTarget);
    }



    public void GetTarget(AbilityContext abilityContext)
    {
        // 타겟 폰을 찾는 로직을 구현합니다.
        _targetPawn = GameManager.Instance.Player;
    }

    public void UpdateTarget(AbilityContext abilityContext)
    {
        //Debug.Log("UpdateTarget");

        abilityContext.TargetPawn = _targetPawn;
    }
}

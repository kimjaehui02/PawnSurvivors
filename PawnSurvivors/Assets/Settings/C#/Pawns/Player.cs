using Game.Core;
using System;
using UnityEngine;

public class Player : Pawn
{

    protected override void Start()
    {
        base.Start(); // Pawn의 Start 메서드를 호출하여 초기화합니다.
        GameManager.Instance.RegisterPlayer(this); // Player를 GameManager에 등록합니다.
    }



}

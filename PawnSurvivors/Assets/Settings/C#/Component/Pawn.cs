using Game.Core;
using Game.Core.Base;
using Game.Core.Configs;
using Game.Core.Contexts;
using Game.Core.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 Pawn의 기반 클래스입니다.
/// 하위 PawnBase 컴포넌트들의 능력을 통합 관리하며,
/// AbilityContext 기반의 커스텀 생명주기 처리를 지원합니다.
/// </summary>
public class Pawn : PawnBase
{
    #region Fields & Properties

    [SerializeField] private PawnConfig pawnConfig = new();
    public PawnConfig PawnConfig
    {
        get => pawnConfig;
        set => pawnConfig = value;
    }

    // 지속 업데이트에서 호출할 Acts 목록
    public List<Acts> actsToUpdate = new();
    // TriggerEnter 이벤트에서 호출할 Acts 목록
    public List<Acts> actsToTriggerEnter = new();

    #endregion

    #region Unity Lifecycle

    protected virtual void Start()
    {
        RegisterAbilities(); // 하위 PawnBase 컴포넌트 능력 통합

        // 필요 액션 등록
        if (GetActions.ContainsKey(Acts.OnUpdateTarget)) actsToUpdate.Add(Acts.OnUpdateTarget);
        if (GetActions.ContainsKey(Acts.OnUpdate)) actsToUpdate.Add(Acts.OnUpdate);
        if (GetActions.ContainsKey(Acts.OnMove)) actsToUpdate.Add(Acts.OnMove);

        actsToTriggerEnter.Add(Acts.OnTriggerEnter);

        RegisterLifecycleCallbacks();
    }

    #endregion

    #region Custom Lifecycle Callbacks

    /// <summary>
    /// 커스텀 라이프사이클 매니저에 Start 및 Update 액션 등록
    /// </summary>
    public void RegisterLifecycleCallbacks()
    {
        GameManager.Instance.CustomLifecycleManager.EnqueueStartAction(PawnStart);

        if (actsToUpdate != null && actsToUpdate.Count > 0)
        {
            GameManager.Instance.CustomLifecycleManager.AddUpdate(UpdateActionTypes.Update, PawnUpdate);
        }
    }

    /// <summary>
    /// Pawn 시작 시 호출되는 액션
    /// </summary>
    public virtual void PawnStart()
    {
        AbilityContext startContext = new();
        RequestAction(Acts.OnStart, startContext);
    }

    /// <summary>
    /// Pawn 지속 처리 액션
    /// </summary>
    public virtual void PawnUpdate()
    {
        AbilityContext updateContext = new();
        RequestActions(actsToUpdate, updateContext);
    }

    /// <summary>
    /// TriggerEnter2D 이벤트 처리
    /// </summary>
    public void OnTriggerEnter2D(Collider2D collision)
    {
        AbilityContext triggerContext = new()
        {
            TargetPawn = collision.GetComponent<Pawn>(),
            SourcePawn = this
        };
        RequestActions(actsToTriggerEnter, triggerContext);
    }

    #endregion

    #region Ability Management

    /// <summary>
    /// 하위 PawnBase 컴포넌트의 델리게이트를 Pawn의 _myDelegates에 통합
    /// </summary>
    public override void RegisterAbilities()
    {
        PawnBase[] allPawnBases = GetComponentsInChildren<PawnBase>(true);

        foreach (PawnBase childBase in allPawnBases)
        {
            if (childBase == this) continue;

            // 하위 PawnBase의 델리게이트 등록
            childBase.RegisterAbilities();

            // 하위 델리게이트를 통합
            foreach (var entry in childBase.GetActions)
            {
                AddAction(entry.Key, entry.Value);
            }
        }

        Debug.Log($"Pawn '{name}' 능력 통합 완료. 총 델리게이트 수: {GetActions.Count}");
    }

    #endregion
}

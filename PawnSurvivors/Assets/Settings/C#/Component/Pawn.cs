using Game.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 Pawn의 기반이 되는 클래스입니다.
/// 자체 능력(PawnAction)을 정의하고, 하위 PawnAction 컴포넌트들의 능력을 통합 관리합니다.
/// </summary>
public class Pawn : PawnAction
{
    // Pawn이 통합된 모든 Acts-Action 델리게이트를 관리하는 딕셔너리입니다.
    // Pawn 자신과 모든 자식 PawnAction 컴포넌트들의 델리게이트를 여기에 합칩니다.
    // protected로 선언하여 자식 클래스(예: Player)에서도 접근 가능하게 합니다.
    // NOTE: PawnAction의 _myDelegates와 이 _allActions의 역할이 명확해야 합니다.
    // 현재 코드에서는 _myDelegates에 통합하고 있으므로, _allActions 필드는 불필요합니다.
    // PawnAction의 _myDelegates가 Pawn의 통합 델리게이트 맵 역할을 합니다.
    // private readonly Dictionary<Acts, Action<AbilityContext>> _allActions = new(); // 현재 코드에서 사용되지 않음

    #region Fields & Properties

    // Pawn의 고유 ID입니다. 내부에서만 변경 가능합니다.
    public string Id { get; private set; } 

    public string Name { get; set; } // Pawn의 이름입니다. 외부에서 설정 가능합니다.

    public string Description { get; set; } // Pawn의 설명입니다. 외부에서 설정 가능합니다.


    public List<Acts> actsToUpdate = new ();
    public List<Acts> actsToTriggerEnter = new ();

    #endregion

    #region Unity Lifecycle

    // MonoBehaviour의 Start 메서드입니다. Pawn의 초기화를 시작합니다.
    protected virtual void Start()
    {
        //AbilityContext = new AbilityContext(); // AbilityContext 인스턴스 초기화
        RegisterAbilities();             // Pawn 능력 통합 시작

        if (GetActions.ContainsKey(Acts.OnUpdateTarget))
        {
            actsToUpdate.Add(Acts.OnUpdateTarget); 

        }

        if (GetActions.ContainsKey(Acts.OnUpdate))
        {
            actsToUpdate.Add(Acts.OnUpdate); // Start 액트 요청 추가

        }

        if (GetActions.ContainsKey(Acts.OnMove))
        {
            actsToUpdate.Add(Acts.OnMove); // Start 액트 요청 추가

        }
        actsToTriggerEnter.Add(Acts.OnTriggerEnter); // OnTriggerEnter2D 액트 추가

        RegisterLifecycleCallbacks();
    }

    // MonoBehaviour의 Update 메서드입니다. 매 프레임 업데이트 관련 액션을 요청합니다.
    //private void Update()
    //{
    //    PawnUpdate();
    //}

    #endregion

    #region Custom Lifecycle Callbacks

    public void RegisterLifecycleCallbacks()
    {
        // MonoBehaviour의 생명주기 메서드에 연결할 액션을 등록합니다.
        //AddAction(Acts.OnStart, PawnStart);

        GameManager.Instance.CustomLifecycleManager.EnqueueStartAction(PawnStart);

        if (actsToUpdate != null && actsToUpdate.Count > 0)
        {
            GameManager.Instance.CustomLifecycleManager.AddUpdate(UpdateActionTypes.Update, PawnUpdate);
        }


    }


    // Pawn의 Start 관련 커스텀 콜백입니다.
    public virtual void PawnStart() 
    {
        AbilityContext startAbilityContext = new();



        RequestAction(Acts.OnStart, startAbilityContext);
    }

    // Pawn의 지속 처리(Update) 관련 커스텀 콜백입니다.
    public virtual void PawnUpdate()
    {
        AbilityContext updateAbilityContext = new();



        RequestActions(actsToUpdate, updateAbilityContext);


    }



    public void OnTriggerEnter2D(Collider2D collision)
    {
        AbilityContext TriggerEnter2DAbilityContext = new()
        {
            TargetPawn = collision.GetComponent<Pawn>(),
            SourcePawn = this // 현재 Pawn을 소스 Pawn으로 설정
        };
        RequestActions(actsToTriggerEnter, TriggerEnter2DAbilityContext);
    }

    #endregion

    #region Ability Management

    /// <summary>
    /// 이 Pawn의 모든 능력(PawnAction 컴포넌트) 델리게이트를 통합하여 관리합니다.
    /// Pawn 자신과 모든 자식 PawnAction 컴포넌트들의 델리게이트를 Pawn의 _myDelegates에 합칩니다.
    /// </summary>
    public override void RegisterAbilities()
    {

        //AddAction(Acts.OnStart, PawnStart);

        #region 델리게이트에 보조기능들의 델리게이트 넣기

        // 하위 기능 컴포넌트들을 가져오기위해 겟컴포넌트로 가져옵니다
        PawnAction[] allPawnActionsInHierarchy = GetComponentsInChildren<PawnAction>(true);

;

        foreach (PawnAction otherPawnAction in allPawnActionsInHierarchy)
        {
            // 자기 자신 (Pawn 컴포넌트)은 이미 PawnAction으로서 _myDelegates를 가집니다.
            // 여기서는 다른 자식 PawnAction 컴포넌트들만 처리합니다.
            if (otherPawnAction == this)
            {
                continue; 
            }

            // 다른 PawnAction 컴포넌트의 RegisterAbilities()를 호출하여,
            // 그 컴포넌트의 _myDelegates에 델리게이트들이 채워지도록 합니다.
            otherPawnAction.RegisterAbilities();
            
            // 다른 PawnAction 컴포넌트의 델리게이트 맵을 순회하고,
            // Pawn 자신의 _myDelegates (통합 델리게이트 맵)에 추가합니다.
            foreach (var entry in otherPawnAction.GetActions)
            {
                AddAction(entry.Key, entry.Value); // PawnAction의 AddAction 메서드를 사용하여 안전하게 추가
            }
        }
        Debug.Log($"Pawn '{name}'의 모든 능력 통합 완료. 현재 등록된 델리게이트 수: {GetActions.Count}");

        #endregion

    }




    #endregion

}
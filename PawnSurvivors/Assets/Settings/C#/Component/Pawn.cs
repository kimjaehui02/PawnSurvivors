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

    // 행동 실행에 사용될 AbilityContext 인스턴스입니다. 매 프레임 재활용됩니다.
    public AbilityContext AbilityContext { get; private set; }

    #endregion

    #region Unity Lifecycle

    // MonoBehaviour의 Start 메서드입니다. Pawn의 초기화를 시작합니다.
    private void Start()
    {
        AbilityContext = new AbilityContext(); // AbilityContext 인스턴스 초기화
        RegisterAbilities();             // Pawn 능력 통합 시작
    }

    // MonoBehaviour의 Update 메서드입니다. 매 프레임 업데이트 관련 액션을 요청합니다.
    private void Update()
    {
        // Acts.OnUpdate 델리게이트에 등록된 모든 함수를 실행합니다.
        // 예를 들어, PlayerInputHandler가 여기에 연결되어 AbilityContext를 채울 수 있습니다.
        RequestAction(Acts.OnUpdate, AbilityContext);

        // Acts.OnMove 델리게이트에 등록된 모든 함수를 실행합니다.
        // 이동 입력이 있을 경우 PlayerInputHandler가 AbilityContext.inputDirection을 채울 것입니다.
        RequestAction(Acts.OnMove, AbilityContext);

        // 디버그: 현재 Pawn 컴포넌트에 통합된 총 델리게이트의 수를 확인합니다.
        // Debug.Log($"Pawn '{name}'의 통합 델리게이트 수: {_myDelegates.Count}"); 
    }

    #endregion

    #region Ability Management

    /// <summary>
    /// 이 Pawn의 모든 능력(PawnAction 컴포넌트) 델리게이트를 통합하여 관리합니다.
    /// Pawn 자신과 모든 자식 PawnAction 컴포넌트들의 델리게이트를 Pawn의 _myDelegates에 합칩니다.
    /// </summary>
    public override void RegisterAbilities()
    {
        // NOTE: PawnAction의 _myDelegates를 통합 딕셔너리로 사용하는 경우,
        // 이 메서드의 첫 번째 루프 (this.GetActions 순회)는 불필요합니다.
        // PawnAction의 base.RegisterAbilities()가 Pawn 자신의 델리게이트를 _myDelegates에 등록할 것이고,
        // 이후 자식 컴포넌트들의 델리게이트를 이 _myDelegates에 추가할 것이기 때문입니다.
        // 만약 PawnAction의 RegisterAbilities에 아무런 델리게이트 등록 로직이 없다면, 이 부분은 비워두어도 됩니다.
        
        // 1. 이 게임 오브젝트와 그 자식 게임 오브젝트에 붙어있는 모든 PawnAction 컴포넌트를 가져옵니다.
        // 'true' 매개변수는 비활성화된 오브젝트의 컴포넌트도 포함합니다.
        PawnAction[] allPawnActionsInHierarchy = GetComponentsInChildren<PawnAction>(true);

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
        Debug.Log($"Pawn '{name}'의 모든 능력 통합 완료. 현재 등록된 델리게이트 수: {_myDelegates.Count}");
    }

    /// <summary>
    /// 외부에서 특정 행동(Acts)을 요청할 때 사용합니다.
    /// 해당 Acts에 등록된 모든 델리게이트를 찾아 주어진 AbilityContext와 함께 실행합니다.
    /// 이 메서드는 PawnAction에 정의되어 있으므로, Pawn은 이를 상속받아 사용합니다.
    /// </summary>
    // public void RequestAction(Acts act, AbilityContext context) { ... }
    // NOTE: RequestAction은 PawnAction에 이미 정의되어 있고 Pawn이 상속받으므로, 여기에 다시 정의할 필요 없습니다.
    // 만약 _allActions 딕셔너리에만 있는 델리게이트를 호출해야 한다면 RequestAction의 내부 로직을 수정해야 합니다.
    // 현재 코드에서는 _myDelegates를 통합 딕셔너리로 사용하고 있으므로, PawnAction의 RequestAction이 그대로 작동합니다.

    #endregion

    #region Custom Lifecycle Callbacks

    // Pawn의 초기화 로직을 처리하는 커스텀 콜백입니다.
    // PawnSpawnManager 등 외부에서 Pawn 생성/로드 후 이 메서드를 호출해야 합니다.
    // 현재는 MonoBehaviour.Start()에서 통합 로직을 호출하므로, 이 메서드는 사용되지 않을 수 있습니다.
    public virtual void PawnAwake() { }

    // Pawn의 Start 관련 커스텀 콜백입니다.
    public virtual void PawnStart() { }

    // Pawn의 지속 처리(Update) 관련 커스텀 콜백입니다.
    public virtual void PawnUpdate() { }

    // Pawn의 비활성화 처리 관련 커스텀 콜백입니다.
    public virtual void PawnDisable() { }

    // Pawn의 소멸 처리 관련 커스텀 콜백입니다.
    public virtual void PawnDespawn() 
    {
        // 폰이 소멸될 때 등록된 모든 델리게이트를 클리어하여 메모리 누수를 방지합니다.
        _myDelegates.Clear(); 
    }

    #endregion
}
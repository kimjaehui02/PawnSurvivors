using Game.Core;
using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 게임의 전반적인 관리와 핵심 시스템에 대한 접근을 제공하는 싱글톤 매니저입니다.
/// </summary>
public class GameManager : ManagerBase
{

    #region 싱글톤
    // 게임 매니저의 유일한 인스턴스를 저장하는 정적 변수입니다.
    public static GameManager Instance { get; private set; }

    private void SingleAwake()
    {
        // 이미 다른 GameManager 인스턴스가 존재한다면, 현재 객체를 파괴합니다.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("GameManager: 이미 인스턴스가 존재합니다. 중복된 GameManager를 파괴합니다.");
        }
        else
        {
            // 현재 객체를 유일한 GameManager 인스턴스로 설정합니다.
            Instance = this;
            // 씬이 변경되어도 이 GameManager 객체가 파괴되지 않도록 설정합니다.
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager: 인스턴스가 성공적으로 초기화되었습니다.");
        }
    }

    #endregion

    #region 매니저들

    public CustomLifecycleManager CustomLifecycleManager;



    #endregion

    public Pawn Player;
    /// <summary>
    /// 스크립트 인스턴스가 로드될 때 호출됩니다.
    /// 싱글톤 인스턴스를 설정하고, 씬 전환 시 파괴되지 않도록 합니다.
    /// </summary>
    void Awake()
    {
        SingleAwake();
        RegisterAbilities();
    }

    private void Update()
    {
        GameEventContext updateAbilityContext = new();



        RequestAction(GameEventType.Update, updateAbilityContext);
    }

    public override void RegisterAbilities()
    {

        //AddAction(Acts.OnStart, PawnStart);

        #region 델리게이트에 보조기능들의 델리게이트 넣기

        // 하위 기능 컴포넌트들을 가져오기위해 겟컴포넌트로 가져옵니다
        ManagerBase[] allManagerBasesInHierarchy = GetComponentsInChildren<ManagerBase>(true);


        foreach (ManagerBase otherManagerBase in allManagerBasesInHierarchy)
        {
            // 자기 자신 (Pawn 컴포넌트)은 이미 PawnAction으로서 _myDelegates를 가집니다.
            // 여기서는 다른 자식 PawnAction 컴포넌트들만 처리합니다.
            if (otherManagerBase == this)
            {
                continue;
            }

            // 다른 PawnAction 컴포넌트의 RegisterAbilities()를 호출하여,
            // 그 컴포넌트의 _myDelegates에 델리게이트들이 채워지도록 합니다.
            otherManagerBase.RegisterAbilities();

            // 다른 PawnAction 컴포넌트의 델리게이트 맵을 순회하고,
            // Pawn 자신의 _myDelegates (통합 델리게이트 맵)에 추가합니다.
            foreach (var entry in otherManagerBase.GetActions)
            {
                AddAction(entry.Key, entry.Value); // PawnAction의 AddAction 메서드를 사용하여 안전하게 추가
            }
        }
        Debug.Log($"게임매니저 '{name}'의 모든 능력 통합 완료. 현재 등록된 델리게이트 수: {GetActions.Count}");

        #endregion

    }

    internal void RegisterPlayer(Pawn player)
    {
        Player = player;
    }


    private void Start()
    {
        GameEventContext gameEventContext = new()
        {
            JsonPath = JsonPath.pawns
        };

        List<GameEventType> gameEventTypes = new()
        {
            GameEventType.JsonLoading,
            GameEventType.PawnSpawn,

        };




        RequestActions(gameEventTypes, gameEventContext);
    }

}

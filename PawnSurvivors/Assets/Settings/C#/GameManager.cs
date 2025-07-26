using System;
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

    public PawnDataLoader PawnDataLoader;

    #endregion

    public Pawn Player;
    /// <summary>
    /// 스크립트 인스턴스가 로드될 때 호출됩니다.
    /// 싱글톤 인스턴스를 설정하고, 씬 전환 시 파괴되지 않도록 합니다.
    /// </summary>
    void Awake()
    {
        SingleAwake();
    }

    internal void RegisterPlayer(Pawn player)
    {
        Player = player;
    }
}

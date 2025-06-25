using UnityEngine;

/// <summary>
/// 게임의 전반적인 관리와 핵심 시스템에 대한 접근을 제공하는 싱글톤 매니저입니다.
/// </summary>
public class GameManager : MonoBehaviour
{
    // 게임 매니저의 유일한 인스턴스를 저장하는 정적 변수입니다.
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// 스크립트 인스턴스가 로드될 때 호출됩니다.
    /// 싱글톤 인스턴스를 설정하고, 씬 전환 시 파괴되지 않도록 합니다.
    /// </summary>
    void Awake()
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

    // 예시: 게임 상태를 변경하는 메서드 (나중에 일시 정지 등 로직 추가)
    public void StartGame()
    {
        Debug.Log("게임 시작!");
        // 여기에 게임 시작 시 필요한 초기화 로직을 추가합니다.
    }

    public void EndGame()
    {
        Debug.Log("게임 종료!");
        // 여기에 게임 종료 시 필요한 정리 로직을 추가합니다.
    }

    // 다른 매니저들(PawnSpawner, UpdateManager 등)의 인스턴스를 여기에 참조로 가질 수 있습니다.
    // [SerializeField] private PawnSpawner _pawnSpawner;
    // [SerializeField] private UpdateManager _updateManager;

    // 예시: Unity의 Update 메서드 (커스텀 업데이트 매니저가 없다면 여기에 게임 루프 관리)
    // void Update()
    // {
    //     // UpdateManager가 있다면 이 곳에서는 UpdateManager를 호출하는 방식이 됩니다.
    //     // _updateManager.ProcessUpdates(Time.deltaTime);
    // }
}

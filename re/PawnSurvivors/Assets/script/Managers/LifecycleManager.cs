using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PawnSurvivors.Managers;

/// <summary>
/// 게임의 생명주기를 관리하는 매니저입니다.
/// Unity의 Update를 EarlyUpdate → Update → LateUpdate 구조로 확장합니다.
/// </summary>
public class LifecycleManager : MonoBehaviour
{
    #region 필드

    public bool IsPaused { get; private set; } = false;
    
    /// <summary>
    /// 게임 델타타임 (일시정지 시 0). Time.deltaTime의 대체제입니다.
    /// </summary>
    public float GameDeltaTime { get; private set; } = 0f;
    
    /// <summary>
    /// 게임 경과 시간 (일시정지 시 증가하지 않음). Time.time의 대체제입니다.
    /// </summary>
    public float GameTime { get; private set; } = 0f;
    
    private readonly Queue<Action> _earlyUpdateQueue = new();
    private readonly Queue<Action> _lateUpdateQueue = new();

    #endregion

    #region Public Methods

    /// <summary>
    /// Update 시작 시 실행될 액션을 큐에 추가합니다.
    /// 일시정지 상태에서도 실행됩니다 (UI, 입력 처리 등).
    /// </summary>
    public void EnqueueEarlyUpdate(Action action)
    {
        if (action != null)
        {
            _earlyUpdateQueue.Enqueue(action);
        }
    }

    /// <summary>
    /// Update 끝에 실행될 액션을 큐에 추가합니다.
    /// 일시정지 상태에서도 실행됩니다 (파괴 요청 등).
    /// </summary>
    public void EnqueueLateUpdate(Action action)
    {
        if (action != null)
        {
            _lateUpdateQueue.Enqueue(action);
        }
    }

    /// <summary>
    /// GameObject를 안전하게 파괴합니다 (Update 끝에 실행).
    /// </summary>
    public void RequestDestruction(GameObject obj)
    {
        if (obj != null)
        {
            EnqueueLateUpdate(() => Destroy(obj));
        }
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        LogManager.LogInfo(LogCategory.System, IsPaused ? "LifecycleManager Paused" : "LifecycleManager Resumed");
    }
    
    /// <summary>
    /// 게임 시간을 리셋합니다.
    /// </summary>
    public void ResetGameTime()
    {
        GameTime = 0f;
    }


    #endregion

    #region Unity Lifecycle

    void Update()
    {
        // ESC 키로 일시정지 토글
        // 이부분은 무조건 의도한 사항임 라이프사이클 로직을 고의적으로 정지시키는게 목적이고 타임스케일을 수정하면 다른것도 같이 멈추거나 문제가 생겨서 일부러 하지 않은거임
        // if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        // {
        //     IsPaused = !IsPaused;
        //     Debug.Log(IsPaused ? "LifecycleManager Paused" : "LifecycleManager Resumed");
        // }

        // 게임 델타타임 계산 (일시정지 시 0)
        GameDeltaTime = IsPaused ? 0f : Time.deltaTime;
        if (!IsPaused)
        {
            GameTime += GameDeltaTime;
        }

        // 1. Early Update: 일회성 액션 실행 (일시정지와 무관하게 실행 - UI, 입력 등)
        ProcessEarlyUpdate();

        // 2. Main Update: 게임 로직 실행 (일시정지 시 실행 안 됨)
        if (!IsPaused)
        {
            ProcessMainUpdate();
        }

        // 3. Late Update: 정리 작업 실행 (일시정지와 무관하게 실행 - 파괴 요청 등)
        ProcessLateUpdate();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Update 시작 시 실행되는 Early Update 단계입니다.
    /// 일시정지 상태에서도 실행됩니다 (UI, 입력 처리 등).
    /// </summary>
    private void ProcessEarlyUpdate()
    {
        while (_earlyUpdateQueue.Count > 0)
        {
            _earlyUpdateQueue.Dequeue()?.Invoke();
        }
    }

    /// <summary>
    /// 메인 게임 로직을 실행하는 Main Update 단계입니다.
    /// 일시정지 상태에서는 실행되지 않습니다.
    /// </summary>
    private void ProcessMainUpdate()
    {
        // StageManager 업데이트
        if (GameManager.Instance?.StageManager != null)
        {
            GameManager.Instance.StageManager.UpdateStage();
        }

        // 모든 Pawn 업데이트 (역순으로 안전하게 순회)
        for (int i = PawnManager.AllPawnManagers.Count - 1; i >= 0; i--)
        {
            if (i < PawnManager.AllPawnManagers.Count)
            {
                PawnManager manager = PawnManager.AllPawnManagers[i];
                if (manager != null)
                {
                    manager.ManagedUpdate();
                }
            }
        }
    }

    /// <summary>
    /// Update 끝에 실행되는 Late Update 단계입니다.
    /// 파괴 등 정리 작업을 수행합니다.
    /// 일시정지 상태에서도 실행됩니다 (파괴 요청 등).
    /// </summary>
    private void ProcessLateUpdate()
    {
        while (_lateUpdateQueue.Count > 0)
        {
            _lateUpdateQueue.Dequeue()?.Invoke();
        }
    }

    #endregion
}

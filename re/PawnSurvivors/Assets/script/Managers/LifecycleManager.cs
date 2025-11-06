using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LifecycleManager : MonoBehaviour
{
    public bool IsPaused { get; private set; } = false;

    private readonly Queue<Action> _oneTimeActions = new();
    private readonly Queue<GameObject> _destructionQueue = new();

    public void EnqueueAction(Action action)
    {
        _oneTimeActions.Enqueue(action);
    }

    public void RequestDestruction(GameObject obj)
    {
        if (obj != null)
        {
            _destructionQueue.Enqueue(obj);
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            IsPaused = !IsPaused;
            Debug.Log(IsPaused ? "LifecycleManager Paused" : "LifecycleManager Resumed");
        }

        if (IsPaused) return;

        while (_oneTimeActions.Count > 0)
        {
            _oneTimeActions.Dequeue().Invoke();
        }

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

        while (_destructionQueue.Count > 0)
        {
            Destroy(_destructionQueue.Dequeue());
        }
    }
}
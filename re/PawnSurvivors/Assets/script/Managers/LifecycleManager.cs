using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LifecycleManager : MonoBehaviour
{
    public bool IsPaused { get; private set; } = false;

    private readonly Queue<Action> _oneTimeActions = new();

    public void EnqueueAction(Action action)
    {
        _oneTimeActions.Enqueue(action);
    }

    void Update()
    {
        // Debug input to toggle pause
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            IsPaused = !IsPaused;
            Debug.Log(IsPaused ? "LifecycleManager Paused" : "LifecycleManager Resumed");
        }

        if (IsPaused) return; // Do not process anything if paused

        // 1. Process all one-time actions (like SubStart)
        while (_oneTimeActions.Count > 0)
        {
            _oneTimeActions.Dequeue().Invoke();
        }

        // 2. Process all recurring actions (SubUpdate)
        // Iterate backwards as the list can change if a pawn is destroyed.
        for (int i = PawnManager.AllPawnManagers.Count - 1; i >= 0; i--)
        {
            // Ensure the manager at this index still exists before calling update
            if(PawnManager.AllPawnManagers[i] != null)
            {
                PawnManager.AllPawnManagers[i].ManagedUpdate();
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class PawnManager : MonoBehaviour
{
    // Using a list for dynamic addition and removal of sub-managers.
    public List<PawnSubManager> pawnSubManagers = new();

    public static readonly List<PawnManager> AllPawnManagers = new();

    private void OnEnable()
    {
        if (!AllPawnManagers.Contains(this))
        {
            AllPawnManagers.Add(this);
        }
    }

    private void OnDisable()
    {
        if (AllPawnManagers.Contains(this))
        {
            AllPawnManagers.Remove(this);
        }
    }

    public void RegisterSubManager(PawnSubManager subManager)
    {
        if (!pawnSubManagers.Contains(subManager))
        {
            pawnSubManagers.Add(subManager);
            // Enqueue the SubStart action instead of calling it directly.
            GameManager.Instance.LifecycleManager.EnqueueAction(subManager.SubStart);
        }
    }

    public void UnregisterSubManager(PawnSubManager subManager)
    {
        if (pawnSubManagers.Contains(subManager))
        {
            pawnSubManagers.Remove(subManager);
        }
    }

    public void ManagedUpdate()
    {
        // Iterate backwards to allow for safe removal during the loop if needed in the future.
        for (int i = pawnSubManagers.Count - 1; i >= 0; i--)
        {
            pawnSubManagers[i].SubUpdate();
        }
    }
}

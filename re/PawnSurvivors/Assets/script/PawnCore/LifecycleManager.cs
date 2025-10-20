using System.Collections.Generic;
using UnityEngine;

public class LifecycleManager : MonoBehaviour
{
    private List<PawnManager> pawnManagers = new();

    private void Awake()
    {
        pawnManagers.AddRange(FindObjectsByType<PawnManager>(FindObjectsSortMode.None));
    }

    void Update()
    {
        foreach (var manager in pawnManagers)
        {
            manager.ManagedUpdate();
        }
    }
}
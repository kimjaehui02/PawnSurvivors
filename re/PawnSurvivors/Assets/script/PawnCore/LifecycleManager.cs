using System.Collections.Generic;
using UnityEngine;

public class LifecycleManager : MonoBehaviour
{
    private List<PawnManager> pawnManagers = new();

    private void Awake()
    {
        pawnManagers.AddRange(FindObjectsOfType<PawnManager>());
    }

    void Start()
    {
        foreach (var manager in pawnManagers)
        {
            manager.ManagedStart();
        }
    }

    void Update()
    {
        foreach (var manager in pawnManagers)
        {
            manager.ManagedUpdate();
        }
    }
}
using UnityEngine;
using PawnCore.Domain;
using PawnCore.Domain.Events;

/// <summary>
/// Pawn이 죽을 때 다른 Pawn을 생성하는 SubManager입니다.
/// PawnDeathEvent를 구독하여 자신이 죽을 때 설정된 Pawn들을 생성합니다.
/// </summary>
public class SpawnOnDeathSubManager : PawnSubManager
{
    /// <summary>죽을 때 생성할 Pawn의 레시피 이름들</summary>
    public string[] spawnRecipeNames = new string[0];
    
    /// <summary>각 Pawn의 생성 확률 (0.0 ~ 1.0)</summary>
    public float[] spawnChances = new float[0];
    
    /// <summary>각 Pawn의 생성 개수</summary>
    public int[] spawnCounts = new int[0];

    public override void SubStart()
    {
        // PawnDeathEvent 구독 (자신의 사망 처리)
        _pawnManager.Subscribe<PawnDeathEvent>(HandlePawnDeath);
    }

    private void OnDisable()
    {
        if (_pawnManager != null)
        {
            _pawnManager.Unsubscribe<PawnDeathEvent>(HandlePawnDeath);
        }
    }

    public override void SubUpdate()
    {
        // 업데이트 불필요
    }

    /// <summary>
    /// Pawn 사망 이벤트를 처리합니다.
    /// </summary>
    private void HandlePawnDeath(PawnDeathEvent evt)
    {
        // 자신의 사망인지 확인
        if (evt.DeadPawn != _pawnManager) return;

        // 설정된 Pawn 생성
        SpawnPawns();
    }

    /// <summary>
    /// 설정된 Pawn들을 생성합니다.
    /// </summary>
    private void SpawnPawns()
    {
        if (GameManager.Instance?.CreationManager == null)
        {
            Debug.LogWarning("[SpawnOnDeathSubManager] CreationManager가 없어 Pawn을 생성할 수 없습니다.");
            return;
        }

        if (spawnRecipeNames == null || spawnRecipeNames.Length == 0)
        {
            return;
        }

        Vector3 spawnPosition = _pawnManager.transform.position;

        for (int i = 0; i < spawnRecipeNames.Length; i++)
        {
            if (string.IsNullOrEmpty(spawnRecipeNames[i]))
            {
                continue;
            }

            // 확률 체크
            float chance = (i < spawnChances.Length) ? spawnChances[i] : 1.0f;
            if (Random.Range(0f, 1f) > chance)
            {
                continue;
            }

            // 생성 개수
            int count = (i < spawnCounts.Length && spawnCounts[i] > 0) ? spawnCounts[i] : 1;

            // Pawn 생성
            for (int j = 0; j < count; j++)
            {
                Vector3 offset = Random.insideUnitCircle * 0.3f; // 약간의 랜덤 오프셋
                GameObject spawned = GameManager.Instance.CreationManager.CreatePawn(
                    spawnRecipeNames[i],
                    spawnPosition + offset,
                    Quaternion.identity
                );

                if (spawned != null)
                {
                    Debug.Log($"[SpawnOnDeathSubManager] {_pawnManager.name} 사망 시 {spawnRecipeNames[i]} 생성: {spawned.name} at {spawnPosition + offset}");
                }
                else
                {
                    Debug.LogWarning($"[SpawnOnDeathSubManager] Pawn 생성 실패: '{spawnRecipeNames[i]}' recipe를 찾을 수 없습니다.");
                }
            }
        }
    }
}


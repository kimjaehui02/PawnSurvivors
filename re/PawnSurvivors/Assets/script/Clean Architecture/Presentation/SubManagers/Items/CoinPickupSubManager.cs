using UnityEngine;
using PawnSurvivors.Domain.Usecases;
using PawnSurvivors.Player;
using PawnSurvivors.Domain.Events;

/// <summary>
/// 코인을 수집할 수 있도록 하는 SubManager입니다.
/// 플레이어와 일정 거리 이하일 때 자동으로 흡수되고, 충돌 시 골드를 추가합니다.
/// MovableSubManager의 HomingMovementStrategy를 활성화하여 이동을 처리합니다.
/// </summary>
public class CoinPickupSubManager : PawnSubManager
{
    /// <summary>이 코인이 주는 골드 양</summary>
    public int goldAmount = 1;
    
    /// <summary>플레이어와의 거리가 이 값 이하일 때 자동 흡수 시작</summary>
    public float magnetRange = 3f;
    
    /// <summary>현재 흡수 중인지 여부</summary>
    private bool _isMagnetized = false;
    
    /// <summary>이미 수집되었는지 여부 (중복 수집 방지)</summary>
    private bool _isCollected = false;

    public override void SubStart()
    {
        // HomingMovementStrategy의 detectionRange를 magnetRange로 미리 제한
        // (MovableSubManager가 자동으로 활성화하더라도 올바른 범위를 사용하도록)
        if (_pawnManager.PawnData != null)
        {
            var movableData = _pawnManager.PawnData.GetOrCreateMovableData();
            if (movableData.homingMovement != null)
            {
                movableData.homingMovement.detectionRange = magnetRange;
            }
        }
    }

    public override void SubUpdate()
    {
        if (_isCollected) return;

        // HomingMovementStrategy의 detectionRange를 항상 magnetRange로 제한
        // (MovableSubManager가 자동 활성화하거나 다른 곳에서 변경해도 유지)
        if (_pawnManager.PawnData != null)
        {
            var movableData = _pawnManager.PawnData.GetOrCreateMovableData();
            if (movableData.homingMovement != null)
            {
                movableData.homingMovement.detectionRange = magnetRange;
            }
        }

        // 플레이어 찾기
        if (GameManager.Instance?.PlayerController == null) return;
        
        Transform closestPlayer = FindClosestPlayer();
        if (closestPlayer == null) return;

        float distance = Vector3.Distance(_pawnManager.transform.position, closestPlayer.position);

        // 자동 흡수 범위 내에 있으면 HomingMovementStrategy 활성화
        if (distance <= magnetRange && !_isMagnetized)
        {
            _isMagnetized = true;
            // HomingMovementStrategy로 전환 (이동은 MovableSubManager가 처리)
            _pawnManager.Publish(new ChangeMovementStrategyEvent(typeof(HomingMovementStrategy)));
        }
        else if (distance > magnetRange && _isMagnetized)
        {
            _isMagnetized = false;
            // 거리가 멀어지면 원래 전략으로 복귀 (현재는 없으므로 그대로 유지)
            // TODO: 나중에 초기 전략으로 복귀 로직 추가 가능
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isCollected) return;

        // 플레이어와 충돌했는지 확인
        if (IsPlayerCollider(other))
        {
            CollectCoin();
        }
    }

    /// <summary>
    /// 가장 가까운 플레이어를 찾습니다.
    /// </summary>
    private Transform FindClosestPlayer()
    {
        if (GameManager.Instance?.PlayerController?.playerPawns == null) return null;
        if (GameManager.Instance.PlayerController.playerPawns.Count == 0) return null;

        Transform closest = null;
        float closestDistance = float.MaxValue;
        Vector3 myPosition = _pawnManager.transform.position;

        foreach (var playerPawn in GameManager.Instance.PlayerController.playerPawns)
        {
            if (playerPawn == null) continue;

            float distance = Vector3.Distance(myPosition, playerPawn.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = playerPawn.transform;
            }
        }

        return closest;
    }

    /// <summary>
    /// 충돌한 오브젝트가 플레이어인지 확인합니다.
    /// </summary>
    private bool IsPlayerCollider(Collider2D other)
    {
        if (GameManager.Instance?.PlayerController?.playerPawns == null) return false;

        GameObject otherObject = other.gameObject;
        return GameManager.Instance.PlayerController.playerPawns.Contains(otherObject);
    }

    /// <summary>
    /// 코인을 수집합니다.
    /// </summary>
    private void CollectCoin()
    {
        if (_isCollected) return;
        
        // 골드 추가
        AddGold();
        
        // 코인 파괴
        _pawnManager.DestroyPawn();
    }

    /// <summary>
    /// 골드를 추가합니다. (중복 방지 포함)
    /// </summary>
    private void AddGold()
    {
        if (_isCollected) return;
        _isCollected = true;

        if (GameManager.Instance?.CurrencyUseCase != null)
        {
            GameManager.Instance.CurrencyUseCase.AddGold(goldAmount);
            Debug.Log($"[CoinPickupSubManager] 코인 수집: +{goldAmount} 골드 (총: {GameManager.Instance.CurrencyUseCase.GetGold()})");
        }
        else
        {
            Debug.LogWarning("[CoinPickupSubManager] CurrencyUseCase를 찾을 수 없어 골드를 추가할 수 없습니다.");
        }
    }

    /// <summary>
    /// 비활성화/파괴 시 골드를 추가합니다. (어떤 방식으로 제거되더라도)
    /// </summary>
    private void OnDisable()
    {
        // 아직 수집되지 않았다면 골드 추가
        // (스테이지 종료 등으로 일괄 파괴되는 경우, SubManager만 제거되는 경우 모두 대응)
        AddGold();
    }
}


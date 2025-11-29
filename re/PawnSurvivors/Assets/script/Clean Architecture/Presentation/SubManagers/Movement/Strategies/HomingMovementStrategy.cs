using UnityEngine;
using System.Linq;
using PawnSurvivors.Domain;
using PawnSurvivors.Domain.Usecases;

public class HomingMovementStrategy : MovementStrategyBase
{
    private PawnData _pawnData;
    private Transform _currentTarget;
    private float _nextRetargetTime;

    public override void Init(PawnManager pawnManager)
    {
        base.Init(pawnManager);
        _pawnData = pawnManager.PawnData;
        
        // MovableData 가져오기 또는 생성
        _pawnData.GetOrCreateMovableData();
    }

    public override void Move()
    {
        if (_pawnManager == null) return;
        if (_pawnData?.movableData?.homingMovement == null) return;

        float currentTime = GetGameTime();
        if (currentTime >= _nextRetargetTime)
        {
            _nextRetargetTime = currentTime + _pawnData.movableData.homingMovement.retargetFrequency;
            _currentTarget = TargetingUsecases.FindClosestTargetByTag(_pawnManager.transform.position, _pawnData.movableData.homingMovement.targetTag, _pawnData.movableData.homingMovement.detectionRange);
        }

        if (_currentTarget != null)
        {
            Vector3 direction = (_currentTarget.position - _pawnManager.transform.position);
            MovementUsecases.MoveInDirection(_pawnManager.transform, direction, _pawnData.movableData.homingMovement.speed, _pawnData);
        }
    }

    /// <summary>
    /// 게임 시간을 가져옵니다. (일시정지 중에는 멈춤)
    /// LifecycleManager가 없으면 Time.time을 반환합니다.
    /// </summary>
    private float GetGameTime()
    {
        if (GameManager.Instance?.LifecycleManager != null)
        {
            return GameManager.Instance.LifecycleManager.GameTime;
        }
        return Time.time; // 폴백
    }
}

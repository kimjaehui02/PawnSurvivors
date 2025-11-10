using UnityEngine;
using PawnCore.Domain;
using PawnCore.Domain.Events;

/// <summary>
/// 충돌 시 데미지를 가하는 SubManager입니다.
/// 충돌이 발생하면 DamageEvent를 발행합니다.
/// </summary>
public class CollisionDamageSubManager : PawnSubManager
{
    private PawnData _pawnData;

    public override void SubStart()
    {
        _pawnData = _pawnManager.PawnData;
    }

    public override void SubUpdate()
    {
        // 특정 업데이트 로직이 필요하지 않음
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 동일한 구성 요소를 가진 다른 개체에 부딪히지 않도록 방지
        if (other.GetComponent<CollisionDamageSubManager>() != null)
        {
            return;
        }

        // 상대방이 PawnManager를 가지고 있는지 확인
        if (other.TryGetComponent<PawnManager>(out var targetPawnManager))
        {
            // 상대방에게 DamageEvent 발행
            targetPawnManager.Publish(new DamageEvent(targetPawnManager, _pawnData.damage, gameObject));

            // 자신은 파괴 (발사체의 경우)
            _pawnManager.DestroyPawn();
        }
    }
}

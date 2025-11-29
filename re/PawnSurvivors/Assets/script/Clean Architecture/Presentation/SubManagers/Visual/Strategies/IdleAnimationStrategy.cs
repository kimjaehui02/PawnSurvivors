using UnityEngine;

/// <summary>
/// 애니메이션을 적용하지 않는 기본 전략입니다.
/// </summary>
public class IdleAnimationStrategy : AnimationStrategyBase
{
    public override void Animate()
    {
        // 아무것도 하지 않음
        if (_visualsObject != null)
        {
            _visualsObject.transform.localPosition = Vector3.zero;
        }
    }
}


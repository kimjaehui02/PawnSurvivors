using UnityEngine;

// 1. IGraphic 인터페이스
public interface IGraphic
{
    // 스프라이트 렌더러 관련
    void SetSprite(Sprite newSprite);
    void SetColor(Color color);
    void FlipX(bool flip); // X축 뒤집기 (방향 전환 시)

    // 애니메이터 관련
    void PlayAnimation(string animationClipName);
    void SetAnimationSpeed(float speed);
    void SetAnimatorParameter(string parameterName, float value); // float 파라미터 예시
    void SetAnimatorParameter(string parameterName, bool value);  // bool 파라미터 예시
    void SetAnimatorParameter(string parameterName, int value);   // int 파라미터 예시
}
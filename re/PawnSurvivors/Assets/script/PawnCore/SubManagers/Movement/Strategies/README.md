# 이동 전략 패턴 (Movement Strategy Pattern)

이 폴더에는 폰(Pawn)의 실제 이동 방식을 구현하는 '전략(Strategy)' 클래스들이 위치합니다.

## 핵심 원리
'전략 패턴'을 사용하여 이동 로직을 캡슐화합니다. 각 이동 방식(키보드 조작, 특정 대상 추적 등)은 `MovementStrategyBase`를 상속받는 별개의 `MonoBehaviour` 컴포넌트로 구현됩니다.

`MovableSubManager`는 이 전략 컴포넌트들 중 현재 활성화된 하나를 찾아 `Move()` 메서드를 호출하는 방식으로 동작합니다.

## 사용법
1.  폰 게임 오브젝트에 `MovableSubManager`와 함께, 사용하고 싶은 모든 전략 컴포넌트들(`TargetMovementStrategy` 등)을 추가합니다.
2.  인스펙터에서 각 전략 컴포넌트의 세부 속성(예: `Target`, `Speed`)을 개별적으로 설정합니다.
3.  게임 시작 시 기본으로 사용할 전략 컴포넌트만 **활성화(Enable)** 상태로 두고, 나머지는 모두 비활성화합니다.

## 런타임 변경
게임 도중 이동 방식을 바꾸고 싶을 때는, 아래와 같이 `ChangeMovementStrategyEvent`를 발행하면 됩니다. `MovableSubManager`가 이벤트를 받아 해당 타입의 전략 컴포넌트를 자동으로 활성화시켜 줍니다.

```csharp
// 예시: TargetMovementStrategy로 변경 요청
pawnManager.Publish(new ChangeMovementStrategyEvent(typeof(TargetMovementStrategy)));
```

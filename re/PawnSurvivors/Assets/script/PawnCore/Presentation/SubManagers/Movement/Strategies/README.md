# 이동 전략 패턴 (Movement Strategy Pattern)

이 폴더에는 폰(Pawn)의 실제 이동 방식을 구현하는 '전략(Strategy)' 클래스들이 위치합니다.

## 핵심 원리
'전략 패턴'을 사용하여 이동 로직을 캡슐화합니다. 각 이동 방식은 `MovementStrategyBase`를 상속받는 별개의 `MonoBehaviour` 컴포넌트로 구현됩니다. `MovableSubManager`는 이 전략 컴포넌트들 중 **JSON 레시피의 `isEnabledByDefault` 설정에 따라 초기에 활성화된 전략**을 찾아 `Move()` 메서드를 호출하는 방식으로 동작합니다.

## 주요 전략
- **KeyboardMovementStrategy:** 키보드(WASD) 입력에 따라 이동합니다. `PawnData.movableData.keyboardMovement.moveSpeed`를 참조합니다.
- **DirectionalMovementStrategy:** 정해진 방향과 속도, 수명에 따라 이동합니다. (주로 투사체에 사용). `PawnData.movableData.directionalMovement`를 참조합니다.
- **TargetMovementStrategy:** `PawnData.movableData.targetMovement`에 지정된 타겟을 향해 이동합니다.
- **HomingMovementStrategy:** `PawnData.movableData.homingMovement`에 지정된 `Tag`를 가진 가장 가까운 게임 오브젝트를 동적으로 찾아 추적합니다.

## 사용법
1.  폰 게임 오브젝트는 `MovableSubManager`와 함께, 사용하고 싶은 모든 전략 컴포넌트들을 **JSON 레시피를 통해 추가**합니다.
2.  JSON 레시피의 `MovementStrategySetupData` 내 `isEnabledByDefault` 값을 `true`로 설정하여 초기에 활성화할 전략을 지정합니다. 각 전략 컴포넌트의 세부 속성(예: `moveSpeed`, `targetTag`)도 JSON 레시피를 통해 개별적으로 설정합니다.

## 런타임 변경
게임 도중 이동 방식을 바꾸고 싶을 때는, 아래와 같이 `ChangeMovementStrategyEvent`를 발행하면 됩니다. `MovableSubManager`가 이벤트를 받아 해당 타입의 전략 컴포넌트를 자동으로 활성화시켜 줍니다.

```csharp
// 예시: HomingMovementStrategy로 변경 요청
pawnManager.Publish(new ChangeMovementStrategyEvent(typeof(HomingMovementStrategy)));
```
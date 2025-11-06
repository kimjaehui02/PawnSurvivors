# 이벤트 시스템 (Event System)

이 폴더에는 `PawnManager`의 제네릭 이벤트 버스를 통해 전달되는 이벤트 클래스들이 정의되어 있습니다.

## 핵심 원리
이벤트 시스템의 목적은 각 컴포넌트 간의 **결합도(Coupling)를 낮추는 것**입니다. 어떤 컴포넌트가 다른 컴포넌트의 기능을 직접 호출하는 대신, 특정 상황이 발생했음을 알리는 '이벤트'를 발행(Publish)하면, 해당 이벤트에 관심 있는 다른 컴포넌트들이 이를 '구독(Subscribe)'하여 자신의 로직을 처리하는 방식입니다.

이를 통해 각 컴포넌트는 다른 컴포넌트의 내부 구현을 알 필요 없이 독립적으로 동작할 수 있어, 코드의 유연성과 확장성이 크게 향상됩니다.

- **이벤트 발행:** `pawnManager.Publish(new MyEvent());`
- **이벤트 구독:** `pawnManager.Subscribe<MyEvent>(HandleMyEvent);`

## 주요 이벤트
- **AttackInputEvent:** 공격 입력이 발생했음을 알립니다. (발생자: `Attacker` GameObject)
- **ChangeMovementStrategyEvent:** 이동 전략을 변경해야 함을 알립니다. (새로운 `IMovementStrategy` 타입 정보 포함)
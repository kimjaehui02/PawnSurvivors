# 이벤트 시스템 (Event System)

이 폴더에는 `PawnManager`의 제네릭 이벤트 버스를 통해 전달되는 이벤트 클래스들이 정의되어 있습니다.

## 핵심 원리
이벤트 시스템의 목적은 각 컴포넌트 간의 **결합도(Coupling)를 낮추는 것**입니다. 어떤 컴포넌트가 다른 컴포넌트의 기능을 직접 호출하는 대신, 특정 상황이 발생했음을 알리는 '이벤트'를 발행(Publish)하면, 해당 이벤트에 관심 있는 다른 컴포넌트들이 이를 '구독(Subscribe)'하여 자신의 로직을 처리하는 방식입니다.

이를 통해 각 컴포넌트는 다른 컴포넌트의 내부 구현을 알 필요 없이 독립적으로 동작할 수 있어, 코드의 유연성과 확장성이 크게 향상됩니다.

- **이벤트 발행:** `pawnManager.Publish(new MyEvent());`
- **이벤트 구독:** `pawnManager.Subscribe<MyEvent>(HandleMyEvent);`
- **구독 해지:** `pawnManager.Unsubscribe<MyEvent>(HandleMyEvent);`

## 주요 이벤트

### 입력 이벤트
- **AttackInputEvent:** 공격 입력이 발생했음을 알립니다.
  - `Attacker`: 공격을 시작한 GameObject

### 전투 이벤트
- **DamageEvent:** Pawn이 데미지를 받았을 때 발행됩니다.
  - `Target`: 데미지를 받은 PawnManager
  - `Amount`: 받은 데미지 양
  - `Attacker`: 데미지를 가한 주체 (선택적)

- **PawnDeathEvent:** Pawn이 사망했을 때 발행됩니다.
  - `DeadPawn`: 사망한 PawnManager
  - `Killer`: 사망 원인을 제공한 주체 (선택적)

- **CollisionDamageEvent:** 충돌로 인한 데미지를 처리해야 할 때 발행됩니다.
  - `Self`: 충돌한 자기 자신의 GameObject
  - `Other`: 충돌한 상대방의 Collider2D
  - `Damage`: 가할 데미지 양

### 이동 이벤트
- **ChangeMovementStrategyEvent:** 이동 전략을 변경해야 함을 알립니다.
  - `StrategyType`: 변경할 이동 전략의 타입

## 이벤트 흐름 예시

### 데미지 처리 흐름
1. `CollisionDamageSubManager`가 충돌 감지
2. 상대방 `PawnManager`에 `DamageEvent` 발행
3. `DamageableSubManager`가 `DamageEvent` 수신 및 체력 감소
4. 체력이 0 이하가 되면 `PawnDeathEvent` 발행
5. `PawnManager`가 `PawnDeathEvent` 수신 및 파괴 처리

### 공격 입력 흐름
1. `PlayerAttackInputSubManager`가 마우스 입력 감지
2. `AttackInputEvent` 발행
3. `ProjectileShooterSubManager`가 `AttackInputEvent` 수신
4. 발사체 생성 및 발사
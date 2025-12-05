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
- **DamageEvent:** Pawn이 데미지를 받아야 할 때 발행됩니다. (데미지 적용 전)
  - `Target`: 데미지를 받을 PawnManager
  - `Amount`: 받을 데미지 양
  - `Attacker`: 데미지를 가한 주체 (선택적)

- **PawnDamagedEvent:** Pawn이 실제로 데미지를 받은 후 발행됩니다. (데미지 적용 후)
  - `Target`: 데미지를 받은 PawnManager
  - `DamageApplied`: 실제로 적용된 데미지 양
  - `Attacker`: 데미지를 가한 주체 (선택적)
  - `RemainingHealth`: 피격 후 남은 체력
  - `IsFatal`: 이번 피격으로 사망했는지 여부
  - **사용 예시**: 무적, 넉백, 피격 이펙트, 피격 사운드 등

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

### 데미지 처리 흐름 (우선순위 순)
1. `CollisionDamageSubManager`가 충돌 감지
2. 상대방 `PawnManager`에 `DamageEvent` 발행
3. **[우선순위 Highest]** `InvincibilitySubManager`가 무적 중이면 `IsCancelled = true` 설정
4. **[우선순위 Normal]** `DamageableSubManager`가 `IsCancelled` 확인 후 체력 감소
5. `DamageableSubManager`가 `PawnDamagedEvent` 발행 (피격 후 추가 효과용)
6. `InvincibilitySubManager` 등이 `PawnDamagedEvent` 수신하여 추가 효과 적용
7. 체력이 0 이하가 되면 `PawnDeathEvent` 발행
8. `PawnManager`가 `PawnDeathEvent` 수신 및 파괴 처리

### 이벤트 우선순위 시스템
`Subscribe()` 메서드에 우선순위를 지정하여 실행 순서를 제어할 수 있습니다:

```csharp
// 최고 우선순위 - 데미지 차단 (무적, 쉴드 등)
_pawnManager.Subscribe<DamageEvent>(HandleDamage, EventPriority.Highest);

// 높은 우선순위 - 데미지 증감 (버프, 디버프 등)
_pawnManager.Subscribe<DamageEvent>(HandleDamage, EventPriority.High);

// 일반 우선순위 - 기본 데미지 처리 (기본값)
_pawnManager.Subscribe<DamageEvent>(HandleDamage, EventPriority.Normal);

// 낮은 우선순위 - 피격 효과 (이펙트, 사운드)
_pawnManager.Subscribe<DamageEvent>(HandleDamage, EventPriority.Low);

// 최저 우선순위 - 로그, 통계
_pawnManager.Subscribe<DamageEvent>(HandleDamage, EventPriority.Lowest);
```

**우선순위 값:**
- `Highest = 0` - 이벤트 차단/수정
- `High = 100` - 전처리
- `Normal = 200` - 기본 처리 (기본값)
- `Low = 300` - 후처리
- `Lowest = 400` - 결과 처리

### 피격 시 추가 효과 구현 방법
`PawnDamagedEvent`를 구독하여 피격 후 다양한 효과를 구현할 수 있습니다:
- **무적**: `InvincibilitySubManager` 참고 (우선순위 Highest로 데미지 차단)
- **넉백**: 피격 방향의 반대로 밀려남
- **이펙트**: 피격 이펙트 재생 (우선순위 Low 권장)
- **사운드**: 피격 사운드 재생 (우선순위 Low 권장)
- **카메라 쉐이크**: 피격 시 화면 흔들림

### 공격 입력 흐름
1. `PlayerAttackInputSubManager`가 마우스 입력 감지
2. `AttackInputEvent` 발행
3. `ProjectileShooterSubManager`가 `AttackInputEvent` 수신
4. 발사체 생성 및 발사
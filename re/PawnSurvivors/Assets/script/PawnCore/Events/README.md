# Pawn 이벤트 시스템

이 디렉토리는 `PawnManager` 내부에 구현된 제네릭 이벤트 버스를 위한 이벤트 정의를 포함합니다. 이 시스템은 `PawnSubManager` 컴포넌트와 Pawn 로직의 다른 부분들 간의 느슨하게 결합된 통신을 가능하게 합니다.

## 목적

이 이벤트 시스템의 주요 목표는 다음과 같습니다:
- **컴포넌트 분리:** 컴포넌트들은 서로 통신하기 위해 직접적인 참조를 가질 필요가 없습니다. 대신, 부모 `PawnManager`에 의해 관리되는 이벤트를 발행하거나 구독합니다.
- **확장성 향상:** 새로운 유형의 이벤트와 새로운 컴포넌트 동작을 기존 코드를 수정하지 않고도 추가할 수 있으며, 특히 `PawnManager` 자체를 수정할 필요가 없습니다.
- **이벤트 관리 중앙 집중화:** 각 `PawnManager`는 해당 Pawn과 관련된 로컬 이벤트 허브 역할을 하여, 이벤트가 범위 내에 있고 다른 Pawn들과 간섭하지 않도록 합니다.
- **유지보수성:** 하드코딩된 종속성을 줄여 코드베이스를 이해하고, 테스트하고, 유지보수하기 쉽게 만듭니다.

## 작동 방식

1.  **이벤트 정의:**
    *   이벤트는 이 `Events` 디렉토리 내에서 간단한 C# 클래스(또는 구조체)로 정의됩니다.
    *   이벤트와 관련된 데이터를 일반적으로 포함합니다. 예를 들어, `AttackInputEvent`는 `Attacker`에 대한 정보를 전달합니다.

2.  **이벤트 버스로서의 `PawnManager`:**
    *   `PawnManager` 클래스는 `Subscribe<TEvent>`, `Unsubscribe<TEvent>`, `Publish<TEvent>` 메소드를 포함합니다.
    *   이벤트 유형(`Type`)을 구독된 핸들러(`Action<TEvent>`) 목록에 매핑하기 위해 딕셔너리(`_eventHandlers`)를 사용합니다.

3.  **이벤트 발행:**
    *   컴포넌트(예: `PlayerAttackInputSubManager`)가 액션(예: 마우스 클릭)을 감지합니다.
    *   관련 이벤트 클래스의 인스턴스를 생성합니다 (예: `new AttackInputEvent(this.gameObject)`).
    *   그런 다음 `_pawnManager.Publish(eventInstance);`를 호출하여 이벤트를 브로드캐스트합니다.

4.  **이벤트 구독:**
    *   이벤트에 반응해야 하는 컴포넌트(예: `ProjectileShooterSubManager`).
    *   `OnEnable()` 메소드에서 `_pawnManager.Subscribe<TEvent>(HandleEventMethod);`를 호출합니다.
    *   `OnDisable()` 메소드에서 `_pawnManager.Unsubscribe<TEvent>(HandleEventMethod);`를 호출하여 메모리 누수를 방지합니다.
    *   `HandleEventMethod`는 이벤트 데이터를 인자로 받아 필요한 동작을 수행하는 컴포넌트 내의 private 메소드입니다.

## 예시: 공격 입력 이벤트

-   **이벤트 정의:** `AttackInputEvent.cs`는 공격 입력 이벤트가 전달하는 데이터(예: `Attacker` GameObject)를 정의합니다.
-   **발행자 (`PlayerAttackInputSubManager`):** 플레이어 입력을 감지하고, `AttackInputEvent`를 생성한 다음 `_pawnManager.Publish(attackEvent);`를 호출합니다.
-   **구독자 (`ProjectileShooterSubManager`):** `_pawnManager.Subscribe<AttackInputEvent>(HandleAttackInput);`를 통해 `AttackInputEvent`를 구독합니다. `AttackInputEvent`가 발행되면 `HandleAttackInput` 메소드가 호출되고, 이는 다시 `Attack()` 로직을 트리거합니다.

# 프로젝트 진행 상황 요약 (2025년 10월 26일)

이 문서는 현재까지 진행된 주요 작업 내용과 변경 사항을 요약합니다.

## 1. 핵심 아키텍처 개선

### 1.1. PawnManager의 제네릭 이벤트 버스 구현
-   **파일:** `Assets/script/PawnCore/PawnManager.cs`
-   **변경 내용:**
    -   `using System;` 및 `using System.Linq;` 추가.
    -   특정 `OnAttackInput` 이벤트 대신, `Subscribe<TEvent>`, `Unsubscribe<TEvent>`, `Publish<TEvent>` 메소드를 포함하는 제네릭 이벤트 버스 시스템 구현.
    -   `_eventHandlers` 딕셔너리를 통해 다양한 이벤트 타입의 핸들러 관리.
    -   `OnDisable()` 시 구독 해지 로직 추가로 메모리 누수 방지.
    -   모든 관련 코드에 한국어 주석 추가.
-   **목적:** Pawn 내부 컴포넌트 간의 느슨한 결합을 통한 유연하고 확장 가능한 통신 시스템 구축.

### 1.2. 이벤트 타입 정의
-   **파일:** `Assets/script/PawnCore/Events/AttackInputEvent.cs` (새로 생성)
-   **변경 내용:**
    -   공격 입력 이벤트를 나타내는 `AttackInputEvent` 클래스 정의.
    -   이벤트 발생자(`Attacker` GameObject) 정보를 포함.
    -   모든 관련 코드에 한국어 주석 추가.
-   **파일:** `Assets/script/PawnCore/Events/ChangeMovementStrategyEvent.cs` (새로 생성)
-   **변경 내용:**
    -   이동 전략 변경 이벤트를 나타내는 `ChangeMovementStrategyEvent` 클래스 정의.
    -   새로운 `IMovementStrategy` 인스턴스 정보를 포함.

### 1.3. PawnSubManager 개선
-   **파일:** `Assets/script/PawnCore/PawnSubManager.cs`
-   **변경 내용:**
    -   `_pawnManager` 필드의 접근 제한자를 `private`에서 `protected`로 변경하여 파생 클래스에서 접근 가능하도록 수정.

## 2. 이동 시스템 리팩토링 (전략 패턴 적용)

### 2.1. IMovementStrategy 인터페이스 정의
-   **파일:** `Assets/script/PawnCore/SubManagers/Movement/IMovementStrategy.cs` (새로 생성)
-   **변경 내용:**
    -   모든 이동 전략이 구현해야 할 `Move(PawnManager pawnManager, float deltaTime)` 메소드 정의.

### 2.2. MovableSubManager (이동 컨트롤러) 구현
-   **파일:** `Assets/script/PawnCore/SubManagers/Movement/MovableSubManager.cs` (새로 생성 및 수정)
-   **변경 내용:**
    -   Pawn의 이동을 총괄하는 대표 서브매니저 역할.
    -   `[SerializeReference]`를 사용하여 `_currentStrategy` 필드에 `IMovementStrategy` 구현체 참조.
    -   `MovementStrategyType` enum을 사용하여 인스펙터에서 초기 전략을 선택할 수 있도록 구현.
    -   `SubStart()`에서 선택된 `enum` 값에 따라 해당 전략 인스턴스화.
    -   `SubUpdate()`에서 `_currentStrategy.Move()`를 호출하여 실제 이동 로직 위임.
    -   `SetStrategy()` 메소드를 통해 런타임에 전략 변경 가능.

### 2.3. 기존 이동 로직을 전략 클래스로 변환
-   **파일:** `Assets/script/PawnCore/SubManagers/Movement/KeyboardMovementStrategy.cs` (이전 `MovableSubManager.cs`에서 이름 변경 및 수정)
    -   `IMovementStrategy` 구현.
    -   `PawnSubManager` 상속 제거.
    -   키보드 입력 기반 이동 로직을 `Move()` 메소드에 구현.
-   **파일:** `Assets/script/PawnCore/SubManagers/Movement/DirectionalMovementStrategy.cs` (이전 `DirectionalMovementSubManager.cs`에서 이름 변경 및 수정)
    -   `IMovementStrategy` 구현.
    -   `PawnSubManager` 상속 제거.
    -   방향 기반 이동 및 수명 관리 로직을 `Move()` 메소드에 구현.
-   **파일:** `Assets/script/PawnCore/SubManagers/Movement/TargetMovementStrategy.cs` (이전 `TargetMovementSubManager.cs`에서 이름 변경 및 수정)
    -   `IMovementStrategy` 구현.
    -   `PawnSubManager` 상속 제거.
    -   타겟 추적 이동 로직을 `Move()` 메소드에 구현.

### 2.4. MovementUsecases 업데이트
-   **파일:** `Assets/script/PawnCore/Usecases/MovementUsecases.cs`
-   **변경 내용:**
    -   `MoveInDirection` 메소드 추가.
    -   `HandleLifetime` 메소드를 `ref` 키워드를 사용하는 방식으로 수정.

## 3. 전투 시스템 리팩토링

### 3.1. CollisionDamageSubManager 개선
-   **파일:** `Assets/script/PawnCore/SubManagers/Combat/CollisionDamageSubManager.cs` (이전 `ProjectileCollisionSubManager.cs`에서 이름 변경 및 수정)
-   **변경 내용:**
    -   `OnTriggerEnter2D` 로직을 `CombatUsecases.HandleCollisionDamage`로 위임.

### 3.2. ProjectileShooterSubManager 개선
-   **파일:** `Assets/script/PawnCore/SubManagers/Combat/ProjectileShooterSubManager.cs`
-   **변경 내용:**
    -   `Attack()` 로직을 `CombatUsecases.HandleProjectileAttack`로 위임.

### 3.3. CombatUsecases 업데이트
-   **파일:** `Assets/script/PawnCore/Usecases/CombatUsecases.cs`
-   **변경 내용:**
    -   `HandleCollisionDamage` 메소드 추가.
    -   `HandleProjectileAttack` 메소드 추가.

## 4. 파일 및 폴더 정리

-   `Assets/script/PawnCore/SubManagers/` 하위의 서브매니저 스크립트들을 `Movement`, `Combat`, `Input` 폴더로 분류하여 이동.
-   `Assets/script/PawnCore/Events/` 폴더 생성 및 이벤트 정의 파일 저장.

---

# PawnCore 아키텍처

`PawnCore`는 이 게임의 핵심 아키텍처로, 게임 내 모든 캐릭터(Pawn)의 공통 기능을 모듈화하여 관리합니다. Pawn의 이동, 전투, 입력, **시각적 표현, 물리적 특성** 등 다양한 기능들이 독립적인 부품처럼 설계되어, 유지보수와 확장이 용이한 구조를 지향합니다.

## 핵심 구성 요소

1.  **PawnManager (중앙 허브):** 각 Pawn 객체에 대한 중앙 관제탑 역할을 합니다. 자신에게 등록된 여러 `PawnSubManager`들의 생명주기를 관리하고, 이들 간의 통신을 중재하는 이벤트 버스를 제공합니다. 또한, Pawn의 모든 상태 데이터를 담는 `PawnData` 객체를 소유하며, `SubManager`들의 초기화 시점을 제어합니다.

2.  **PawnData (도메인 엔티티):** Pawn의 모든 상태(체력, 속도, 공격력, **시각적 프리팹 경로, 물리 설정** 등)를 담는 순수한 C# 데이터 객체입니다. `SubManager`들은 이 `PawnData`를 참조하여 데이터를 사용합니다.

3.  **Domain 계층 (`PawnCore/Domain`):**
    *   **Events (소통 채널):** `PawnManager`가 제공하는 제네릭 이벤트 시스템을 통해, 각 컴포넌트들은 서로를 직접 참조하지 않고도 상호작용할 수 있습니다. (Decoupling)
    *   **Usecases (순수 비즈니스 로직):** 애플리케이션 고유의 순수 비즈니스 로직을 담고 있습니다. `PawnData`를 직접 받아 로직을 처리합니다.

4.  **Presentation 계층 (`PawnCore/Presentation`):**
    *   **SubManagers (기능 단위):** Pawn의 특정 기능(예: 이동, 전투, **시각, 물리**)을 담당하는 독립적인 컴포넌트입니다. Unity 엔진의 세계와 도메인 계층을 연결하는 어댑터 역할을 수행합니다. `PawnData`를 참조하여 데이터를 사용합니다.

5.  **Recipes (설계도):** `CreationManager`가 사용하는 **JSON 기반** 데이터 시스템입니다. **JSON 파일**을 통해 Pawn의 구성을 정의하여, 복잡한 프리팹 없이도 일관된 방식으로 Pawn을 생성할 수 있습니다.

## 클린 아키텍처 관점

`PawnCore`는 클린 아키텍처의 **도메인 계층(Domain Layer)** 에 해당합니다. 게임의 핵심 규칙(Pawn은 무엇인가, 어떻게 동작하는가)을 정의하며, Unity 프레임워크나 외부 환경에 대한 의존성을 최소화합니다.

- **Entities:** `PawnData` 객체가 핵심 엔티티입니다.
- **Use Cases:** `Domain/Usecases` 폴더의 클래스들은 애플리케이션 고유의 순수 비즈니스 로직을 담고 있습니다.
- **Interface Adapters:** `Presentation/SubManagers` 폴더의 클래스들은 프레젠테이션 계층(Unity 엔진)과 도메인 계층을 연결하는 어댑터 역할을 수행합니다. 새로 추가된 **`VisualSubManager`와 `PhysicsSubManager`도 이 계층의 일부**입니다.
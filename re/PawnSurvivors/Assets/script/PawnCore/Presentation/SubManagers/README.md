# 서브매니저 (SubManagers)

이 폴더에는 `Pawn`의 구체적인 기능들을 담당하는 서브매니저(SubManager) 클래스들이 기능별 하위 폴더로 나뉘어 저장됩니다. **최근 `Visual` 및 `Physics` 관련 서브매니저가 추가되어 Pawn의 시각적 요소와 물리적 특성을 동적으로 관리합니다.**

모든 서브매니저는 `PawnSubManager` 베이스 클래스를 상속받으며, `PawnManager`에 의해 생명주기가 관리됩니다.

## 주요 서브매니저 구조

### VisualSubManager
- **역할**: Pawn의 시각적 표현을 담당합니다.
- **구조**:
    - `VisualSubManager`는 자신의 `GameObject`에 직접 `SpriteRenderer`를 추가하는 대신, **"Visuals"** 라는 이름의 자식 `GameObject`를 생성합니다.
    - 모든 시각적 컴포넌트(현재는 `SpriteRenderer`)는 이 "Visuals" 자식 `GameObject`에 추가됩니다.
- **장점**:
    - Pawn의 물리적 위치/회전과 시각적 표현을 분리하여, 애니메이션 등의 시각 효과가 물리 동작에 영향을 주지 않도록 합니다.
    - 향후 파티클, 추가 스프라이트 등 복잡한 시각 요소를 "Visuals" 자식 아래에 체계적으로 구성할 수 있습니다.

### PhysicsSubManager
- **역할**: Pawn의 물리적 특성을 JSON 레시피에 따라 동적으로 설정합니다.
- **기능**: `Rigidbody2D`, `Collider2D`의 타입과 속성, `Layer`, `Tag` 등을 `PawnData`에 정의된 값으로 `GameObject`에 직접 설정합니다.

### MovableSubManager
- **역할**: Pawn의 이동을 총괄하며, 다양한 이동 방식을 '전략(Strategy)'으로 관리합니다.
- **구조**: `MovableSubManager` 자체는 이동 로직을 가지지 않으며, `KeyboardMovementStrategy`, `HomingMovementStrategy` 등 `MovementStrategyBase`를 상속받는 실제 전략 컴포넌트 중 하나를 활성화하여 이동을 위임합니다.

## 클린 아키텍처 관점
서브매니저들은 클린 아키텍처의 **프레젠테이션 계층(Presentation Layer)** 에 해당합니다.

이들은 Unity 엔진의 세계(입력, 업데이트 루프, 물리 등)와 `PawnCore`의 순수한 도메인 계층(Usecases, Events 등) 사이를 연결하는 **어댑터(Adapter)** 역할을 수행합니다. 예를 들어, Unity의 입력을 받아 도메인 이벤트를 발생시키거나, `Update` 루프에 맞춰 도메인의 유스케이스를 호출하는 등의 작업을 처리합니다. **새로 추가된 `VisualSubManager`는 시각적 프리팹을 관리하고, `PhysicsSubManager`는 콜라이더, 리지드바디, 레이어, 태그 등의 물리 설정을 코드 베이스에서 동적으로 처리합니다.**

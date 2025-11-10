# 프로젝트 진행 상황 요약 (2025년 11월 10일)

이 문서는 2025년 11월 10일에 진행된 주요 작업 내용과 변경 사항을 요약합니다.

## 1. 제네릭 이벤트 버스 시스템 전면 적용

### 브랜치: feature/event-bus-integration

### 1.1. 새로운 이벤트 타입 정의
-   **파일:** `Assets/script/PawnCore/Domain/Events/`
-   **추가된 이벤트:**
    -   `DamageEvent.cs`: Pawn이 데미지를 받았을 때 발행되는 이벤트
    -   `PawnDeathEvent.cs`: Pawn이 사망했을 때 발행되는 이벤트
    -   `CollisionDamageEvent.cs`: 충돌로 인한 데미지를 처리해야 할 때 발행되는 이벤트
-   **변경 내용:**
    -   기존 이벤트들(`AttackInputEvent`, `ChangeMovementStrategyEvent`)에 `PawnCore.Domain.Events` namespace 추가
    -   모든 이벤트에 상세한 주석 추가

### 1.2. DamageableSubManager 이벤트 기반으로 전환
-   **파일:** `Assets/script/PawnCore/Presentation/SubManagers/Combat/DamageableSubManager.cs`
-   **변경 내용:**
    -   `DamageEvent`를 구독하여 데미지를 처리하도록 변경
    -   체력이 0 이하가 되면 `PawnDeathEvent`를 발행
    -   `TakeDamage()` 메서드는 내부적으로 `DamageEvent`를 발행하도록 수정 (하위 호환성 유지)
    -   `OnDisable()`에서 이벤트 구독 해지 추가
-   **목적:** 직접 호출 방식에서 이벤트 기반 방식으로 전환하여 결합도 감소

### 1.3. CollisionDamageSubManager 이벤트 기반으로 전환
-   **파일:** `Assets/script/PawnCore/Presentation/SubManagers/Combat/CollisionDamageSubManager.cs`
-   **변경 내용:**
    -   충돌 시 상대방 `PawnManager`에 직접 `DamageEvent`를 발행
    -   `CombatUsecases.HandleCollisionDamage()` 호출 제거
    -   충돌 후 자신을 파괴하는 로직을 직접 처리
-   **목적:** SubManager가 이벤트 버스를 직접 활용하여 독립적으로 작동

### 1.4. PawnManager에 사망 이벤트 처리 추가
-   **파일:** `Assets/script/PawnCore/PawnManager.cs`
-   **변경 내용:**
    -   `OnEnable()`에서 `PawnDeathEvent` 구독
    -   `HandlePawnDeath()` 메서드 추가: 자신의 사망 이벤트를 받으면 `DestroyPawn()` 호출
    -   `PawnCore.Domain.Events` namespace import 추가
-   **목적:** Pawn의 생명주기를 이벤트 기반으로 관리

### 1.5. CombatUsecases 단순화
-   **파일:** `Assets/script/PawnCore/Domain/Usecases/CombatUsecases.cs`
-   **변경 내용:**
    -   `ApplyDamage()` 메서드 제거 (이제 `DamageableSubManager`가 이벤트로 처리)
    -   `HandleCollisionDamage()` 메서드 제거 (이제 `CollisionDamageSubManager`가 직접 처리)
    -   발사체 관련 로직만 유지
-   **목적:** Usecase를 단순화하고, 각 SubManager가 자율적으로 동작하도록 변경

### 1.6. 모든 SubManager에 namespace import 추가
-   **파일:** 
    -   `PlayerAttackInputSubManager.cs`
    -   `ProjectileShooterSubManager.cs`
    -   `MovableSubManager.cs`
-   **변경 내용:**
    -   `using PawnCore.Domain.Events;` 추가
    -   각 클래스에 상세한 주석 추가
-   **목적:** 이벤트 타입을 일관되게 사용하고, 코드 가독성 향상

### 1.7. 문서화 업데이트
-   **파일:** `Assets/script/PawnCore/Domain/Events/README.md`
-   **변경 내용:**
    -   새로 추가된 이벤트들 문서화
    -   이벤트 흐름 예시 추가 (데미지 처리 흐름, 공격 입력 흐름)
    -   각 이벤트의 속성 상세 설명 추가

## 주요 개선 사항
-   **완전한 이벤트 기반 아키텍처:** 모든 SubManager가 제네릭 이벤트 버스를 통해 통신
-   **낮은 결합도:** SubManager들이 서로를 직접 참조하지 않음
-   **높은 확장성:** 새로운 이벤트와 핸들러를 쉽게 추가 가능
-   **메모리 안전성:** 모든 SubManager가 `OnDisable()`에서 이벤트 구독 해지
-   **명확한 책임 분리:** 각 SubManager가 자신의 역할에만 집중

## 2. LifecycleManager 개선

### 2.1. EarlyUpdate/LateUpdate 큐 시스템
-   **파일:** `Assets/script/Managers/LifecycleManager.cs`
-   **변경 내용:**
    -   Update를 3단계로 분리: EarlyUpdate → MainUpdate → LateUpdate
    -   `EnqueueEarlyUpdate()`: Update 시작 시 실행할 액션 큐
    -   `EnqueueLateUpdate()`: Update 끝에 실행할 액션 큐
    -   `RequestDestruction()`: LateUpdate 큐를 활용한 안전한 파괴
    -   각 단계별 메서드 분리로 가독성 향상
-   **목적:** Unity의 Update/LateUpdate 개념을 확장하여 실행 순서 명확화

### 2.2. 일시정지 시스템
-   **변경 내용:**
    -   `IsPaused` bool 플래그로 일시정지 제어 (Time.timeScale 사용 안 함)
    -   ESC 키로 일시정지 토글
-   **이유:** Time.timeScale은 물리/애니메이션도 멈춰서 의도하지 않은 부작용 발생

## 3. UI 시스템 기초 작업

### 3.1. 단순화된 UI 화면 스크립트
-   **파일:** `Assets/script/UI/Screens/`
-   **생성된 화면:**
    -   `TitleScreen.cs`: 타이틀 화면 (Start, Quit 버튼)
    -   `MainMenuScreen.cs`: 메인 메뉴 (스테이지 선택, Back 버튼)
    -   `GameplayHUD.cs`: 게임플레이 HUD (체력, 점수, 시간 표시)
    -   `PauseMenuScreen.cs`: 일시정지 메뉴 (ESC 키 토글)
    -   `GameOverScreen.cs`: 게임 오버 화면
    -   `StageClearScreen.cs`: 스테이지 클리어 화면
-   **설계 원칙:**
    -   각 화면이 완전히 독립적으로 작동
    -   씬에 Canvas 배치만 하면 자동 작동
    -   복잡한 State Machine 제거 (과도한 설계 방지)
    -   GameManager.Instance로 필요한 기능 접근
-   **목적:** 간단하고 직관적인 UI 시스템

### 3.2. GameManager 통합
-   **파일:** `Assets/script/Managers/GameManager.cs`
-   **변경 내용:**
    -   `DontDestroyOnLoad(gameObject)` 추가
    -   `StartStage(stageName)` 메서드 추가: UI에서 호출 가능한 스테이지 시작 함수
    -   자동 실행 로직 제거 (UI 버튼으로만 게임 시작)
-   **목적:** UI와의 연동 지원

### 3.3. 문서 작업
-   **파일:** `Assets/script/UI/`
    -   `README.md`: UI 시스템 개요
    -   `SETUP_GUIDE.md`: Unity 에디터 설정 가이드
-   **파일:** `Assets/script/Managers/README.md`
    -   매니저 간 연계 구조 설명 추가

## 4. 전투 시스템 개선

### 4.1. ProjectileShooter 자동 발사로 변경
-   **파일:** `Assets/script/PawnCore/Presentation/SubManagers/Combat/ProjectileShooterSubManager.cs`
-   **변경 내용:**
    -   클릭 발사 이벤트 구독 주석 처리 (임시 비활성화)
    -   `SubUpdate()`에서 매 프레임 가장 가까운 적 탐지
    -   적이 있으면 자동 발사
-   **목적:** 클릭 없이 자동으로 적 공격 (게임플레이 개선)

## 5. 코드 정리

### 5.1. 불필요한 파일 제거
-   **제거된 폴더:** `Assets/script/PawnCore/Recipes/Setups/`
-   **이유:** ScriptableObject 방식 사용 안 함, JSON 방식으로 대체됨

---

## 📝 향후 계획

### UI 시스템
-   [ ] 체력 UI 시스템 구현
    -   플레이어: 고정 위치 UI
    -   적: 머리 위 체력바 (선택적)
    -   다수 아군 관리 고려 (최적화 중요)
-   [ ] 씬 전환 시스템 구현
    -   타이틀 → 메인 메뉴 → 게임플레이
-   [ ] 일시정지 메뉴와 LifecycleManager 연동
-   [ ] 게임 오버/스테이지 클리어 조건 구현

### 게임플레이 시스템
-   [ ] 아군 집결 시스템 구획 (기획 중)
    -   중앙 집결 좌표 조종
    -   아군이 집결 좌표를 따라옴
    -   다수 유닛 관리 최적화
-   [ ] 점수 시스템
-   [ ] 스테이지 클리어 조건
-   [ ] 적 AI 개선

---

# 프로젝트 진행 상황 요약 (2025년 11월 9일)

이 문서는 2025년 11월 9일에 진행된 주요 작업 내용과 변경 사항을 요약합니다.

## 1. 시각 시스템 리팩토링 및 사망 로직 구현

### 1.1. VisualSubManager 구조 개선
-   **파일:** `Assets/script/PawnCore/Presentation/SubManagers/Visual/VisualSubManager.cs`
-   **변경 내용:**
    -   `VisualSubManager`가 Pawn의 루트 `GameObject`에 직접 `SpriteRenderer`를 추가하는 대신, "Visuals"라는 이름의 자식 `GameObject`를 생성하고 여기에 `SpriteRenderer`를 추가하도록 변경했습니다.
-   **목적:**
    -   Pawn의 로직/물리적 Transform과 시각적 Transform을 분리하여 관심사를 명확히 합니다.
    -   향후 애니메이션(예: 이동 시 콩콩 뛰는 효과) 구현 시 물리적 충돌에 영향을 주지 않고 시각적 요소만 독립적으로 제어할 수 있는 유연한 구조를 확보합니다.

### 1.2. Pawn 사망 로직 구현
-   **파일:** `Assets/script/PawnCore/Domain/Usecases/CombatUsecases.cs`, `Assets/script/PawnCore/Presentation/SubManagers/Combat/DamageableSubManager.cs`
-   **변경 내용:**
    -   `CombatUsecases.ApplyDamage` 메서드가 `PawnData` 대신 `PawnManager` 인스턴스를 받도록 수정했습니다.
    -   `DamageableSubManager`는 `TakeDamage` 호출 시 자신의 `PawnManager`를 `ApplyDamage`로 전달합니다.
    -   `ApplyDamage` 내에서 체력을 감소시킨 후, `currentHealth`가 0 이하일 경우 `pawnManager.DestroyPawn()`을 호출하여 해당 Pawn의 제어된 파괴를 요청하는 로직을 추가했습니다.
-   **목적:** Pawn의 체력이 0이 되었을 때 `LifecycleManager`를 통해 안전하게 게임 세계에서 제거되는 핵심 게임플레이 루프를 구현합니다.

---

# 프로젝트 진행 상황 요약 (2025년 11월 6일)

이 문서는 현재까지 진행된 주요 작업 내용과 변경 사항을 요약합니다. 이전에 작성된 내용에 더해 최근 변경사항들을 포함합니다.

## 1. 핵심 아키텍처 개선

### 1.1. PawnManager의 제네릭 이벤트 버스 구현
-   **파일:** `Assets/script/PawnCore/PawnManager.cs`
-   **변경 내용:**
    -   `using System;` 및 `using System.Linq;` 추가.
    -   특정 `OnAttackInput` 이벤트 대신, `Subscribe<TEvent>`, `Unsubscribe<TEvent>`, `Publish<TEvent>` 메소드를 포함하는 제네릭 이벤트 버스 시스템 구현.
    -   `_eventHandlers` 딕셔너리를 통해 다양한 이벤트 타입의 핸들러 관리.
    -   `OnDisable()` 시 구독 해지 로직 추가로 메모리 누수 방지.
    -   모든 관련 코드에 한국어 주석 추가.
    -   **PawnSubManager 초기화 로직 개선**: `LifecycleManager`를 통한 `SubStart()` 호출 대신, `PawnManager` 내부에 `InitializeSubManagers()` 메서드를 추가하여 `PawnData`가 완전히 초기화된 후에 `SubManager`들의 `SubStart()`가 호출되도록 변경.
-   **목적:** Pawn 내부 컴포넌트 간의 느슨한 결합을 통한 유연하고 확장 가능한 통신 시스템 구축 및 초기화 시점의 안정성 확보.

### 1.2. 이벤트 타입 정의
-   **파일:** `Assets/script/PawnCore/Domain/Events/AttackInputEvent.cs` (새로 생성)
-   **변경 내용:**
    -   공격 입력 이벤트를 나타내는 `AttackInputEvent` 클래스 정의.
    -   이벤트 발생자(`Attacker` GameObject) 정보를 포함.
    -   모든 관련 코드에 한국어 주석 추가.
-   **파일:** `Assets/script/PawnCore/Domain/Events/ChangeMovementStrategyEvent.cs` (새로 생성)
-   **변경 내용:**
    -   이동 전략 변경 이벤트를 나타내는 `ChangeMovementStrategyEvent` 클래스 정의.
    -   새로운 `IMovementStrategy` 인스턴스 정보를 포함.

### 1.3. PawnSubManager 개선
-   **파일:** `Assets/script/PawnCore/PawnSubManager.cs`
-   **변경 내용:**
    -   `_pawnManager` 필드의 접근 제한자를 `private`에서 `protected`로 변경하여 파생 클래스에서 접근 가능하도록 수정.

## 2. 이동 시스템 리팩토링 (전략 패턴 적용)

### 2.1. IMovementStrategy 인터페이스 및 MovementStrategyBase 정의
-   **파일:** `Assets/script/PawnCore/Presentation/SubManagers/Movement/IMovementStrategy.cs` (새로 생성), `Assets/script/PawnCore/Presentation/SubManagers/Movement/Strategies/MovementStrategyBase.cs`
-   **변경 내용:**
    -   모든 이동 전략이 구현해야 할 `Move(PawnManager pawnManager, float deltaTime)` 메소드 정의.
    -   `MovementStrategyBase`에 `Init(PawnManager)` 및 `SetInitialEnabledState(bool)` 메서드 추가하여 `PawnManager` 참조 초기화 및 초기 활성화 상태 설정 기능 제공.

### 2.2. MovableSubManager (이동 컨트롤러) 구현 및 개선
-   **파일:** `Assets/script/PawnCore/Presentation/SubManagers/Movement/MovableSubManager.cs`
-   **변경 내용:**
    -   Pawn의 이동을 총괄하는 대표 서브매니저 역할.
    -   `[SerializeReference]`를 사용하여 `_currentStrategy` 필드에 `IMovementStrategy` 구현체 참조.
    -   `SubStart()`에서 `MovementStrategySetupData.isEnabledByDefault` 값을 기반으로 초기 `_currentStrategy`를 설정하고, 하나의 전략만 활성화되도록 로직 개선.
    -   `SubUpdate()`에서 `_currentStrategy.Move()`를 호출하여 실제 이동 로직 위임.
    -   `SetStrategy()` 메소드를 통해 런타임에 전략 변경 가능.

### 2.3. 기존 이동 로직을 전략 클래스로 변환
-   **파일:** `Assets/script/PawnCore/Presentation/SubManagers/Movement/Strategies/KeyboardMovementStrategy.cs`, `Assets/script/PawnCore/Presentation/SubManagers/Movement/Strategies/DirectionalMovementStrategy.cs`, `Assets/script/PawnCore/Presentation/SubManagers/Movement/Strategies/TargetMovementStrategy.cs`, `Assets/script/PawnCore/Presentation/SubManagers/Movement/Strategies/HomingMovementStrategy.cs`
-   **변경 내용:**
    -   각 전략 클래스가 `MovementStrategyBase`를 상속받고 `Init()` 및 `Move()` 메서드를 구현하여 `PawnData`를 통해 이동 관련 속성 (`moveSpeed`, `speed`, `lifetime` 등)을 가져오도록 변경.

### 2.4. MovementUsecases 업데이트
-   **파일:** `Assets/script/PawnCore/Domain/Usecases/MovementUsecases.cs`
-   **변경 내용:**
    -   `MoveInDirection` 메소드 추가.
    -   `HandleLifetime` 메소드를 `ref` 키워드를 사용하는 방식으로 수정.
    -   `MoveWithInput` 등에서 `PawnData`의 `MovableData`를 참조하여 이동 속성 사용.

## 3. 전투 시스템 리팩토링

### 3.1. CollisionDamageSubManager 개선
-   **파일:** `Assets/script/PawnCore/Presentation/SubManagers/Combat/CollisionDamageSubManager.cs`
-   **변경 내용:**
    -   `OnTriggerEnter2D` 로직을 `CombatUsecases.HandleCollisionDamage`로 위임.

### 3.2. ProjectileShooterSubManager 개선
-   **파일:** `Assets/script/PawnCore/Presentation/SubManagers/Combat/ProjectileShooterSubManager.cs`
-   **변경 내용:**
    -   `Attack()` 로직을 `CombatUsecases.HandleProjectileAttack`로 위임.
    -   `firePoint`는 `PhysicsSubManager` 또는 `VisualSubManager`를 통해 생성된 `GameObject`의 자식으로 자동 생성되도록 변경.

### 3.3. CombatUsecases 업데이트
-   **파일:** `Assets/script/PawnCore/Domain/Usecases/CombatUsecases.cs`
-   **변경 내용:**
    -   `HandleCollisionDamage` 메소드 추가: 충돌한 객체의 `DamageableSubManager`를 찾아 데미지 적용 후 총알 `Pawn` 파괴.
    -   `HandleProjectileAttack` 메소드 추가.
    -   `ApplyDamage` 메서드 추가: `PawnData`의 `currentHealth`를 직접 감소시키고 0 미만으로 내려가지 않도록 처리.

## 4. 시각 및 물리 시스템 통합 (JSON 레시피 기반)

### 4.1. VisualSubManager 도입
-   **파일:** `Assets/script/PawnCore/Presentation/SubManagers/Visual/VisualSubManager.cs` (새로 생성)
-   **변경 내용:**
    -   `PawnData.visualPrefabName`을 읽어 시각적 프리팹을 로드하고 인스턴스화하는 역할 담당.
    -   `visualPrefabName`이 없거나 프리팹을 찾지 못할 경우 `Assets/Resources/Prefabs/Circle.prefab`을 기본값으로 사용.
-   **목적:** Pawn의 시각적 요소를 JSON 레시피를 통해 유연하게 관리.

### 4.2. PhysicsSubManager 도입
-   **파일:** `Assets/script/PawnCore/Presentation/SubManagers/Physics/PhysicsSubManager.cs` (새로 생성)
-   **변경 내용:**
    -   `PawnData.physicsData`를 기반으로 `Collider2D`, `Rigidbody2D` 컴포넌트를 동적으로 추가하고 설정 (Type, isTrigger, BodyType, GravityScale).
    -   `GameObject`의 `Layer`와 `Tag`를 `PawnData.physicsData`에 정의된 이름으로 할당.
-   **목적:** Pawn의 물리적 특성을 JSON 레시피를 통해 동적으로 설정하고, 수동 설정의 필요성 제거.

### 4.3. PawnData에 물리 데이터 추가
-   **파일:** `Assets/script/PawnCore/Domain/PawnData.cs`
-   **변경 내용:**
    -   `PhysicsData` 중첩 클래스와 `ColliderType`, `RigidbodyType` Enum 정의.
    -   `PawnData`에 `physicsData` 필드 추가하여 물리 관련 속성들을 통합 관리.
    -   `visualPrefabName` 필드 추가.

### 4.4. RecipeData.cs에 SetupData 추가 및 수정
-   **파일:** `Assets/script/PawnCore/Recipes/Json/RecipeData.cs`
-   **변경 내용:**
    -   `SubManagerSetupData` 추상 클래스에 `AddSubManagerComponent(GameObject pawnObject)` 추상 메서드 추가.
    -   각 구체적인 `SubManagerSetupData` (예: `DamageableSubManagerSetupData`, `MovableSubManagerSetupData`, `VisualSubManagerSetupData`, `PhysicsSubManagerSetupData` 등)에 `ApplyToPawnData()` 및 `AddSubManagerComponent()` 메서드 구현.
    -   `MovementStrategySetupData` 추상 클래스에 `AddMovementStrategyComponent(GameObject pawnObject)` 추상 메서드 추가 및 각 파생 클래스에서 구현.
    -   `PawnRecipeData`에서 `visualPrefab` 필드 제거 (VisualSubManagerSetupData로 대체).

### 4.5. JSON 레시피 파일 업데이트 (`Player.json`, `Bullet.json`, `Enemy.json`)
-   **파일:** `Assets/StreamingAssets/Recipes/Player.json`, `Bullet.json`, `Enemy.json`
-   **변경 내용:**
    -   각 레시피에서 `visualPrefab` 필드를 제거하고, `subManagerSetups`에 `PawnCore.Recipes.Json.VisualSubManagerSetupData`와 `PawnCore.Recipes.Json.PhysicsSubManagerSetupData`를 추가하여 시각 및 물리 설정을 JSON으로 관리.
    -   `Enemy.json` 파일에 주석 제거.

## 5. 전역 매니저 개선

### 5.1. GameManager 개선
-   **파일:** `Assets/script/Managers/GameManager.cs`
-   **변경 내용:**
    -   `Start()` 메서드에 플레이어 및 적 `Pawn`을 `CreationManager`를 통해 생성하는 임시 로직 추가.
    -   `CreationManager.LoadAllRecipes()` 호출 제거 (CreationManager의 `Awake()`에서 처리).

### 5.2. CreationManager 개선
-   **파일:** `Assets/script/Managers/CreationManager.cs`
-   **변경 내용:**
    -   `Awake()`에서 `RecipeLoader`를 초기화하고 모든 레시피를 로드.
    -   `CreatePawn(PawnRecipeData, ...)` 메서드에서 `PawnManager.InitializeSubManagers()`를 호출하여 `SubManager` 초기화 시점 보장.
    -   `visualPrefab`을 직접 로드하는 로직 제거.

## 6. 파일 및 폴더 정리

-   `Assets/script/PawnCore/Recipes/Setups/` 폴더 내의 모든 ScriptableObject 기반 `SubManagerSetup.cs` 파일들 삭제 (JSON 기반 레시피로 대체).
-   `Assets/script/PawnCore/Presentation/SubManagers/` 하위에 `Visual/` 및 `Physics/` 폴더 생성 및 관련 `SubManager` 파일 저장.
-   `Assets/Resources/Prefabs/` 폴더 생성 및 `Circle.prefab` 이동 (Resources.Load() 호환성 확보).

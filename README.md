# PawnSurvivors

Unity 기반의 생존 게임 프로젝트입니다. 클린 아키텍처 원칙을 따라 설계되었으며, JSON 레시피 시스템을 통해 데이터 기반으로 게임 오브젝트를 생성하는 구조를 가지고 있습니다.

## 📋 프로젝트 개요

PawnSurvivors는 캐릭터(Pawn) 기반의 생존 게임으로, **클린 아키텍처**와 **이벤트 기반 아키텍처**를 적용하여 확장 가능하고 유지보수하기 쉬운 구조로 설계되었습니다.

---

## 🛠️ 기술 스택 및 아키텍처

### 핵심 기술 및 사용 이유

#### 1. 클린 아키텍처 (Clean Architecture)
**사용 이유**: 
- 도메인 로직과 Unity 프레임워크의 명확한 분리
- 비즈니스 로직의 재사용성 및 테스트 용이성 확보
- 계층 간 의존성 방향 제어로 유지보수성 향상

**구현 방식**:
```
Presentation 계층 (Unity 연동)
    ↓ (UseCase 사용)
Domain 계층 (비즈니스 로직)
    ↓ (Repository 인터페이스 사용)
Data 계층 (데이터 저장)
```

**주요 특징**:
- **엄격한 데이터 접근 규칙**: `GameSessionData`는 Data 계층 내부에만 존재, 외부는 `ISessionDataRepository` 인터페이스를 통해서만 접근
- **UseCase 중심 설계**: 모든 비즈니스 로직은 UseCase를 통해 처리 (`DamageTrackingUseCase`, `KillTrackingUseCase`, `SessionManagementUseCase` 등)
- **Repository 패턴**: 데이터 접근을 인터페이스로 추상화하여 구현체 교체 가능

#### 2. Repository 패턴
**사용 이유**:
- 데이터 접근 로직을 비즈니스 로직과 분리
- 데이터 소스 변경 시 영향 범위 최소화 (메모리 → 파일 → 네트워크 등)
- 테스트 시 Mock Repository 사용 가능

**구현 방식**:
- `ISessionDataRepository` (Domain 계층): 데이터 접근 인터페이스 정의
- `SessionDataRepository` (Data 계층): 실제 구현체
- UseCase는 인터페이스만 의존하여 Data 계층과 분리

#### 3. UseCase 패턴
**사용 이유**:
- 비즈니스 로직을 명시적이고 재사용 가능한 단위로 캡슐화
- Presentation 계층과 Data 계층의 완전한 분리
- 각 UseCase가 단일 책임을 가지도록 설계

**구현 방식**:
- `DamageTrackingUseCase`: 플레이어 데미지 추적
- `KillTrackingUseCase`: 적 처치 수 추적
- `SurvivalTimeTrackingUseCase`: 생존 시간 추적
- `SessionManagementUseCase`: 세션 데이터 관리

#### 4. 이벤트 버스 패턴 (Event Bus)
**사용 이유**:
- 컴포넌트 간 느슨한 결합 (Loose Coupling)
- PawnManager 내부의 SubManager들이 서로를 직접 참조하지 않고 통신
- 런타임에 동적으로 이벤트 구독/해지 가능

**구현 방식**:
- `PawnManager`에 제네릭 이벤트 버스 내장
- `Subscribe<T>()`, `Publish<T>()`, `Unsubscribe<T>()` 메서드 제공
- 이벤트 우선순위 시스템으로 실행 순서 제어

**사용 예시**:
```csharp
// 이벤트 구독
_pawnManager.Subscribe<DamageEvent>(HandleDamage, EventPriority.Normal);

// 이벤트 발행
_pawnManager.Publish(new DamageEvent(target, damage, attacker));
```

#### 5. 전략 패턴 (Strategy Pattern)
**사용 이유**:
- 이동 방식을 런타임에 동적으로 변경 가능
- 새로운 이동 방식 추가 시 기존 코드 수정 불필요
- 각 이동 전략을 독립적으로 테스트 가능

**구현 방식**:
- `IMovementStrategy` 인터페이스
- 구현체: `KeyboardMovementStrategy`, `DirectionalMovementStrategy`, `TargetMovementStrategy`, `HomingMovementStrategy`
- `ChangeMovementStrategyEvent`로 런타임 전략 변경

#### 6. 서브매니저 패턴 (SubManager Pattern)
**사용 이유**:
- Pawn의 기능을 모듈화하여 관심사 분리
- 필요한 기능만 선택적으로 추가 가능
- 각 SubManager가 독립적으로 동작하여 유지보수 용이

**구현 방식**:
- `PawnSubManager` 추상 클래스 상속
- `SubStart()`, `SubUpdate()` 생명주기 메서드
- JSON 레시피를 통해 동적으로 구성

#### 7. JSON 기반 데이터 주도 설계 (Data-Driven Design)
**사용 이유**:
- 코드 수정 없이 게임 데이터 변경 가능
- 디자이너가 프로그래머 없이 콘텐츠 수정 가능
- 프리팹 없이 순수 코드로 Pawn 생성

**구현 방식**:
- `StreamingAssets/Recipes/` 폴더에 JSON 레시피 파일
- `PawnRecipeData` 클래스로 역직렬화
- `CreationManager`가 JSON을 읽어 런타임에 GameObject 생성

#### 8. Singleton 패턴
**사용 이유**:
- Unity 게임 개발에서 표준적인 패턴
- MonoBehaviour 기반 아키텍처와 자연스럽게 통합
- 전역 접근이 필요한 매니저들에 실용적

**구현 방식**:
- `GameManager.Instance`, `UIManager.Instance` 등
- `DontDestroyOnLoad`로 씬 전환 시 유지
- Presentation 계층에서만 사용 (Domain 계층에는 영향 없음)

---

## 🏗️ 아키텍처 구조

```
PawnCore/
├── Domain/           # 도메인 계층 (비즈니스 로직)
│   ├── Events/       # 이벤트 정의
│   ├── Repositories/ # Repository 인터페이스
│   └── Usecases/     # 유스케이스 (순수 로직)
├── Presentation/     # 프레젠테이션 계층 (Unity 연동)
│   └── SubManagers/  # 기능별 서브매니저
└── Recipes/          # 레시피 시스템 (JSON 기반)

Data/
├── Repositories/     # Repository 구현체
├── Models/           # 데이터 모델 (GameSessionData)
└── Storage/          # 저장 시스템 (향후 확장용)
```

### 아키텍처 원칙

1. **데이터 접근 규칙**
   - `GameSessionData`는 Data 계층 내부에만 존재
   - 외부는 `ISessionDataRepository` 인터페이스를 통해서만 접근
   - 모든 데이터 접근은 UseCase를 통해 수행

2. **의존성 방향**
   ```
   Presentation → Domain (UseCase) → Domain (Repository 인터페이스) → Data (Repository 구현체) → Data (모델)
   ```

3. **UseCase 중심 설계**
   - Presentation 계층은 Repository를 직접 접근하지 않음
   - 모든 비즈니스 로직은 UseCase를 통해 처리
   - UseCase는 Repository 인터페이스만 의존

### 의도적인 아키텍처 예외사항

Unity 게임 개발의 실용성을 위해 다음 예외를 두었습니다:

#### 1. Singleton 패턴 사용
**위치**: `GameManager.Instance`, `UIManager.Instance` 등

**이유**:
- Unity 게임 개발에서 Singleton은 표준적인 패턴
- MonoBehaviour 기반 아키텍처와 자연스럽게 통합
- 전역 접근이 필요한 매니저들에 실용적
- 프로젝트 전반에 걸쳐 일관된 접근 방식 제공

**영향**:
- Presentation 계층에서만 사용 (Domain 계층에는 영향 없음)
- 테스트는 통합 테스트 중심으로 진행 (Unity 특성상 적합)

#### 2. Domain 계층의 일부 Unity 의존성
**위치**: 
- `SurvivalTimeTrackingUseCase` → `LifecycleManager` 의존
- `MovementUsecases` → `GameManager.Instance.LifecycleManager` 참조
- `CombatUsecases` → `GameManager.Instance.CreationManager` 참조

**이유**:
- Unity 전용 프로젝트이므로 플랫폼 이식성보다 실용성 우선
- 완전한 분리를 위해 인터페이스 도입 시 오버엔지니어링
- 현재 구조가 잘 작동하고 있으며 복잡도 증가 대비 이점이 적음
- Unity 게임에서는 통합 테스트가 단위 테스트보다 실용적

**향후 개선 계획**:
- 단위 테스트가 필요해지면 `ITimeProvider`, `IPawnFactory` 인터페이스 도입
- 다른 플랫폼 이식이 필요해지면 의존성 주입 패턴 적용
- 현재는 문제가 발생하지 않으므로 YAGNI 원칙 적용

#### 3. 이벤트 버스가 Presentation 계층 내부에 위치
**위치**: `PawnManager`의 이벤트 버스 시스템

**이유**:
- 이벤트 버스는 Unity GameObject 생명주기와 밀접하게 연관
- Presentation 계층 컴포넌트 간 통신에 최적화
- UseCase는 이벤트를 구독하지 않고 Presentation 계층에서 직접 호출
- 이는 의도된 설계로, 이벤트는 Presentation 계층의 관심사

**설계 철학**:
- 이벤트 버스: Presentation 계층 내부의 느슨한 결합
- UseCase: Domain 계층의 명시적 비즈니스 로직
- 두 시스템이 분리되어 있어 각각의 목적에 집중 가능

### 아키텍처 결정 기록 (ADR)

| 결정 | 이유 | 대안 | 선택 이유 |
|------|------|------|----------|
| Singleton 패턴 | Unity 표준 패턴 | 의존성 주입 | 실용성, Unity 생태계 적합성 |
| Domain의 Unity 의존 | 실용성 우선 | 완전한 분리 | 오버엔지니어링 방지, YAGNI |
| 이벤트 버스 위치 | GameObject 생명주기 연동 | Domain 이벤트 | Unity 특성에 맞는 설계 |
| UseCase 직접 호출 | 명시적 비즈니스 로직 | 이벤트 구독 | 코드 가독성, 디버깅 용이성 |

---

## 🎮 주요 기능

### Pawn 시스템
- **PawnManager**: 각 Pawn의 중앙 관제탑 역할, 생명주기 관리 및 제네릭 이벤트 버스 제공
- **PawnData**: Pawn의 모든 상태 데이터를 담는 도메인 엔티티 (체력, 시각, 물리, 이동 등)
- **SubManagers**: 이동, 전투, 입력, 시각, 물리 등 기능별 모듈화된 컴포넌트
- **제어된 파괴 시스템**: `LifecycleManager`를 통한 안전한 오브젝트 파괴

### 전투 시스템
- **충돌 기반 데미지 처리**: `CollisionDamageSubManager`를 통한 충돌 시 데미지 적용
- **발사체 공격 시스템**: `ProjectileShooterSubManager`를 통한 자동 발사
- **체력 관리 시스템**: `DamageableSubManager`를 통한 체력 관리

### 이동 시스템
- **키보드 입력 이동** (WASD): `KeyboardMovementStrategy`
- **방향성 이동** (투사체용): `DirectionalMovementStrategy`
- **타겟 추적 이동**: `TargetMovementStrategy`
- **호밍 이동**: `HomingMovementStrategy` - 가장 가까운 타겟 자동 추적

### 스테이지 시스템
- **JSON 기반 스테이지 데이터**: `StageData` 클래스로 스테이지 설정 정의
- **적 스폰 관리**: `StageManager`를 통한 주기적 적 스폰
- **스테이지 진행 관리**: `StageLoader`를 통한 JSON 스테이지 파일 로드

---

## 📁 프로젝트 구조

```
re/PawnSurvivors/
├── Assets/
│   ├── script/
│   │   ├── Managers/           # 전역 매니저
│   │   ├── PawnCore/
│   │   │   ├── Domain/         # 도메인 계층
│   │   │   ├── Presentation/   # 프레젠테이션 계층
│   │   │   └── Recipes/        # 레시피 시스템
│   │   ├── Data/               # 데이터 계층
│   │   │   ├── Repositories/   # Repository 구현체
│   │   │   └── Models/         # 데이터 모델
│   │   ├── UI/                 # UI 시스템
│   │   └── Player/             # 플레이어 컨트롤러
│   └── StreamingAssets/
│       ├── Recipes/            # Pawn 레시피 JSON 파일
│       └── Stages/             # 스테이지 JSON 파일
└── README.md
```

---

## 🚀 시작하기

### 요구 사항
- Unity Editor 6000.0.56f1 이상
- Unity Input System 패키지
- Newtonsoft.Json (JSON 직렬화)

### 실행 방법
1. Unity Hub에서 프로젝트 열기
2. `Assets/Scenes/` 폴더의 씬 열기
3. 씬에 `GameManager`가 있는 GameObject가 있는지 확인
4. 플레이 모드 실행

---

## 📚 상세 문서

프로젝트 내에는 여러 위치에 상세한 README 문서들이 있습니다:

- **전역 매니저**: `Assets/script/Managers/README.md`
- **PawnCore 아키텍처**: `Assets/script/PawnCore/README.md`
- **레시피 시스템**: `Assets/script/PawnCore/Recipes/README.md`
- **이동 시스템**: `Assets/script/PawnCore/Presentation/SubManagers/Movement/README.md`
- **이벤트 시스템**: `Assets/script/PawnCore/Domain/Events/README.md`
- **유스케이스**: `Assets/script/PawnCore/Domain/Usecases/README.md`

---

## 📝 주요 설계 패턴

- **클린 아키텍처**: Domain, Presentation, Data 계층 분리
- **Repository 패턴**: 데이터 접근 추상화
- **UseCase 패턴**: 비즈니스 로직 캡슐화
- **전략 패턴**: 이동 시스템의 유연한 확장
- **이벤트 버스 패턴**: 컴포넌트 간 느슨한 결합
- **서브매니저 패턴**: 기능별 모듈화
- **Singleton 패턴**: Unity 매니저 접근
- **팩토리 패턴**: JSON 기반 Pawn 생성

---

## 📄 라이선스

이 프로젝트의 라이선스 정보는 별도로 명시되지 않았습니다.

---

**참고**: 더 자세한 정보는 각 폴더의 README.md 파일을 참고하세요.

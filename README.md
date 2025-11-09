# PawnSurvivors

Unity 기반의 생존 게임 프로젝트입니다. 클린 아키텍처 원칙을 따라 설계되었으며, JSON 레시피 시스템을 통해 데이터 기반으로 게임 오브젝트를 생성하는 구조를 가지고 있습니다.

## 📋 프로젝트 개요

PawnSurvivors는 캐릭터(Pawn) 기반의 생존 게임으로, 다음과 같은 핵심 특징을 가지고 있습니다:

- **JSON 레시피 시스템**: 프리팹 없이 JSON 파일만으로 Pawn을 생성
- **클린 아키텍처**: 도메인 로직과 프레젠테이션 계층의 명확한 분리
- **이벤트 기반 아키텍처**: 느슨한 결합을 통한 확장 가능한 시스템
- **전략 패턴**: 이동 시스템에 전략 패턴 적용으로 유연한 확장성
- **스테이지 시스템**: JSON 기반 스테이지 데이터 관리

## 🎮 주요 기능

### Pawn 시스템
- **PawnManager**: 각 Pawn의 중앙 관제탑 역할, 생명주기 관리 및 제네릭 이벤트 버스 제공
- **PawnData**: Pawn의 모든 상태 데이터를 담는 도메인 엔티티 (체력, 시각, 물리, 이동 등)
- **SubManagers**: 이동, 전투, 입력, 시각, 물리 등 기능별 모듈화된 컴포넌트
- **제어된 파괴 시스템**: `LifecycleManager`를 통한 안전한 오브젝트 파괴

### 시각 시스템
- **VisualSubManager**: Pawn의 시각적 표현을 담당
  - "Visuals" 자식 GameObject를 생성하여 물리 Transform과 시각 Transform 분리
  - JSON 레시피를 통해 스프라이트와 색상 설정
  - 향후 애니메이션 구현 시 유연한 구조 제공

### 물리 시스템
- **PhysicsSubManager**: Pawn의 물리적 특성을 동적으로 설정
  - Collider2D 타입 (Box, Circle, Capsule) 동적 추가
  - Rigidbody2D 타입 (Dynamic, Kinematic, Static) 설정
  - Layer와 Tag를 JSON 레시피로 관리
  - JSON 기반 설정으로 프리팹 없이 물리 속성 구성

### 전투 시스템
- **충돌 기반 데미지 처리**: `CollisionDamageSubManager`를 통한 충돌 시 데미지 적용
- **발사체 공격 시스템**: `ProjectileShooterSubManager`를 통한 자동 발사
  - 가장 가까운 적을 자동으로 찾아 발사
  - 발사 방향 자동 계산
- **체력 관리 시스템**: `DamageableSubManager`를 통한 체력 관리
- **사망 로직**: 체력이 0 이하가 되면 `LifecycleManager`를 통해 Pawn 자동 제거

### 이동 시스템
- **키보드 입력 이동** (WASD): `KeyboardMovementStrategy`
- **방향성 이동** (투사체용): `DirectionalMovementStrategy`
- **타겟 추적 이동**: `TargetMovementStrategy`
- **호밍 이동**: `HomingMovementStrategy` - 가장 가까운 타겟 자동 추적
- **런타임 전략 변경**: `ChangeMovementStrategyEvent`를 통한 동적 전략 전환

### 스테이지 시스템
- **JSON 기반 스테이지 데이터**: `StageData` 클래스로 스테이지 설정 정의
- **적 스폰 관리**: `StageManager`를 통한 주기적 적 스폰
  - 원형 스폰 위치 계산 (카메라 뷰포트 바깥쪽)
  - 스폰 간격, 반지름, 카메라로부터의 거리 설정 가능
- **스테이지 진행 관리**: `StageLoader`를 통한 JSON 스테이지 파일 로드

## 🏗️ 아키텍처

이 프로젝트는 **클린 아키텍처** 원칙을 따라 설계되었습니다:

```
PawnCore/
├── Domain/           # 도메인 계층 (비즈니스 로직)
│   ├── Events/       # 이벤트 정의
│   └── Usecases/     # 유스케이스 (순수 로직)
├── Presentation/     # 프레젠테이션 계층 (Unity 연동)
│   └── SubManagers/  # 기능별 서브매니저
└── Recipes/          # 레시피 시스템 (JSON 기반)
```

### 핵심 구성 요소

1. **PawnManager**: Pawn의 생명주기 관리 및 제네릭 이벤트 버스 제공
   - `InitializeSubManagers()`: PawnData 초기화 후 SubManager 초기화 보장
   - `DestroyPawn()`: LifecycleManager를 통한 제어된 파괴 요청
   - `Subscribe<T>/Publish<T>/Unsubscribe<T>`: 제네릭 이벤트 버스 시스템

2. **PawnData**: Pawn의 모든 상태 데이터 (도메인 엔티티)
   - 체력, 시각 정보 (스프라이트, 색상), 물리 데이터, 이동 데이터 등

3. **SubManagers**: 기능별 독립적인 컴포넌트
   - **Combat**: DamageableSubManager, CollisionDamageSubManager, ProjectileShooterSubManager
   - **Movement**: MovableSubManager + MovementStrategy 구현체들
   - **Visual**: VisualSubManager (자식 GameObject 구조)
   - **Physics**: PhysicsSubManager (동적 컴포넌트 추가)
   - **Input**: PlayerAttackInputSubManager

4. **이벤트 시스템**: 컴포넌트 간 통신을 위한 제네릭 이벤트 버스
   - `AttackInputEvent`: 공격 입력 이벤트
   - `ChangeMovementStrategyEvent`: 이동 전략 변경 이벤트

5. **레시피 시스템**: JSON 기반 Pawn 생성 시스템
   - `PawnRecipeData`: Pawn 타입 정의
   - `SubManagerSetupData`: 각 SubManager 설정
   - `MovementStrategySetupData`: 이동 전략 설정

6. **스테이지 시스템**: JSON 기반 스테이지 관리
   - `StageData`: 스테이지 설정 (스폰 간격, 반지름 등)
   - `StageManager`: 적 스폰 및 스테이지 진행 관리
   - `StageLoader`: JSON 스테이지 파일 로드

## 📁 프로젝트 구조

```
re/PawnSurvivors/
├── Assets/
│   ├── script/
│   │   ├── Managers/           # 전역 매니저
│   │   │   ├── GameManager.cs
│   │   │   ├── LifecycleManager.cs
│   │   │   ├── CreationManager.cs
│   │   │   ├── StageManager.cs
│   │   │   └── README.md       # 전역 매니저 설명
│   │   └── PawnCore/
│   │       ├── Domain/         # 도메인 계층
│   │       │   ├── Events/
│   │       │   │   └── README.md  # 이벤트 시스템 설명
│   │       │   └── Usecases/
│   │       │       └── README.md  # 유스케이스 설명
│   │       ├── Presentation/   # 프레젠테이션 계층
│   │       │   └── SubManagers/
│   │       │       ├── README.md  # 서브매니저 개요
│   │       │       ├── Movement/
│   │       │       │   ├── README.md  # 이동 시스템 설명
│   │       │       │   └── Strategies/
│   │       │       │       └── README.md  # 이동 전략 패턴 설명
│   │       │       ├── Combat/
│   │       │       ├── Input/
│   │       │       ├── Visual/
│   │       │       └── Physics/
│   │       ├── Recipes/
│   │       │   └── README.md   # 레시피 시스템 설명
│   │       └── README.md       # PawnCore 아키텍처 설명
│   └── StreamingAssets/
│       ├── Recipes/            # Pawn 레시피 JSON 파일
│       │   ├── Player.json
│       │   ├── Enemy.json
│       │   └── Bullet.json
│       └── Stages/             # 스테이지 JSON 파일
│           └── Stage1.json
└── README.md                   # 이 파일
```

## 📚 상세 문서

프로젝트 내에는 여러 위치에 상세한 README 문서들이 있습니다:

### 전역 매니저
- **위치**: `re/PawnSurvivors/Assets/script/Managers/README.md`
- **내용**: GameManager, LifecycleManager, CreationManager 등 전역 매니저들의 역할과 사용법

### PawnCore 아키텍처
- **위치**: `re/PawnSurvivors/Assets/script/PawnCore/README.md`
- **내용**: PawnCore의 전체 아키텍처와 클린 아키텍처 관점에서의 설명

### 레시피 시스템
- **위치**: `re/PawnSurvivors/Assets/script/PawnCore/Recipes/README.md`
- **내용**: JSON 기반 Pawn 레시피 시스템의 사용법과 구조

### 이동 시스템
- **위치**: `re/PawnSurvivors/Assets/script/PawnCore/Presentation/SubManagers/Movement/README.md`
- **내용**: 이동 서브시스템의 구조와 동작 방식

### 이동 전략 패턴
- **위치**: `re/PawnSurvivors/Assets/script/PawnCore/Presentation/SubManagers/Movement/Strategies/README.md`
- **내용**: 전략 패턴을 사용한 이동 시스템의 구현 방법

### 서브매니저 개요
- **위치**: `re/PawnSurvivors/Assets/script/PawnCore/Presentation/SubManagers/README.md`
- **내용**: 서브매니저들의 역할과 클린 아키텍처 관점에서의 위치

### 이벤트 시스템
- **위치**: `re/PawnSurvivors/Assets/script/PawnCore/Domain/Events/README.md`
- **내용**: 제네릭 이벤트 버스 시스템의 사용법

### 유스케이스
- **위치**: `re/PawnSurvivors/Assets/script/PawnCore/Domain/Usecases/README.md`
- **내용**: 순수 비즈니스 로직을 담는 유스케이스 클래스들의 역할

### 진행 상황 문서
- **위치**: `re/PawnSurvivors/README.md`
- **내용**: 프로젝트 진행 상황 및 변경 이력 (최신 변경사항 포함)

### 스테이지 시스템
- **위치**: `re/PawnSurvivors/Assets/script/Managers/StageManager.cs`, `StageData.cs`, `StageLoader.cs`
- **내용**: JSON 기반 스테이지 데이터 관리, 적 스폰 시스템, 원형 스폰 위치 계산

## 🚀 시작하기

### 요구 사항
- Unity Editor 6000.0.56f1 이상
- Unity Input System 패키지

### 실행 방법
1. Unity Hub에서 프로젝트 열기
2. `Assets/Scenes/` 폴더의 씬 열기
3. 씬에 `GameManager`가 있는 GameObject가 있는지 확인
4. 플레이 모드 실행

### Pawn 생성
Pawn은 JSON 레시피를 통해 생성됩니다:

```csharp
// CreationManager를 통해 Pawn 생성
PawnRecipeData recipe = CreationManager.GetRecipe("Player");
CreationManager.CreatePawn(recipe, position, rotation);

// 방향이 있는 발사체 생성 (옵션)
Vector3 direction = (target.position - spawnPosition).normalized;
CreationManager.CreatePawn(bulletRecipe, spawnPosition, rotation, direction);
```

### 레시피 작성
`Assets/StreamingAssets/Recipes/` 폴더에 JSON 파일을 생성하여 새로운 Pawn 타입을 정의할 수 있습니다.

예시는 `Player.json`, `Enemy.json`, `Bullet.json` 파일을 참고하세요.

### 스테이지 설정
`Assets/StreamingAssets/Stages/` 폴더에 JSON 파일을 생성하여 스테이지 설정을 정의할 수 있습니다.

```json
{
  "stageName": "Stage1",
  "spawnInterval": 2.5,
  "spawnRadius": 10.0,
  "spawnDistanceFromCamera": 2.0
}
```

## 🛠️ 기술 스택

- **Unity**: 6000.0.56f1
- **C#**: .NET Framework
- **Unity Input System**: 입력 처리
- **Newtonsoft.Json**: JSON 직렬화/역직렬화 (레시피 및 스테이지 데이터)
- **Unity 2D Physics**: 2D 충돌 및 물리 시스템

## 📝 주요 설계 패턴

- **전략 패턴**: 이동 시스템 - 다양한 이동 방식을 전략으로 캡슐화
- **팩토리 패턴**: CreationManager를 통한 Pawn 생성
- **이벤트 버스 패턴**: 컴포넌트 간 느슨한 결합을 위한 통신
- **서브매니저 패턴**: 기능별 모듈화 및 관심사 분리
- **클린 아키텍처**: Domain, Presentation 계층 분리
- **제어된 파괴 패턴**: LifecycleManager를 통한 안전한 오브젝트 파괴

## 🔄 개발 흐름

1. **레시피 작성**: JSON 파일로 Pawn 타입 정의
   - `Assets/StreamingAssets/Recipes/`에 JSON 파일 생성
   - `SubManagerSetupData`로 필요한 SubManager 설정
   - `MovementStrategySetupData`로 이동 전략 설정

2. **스테이지 설정**: JSON 파일로 스테이지 정의
   - `Assets/StreamingAssets/Stages/`에 JSON 파일 생성
   - 스폰 간격, 반지름, 카메라 거리 등 설정

3. **SubManager 구현**: 필요시 새로운 기능 모듈 추가
   - `PawnSubManager`를 상속받아 구현
   - `SubStart()`, `SubUpdate()` 메서드 구현

4. **이벤트 정의**: 컴포넌트 간 통신이 필요한 경우 이벤트 추가
   - `Domain/Events/` 폴더에 이벤트 클래스 생성
   - `PawnManager.Publish<T>()`로 이벤트 발행
   - `PawnManager.Subscribe<T>()`로 이벤트 구독

5. **Usecase 구현**: 순수 비즈니스 로직 추가
   - `Domain/Usecases/` 폴더에 정적 클래스로 구현
   - `PawnData` 또는 `PawnManager`를 받아 로직 처리

## 📄 라이선스

이 프로젝트의 라이선스 정보는 별도로 명시되지 않았습니다.

## 🤝 기여

프로젝트 개선을 위한 제안이나 버그 리포트는 이슈로 등록해주세요.

---

**참고**: 더 자세한 정보는 각 폴더의 README.md 파일을 참고하세요.


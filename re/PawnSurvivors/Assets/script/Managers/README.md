# 전역 매니저 (Global Managers)

이 폴더에는 게임의 전반적인 생명주기, 상태, 객체 생성 등 핵심적인 '글로벌' 기능을 관리하는 매니저 클래스들이 위치합니다.

이 매니저들은 일반적으로 씬(Scene)에 단 하나만 존재하는 싱글톤(Singleton) 형태로 구현되며, 다른 모든 객체들이 필요로 하는 공통 기능에 대한 핵심 접근점(예: `GameManager.Instance`)을 제공합니다.

---

## 🔗 매니저 구조

```
GameManager (최상위)
├── LifecycleManager (생명주기 관리)
├── CreationManager (Pawn 생성)
├── StageManager (스테이지 관리)
└── GameStateManager (게임 상태 관리)
```

---

## 📊 각 매니저의 역할과 수정 방법

### GameManager
**역할:**
- 게임의 주 진입점이자 다른 매니저들을 총괄하는 최상위 매니저
- UseCase 인스턴스 관리 (Domain 계층 접근점)
- PlayerController 생성 및 관리
- 플레이어 Pawn 추가/관리

**주요 메서드:**
- `StartStage(stageName, resetSession)`: 스테이지 시작 (StageManager에 위임)
- `EndStage()`: 스테이지 종료 (StageManager에 위임)
- `CreatePlayerController()`: PlayerController 생성 및 초기화
- `AddPlayerPawn(recipeName)`: 플레이어 Pawn 추가

**수정이 필요할 때:**
- **스테이지 시작/종료 로직**: `StageManager`를 수정하세요 (GameManager는 위임만 함)
- **PlayerController 생성 로직**: `CreatePlayerController()` 메서드 수정
- **플레이어 Pawn 생성 로직**: `AddPlayerPawn()` 메서드 수정
- **UseCase 추가**: `Awake()`에서 UseCase 인스턴스 생성 및 초기화

---

### LifecycleManager
**역할:**
- 게임 루프 및 생명주기 관리
- 모든 Pawn의 `ManagedUpdate()` 호출
- StageManager의 `UpdateStage()` 호출
- 일회성 액션 큐 처리 (EarlyUpdate, LateUpdate)
- 오브젝트 파괴 큐 처리 (제어된 파괴)

**주요 메서드:**
- `ManagedUpdate()`: 모든 Pawn의 업데이트 호출
- `RequestDestruction(GameObject)`: 안전한 오브젝트 파괴 요청
- `EnqueueEarlyUpdate(Action)`: Update 시작 시 실행할 액션
- `EnqueueLateUpdate(Action)`: Update 끝에 실행할 액션
- `TogglePause()`: 일시정지 토글

**수정이 필요할 때:**
- **업데이트 순서 변경**: `Update()` 메서드 내부 순서 조정
- **새로운 시스템 업데이트 추가**: `Update()` 메서드에 호출 추가
- **일시정지 로직 변경**: `TogglePause()`, `IsPaused` 관련 로직 수정
- **게임 시간 관리**: `GameTime`, `GameDeltaTime` 관련 로직 수정

---

### CreationManager
**역할:**
- "Pawn 설계도" 시스템의 핵심 팩토리
- JSON 기반 레시피 파일 로드
- Pawn GameObject 생성 및 SubManager 등록

**주요 메서드:**
- `GetRecipe(recipeName)`: 레시피 데이터 가져오기
- `CreatePawn(recipe, position, rotation)`: Pawn 생성
- `DestroyAllPawns()`: 모든 Pawn 파괴

**수정이 필요할 때:**
- **Pawn 생성 로직 변경**: `CreatePawn()` 메서드 수정
- **레시피 로드 방식 변경**: `Awake()` 또는 레시피 로더 관련 로직 수정
- **SubManager 초기화 순서 변경**: `CreatePawn()` 내부의 `InitializeSubManagers()` 호출 부분 수정

---

### StageManager ⭐ **스테이지 관리의 핵심**
**역할:**
- 스테이지 전체 라이프사이클 관리
- 스테이지 시작/종료의 모든 로직 담당
- 웨이브 기반 적 스폰 시스템
- 스테이지 데이터 기반 동작

**주요 메서드:**
- `StartStage(stageName, resetSession)`: 스테이지 시작 (전체 라이프사이클 담당)
- `EndStage()`: 스테이지 종료
- `UpdateStage()`: 스테이지 업데이트 (LifecycleManager에서 호출)

**의존성:**
- `CreationManager`: Pawn 생성
- `StageDataSource`: 스테이지 데이터 로드
- `StageManagementUseCase`: 스테이지 비즈니스 로직 (세션 관리)

**수정이 필요할 때:**
- **스테이지 시작 로직 수정**: `StartStage()` 메서드만 수정하면 됩니다
  - UseCase 호출 (세션 준비)
  - PlayerController 생성
  - 플레이어 생성
  - 스테이지 로드 및 초기화
  - 모든 로직이 여기에 집중되어 있습니다

- **스테이지 종료 로직 수정**: `EndStage()` 메서드만 수정하면 됩니다
  - 웨이브 타이머 초기화
  - 스테이지 데이터 정리

- **스테이지 업데이트 로직 수정**: `UpdateStage()` 메서드만 수정하면 됩니다
  - 웨이브 스폰 로직
  - 시간 관리

- **웨이브 스폰 로직 수정**: `ProcessWave()`, `SpawnEnemyFromWave()` 메서드 수정
- **스폰 위치 계산 수정**: `GetCircularSpawnPosition()` 메서드 수정

---

### GameStateManager
**역할:**
- 게임 상태 전환 관리 (상태 머신)
- 상태별 UI 표시 제어
- 상태 전환 시 적절한 매니저 호출

**주요 상태:**
- `Title`: 타이틀 화면
- `MainMenu`: 메인 메뉴
- `Stage`: 스테이지 플레이 중
- `Paused`: 일시정지
- `Shop`: 상점
- `GameOver`: 게임 오버
- `StageClear`: 스테이지 클리어

**주요 메서드:**
- `ChangeState(GameState)`: 상태 변경
- `StartStage(stageName)`: 스테이지 시작 (편의 메서드)
- `PauseGame()` / `ResumeGame()`: 일시정지 제어
- `GoToShop()`: 상점으로 이동
- `GoToMainMenu()`: 메인 메뉴로 이동

**수정이 필요할 때:**
- **새로운 상태 추가**: `GameState` enum에 추가, `IsValidTransition()`에 전환 규칙 추가, `OnStateEnter()`에 상태별 동작 추가
- **상태 전환 규칙 변경**: `IsValidTransition()` 메서드 수정
- **상태별 동작 변경**: `OnStateEnter()`, `OnStateExit()` 메서드 수정
- **ESC 키 동작 변경**: `HandleEscapeKey()` 메서드 수정

---

## 🎯 스테이지 관리 구조

### 책임 분리

```
GameStateManager (상태 전환)
    ↓
GameManager (오케스트레이션)
    ↓
StageManager (스테이지 라이프사이클 전담) ⭐
    ├── StageManagementUseCase (비즈니스 로직)
    ├── StageDataSource (데이터 로드)
    └── CreationManager (Pawn 생성)
```

### 스테이지 시작 흐름

```
GameStateManager.ChangeState(GameState.Stage)
    ↓
GameManager.StartStage(stageName, resetSession)
    ↓
StageManager.StartStage(stageName, resetSession)
    ├── StageManagementUseCase.PrepareStageStart()  // 세션 준비
    ├── GameManager.CreatePlayerController()        // PlayerController 생성
    ├── GameManager.AddPlayerPawn()                 // 플레이어 생성
    ├── StageDataSource.GetStage()                  // 스테이지 데이터 로드
    ├── InitializeStageData()                       // 스테이지 데이터 초기화
    └── StartStageInternal()                        // 내부 시작 로직
```

### 스테이지 종료 흐름

```
GameStateManager.ChangeState(GameState.Shop)
    ↓
GameManager.EndStage()
    ├── StageManagementUseCase.PrepareStageEnd()    // 세션 정리
    └── StageManager.EndStage()                     // 스테이지 종료
```

### 📝 수정 가이드

**스테이지 관련 로직을 수정할 때:**

| 수정할 내용 | 수정할 파일 | 수정할 메서드 |
|------------|------------|-------------|
| 스테이지 시작 로직 | `StageManager.cs` | `StartStage()` |
| 스테이지 종료 로직 | `StageManager.cs` | `EndStage()` |
| 스테이지 업데이트 로직 | `StageManager.cs` | `UpdateStage()` |
| 웨이브 스폰 로직 | `StageManager.cs` | `ProcessWave()`, `SpawnEnemyFromWave()` |
| 스폰 위치 계산 | `StageManager.cs` | `GetCircularSpawnPosition()` |
| 세션 관리 로직 | `StageManagementUseCase.cs` | `PrepareStageStart()`, `PrepareStageEnd()` |
| 스테이지 데이터 구조 | `StageData.cs` | 클래스 정의 |
| 스테이지 JSON 파일 | `StreamingAssets/Stages/*.json` | JSON 구조 |

**핵심 원칙:**
- **스테이지 시작/종료/업데이트 로직은 모두 `StageManager`에 집중**
- **GameManager는 단순히 StageManager를 호출하는 오케스트레이터**
- **비즈니스 로직은 UseCase에서 처리**

---

## 🔧 매니저 간 의존성

**의존성 규칙:**
- GameManager는 모든 매니저를 알고 있음
- GameStateManager는 GameManager를 통해 다른 매니저 접근
- LifecycleManager, CreationManager, StageManager는 UI 모름
- **단방향 의존성 유지**

**의존성 주입:**
- `StageManager`는 `GameManager.Awake()`에서 의존성 주입됨:
  ```csharp
  StageManager.Initialize(CreationManager, StageDataSource, StageManagementUseCase);
  ```

---

## ⚠️ 주의사항

### 게임 시작 순서
1. `GameManager.Awake()` - 모든 매니저 참조 획득, UseCase 초기화, StageManager 의존성 주입
2. `GameStateManager.Awake()` - 싱글톤 초기화
3. `GameStateManager.Start()` - 초기 상태 설정 (Title)
4. 각 매니저의 `Start()` - 필요 시 추가 초기화

### 메모리 관리
- 화면 전환 시 이전 화면은 `Hide()`만 호출 (파괴 안함)
- 메인 메뉴 복귀 시 모든 Pawn 파괴 (`CreationManager.DestroyAllPawns()`)

### 스테이지 시작 시 주의
- `GameManager.StartStage()`는 `StageManager.StartStage()`만 호출
- 실제 로직은 모두 `StageManager`에 있으므로, 스테이지 관련 수정은 `StageManager`에서만 하면 됨

---

## 📝 빠른 참조

### 스테이지 시작
```csharp
// GameManager를 통해 (권장)
GameManager.Instance.StartStage("Stage1", resetSession: true);

// GameStateManager를 통해 (상태 전환 포함)
GameStateManager.Instance.StartStage("Stage1");
```

### 스테이지 종료
```csharp
// GameManager를 통해 (권장)
GameManager.Instance.EndStage();
```

### 플레이어 Pawn 추가
```csharp
GameManager.Instance.AddPlayerPawn("Player");
GameManager.Instance.AddPlayerPawn("PlayerButter");
```

### 오브젝트 안전하게 파괴
```csharp
GameManager.Instance.LifecycleManager.RequestDestruction(gameObject);
```

---

## 🔍 상세 문서

스테이지 JSON 파일 구조, 웨이브 시스템 등 기술적인 세부사항은 **루트 README.md**를 참고하세요.

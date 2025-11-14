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
└── UIManager (UI 관리)
```

---

## 📊 주요 클래스

### GameManager
**역할:** 게임의 주 진입점이자 다른 매니저들을 총괄하는 최상위 매니저
- 싱글톤 인스턴스 제공
- 다른 매니저들의 참조 보유 (`GetComponent`로 획득)
- 게임 초기화 및 시작 제어

**주요 메서드:**
- `StartGameDirectly()`: UI 없이 게임 직접 시작 (테스트용)
- `LoadStage(stageName)`: 스테이지 데이터 로드

### LifecycleManager
**역할:** 게임 루프 및 생명주기 관리
- 모든 Pawn의 `ManagedUpdate()` 호출
- StageManager의 `UpdateStage()` 호출
- 일회성 액션 큐 처리
- 오브젝트 파괴 큐 처리 (제어된 파괴)
- `PawnSubManager`의 `SubStart()` 호출은 `PawnManager`가 직접 제어

**특징:**
- `RequestDestruction()`을 통해 업데이트 루프 중 발생하는 충돌 방지

### CreationManager
**역할:** "Pawn 설계도" 시스템의 핵심 팩토리
- `Assets/StreamingAssets/Recipes/`에서 JSON 기반 레시피 파일 로드
- `PawnRecipeData`를 읽어 프로그래밍 방식으로 Pawn GameObject 생성
- SubManager 등록 및 초기화
- `PawnManager.InitializeSubManagers()` 호출하여 초기화 시점 보장

### StageManager
**역할:** 스테이지 진행 관리
- 웨이브 기반 적 스폰 시스템
- 스테이지 데이터 기반 동작
- LifecycleManager에 의해 매 프레임 업데이트

**주요 기능:**
- 시간대별 웨이브 관리
- 가중치 기반 적 종류 선택
- 보스/특수 적 일회성 소환 지원

### UIManager
**역할:** UI 및 게임 상태 관리
- 게임 상태(GameState) 관리
- 화면 전환 제어
- 게임 흐름 제어 (시작, 종료, 재시작)

**설정 방법:**
- GameManager와 **같은 GameObject**에 컴포넌트로 추가
- GameManager가 `GetComponent<UIManager>()`로 참조 획득
- 싱글톤이지만 다른 매니저들과 동일한 방식으로 관리

**주요 메서드:**
- `ChangeState(GameState)`: 게임 상태 변경
- `StartStage(stageName)`: 스테이지 시작
- `PauseGame()` / `ResumeGame()`: 일시정지 제어

**코드 사용 예시:**
```csharp
// 다른 스크립트에서 사용
UIManager.Instance.ShowGameOver();
UIManager.Instance.PauseGame();
```

---

## 🎮 게임 시작 흐름

### UI 시스템 있을 때
```
GameManager.Start()
    ↓
UIManager 감지
    ↓
UIManager.Start() → TitleScreen 표시
    ↓ (사용자 Start 클릭)
UIManager.StartGame() → MainMenuScreen 표시
    ↓ (사용자 Start Stage 클릭)
UIManager.StartStage()
    ↓
플레이어 Pawn 생성 + 스테이지 시작
```

### UI 시스템 없을 때 (테스트 모드)
```
GameManager.Start()
    ↓
UIManager 없음 감지
    ↓
GameManager.StartGameDirectly()
    ↓
플레이어 Pawn 생성 + 스테이지 시작
```

---

## 🔧 매니저 간 의존성

**의존성 규칙:**
- GameManager는 모든 매니저를 알고 있음
- UIManager는 GameManager를 통해 다른 매니저 접근
- LifecycleManager, CreationManager, StageManager는 UI 모름
- **단방향 의존성 유지**

---

## ⚠️ 주의사항

### 게임 시작 순서
1. GameManager.Awake() - 모든 매니저 참조 획득
2. UIManager.Awake() - 화면 초기화
3. GameManager.Start() - UI 존재 확인
4. UIManager.Start() - 타이틀 화면 표시

### 메모리 관리
- 화면 전환 시 이전 화면은 `Hide()`만 호출 (파괴 안함)
- 메인 메뉴 복귀 시 모든 Pawn 파괴

---

## 🎯 스테이지 시스템 (Stage System)

스테이지 시스템은 **웨이브 기반 하이브리드 방식**으로 적을 소환합니다. JSON 파일로 스테이지 설정을 관리하며, 시간대별로 다른 적을 소환할 수 있습니다.

### 📁 파일 위치

- **스테이지 JSON 파일**: `Assets/StreamingAssets/Stages/*.json`
- **레시피 파일**: `Assets/StreamingAssets/Recipes/*.json` (적 종류 정의)

### 📋 JSON 구조

스테이지 JSON 파일은 다음과 같은 구조를 가집니다:

```json
{
  "stageName": "Stage1",
  "spawnRadius": 10.0,
  "spawnDistanceFromCamera": 2.0,
  "stageDuration": 300.0,
  "enemyWaves": [
    {
      "startTime": 0.0,
      "endTime": 30.0,
      "spawnInterval": 2.5,
      "spawnOnce": false,
      "enemyTypes": [
        {
          "recipeName": "Enemy",
          "weight": 100
        }
      ]
    }
  ]
}
```

### 🔍 필드 설명

#### StageData (최상위)

| 필드 | 타입 | 설명 | 예시 |
|------|------|------|------|
| `stageName` | string | 스테이지 이름 (고유 식별자) | `"Stage1"` |
| `spawnRadius` | float | 스폰 반지름 (현재 미사용, 향후 확장용) | `10.0` |
| `spawnDistanceFromCamera` | float | 카메라로부터의 스폰 거리 | `2.0` |
| `stageDuration` | float | 스테이지 지속 시간 (초). `-1`이면 무한 | `300.0` |
| `enemyWaves` | array | 적 웨이브 목록 | `[...]` |

#### EnemyWave (웨이브)

| 필드 | 타입 | 설명 | 예시 |
|------|------|------|------|
| `startTime` | float | 웨이브 시작 시간 (초) | `0.0` |
| `endTime` | float | 웨이브 종료 시간 (초). `-1`이면 스테이지 종료까지 | `30.0` |
| `spawnInterval` | float | 적 소환 간격 (초) | `2.5` |
| `spawnOnce` | bool | `true`면 한 번만 소환 (보스 등) | `false` |
| `enemyTypes` | array | 소환할 적 종류 목록 | `[...]` |

#### EnemyType (적 종류)

| 필드 | 타입 | 설명 | 예시 |
|------|------|------|------|
| `recipeName` | string | 레시피 파일 이름 (확장자 제외) | `"Enemy"` |
| `weight` | int | 가중치 (높을수록 더 자주 소환됨) | `100` |

### 🎮 작동 방식

1. **웨이브 활성화**: 스테이지 경과 시간이 `startTime`에 도달하면 웨이브가 활성화됩니다.
2. **적 소환**: `spawnInterval`마다 웨이브의 `enemyTypes` 중 가중치 기반으로 적을 선택하여 소환합니다.
3. **웨이브 종료**: `endTime`에 도달하거나 (`-1`이 아닌 경우) `spawnOnce`가 `true`이고 한 번 소환되면 웨이브가 비활성화됩니다.

### 📝 예시 시나리오

#### 예시 1: 기본 웨이브
```json
{
  "startTime": 0.0,
  "endTime": 60.0,
  "spawnInterval": 2.5,
  "spawnOnce": false,
  "enemyTypes": [
    {"recipeName": "Enemy", "weight": 100}
  ]
}
```
- 0초부터 60초까지
- 2.5초마다 "Enemy" 소환

#### 예시 2: 여러 적 종류 (가중치)
```json
{
  "startTime": 30.0,
  "endTime": 120.0,
  "spawnInterval": 2.0,
  "spawnOnce": false,
  "enemyTypes": [
    {"recipeName": "Enemy", "weight": 70},
    {"recipeName": "FastEnemy", "weight": 30}
  ]
}
```
- 30초부터 120초까지
- 2초마다 소환
- 70% 확률로 "Enemy", 30% 확률로 "FastEnemy"

#### 예시 3: 보스 소환 (일회성)
```json
{
  "startTime": 120.0,
  "endTime": -1,
  "spawnInterval": 0.0,
  "spawnOnce": true,
  "enemyTypes": [
    {"recipeName": "Boss", "weight": 100}
  ]
}
```
- 120초에 한 번만 "Boss" 소환

### ✏️ 스테이지 수정 방법

#### 1. 새 스테이지 추가
1. `Assets/StreamingAssets/Stages/` 폴더에 새 JSON 파일 생성 (예: `Stage2.json`)
2. 위 구조에 맞춰 작성
3. `GameManager.StartStage("Stage2")` 호출

#### 2. 기존 스테이지 수정
1. `Assets/StreamingAssets/Stages/Stage1.json` 파일 열기
2. 원하는 필드 수정:
   - **스테이지 시간 변경**: `stageDuration` 수정
   - **웨이브 추가**: `enemyWaves` 배열에 새 웨이브 추가
   - **적 종류 변경**: `enemyTypes` 배열 수정
   - **소환 간격 변경**: `spawnInterval` 수정
3. Unity 에디터에서 재시작 (JSON은 런타임에 로드됨)

#### 3. 새 적 종류 추가
1. `Assets/StreamingAssets/Recipes/` 폴더에 새 레시피 JSON 파일 생성 (예: `FastEnemy.json`)
2. 레시피 파일 작성 (PawnRecipeData 구조)
3. 스테이지 JSON의 `enemyTypes`에 추가:
   ```json
   {
     "recipeName": "FastEnemy",
     "weight": 50
   }
   ```

### ⚠️ 주의사항

1. **레시피 이름**: `recipeName`은 레시피 파일 이름과 정확히 일치해야 합니다 (대소문자 구분, 확장자 제외)
2. **시간 중복**: 여러 웨이브의 시간 범위가 겹치면 동시에 소환됩니다
3. **가중치 계산**: 가중치는 상대적입니다. 예: `[100, 50]` = 66.7%, 33.3%
4. **스폰 위치**: 모든 적은 카메라 뷰포트 바깥쪽 원형 위치에 소환됩니다

### 🔧 코드에서 접근

```csharp
// 스테이지 시작
GameManager.Instance.StartStage("Stage1");

// 스테이지 데이터 로드
StageData stageData = GameManager.Instance.LoadStage("Stage1");
```

---

## 🏗️ 클린 아키텍처 관점

이 매니저들은 애플리케이션을 부트스트랩하고, 각 계층(Layer)이 원활하게 동작하도록 조율하는 **인프라스트럭처(Infrastructure)** 계층의 일부입니다.

**설계 원칙:**
- 기존 매니저들의 핵심 기능 유지
- UI 시스템은 선택적 (없어도 게임 작동)
- 명확한 책임 분리
- 테스트 용이성 확보

---

## 🔧 향후 개선 가능 사항 (선택적)

현재 구조는 프로젝트 규모에 적합하지만, 프로젝트가 커지면 다음 개선 사항을 고려할 수 있습니다.

### 1. Queue(Action) 할당 비용 최적화

**현재 상태:**
- `EnqueueEarlyUpdate/LateUpdate`는 현재 거의 사용되지 않아 문제 없음
- `RequestDestruction` 정도만 사용

**개선 필요 시점:**
- 매 프레임 `Enqueue` 호출이 많아질 때
- GC 스파이크가 발생할 때

**개선 방법:**
```csharp
// Action 대신 struct command 패턴
public struct UpdateCommand
{
    public Action action;
    // 또는 ObjectPool<Action> 사용
}
```

### 2. 시스템 분리 (Dispatcher 패턴)

**현재 상태:**
- `LifecycleManager`가 `StageManager`, `PawnManager`를 직접 호출
- 현재 시스템 수가 적어 문제 없음

**개선 필요 시점:**
- 시스템이 5개 이상으로 늘어날 때
- 각 시스템의 업데이트 우선순위가 필요할 때

**개선 방법:**
```csharp
// LifecycleManager는 틀만 유지
// 실제 업데이트는 GameUpdateDispatcher가 위임
LifecycleManager
    └── GameUpdateDispatcher
            ├── StageManager
            ├── PawnSystem
            ├── AISystem
            ├── CombatSystem
```

### 3. 선택적 업데이트

**현재 상태:**
- 모든 시스템이 매 프레임 업데이트됨
- 현재는 성능 문제 없음

**개선 필요 시점:**
- 특정 시스템이 업데이트할 필요가 없는 프레임이 많을 때
- 성능 최적화가 필요할 때

**개선 방법:**
```csharp
// 각 시스템이 UpdateNeeded 플래그 제공
if (stageManager.NeedsUpdate)
{
    stageManager.UpdateStage();
}
```

### 4. 의존성 주입

**현재 상태:**
- `GameManager.Instance` 직접 참조
- 현재 규모에서는 문제 없음

**개선 필요 시점:**
- 단위 테스트 작성이 필요할 때
- 시스템 간 결합도를 낮추고 싶을 때

**개선 방법:**
```csharp
// 인터페이스 기반 의존성 주입
public interface IStageManager
{
    void UpdateStage();
}

// LifecycleManager는 인터페이스만 알고 있음
private IStageManager _stageManager;
```

### 5. FixedUpdate 통합

**현재 상태:**
- Update만 관리
- 물리 시스템이 필요 없어 문제 없음

**개선 필요 시점:**
- 물리 기반 게임플레이가 필요할 때
- 고정 프레임레이트 물리 업데이트가 필요할 때

**개선 방법:**
```csharp
void FixedUpdate()
{
    if (IsPaused) return;
    // 물리 시스템만 별도로 처리
    ProcessPhysicsUpdate();
}
```

### 6. 멀티스레딩 고려 (JobQueue)

**현재 상태:**
- 단일 스레드
- 현재는 문제 없음

**개선 필요 시점:**
- 수백 개의 객체를 동시에 처리해야 할 때
- 복잡한 AI 계산이 필요할 때

**개선 방법:**
```csharp
// Unity Job System 활용
// 또는 자체 JobQueue 구현
```

---

## 📝 참고

이러한 개선 사항들은 **필수가 아닌 선택적**입니다. 현재 구조로도 안정적으로 큰 프로젝트를 진행할 수 있습니다. 프로젝트 규모와 필요에 따라 점진적으로 개선하는 것을 권장합니다.

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
- 적 스폰 타이밍 제어
- 스테이지 데이터 기반 동작
- LifecycleManager에 의해 매 프레임 업데이트

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

## 🏗️ 클린 아키텍처 관점

이 매니저들은 애플리케이션을 부트스트랩하고, 각 계층(Layer)이 원활하게 동작하도록 조율하는 **인프라스트럭처(Infrastructure)** 계층의 일부입니다.

**설계 원칙:**
- 기존 매니저들의 핵심 기능 유지
- UI 시스템은 선택적 (없어도 게임 작동)
- 명확한 책임 분리
- 테스트 용이성 확보

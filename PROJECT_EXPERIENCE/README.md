# Pawn Survivors - 프로젝트 경험 정리

> Unity 2D 로그라이크 서바이벌 게임 개발 경험 및 트러블슈팅

---

## 📋 목차

1. [아키텍처 개선 경험](#아키텍처-개선-경험)
2. [트러블슈팅 사례](#트러블슈팅-사례)
3. [최적화 및 성능 개선](#최적화-및-성능-개선)
4. [개발 도구 구축](#개발-도구-구축)

---

## 🏗️ 아키텍처 개선 경험

### 1. State Pattern 리팩토링

**문제 상황:**
- 기존: Enum 기반 상태 머신으로 하드코딩된 전환 로직
- 상태 전환 시 복잡한 조건문과 예외 처리로 코드 가독성 저하
- 새로운 상태 추가 시 기존 코드 수정 필요

**해결 과정:**
```csharp
// Before: Enum 기반
enum GameState { Title, CharacterSelect, Stage, Shop, GameOver }
void TransitionTo(GameState state) { /* 복잡한 switch문 */ }

// After: State Pattern
interface IGameState {
    void OnEnter();
    void OnExit();
    bool CanTransitionTo(IGameState nextState);
}
```

**성과:**
- ✅ 각 상태의 책임 명확화 (SRP 준수)
- ✅ 상태 전환 로직 중앙화 (GameStateManager)
- ✅ 새로운 상태 추가 시 기존 코드 수정 불필요 (OCP 준수)
- ✅ 상태별 독립적인 테스트 가능

**기술 스택:** C#, Design Patterns (State Pattern)

---

### 2. Single Scene → Multi-Scene 아키텍처 전환

**문제 상황:**
- Single Scene 구조에서 씬 전환 없이 모든 UI와 오브젝트가 한 씬에 존재
- 수동으로 오브젝트 생성/제거 관리 필요
- 씬별 독립성 부족으로 상태 관리 복잡

**해결 과정:**
1. **씬 분리 설계**
   - TitleScene, CharacterSelectScene, StageSelectScene, StageScene, ShopScene, GameOverScene
   - 각 씬별 독립적인 Screen 컴포넌트 (MVC 패턴)

2. **씬별 독립성 확보**
   ```csharp
   // 각 Screen이 Awake/Start에서 자체 초기화
   public class StageScreen : MonoBehaviour {
       private void Start() {
           InitializeStageManager();
           CreateBackgroundTilemap();
           StartStageIfNeeded(); // 씬 로드 시 자동 시작
       }
   }
   ```

3. **State-Scene 매핑**
   ```csharp
   private Dictionary<Type, string> _stateToSceneMap = new Dictionary<Type, string> {
       { typeof(StageState), "StageScene" },
       { typeof(ShopState), "ShopScene" },
       // ...
   };
   ```

**성과:**
- ✅ 씬 전환 시 자동 정리 (Unity 엔진 기능 활용)
- ✅ 씬별 독립적인 초기화 로직
- ✅ 수동 정리 코드 제거 (약 200줄 감소)
- ✅ 유지보수성 향상

**기술 스택:** Unity Scene Management, State Pattern

---

### 3. Clean Architecture 적용

**구조:**
```
Domain (비즈니스 로직, Unity 독립)
  ├── Usecases/        # 게임 규칙
  ├── States/          # State Pattern
  └── Events/          # 이벤트 정의

Presentation (Unity 의존)
  └── SubManagers/     # Unity 컴포넌트

Data (저장소)
  └── Repositories/     # JSON 기반 저장소
```

**핵심 원칙:**
- 의존성 규칙: 상위 계층만 하위 계층 호출
- Domain 계층은 Unity에 의존하지 않음
- UseCase를 통한 비즈니스 로직 중앙화

**성과:**
- ✅ 테스트 용이성 향상 (Domain 계층은 Unity 없이 테스트 가능)
- ✅ 계층별 책임 명확화
- ✅ 확장성 향상 (새 기능 추가 시 다른 계층 영향 최소화)

**기술 스택:** Clean Architecture, Dependency Injection

---

## 🔧 트러블슈팅 사례

### 1. ESC 키 동작 불일치 문제

**문제:**
- 첫 ESC: 옵션창 열림 ✅
- 두 번째 ESC: 옵션창 닫힘 (하지만 내부적으로 여전히 Paused 상태)
- 세 번째 ESC: 반응 없음 (내부적으로 Unpause)
- 네 번째 ESC: 다시 옵션창 열림

**원인 분석:**
- `UIManager.HidePauseMenu()`가 UI만 숨기고 `GameStateManager`의 상태는 업데이트하지 않음
- `GameStateManager`와 `UIManager` 간 상태 동기화 누락
- `PausedState.OnExit()`에서 `HidePauseMenu()` 호출 시 무한 루프 방지를 위한 플래그 부재

**해결:**
```csharp
// UIManager.HidePauseMenu()에 updateState 파라미터 추가
public void HidePauseMenu(bool updateState = true) {
    _pauseMenuScreen.SetActive(false);
    // ...
    if (updateState && GameStateManager.Instance != null) {
        GameStateManager.Instance.GoToStage();
    }
}

// PausedState.OnExit()에서 updateState=false로 호출
public void OnExit() {
    UIManager.Instance.HidePauseMenu(updateState: false);
    // GameStateManager가 이미 상태 전환을 처리하므로
    // HidePauseMenu는 UI만 숨김
}
```

**성과:**
- ✅ UI 상태와 게임 상태 동기화
- ✅ ESC 키 동작 일관성 확보
- ✅ 상태 전환 책임 명확화

---

### 2. 씬 전환 시 UI 잔존 문제

**문제:**
- 옵션창, 일시정지 메뉴 등 오버레이 UI가 씬 전환 후에도 남아있음
- `UIManager`가 `DontDestroyOnLoad`로 설정되어 자식 UI도 함께 유지됨

**원인:**
- 씬 전환 시 오버레이 UI를 숨기는 로직 부재
- `SceneManager.sceneLoaded` 이벤트 미구독

**해결:**
```csharp
private void Awake() {
    // ...
    SceneManager.sceneLoaded += OnSceneLoaded;
}

private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
    // 모든 오버레이 UI 숨기기
    if (_pauseMenuScreen != null && _pauseMenuScreen.activeSelf) {
        _pauseMenuScreen.SetActive(false);
    }
    if (_optionsScreen != null && _optionsScreen.activeSelf) {
        _optionsScreen.SetActive(false);
    }
    // AudioSettingsScreen도 찾아서 숨기기
    GameObject audioSettingsScreen = GameObject.Find("AudioSettingsScreen");
    if (audioSettingsScreen != null && audioSettingsScreen.activeSelf) {
        audioSettingsScreen.SetActive(false);
    }
}
```

**성과:**
- ✅ 씬 전환 시 자동으로 오버레이 UI 정리
- ✅ 사용자 경험 개선

---

### 3. 데미지 텍스트 표시 문제

**문제:**
- 데미지 텍스트가 생성되지만 화면에 표시되지 않음
- 씬 전환 후 데미지 텍스트가 작동하지 않음

**원인 분석:**
- `FloatingEffectManager`가 `GameManager`에 컴포넌트로 있어 `DontDestroyOnLoad`로 유지됨
- 씬 전환 시 카메라 참조가 무효화됨
- `PawnDamagedEvent` 구독이 씬 전환 후 해제됨

**해결:**
```csharp
private void OnEnable() {
    SubscribeToDamageEvents();
    SceneManager.sceneLoaded += OnSceneLoaded;
}

private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
    // StageScene에서만 데미지 텍스트 표시
    if (scene.name == "StageScene") {
        // 카메라 재설정
        _mainCamera = null;
        EnsureCamera();
        
        // 이벤트 재구독
        SubscribeToDamageEvents();
    }
}
```

**성과:**
- ✅ 씬 전환 후에도 데미지 텍스트 정상 작동
- ✅ StageScene에서만 데미지 텍스트 활성화 (성능 최적화)

---

### 4. 경험치/레벨 영구 저장 문제

**문제:**
- 스테이지 간 캐릭터 경험치와 레벨이 저장되지 않음
- 재시작 시 경험치가 초기화됨

**원인:**
- `PawnPersistenceUseCase`는 존재하지만 저장/로드 호출 시점 불일치
- `GameManager.EndStage()`에서 저장은 하지만, `PlayerController.AddPlayerPawn()`에서 로드하지 않음
- `LevelUpSubManager`에서 레벨 복원 로직 부재

**해결:**
```csharp
// 1. GameManager.EndStage()에서 저장
public void EndStage() {
    SaveAllPawnPersistentData(); // 모든 Pawn 데이터 저장
    // ...
}

// 2. PlayerController.AddPlayerPawn()에서 로드
public void AddPlayerPawn(/* ... */) {
    // ...
    // 영구 데이터 복원
    GameManager.Instance.PawnPersistenceUseCase.RestorePawnPersistentData(
        character, playerIndex, pawnManager
    );
}

// 3. LevelUpSubManager.SubStart()에서 레벨 복원
public void SubStart() {
    RestoreLevelAndProgress(); // 저장된 레벨 복원
    ActivateStrategyForCurrentLevel(); // 레벨에 맞는 전략 활성화
}
```

**성과:**
- ✅ 스테이지 간 경험치/레벨 유지
- ✅ 게임 진행 연속성 확보

---

### 5. 재시작 로직 문제

**문제:**
- 재시작 시 기존 Pawn이 유지되거나 제대로 초기화되지 않음
- 재시작 후 이동 입력이 작동하지 않음
- BGM이 계속 재생됨

**원인:**
- `CharacterSelectState.OnEnter()`에서 초기화 로직 부족
- `PlayerController` 제거 후 재생성 시점 불일치
- `LifecycleManager`의 Pause 상태가 해제되지 않음

**해결:**
```csharp
public void OnEnter() {
    // 재시작 시 완전히 새로 시작하기 위해 PlayerController 제거
    if (GameManager.Instance != null) {
        GameManager.Instance.ClearPlayerController();
    }
    
    // BGM 정지 (재시작 시 이전 BGM이 남아있지 않도록)
    if (GameManager.Instance != null) {
        GameManager.Instance.StopBGM();
    }
    
    // 게임 일시정지 해제 (재시작 시 정상 상태로)
    if (GameManager.Instance?.LifecycleManager != null) {
        if (GameManager.Instance.LifecycleManager.IsPaused) {
            GameManager.Instance.LifecycleManager.TogglePause();
        }
    }
    
    // 게임 상태 리셋
    if (GameManager.Instance?.StageFlowUseCase != null) {
        GameManager.Instance.StageFlowUseCase.ResetState();
    }
}
```

**성과:**
- ✅ 재시작 시 완전히 초기화된 상태로 시작
- ✅ 사용자 경험 일관성 확보

---

## ⚡ 최적화 및 성능 개선

### 1. UI 요소 참조 개선 (String → Enum → Dictionary)

**문제:**
- `transform.Find("ButtonName")` 같은 string 기반 검색
- 타입 안전성 부족, 오타 위험
- 런타임 성능 저하 (문자열 비교)

**개선 과정:**
```csharp
// Step 1: Enum 도입
private enum UIElementType {
    Title, BackButton, CharacterList, // ...
}

// Step 2: Dictionary로 직접 참조
private Dictionary<UIElementType, GameObject> _uiElements = 
    new Dictionary<UIElementType, GameObject>();

// Step 3: 초기화 시 매핑
private void InitializeUIElements() {
    for (int i = 0; i < transform.childCount; i++) {
        Transform child = transform.GetChild(i);
        if (Enum.TryParse<UIElementType>(child.name, out UIElementType type)) {
            _uiElements[type] = child.gameObject;
        }
    }
}
```

**성과:**
- ✅ 타입 안전성 확보 (컴파일 타임 체크)
- ✅ 런타임 성능 향상 (O(1) 접근)
- ✅ 오타 방지

---

### 2. 씬별 독립성 확보로 불필요한 체크 제거

**문제:**
- `GameManager`가 모든 씬에서 사용되는 로직을 보유
- 씬 전환 시 수동으로 사용 여부 체크 필요
- 예: `StageManager`가 모든 씬에 존재하지만 StageScene에서만 사용

**개선:**
```csharp
// Before: GameManager에 StageManager
public class GameManager : MonoBehaviour {
    public StageManager StageManager { get; private set; }
    // 모든 씬에서 존재하지만 StageScene에서만 사용
}

// After: StageScreen에 StageManager
public class StageScreen : MonoBehaviour {
    private StageManager _stageManager;
    
    private void InitializeStageManager() {
        _stageManager = gameObject.AddComponent<StageManager>();
        // StageScene에서만 초기화
    }
}
```

**성과:**
- ✅ 씬별 로직 분리 (관심사 분리)
- ✅ 불필요한 업데이트 제거 (성능 향상)
- ✅ 수동 체크 코드 제거 (약 50줄 감소)

---

### 3. 적 생성 최적화

**문제:**
- 스테이지 종료 후에도 적이 계속 생성됨
- 재시작 시 이전 적이 남아있음

**해결:**
```csharp
public void UpdateStage() {
    // 스테이지가 실행 중이 아니면 업데이트하지 않음
    if (!IsStageRunning()) return;
    
    // 적 생성 로직
    // ...
}

public void EndStage() {
    // 웨이브 타이머 초기화
    _waveTimers.Clear();
    _waveSpawnedOnce.Clear();
    
    _currentStageData = null;
    _stageElapsedTime = 0f;
}
```

**성과:**
- ✅ 불필요한 적 생성 방지
- ✅ 메모리 사용량 감소

---

## 🛠️ 개발 도구 구축

### 1. 씬 자동 생성 도구

**목적:**
- 프로젝트 전환 시 여러 씬을 일관되게 생성
- 필수 오브젝트 자동 배치

**구현:**
```csharp
[MenuItem("Tools/Create Game Scenes")]
public static void CreateAllGameScenes() {
    CreateScene("TitleScene", /* ... */);
    CreateScene("CharacterSelectScene", /* ... */);
    CreateScene("StageScene", /* ... */);
    // ...
}
```

**성과:**
- ✅ 씬 생성 시간 단축 (수동 작업 대비 90% 감소)
- ✅ 일관성 확보

---

### 2. UI 프리팹 자동 생성 도구

**목적:**
- UI Screen 컴포넌트를 프리팹으로 자동 생성
- 런타임 UI 생성 로직을 프리팹에 포함

**구현:**
```csharp
private static void CreateScreenPrefab<T>(string prefabPath) where T : MonoBehaviour {
    // 새 씬 생성
    Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
    
    // GameObject 생성 및 컴포넌트 추가
    GameObject obj = new GameObject(typeof(T).Name);
    T component = obj.AddComponent<T>();
    
    // Awake/Start 강제 호출 (UI 생성)
    component.SendMessage("Awake", SendMessageOptions.DontRequireReceiver);
    component.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
    
    // 프리팹으로 저장
    PrefabUtility.SaveAsPrefabAsset(obj, prefabPath);
}
```

**성과:**
- ✅ UI 프리팹 일관성 확보
- ✅ 수동 작업 제거

---

## 📊 프로젝트 통계

- **코드 라인 수:** 약 15,000줄 (C#)
- **아키텍처 계층:** 3계층 (Domain, Presentation, Data)
- **디자인 패턴:** State, Strategy, Observer, Component, Repository, Factory
- **리팩토링 규모:** Enum 기반 상태 머신 → State Pattern (약 500줄 수정)
- **성능 개선:** 불필요한 업데이트 제거로 프레임 드롭 감소

---

## 🎯 핵심 성과

1. **아키텍처 개선**
   - State Pattern 적용으로 상태 관리 체계화
   - Multi-Scene 아키텍처로 씬별 독립성 확보
   - Clean Architecture 적용으로 테스트 용이성 향상

2. **트러블슈팅**
   - ESC 키 동작 불일치 해결 (상태 동기화)
   - 씬 전환 시 UI 잔존 문제 해결
   - 데미지 텍스트 표시 문제 해결
   - 경험치/레벨 영구 저장 구현

3. **최적화**
   - UI 요소 참조 개선 (String → Enum → Dictionary)
   - 씬별 로직 분리로 불필요한 업데이트 제거
   - 적 생성 최적화

4. **개발 효율성**
   - 씬/UI 프리팹 자동 생성 도구 구축
   - 수동 작업 시간 90% 감소

---

## 💡 배운 점

1. **아키텍처 설계의 중요성**
   - 초기 설계가 후속 개발에 미치는 영향이 큼
   - 리팩토링 비용을 고려한 설계 필요

2. **상태 관리의 복잡성**
   - UI 상태와 게임 상태의 동기화 중요
   - State Pattern으로 상태 전환 로직 명확화

3. **씬 관리 전략**
   - Single Scene vs Multi-Scene의 트레이드오프
   - 씬별 독립성 확보의 중요성

4. **트러블슈팅 접근법**
   - 문제의 근본 원인 파악이 중요
   - 임시 해결책보다 구조적 해결책 선호

---

## 🔗 관련 기술

- **언어:** C#
- **엔진:** Unity 2022.3 LTS
- **아키텍처:** Clean Architecture, State Pattern
- **디자인 패턴:** Strategy, Observer, Component, Repository, Factory
- **데이터:** JSON (StreamingAssets)
- **버전 관리:** Git

---

*이 문서는 프로젝트 진행 중 얻은 경험과 트러블슈팅을 정리한 것입니다.*



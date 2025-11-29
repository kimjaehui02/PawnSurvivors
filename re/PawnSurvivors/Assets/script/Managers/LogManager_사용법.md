# LogManager 사용법 가이드

## 📋 개요

`LogManager`는 Unity 프로젝트의 모든 디버그 로그를 중앙에서 관리하는 시스템입니다. 카테고리별, 레벨별 필터링으로 로그 스팸을 제어할 수 있습니다.

---

## 🚀 기본 사용법

### 1. Unity Inspector에서 설정

#### LogManager 컴포넌트 설정
1. Unity Editor에서 씬에 빈 GameObject 생성
2. `LogManager` 컴포넌트 추가
3. Inspector에서 `LogManager` 컴포넌트 설정
4. **중요**: 씬에 LogManager가 없으면 모든 로그가 출력되지 않습니다

#### 기본 설정
```
LogManager (Component)
├─ Enable All: [✓]  ← 전체 로그 켜기/끄기
└─ Category Filters:  ← 카테고리별 설정
   ├─ Recipe
   │  ├─ Enabled: [✓]
   │  └─ Min Level: [Info ▼]
   ├─ Item
   │  ├─ Enabled: [✓]
   │  └─ Min Level: [Info ▼]
   ├─ Debug
   │  ├─ Enabled: [ ]  ← 평소에는 꺼둠
   │  └─ Min Level: [Debug ▼]
   └─ ...
```

#### 권장 설정 (평상시)
- **Debug**: `Enabled = false` (디버깅 시에만 켜기)
- **Recipe**: `Min Level = Warning` (Info 로그 숨김)
- **UI**: `Min Level = Error` (경고/정보 로그 숨김)
- **System**: `Min Level = Warning` (중요한 것만)
- **Stage**: `Min Level = Info` (정상)
- **Combat**: `Min Level = Error` (에러만)
- **Item**: `Min Level = Warning` (중요한 것만)
- **Pawn**: `Min Level = Warning` (중요한 것만)
- **Data**: `Min Level = Warning` (중요한 것만)

---

## 💻 코드에서 사용하기

### 기본 사용법

```csharp
using PawnSurvivors.Managers;

// 기존 방식
Debug.Log("메시지");
Debug.LogWarning("경고");
Debug.LogError("에러");

// LogManager 방식
LogManager.LogInfo(LogCategory.System, "메시지");
LogManager.LogWarning(LogCategory.System, "경고");
LogManager.LogError(LogCategory.System, "에러");
```

### 편의 메서드

```csharp
// Debug 레벨 (가장 상세)
LogManager.LogDebug(LogCategory.Debug, "디버깅 메시지");

// Info 레벨 (일반 정보)
LogManager.LogInfo(LogCategory.System, "시스템 초기화 완료");

// Warning 레벨 (경고)
LogManager.LogWarning(LogCategory.Item, "아이템을 찾을 수 없습니다");

// Error 레벨 (에러, 항상 표시됨)
LogManager.LogError(LogCategory.System, "치명적 오류 발생");
```

### 카테고리 선택 가이드

| 카테고리 | 사용 예시 | 권장 Min Level |
|---------|----------|---------------|
| **Recipe** | RecipeDataSource, ItemPoolDataSource | Warning |
| **Item** | 아이템 추가/제거, 구매 | Warning |
| **Combat** | 데미지, 전투 이벤트 | Error |
| **UI** | UI 생성/업데이트 | Error |
| **Debug** | 디버깅용 테스트 코드 | Debug (평소 꺼둠) |
| **System** | GameManager, StageManager | Warning |
| **Pawn** | Pawn 생성/파괴 | Warning |
| **Data** | 데이터 로딩/저장 | Warning |
| **Stage** | 스테이지 시작/종료 | Info |

---

## 🎯 실제 사용 예시

### 예시 1: RecipeDataSource
```csharp
// 레시피 로드 성공
LogManager.LogInfo(LogCategory.Recipe, $"레시피 로드: {recipeName}");

// 레시피 로드 실패
LogManager.LogError(LogCategory.Recipe, $"레시피 로드 실패: {recipeName}");
```

### 예시 2: GameManager
```csharp
// 시스템 초기화
LogManager.LogInfo(LogCategory.System, "GameManager 초기화 완료");

// 에러 발생
LogManager.LogError(LogCategory.System, "LifecycleManager를 찾을 수 없습니다");
```

### 예시 3: UI
```csharp
// UI 생성
LogManager.LogInfo(LogCategory.UI, "상점 UI 생성 완료");

// UI 에러
LogManager.LogError(LogCategory.UI, "Canvas를 찾을 수 없습니다");
```

### 예시 4: 디버깅용 (ItemPoolTestManager)
```csharp
// 디버깅 로그 (평소에는 숨김)
LogManager.LogDebug(LogCategory.Debug, "테스트 시작");
LogManager.LogInfo(LogCategory.Debug, "테스트 완료");
```

---

## ⚙️ 고급 설정

### 런타임에서 필터 변경

```csharp
// 특정 카테고리 켜기/끄기
var filter = LogManager.Instance.categoryFilters
    .FirstOrDefault(f => f.category == LogCategory.Debug);
if (filter != null)
{
    filter.enabled = true;  // 디버깅 모드 켜기
}

// 전체 로그 끄기
LogManager.Instance.enableAll = false;
```

### 조건부 로그

```csharp
// 특정 조건에서만 로그 출력
if (someCondition)
{
    LogManager.LogInfo(LogCategory.System, "조건 만족");
}
```

---

## 🔍 문제 해결

### 로그가 안 나와요
1. **씬에 LogManager 있는지 확인**: Hierarchy에서 LogManager GameObject 확인
2. **Inspector 확인**: `Enable All`이 켜져 있는지 확인
3. **카테고리 확인**: 해당 카테고리의 `Enabled`가 켜져 있는지 확인
4. **레벨 확인**: `Min Level`이 로그 레벨보다 낮거나 같은지 확인
   - 예: `Min Level = Warning`이면 `LogInfo`는 안 나옴

### 로그가 너무 많아요
1. **Debug 카테고리 끄기**: `Enabled = false`
2. **Min Level 올리기**: `Info` → `Warning` → `Error`
3. **전체 끄기**: `Enable All = false` (Error는 여전히 표시됨)

### Error 로그도 안 나와요
- `Enable All = false`이면 Error도 차단됩니다
- 씬에 LogManager가 없으면 모든 로그가 출력되지 않습니다
- 코드에서 `LogManager.LogError`가 호출되는지 확인

---

## 📊 성능 최적화

### Early-out 최적화
LogManager는 문자열 조합 전에 필터링을 확인하므로, 필터링된 로그는 성능 오버헤드가 거의 없습니다.

```csharp
// 이 코드는 필터링되면 문자열 조합도 하지 않음
LogManager.LogInfo(LogCategory.Debug, $"복잡한 문자열: {복잡한계산()}");
```

### Dictionary 최적화
필터 조회는 O(1) 시간 복잡도로 최적화되어 있습니다.

---

## 🎨 카테고리별 색상 (Unity Console)

Unity Console에서 카테고리별로 구분하기 쉽도록 메시지에 `[카테고리명]`이 자동으로 붙습니다:

```
[System] GameManager 초기화 완료
[Recipe] 레시피 로드: Player
[UI] 상점 UI 생성 완료
[Debug] 테스트 시작
```

---

## 📝 주의사항

1. **Enable All = false**: 모든 로그가 차단됩니다 (Error 포함)
2. **씬에 LogManager 필요**: 씬에 LogManager GameObject가 없으면 모든 로그가 출력되지 않습니다
3. **수동 설정 필요**: LogManager는 자동 생성되지 않으므로 씬에 직접 추가해야 합니다
4. **DontDestroyOnLoad**: LogManager는 씬 전환 시에도 유지됩니다 (Awake에서 설정됨)
5. **기존 Debug.Log**: 기존 `Debug.Log`는 그대로 작동하지만, 필터링되지 않습니다

---

## 🔄 마이그레이션 가이드

### 기존 코드를 LogManager로 변경

**Before:**
```csharp
Debug.Log("[GameManager] 초기화 완료");
Debug.LogWarning("[GameManager] 경고 메시지");
Debug.LogError("[GameManager] 에러 발생");
```

**After:**
```csharp
LogManager.LogInfo(LogCategory.System, "초기화 완료");
LogManager.LogWarning(LogCategory.System, "경고 메시지");
LogManager.LogError(LogCategory.System, "에러 발생");
```

**변경 포인트:**
- `[태그]` 제거 (카테고리가 자동으로 붙음)
- `Debug.Log` → `LogManager.LogInfo`
- `Debug.LogWarning` → `LogManager.LogWarning`
- `Debug.LogError` → `LogManager.LogError`
- 적절한 `LogCategory` 선택

---

## 📚 추가 정보

- **파일 위치**: `Assets/script/Managers/LogManager.cs`
- **네임스페이스**: `PawnSurvivors.Managers`
- **싱글톤 패턴**: `LogManager.Instance`로 접근
- **자동 초기화**: `RuntimeInitializeOnLoadMethod`로 씬 로드 전 초기화

---

## ✅ 체크리스트

- [ ] **씬에 LogManager GameObject 추가** (필수)
- [ ] LogManager 컴포넌트가 있는지 확인
- [ ] Inspector에서 기본 필터 설정 확인
- [ ] Debug 카테고리는 평소에 꺼두기
- [ ] 중요한 카테고리는 Min Level 조정
- [ ] 코드에서 적절한 카테고리 사용
- [ ] Enable All을 끄면 모든 로그가 차단되는지 확인

---

**마지막 업데이트**: 2024년 (LogManager 구현 완료)


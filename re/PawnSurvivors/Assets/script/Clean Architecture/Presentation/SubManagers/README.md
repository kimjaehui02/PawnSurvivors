# 서브매니저 (SubManagers)

이 폴더에는 `Pawn`의 구체적인 기능들을 담당하는 서브매니저(SubManager) 클래스들이 기능별 하위 폴더로 나뉘어 저장됩니다.

모든 서브매니저는 `PawnSubManager` 베이스 클래스를 상속받으며, `PawnManager`에 의해 생명주기가 관리됩니다.

---

## 📋 SubManager 생명주기

### 1. 등록 (Registration)
```csharp
// CreationManager에서 Pawn 생성 시
PawnManager pawnManager = pawnObject.AddComponent<PawnManager>();
MonoBehaviour subManager = setup.AddSubManagerComponent(pawnObject);
pawnManager.RegisterSubManager(subManager as PawnSubManager);
```

### 2. 초기화 (Initialization)
```csharp
// PawnManager.InitializeSubManagers()
foreach (var subManager in pawnSubManagers)
{
    subManager.SubStart();  // ← 각 SubManager의 초기화
}
```

### 3. 업데이트 (Update)
```csharp
// LifecycleManager → PawnManager.ManagedUpdate()
foreach (var subManager in pawnSubManagers)
{
    subManager.SubUpdate();  // ← 매 프레임 호출
}
```

### 4. 파괴 (Destruction)
```csharp
// PawnManager.OnDisable()
// SubManager들도 자동으로 파괴됨
```

---

## 🔗 데이터 접근 방식

### PawnData 모듈화 구조
```csharp
PawnData
├── healthData (HealthData)      // 체력 관련
├── visualData (VisualData)      // 시각 관련
├── combatData (CombatData)      // 전투 관련
├── movableData (MovableData)    // 이동 관련
└── physicsData (PhysicsData)    // 물리 관련
```

### SubManager별 데이터 사용
각 SubManager는 **필요한 모듈만** 사용합니다:

```csharp
// DamageableSubManager
public override void SubStart()
{
    _pawnData = _pawnManager.PawnData;
    
    // HealthData만 사용
    if (_pawnData.healthData == null)
        _pawnData.healthData = new HealthData();
    
    _pawnData.healthData.currentHealth = _pawnData.healthData.maxHealth;
}
```

**장점:**
- 필요한 데이터만 보유 (메모리 효율)
- 명확한 의존성
- 런타임에 SubManager 추가/제거 가능

---

## 🎯 SubManager 간 통신

### 이벤트 버스 시스템
SubManager들은 **서로를 직접 참조하지 않고** 이벤트로 통신합니다:

```csharp
// 발행자 (CollisionDamageSubManager)
targetPawnManager.Publish(new DamageEvent(target, damage, attacker));

// 구독자 (DamageableSubManager)
_pawnManager.Subscribe<DamageEvent>(HandleDamageEvent);
```

**통신 흐름 예시:**
```
CollisionDamageSubManager (충돌 감지)
    ↓ Publish(DamageEvent)
PawnManager (이벤트 버스)
    ↓ 구독자들에게 전달
DamageableSubManager (데미지 처리)
    ↓ 체력 0 이하
    ↓ Publish(PawnDeathEvent)
PawnManager (사망 처리)
```

---

## 📦 주요 SubManager 목록

### Combat (전투)
- **DamageableSubManager**: 데미지를 받고 체력 관리, DamageEvent 구독
- **CollisionDamageSubManager**: 충돌 시 DamageEvent 발행
- **ProjectileShooterSubManager**: 발사체 발사, 자동 타겟팅

### Movement (이동)
- **MovableSubManager**: 이동 전략 관리 및 실행
- **Strategies**: KeyboardMovement, DirectionalMovement, HomingMovement, TargetMovement

### Input (입력)
- **PlayerAttackInputSubManager**: 마우스 입력 감지, AttackInputEvent 발행

### Visual (시각)
- **VisualSubManager**: 스프라이트 렌더링, "Visuals" 자식 GameObject 생성

### Physics (물리)
- **PhysicsSubManager**: Collider, Rigidbody, Layer, Tag 동적 설정

---

## 🛠️ 새 SubManager 만들기 가이드

### 1. 클래스 생성
```csharp
public class MySubManager : PawnSubManager
{
    private PawnData _pawnData;
    
    public override void SubStart()
    {
        _pawnData = _pawnManager.PawnData;
        
        // 필요한 데이터 모듈 체크 및 생성
        if (_pawnData.myData == null)
            _pawnData.myData = new MyData();
        
        // 이벤트 구독
        _pawnManager.Subscribe<MyEvent>(HandleMyEvent);
    }
    
    private void OnDisable()
    {
        // 이벤트 구독 해제
        if (_pawnManager != null)
            _pawnManager.Unsubscribe<MyEvent>(HandleMyEvent);
    }
    
    public override void SubUpdate()
    {
        // 매 프레임 로직
    }
    
    private void HandleMyEvent(MyEvent evt)
    {
        // 이벤트 처리
    }
}
```

### 2. PawnData에 데이터 모듈 추가
```csharp
// PawnData.cs
public class PawnData
{
    public MyData myData;  // ← 추가
}

[Serializable]
public class MyData
{
    public float someValue;
}
```

### 3. RecipeData에 SetupData 추가
```csharp
// RecipeData.cs
[Serializable]
public class MySubManagerSetupData : SubManagerSetupData
{
    public float someValue;
    
    public override void ApplyToPawnData(PawnData pawnData)
    {
        // 데이터 모듈 생성
        if (pawnData.myData == null)
            pawnData.myData = new MyData();
        
        pawnData.myData.someValue = someValue;
    }
    
    public override MonoBehaviour AddSubManagerComponent(GameObject pawnObject)
    {
        return pawnObject.AddComponent<MySubManager>();
    }
}
```

### 4. JSON 레시피에 추가
```json
{
  "subManagerSetups": [
    {
      "$type": "PawnCore.Recipes.Json.MySubManagerSetupData",
      "someValue": 10.0
    }
  ]
}
```

---

## ⚠️ 주의사항

### 1. Null 안전성
- 모든 데이터 모듈은 nullable
- 사용 전 null 체크 필수
- 없으면 자동 생성 또는 무시

### 2. 이벤트 구독 해제
- `OnDisable()`에서 반드시 `Unsubscribe()` 호출
- 메모리 누수 방지

### 3. SubManager 추가/제거
- 런타임에 추가/제거 가능
- 추가 시: 즉시 작동
- 제거 시: 즉시 멈춤
- 에러 없이 자연스럽게 동작

---

## 🏗️ 클린 아키텍처 관점

서브매니저들은 **프레젠테이션 계층(Presentation Layer)** 에 해당합니다.

**역할:**
- Unity 엔진 세계 ↔ 도메인 계층 연결
- 입력 → 도메인 이벤트 변환
- Update 루프 → UseCase 호출
- **어댑터(Adapter) 패턴**

**의존성 방향:**
```
SubManager → PawnData (Domain)
SubManager → Usecases (Domain)
SubManager ↔ 이벤트 버스 (Domain)
```

Unity에 의존하지만, 도메인은 Unity를 모릅니다.

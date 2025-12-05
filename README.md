# Pawn Survivors

> Unity 2D 로그라이크 서바이벌 게임 | Clean Architecture 적용

**핵심 가치:** JSON 기반 데이터 주도 설계로 **코드 수정 없이 밸런스 조정** 가능  
**기술적 특징:** 이벤트 버스 + SubManager 패턴으로 **느슨한 결합**과 **높은 확장성** 달성  
**포트폴리오:** 디자인 패턴과 Clean Architecture를 실전 적용한 프로젝트

---

## 🎮 게임 개요

**장르:** 로그라이크 서바이벌 액션  
**플랫폼:** WebGL (브라우저에서 플레이 가능)

**주요 시스템:**
- 10명의 캐릭터 (각기 다른 공격 방식)
- 캠페인/스테이지 시스템
- 아이템 & 상점 시스템
- 레벨업 & 능력치 강화

---

## 🏗️ 핵심 아키텍처

<details>
<summary><b>Clean Architecture 3계층</b></summary>

```
Presentation (Unity 의존)
     ↓ (호출)
Domain (비즈니스 로직, Unity 독립)
     ↓ (호출)
Data (저장소, JSON)
```

**의존성 규칙:** 상위 계층만 하위 계층을 호출 (역방향 X)

**장점:**
- ✅ 테스트 용이 (Domain은 Unity 없이 테스트 가능)
- ✅ 유지보수성 향상 (계층별 역할 명확)
- ✅ 확장성 (새 기능 추가 시 다른 계층 영향 최소화)

</details>

<details>
<summary><b>SubManager 패턴</b></summary>

### 문제
전통적인 방식: 캐릭터마다 거대한 MonoBehaviour 클래스  
→ 코드 중복, 확장 어려움, 테스트 힘듦

### 해결
**SubManager로 기능 분할** (Component 패턴)

```
Pawn GameObject
  ├── AttackSubManager (공격)
  ├── HealthBarSubManager (체력바)
  ├── AudioSubManager (사운드)
  └── MovableSubManager (이동)
```

**각 SubManager = 단일 책임**

### 결과
- ✅ JSON으로 SubManager 조합 가능
- ✅ 새 캐릭터 = JSON 파일 복사 후 수정만
- ✅ 코드 재사용률 대폭 증가

</details>

<details>
<summary><b>이벤트 버스</b></summary>

### 문제
SubManager끼리 직접 참조 → 결합도 높음

```csharp
// 나쁜 예
DamageableSubManager가 직접:
  hitFlash.Flash();
  healthBar.Update();
  audioManager.PlayHit();
```

### 해결
**이벤트 발행/구독** (Observer 패턴)

```csharp
// 발행
Publish(new PawnDamagedEvent(target, damage));

// 구독 (각 SubManager가 독립적으로)
Subscribe<PawnDamagedEvent>(OnDamaged);
```

### 결과
- ✅ SubManager끼리 모름 (느슨한 결합)
- ✅ 새 SubManager 추가해도 기존 코드 수정 불필요
- ✅ 이벤트 우선순위 지원

</details>

---

## 💾 데이터 주도 설계

<details>
<summary><b>JSON 레시피 시스템</b></summary>

### 설계 결정
**모든 게임 오브젝트를 JSON으로 정의**

### 이유
- 밸런스 조정 시 코드 리컴파일 불필요
- 디자이너가 프로그래머 없이 작업 가능
- Git으로 변경 이력 추적 용이

### 예시
```json
{
  "pawnName": "EnemyBoss",
  "subManagerSetups": [
    {"$type": "DamageableSubManagerSetupData", "maxHealth": 500},
    {"$type": "AttackSubManagerSetupData", "fireRate": 1.5, "damage": 15}
  ]
}
```

**코드 수정 없이 체력 500 → 300 변경 가능**

### 지원 데이터
- 캐릭터 14종 (플레이어 10, 적 4)
- 스테이지 3개 (웨이브 시스템)
- 아이템 8종
- 캠페인 2개

</details>

<details>
<summary><b>Resources 폴더 사용</b></summary>

### 문제
`StreamingAssets` = WebGL에서 비동기 로딩 필요 (UnityWebRequest)

### 해결
**JSON도 Resources 폴더에 배치**

```csharp
// 동기 로딩 가능
var recipe = Resources.Load<TextAsset>("StreamingAssets/Recipes/Players/PlayerTig");
```

### 결과
- ✅ WebGL 빌드 안정성
- ✅ 동기 로딩 (코드 간결)
- ✅ 빌드 크기 자동 최적화

</details>

---

## 🛠️ 주요 기술 구현

<details>
<summary><b>1. 확장 가능한 공격 시스템</b></summary>

### 설계
공격 = **Method (방식)** × **Pattern (패턴)**

```
3가지 Method × 2가지 Pattern = 6가지 조합
```

**Method:**
- Projectile (투사체)
- Instant (즉시 타격)
- Area (범위 공격)

**Pattern:**
- Single (단발)
- Burst (연사)

### 확장성
새 공격 추가 = `IAttackMethod` 구현 후 JSON에서 사용

**기존 코드 수정 불필요**

</details>

<details>
<summary><b>2. 체력바 시스템</b></summary>

### 선택지
1. WorldSpace Canvas + Slider
2. SpriteRenderer + Scale 조절

### 결정: SpriteRenderer 선택

**이유:**
- Canvas = GameObject당 5~10개 컴포넌트 필요 (무거움)
- 적 수십 마리 → Canvas 수십 개 = 성능 문제
- 단순한 바 형태만 필요 (Slider의 복잡한 기능 불필요)

**구현:**
```csharp
// Background (회색) + Foreground (빨강)
foreground.localScale.x = healthPercent; // 0.0 ~ 1.0
```

**Trade-off:**
- ✅ 가벼움
- ⚠️ 복잡한 UI 불가 (단순 바만 가능)

</details>

<details>
<summary><b>3. 충돌 시스템 (Physics2D)</b></summary>

### 요구사항
- 적끼리 충돌 ✅
- 플레이어-적 충돌 ✅
- 플레이어끼리 겹침 허용 ✅
- 총알끼리 충돌 X ✅

### 해결
**Layer Collision Matrix + RigidbodyType 조합**

```
Player: Kinematic (isTrigger=false)
Enemy: Dynamic (isTrigger=false)
Bullet: Dynamic (별도 Layer)

Unity Physics 규칙:
- Dynamic ↔ Dynamic = 충돌 O
- Kinematic ↔ Kinematic = 충돌 X
```

**핵심:** `OnCollisionEnter2D` 사용 (OnTrigger 아님)

**Why:** Trigger로는 물리 충돌 불가능

</details>

<details>
<summary><b>4. 웨이브 기반 적 스폰</b></summary>

### 문제
고정된 스폰 = 지루함, 난이도 조절 어려움

### 해결
**시간대별 웨이브 + 가중치 랜덤**

```json
{
  "enemyWaves": [
    {
      "startTime": 0,
      "endTime": 30,
      "enemyTypes": [
        {"recipeName": "Enemy", "weight": 100},
        {"recipeName": "Elite", "weight": 30}
      ]
    }
  ]
}
```

**0~30초:** 일반 적 70%, 엘리트 30%  
**30초 이후:** 다음 웨이브 (보스 등)

### 장점
- ✅ 난이도 곡선 조절 쉬움
- ✅ 코드 수정 없이 밸런스 조정

</details>

<details>
<summary><b>5. 볼륨 시스템</b></summary>

### 구조
```
실제 볼륨 = 마스터 × 개별 볼륨

BGM 볼륨 = masterVolume × bgmVolume
SFX 볼륨 = masterVolume × sfxVolume
```

**Why 이 구조?**
- 마스터로 전체 조절 가능
- 개별로 BGM/SFX 비율 조절 가능

**저장:** PlayerPrefs (게임 종료 후에도 유지)

</details>

---

## 🎨 디자인 패턴 적용

| 패턴 | 적용 위치 | 목적 |
|------|-----------|------|
| **Strategy** | 이동/공격/레벨업 | 런타임에 동작 교체 |
| **Observer** | 이벤트 버스 | 느슨한 결합 |
| **Component** | SubManager | 기능 모듈화 |
| **Repository** | Data 계층 | 데이터 접근 추상화 |
| **Factory** | CreationManager | Pawn 생성 중앙화 |

---

## 📂 프로젝트 구조

```
script/
├── Clean Architecture/
│   ├── Domain/              # 비즈니스 로직 (Unity 독립)
│   │   ├── Usecases/        # Use Case (게임 규칙)
│   │   ├── Combat/          # 공격 시스템 (Strategy)
│   │   └── Events/          # 이벤트 정의
│   ├── Presentation/        # Unity 통신
│   │   └── SubManagers/     # 기능별 컴포넌트
│   └── Data/                # 저장소
├── Managers/                # GameManager, StageManager 등
└── UI/                      # 화면별 UI

Resources/StreamingAssets/
├── Recipes/                 # 캐릭터/적/아이템 JSON
├── Stages/                  # 스테이지 JSON
└── Campaigns/               # 캠페인 JSON
```

---

## 🔧 주요 설계 결정

<details>
<summary><b>왜 이렇게 만들었나</b></summary>

### 1. SubManager vs 거대한 클래스

**기존 방식:**
```csharp
class Player : MonoBehaviour
{
    void Attack() { ... }      // 500줄
    void Move() { ... }        // 300줄
    void TakeDamage() { ... }  // 200줄
    // 총 1000줄+
}
```

**SubManager 방식:**
```csharp
AttackSubManager : 100줄
MovableSubManager : 80줄
DamageableSubManager : 60줄
```

**Trade-off:**
- ✅ 관리 용이 (파일 작음)
- ⚠️ GameObject당 컴포넌트 많아짐 (성능 미미한 영향)

---

### 2. 이벤트 버스 vs 직접 호출

**직접 호출의 문제:**
```csharp
healthBar = GetComponent<HealthBar>();
healthBar.UpdateHealth(); // 결합됨!
```

**이벤트 버스:**
```csharp
Publish(event); // 누가 듣는지 모름
```

**Why 이벤트 버스?**
- 새 SubManager 추가 시 기존 코드 안 건드림
- 예: 피격 이펙트 추가 → Subscribe만 하면 끝

---

### 3. Canvas: 여러 개 vs 하나

**결정:** 화면당 Canvas 생성 (현재)

**Why:**
- 코드 생성 UI라서 부모 Canvas 찾기 복잡
- sortingOrder로 계층 관리

**개선 가능:** UIManager에 통합 Canvas (향후)

---

### 4. isTrigger vs Collision

**결정:** `isTrigger=false` + `OnCollisionEnter2D`

**Why:**
- 물리 충돌 필요 (적끼리 안 겹침)
- OnTrigger = 충돌 감지만, 밀어내기 불가
- OnCollision = 충돌 감지 + 물리 반응

**Unity 규칙:**
- isTrigger=true → 물리 반응 X
- isTrigger=false + Dynamic → 물리 반응 O

</details>

---

## 💡 기술적 하이라이트

<details>
<summary><b>1. 조합 가능한 공격 시스템</b></summary>

### 구현
```csharp
interface IAttackMethod { bool Execute(...); }
interface IAttackPattern { void Fire(...); }

AttackSubManager
{
    IAttackMethod method; // Projectile, Instant, Area
    IAttackPattern pattern; // Single, Burst
}
```

### 조합 예시
- `Projectile + Burst` = 3연발 총알
- `Area + Single` = 광역 폭발
- `Instant + Single` = 근접 타격

**새 조합 = JSON만 수정**

</details>

<details>
<summary><b>2. 전략 패턴으로 유연한 이동</b></summary>

### 구현
```csharp
MovableSubManager
{
    List<IMovementStrategy> strategies;
    
    void Update() {
        foreach (var strategy in enabledStrategies)
            strategy.Execute();
    }
}
```

**전략들:**
- KeyboardMovementStrategy (플레이어)
- HomingMovementStrategy (적)
- DirectionalMovementStrategy (총알)

**런타임에 Enable/Disable 가능**

### 활용 예시
코인: 처음엔 정지 → 플레이어 근처 오면 Homing 활성화

</details>

<details>
<summary><b>3. 이벤트 기반 사운드/비주얼</b></summary>

### 흐름
```
DamageableSubManager (데미지 처리)
  ↓ Publish
PawnDamagedEvent
  ↓ Subscribe
├─ HitFlashSubManager → 깜빡임
├─ HealthBarSubManager → 체력바 업데이트
└─ AudioSubManager → 피격음 재생
```

**핵심:** DamageableSubManager는 다른 SubManager를 모름

### 확장 예시
피격 시 이펙트 추가하고 싶다?
→ `ParticleSubManager` 만들고 Subscribe만 하면 끝

</details>

---

## 📊 완료된 기능

### 게임 시스템
- ✅ 캐릭터 선택 (10종)
- ✅ 캠페인 시스템 (여러 스테이지 묶음)
- ✅ 적 스폰 (웨이브 기반, 가중치 랜덤)
- ✅ 공격 시스템 (Method × Pattern = 6가지 조합)
- ✅ 레벨업 (4가지 조건: 킬/시간/데미지/이벤트)
- ✅ 아이템 (전역/장착, 스탯 보너스)
- ✅ 상점 (스테이지 클리어 후)

### 시각/사운드
- ✅ 체력바 (SpriteRenderer, 모든 Pawn)
- ✅ 피격/사망 사운드 (이벤트 기반)
- ✅ BGM (스테이지별 설정 가능)
- ✅ 볼륨 조절 (마스터/BGM/효과음)

### UI
- ✅ 타이틀, 캐릭터 선택, 캠페인 선택
- ✅ 일시정지, 옵션 (소리 설정)
- ✅ 게임오버, 스테이지 클리어
- ✅ 상점

**모든 UI 코드로 동적 생성 (프리팹 최소화)**

---

## 🎯 핵심 코드 예시

<details>
<summary><b>새 캐릭터 추가 (JSON만)</b></summary>

```json
{
  "pawnName": "NewCharacter",
  "subManagerSetups": [
    {
      "$type": "DamageableSubManagerSetupData",
      "maxHealth": 100
    },
    {
      "$type": "AttackSubManagerSetupData",
      "attackMethodType": "Area",
      "attackPatternType": "Burst",
      "fireRate": 2.0,
      "damage": 20,
      "attackRange": 5.0,
      "burstCount": 3
    },
    {
      "$type": "MovableSubManagerSetupData",
      "strategySetups": [
        {
          "$type": "KeyboardStrategySetupData",
          "moveSpeed": 5.0
        }
      ]
    },
    {
      "$type": "HealthBarSubManagerSetupData",
      "barWidth": 1.5
    }
  ]
}
```

**파일 하나 = 새 캐릭터 완성**

</details>

<details>
<summary><b>이벤트 구독 예시</b></summary>

```csharp
// AudioSubManager.cs
public override void SubStart()
{
    _pawnManager.Subscribe<PawnDamagedEvent>(OnDamaged);
    _pawnManager.Subscribe<PawnDeathEvent>(OnDeath);
}

private void OnDamaged(PawnDamagedEvent evt)
{
    if (evt.Target == _pawnManager)
    {
        AudioHelper.PlayRandomSound(hitSounds, transform.position);
    }
}
```

**장점:** AudioSubManager 추가/제거해도 DamageableSubManager는 모름

</details>

---

## 🔍 기술 스택

**엔진:** Unity 2022.3 LTS (2D)  
**언어:** C# 9.0+  
**아키텍처:** Clean Architecture, SOLID 원칙  
**패턴:** Strategy, Observer, Component, Repository, Factory

**주요 라이브러리:**
- Newtonsoft.Json (JSON 파싱)
- TextMeshPro (UI 텍스트)
- Unity Physics2D (충돌)

---

## 🚀 빌드

**WebGL:** `File > Build Settings > WebGL`  
**필수 설정:** Layer 설정 (Player, Enemy, Bullet, EnemyProjectile)

---

## 📈 향후 개선

**기술적:**
- [ ] Object Pooling (총알, 적)
- [ ] UI Canvas 통합
- [ ] 세이브/로드 시스템

**게임플레이:**
- [ ] 더 많은 캐릭터 & 스테이지
- [ ] 보스 패턴 AI
- [ ] 패시브 스킬 시스템

---

## 🧑‍💻 개발자 노트

이 프로젝트는 **"확장 가능한 게임 아키텍처"**를 목표로 만들어졌습니다.

**핵심 가치:**
- 데이터 주도 (JSON)
- 느슨한 결합 (이벤트 버스)
- 모듈화 (SubManager)

**결과:** 새 기능 추가가 빠르고, 기존 코드 수정이 적음

---

## 📜 라이선스

개인 포트폴리오 프로젝트

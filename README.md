# Pawn Survivors

**Unity 2D 로그라이크 서바이벌 게임**

Clean Architecture 패턴을 적용한 확장 가능한 게임 프로젝트입니다.

---

## 🎮 게임 소개

<details>
<summary><b>게임 플레이</b></summary>

### 장르
- 로그라이크 서바이벌 액션

### 핵심 시스템
- **캐릭터 선택**: 10명의 고유한 캐릭터 (각기 다른 공격 방식)
- **캠페인 시스템**: 여러 스테이지로 구성된 캠페인 선택
- **전투 시스템**: Projectile, Instant, Area 공격 + Burst 패턴
- **레벨업 시스템**: 조건 기반 레벨업 (킬 수, 시간, 데미지)
- **아이템 시스템**: 전역/장착 아이템, 스탯 보너스
- **상점 시스템**: 스테이지 클리어 후 아이템/캐릭터 구매

### 캐릭터 예시
- **티그**: 원거리 투사체 공격
- **우이**: 주변 범위 공격 (공격 범위 시각화)
- **에르핀, 버터, 오팔** 등 7명 추가

</details>

<details>
<summary><b>주요 기능</b></summary>

- ✅ JSON 기반 레시피 시스템 (데이터 주도)
- ✅ 동적 UI 생성 (코드 기반)
- ✅ 체력바 시스템 (SpriteRenderer)
- ✅ 사운드 시스템 (이벤트 기반)
- ✅ 볼륨 조절 (마스터/BGM/효과음)
- ✅ 물리 충돌 (적끼리, 플레이어-적)
- ✅ 카메라 시스템 (여러 플레이어 추적)
- ✅ 웹 빌드 지원

</details>

---

## 🏗️ 아키텍처

<details>
<summary><b>Clean Architecture 구조</b></summary>

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│  (SubManagers, UI, Input Handling)      │
├─────────────────────────────────────────┤
│           Domain Layer                  │
│  (Use Cases, Entities, Events)          │
├─────────────────────────────────────────┤
│            Data Layer                   │
│  (Repositories, DataSources, JSON)      │
└─────────────────────────────────────────┘
```

### 계층별 역할

**Presentation Layer**
- SubManager 패턴으로 기능 모듈화
- UI 동적 생성 (코드 기반)
- Unity 컴포넌트와 직접 통신

**Domain Layer**
- 비즈니스 로직 (공격, 레벨업, 아이템)
- Use Case 패턴
- 이벤트 버스 (느슨한 결합)

**Data Layer**
- JSON 기반 데이터 관리
- Repository 패턴
- WebGL 호환 (Resources 폴더)

</details>

<details>
<summary><b>SubManager 시스템</b></summary>

### 개념
각 Pawn은 여러 SubManager로 구성되며, 각 SubManager는 단일 책임을 가집니다.

### 주요 SubManager

**전투 관련**
- `AttackSubManager`: 공격 로직 통합 관리
- `CollisionDamageSubManager`: 충돌 데미지 처리
- `DamageableSubManager`: 피격 및 체력 관리

**이동 관련**
- `MovableSubManager`: 이동 전략 관리
- `SeparationSubManager`: 겹침 방지 (적끼리, 플레이어-적)

**시각 관련**
- `VisualSubManager`: 스프라이트 렌더링
- `HealthBarSubManager`: 체력바 표시 (SpriteRenderer)
- `HitFlashSubManager`: 피격 시 깜빡임

**오디오 관련**
- `AudioSubManager`: 피격/사망 사운드 (이벤트 구독)

**기타**
- `PhysicsSubManager`: Collider/Rigidbody 관리
- `LevelUpSubManager`: 레벨업 조건 및 보상
- `CoinPickupSubManager`: 코인 획득

### 장점
- ✅ 높은 재사용성 (JSON으로 조합)
- ✅ 단일 책임 원칙
- ✅ 쉬운 확장 (새 SubManager 추가)
- ✅ 독립적 테스트 가능

</details>

<details>
<summary><b>전략 패턴 (Strategy Pattern)</b></summary>

### 이동 전략
- `KeyboardMovementStrategy`: 키보드 입력
- `HomingMovementStrategy`: 타겟 추적
- `DirectionalMovementStrategy`: 직선 이동
- `TargetMovementStrategy`: 특정 위치로 이동

### 애니메이션 전략
- `BounceAnimationStrategy`: 통통 튀는 애니메이션
- `IdleAnimationStrategy`: 정적 표시

### 레벨업 전략
- `KillCountLevelUpStrategy`: 킬 수 기반
- `SurvivalTimeLevelUpStrategy`: 생존 시간 기반
- `DamageDealtLevelUpStrategy`: 데미지량 기반
- `EventTriggerLevelUpStrategy`: 특정 이벤트 기반

</details>

<details>
<summary><b>이벤트 버스 시스템</b></summary>

### 이벤트 종류
- `PawnDamagedEvent`: 피격 시
- `PawnDeathEvent`: 사망 시
- `PawnLevelUpEvent`: 레벨업 시
- `CurrencyChangedEvent`: 골드 변경 시

### 구독 예시
```csharp
// HitFlashSubManager
_pawnManager.Subscribe<PawnDamagedEvent>(OnDamaged);

void OnDamaged(PawnDamagedEvent evt)
{
    if (evt.Target == _pawnManager)
    {
        StartFlash(); // 깜빡임
    }
}
```

### 장점
- ✅ 느슨한 결합 (SubManager끼리 독립적)
- ✅ 확장 용이 (새 구독자 추가 쉬움)
- ✅ 이벤트 우선순위 지원

</details>

---

## 💾 데이터 주도 설계

<details>
<summary><b>JSON 레시피 시스템</b></summary>

### 구조
모든 Pawn은 JSON 파일로 정의됩니다.

```json
{
  "pawnName": "PlayerTig",
  "subManagerSetups": [
    {
      "$type": "AttackSubManagerSetupData",
      "attackMethodType": "Projectile",
      "fireRate": 2.0,
      "damage": 10
    },
    {
      "$type": "HealthBarSubManagerSetupData",
      "barWidth": 1.5,
      "healthColor": {"r": 0.2, "g": 0.8, "b": 0.2}
    }
  ]
}
```

### 장점
- ✅ 코드 수정 없이 밸런스 조정
- ✅ 새 캐릭터 추가 용이
- ✅ 디자이너 친화적
- ✅ 버전 관리 쉬움

### 지원 데이터
- **캐릭터**: Players/ (10개)
- **적**: Enemies/ (4종)
- **투사체**: Projectiles/ (Bullet, EnemyBullet, Coin)
- **스테이지**: Stages/ (DebugStage, Stage1, Stage2)
- **캠페인**: Campaigns/CampaignList.json
- **아이템**: Items/ (공격력, 체력, 속도 등)

</details>

<details>
<summary><b>스테이지 시스템</b></summary>

### 웨이브 기반 적 스폰

```json
{
  "stageName": "Stage1",
  "stageDuration": 60,
  "bgmName": "Audio/BGM",
  "enemyWaves": [
    {
      "startTime": 0,
      "endTime": 30,
      "spawnInterval": 2.5,
      "enemyTypes": [
        {"recipeName": "Enemy", "weight": 100},
        {"recipeName": "EnemyShooter", "weight": 50}
      ]
    }
  ]
}
```

### 기능
- ✅ 시간대별 웨이브 설정
- ✅ 가중치 기반 랜덤 스폰
- ✅ 보스 전용 (spawnOnce)
- ✅ 스테이지별 BGM

</details>

---

## 🛠️ 기술 스택

<details>
<summary><b>핵심 기술</b></summary>

### 엔진 & 언어
- **Unity 2022.3 LTS** (2D)
- **C# 9.0+**

### 아키텍처 패턴
- **Clean Architecture**: 계층 분리
- **Repository Pattern**: 데이터 추상화
- **Strategy Pattern**: 이동/애니메이션/레벨업
- **Observer Pattern**: 이벤트 버스
- **Component Pattern**: SubManager 시스템

### 라이브러리
- **Newtonsoft.Json**: JSON 파싱
- **TextMeshPro**: UI 텍스트 렌더링
- **Unity Input System**: 새로운 입력 시스템

### 물리
- **Physics2D**: 충돌 감지
- **Layer Collision Matrix**: 팀별 충돌 설정
- **Dynamic Rigidbody**: 적끼리 충돌 방지

</details>

<details>
<summary><b>주요 구현</b></summary>

### 1. 공격 시스템
```csharp
// 전략 패턴 적용
IAttackMethod (인터페이스)
  ├── ProjectileAttackMethod (투사체)
  ├── InstantAttackMethod (즉시 타격)
  └── AreaAttackMethod (범위 공격)

IAttackPattern (인터페이스)
  ├── SingleAttackPattern (단발)
  └── BurstAttackPattern (연사)
```

**장점**: 공격 방식을 조합 가능 (Projectile + Burst 등)

### 2. 이벤트 버스
```csharp
// 발행
_pawnManager.Publish(new PawnDamagedEvent(target, damage));

// 구독
_pawnManager.Subscribe<PawnDamagedEvent>(OnDamaged);
```

**장점**: SubManager 간 결합도 ↓, 확장성 ↑

### 3. 동적 UI 생성
```csharp
// 코드로 UI 생성 (프리팹 없이)
GameObject CreateButton(string text, Action callback)
{
    GameObject btn = new GameObject("Button");
    btn.AddComponent<Button>().onClick.AddListener(callback);
    // ...
}
```

**장점**: 런타임 UI 생성, 프리팹 관리 불필요

### 4. Resources 기반 로딩
```csharp
// WebGL 호환
PawnRecipe recipe = Resources.Load<PawnRecipe>("Recipes/Players/PlayerTig");
AudioClip sound = Resources.Load<AudioClip>("Audio/Hit0");
```

**장점**: StreamingAssets 대신 Resources 사용 (웹 빌드 안정)

</details>

<details>
<summary><b>성능 최적화</b></summary>

### 적용된 최적화
- **Object Pooling**: 총알, 코인 재사용 (구현 예정)
- **Separation 체크 빈도 제한**: 0.1초마다 (매 프레임 X)
- **Layer Collision Matrix**: 불필요한 충돌 제거
- **이벤트 우선순위**: 중요한 이벤트 먼저 처리

### 측정 가능한 개선
- 적 스폰: ~100 동시 처리 가능
- UI 생성: 즉시 로드 (프리팹 의존 X)

</details>

---

## 📂 프로젝트 구조

<details>
<summary><b>폴더 구조</b></summary>

```
Assets/
├── script/
│   ├── Clean Architecture/
│   │   ├── Domain/           # 비즈니스 로직
│   │   │   ├── Usecases/     # Use Case (게임 규칙)
│   │   │   ├── Combat/       # 공격 시스템
│   │   │   ├── Events/       # 이벤트 정의
│   │   │   └── Utilities/    # Domain Helper
│   │   ├── Presentation/     # Unity 통신
│   │   │   └── SubManagers/  # 기능별 컴포넌트
│   │   │       ├── Combat/
│   │   │       ├── Movement/
│   │   │       ├── Visual/
│   │   │       ├── Physics/
│   │   │       └── Audio/
│   │   └── Data/             # 데이터 구조
│   │       └── Repositories/ # 데이터 접근 추상화
│   ├── Managers/             # 게임 관리자
│   ├── UI/                   # UI 시스템
│   │   └── Screens/          # 화면별 UI
│   └── PawnCore/             # Pawn 핵심 시스템
│       └── Recipes/          # JSON 파싱
│
├── Resources/
│   ├── Audio/                # 사운드 파일
│   ├── Sprites/              # 스프라이트
│   └── StreamingAssets/
│       ├── Recipes/          # 캐릭터/적/아이템 JSON
│       ├── Stages/           # 스테이지 JSON
│       └── Campaigns/        # 캠페인 JSON
│
└── Prefabs/
    └── UI/                   # UI 프리팹 (레거시)
```

</details>

<details>
<summary><b>핵심 클래스</b></summary>

### Managers
- `GameManager`: 게임 전체 관리 (싱글톤)
- `UIManager`: UI 화면 전환
- `StageManager`: 스테이지 흐름, 적 스폰
- `CreationManager`: Pawn 생성 (레시피 → GameObject)

### Domain Use Cases
- `CombatUsecases`: 공격 로직
- `ItemManagementUseCase`: 아이템 관리
- `StageFlowUseCase`: 스테이지 진행
- `CharacterSelectionUseCase`: 캐릭터 선택

### Presentation SubManagers
- `AttackSubManager`: 공격 (Method + Pattern 조합)
- `HealthBarSubManager`: 체력바 (이벤트 기반)
- `AudioSubManager`: 사운드 (이벤트 기반)
- `MovableSubManager`: 이동 (전략 패턴)

</details>

---

## 🎨 기술적 특징

<details>
<summary><b>1. 확장 가능한 공격 시스템</b></summary>

### 구현
공격을 **Method (방식)** + **Pattern (패턴)**으로 분리

```csharp
AttackSubManager
  ├── IAttackMethod (어떻게 공격?)
  │   ├── Projectile (투사체)
  │   ├── Instant (즉시 타격)
  │   └── Area (범위 공격)
  └── IAttackPattern (어떤 패턴?)
      ├── Single (단발)
      └── Burst (연사)
```

### 조합 예시
- Projectile + Single = 일반 총알
- Projectile + Burst = 3연발 총알
- Area + Single = 광역 폭발

### 확장성
새 공격 방식 추가 시 `IAttackMethod` 구현만 하면 됨!

</details>

<details>
<summary><b>2. 이벤트 기반 오디오/비주얼</b></summary>

### 문제
기존: DamageableSubManager가 HitFlash, HealthBar, Audio를 직접 호출
→ 결합도 높음, 확장 어려움

### 해결
이벤트 버스 도입

```csharp
// DamageableSubManager (발행)
_pawnManager.Publish(new PawnDamagedEvent(this, damage));

// HitFlashSubManager (구독)
_pawnManager.Subscribe<PawnDamagedEvent>(OnDamaged);

// HealthBarSubManager (구독)
_pawnManager.Subscribe<PawnDamagedEvent>(OnDamaged);

// AudioSubManager (구독)
_pawnManager.Subscribe<PawnDamagedEvent>(OnDamaged);
```

### 장점
- ✅ DamageableSubManager는 다른 SubManager를 모름
- ✅ 새 SubManager 추가해도 기존 코드 수정 불필요
- ✅ 이벤트 우선순위 지원

</details>

<details>
<summary><b>3. JSON 기반 데이터 주도</b></summary>

### 특징
모든 게임 오브젝트를 JSON으로 정의

### 예시: 보스 적
```json
{
  "pawnName": "EnemyBoss",
  "subManagerSetups": [
    {"$type": "CollisionDamageSubManagerSetupData", "damage": 15},
    {"$type": "DamageableSubManagerSetupData", "maxHealth": 500},
    {"$type": "ProjectileShooterSubManagerSetupData", "fireRate": 1.5},
    {"$type": "HealthBarSubManagerSetupData", "barWidth": 3.0},
    {"$type": "AudioSubManagerSetupData", "volume": 1.2}
  ]
}
```

### 장점
- ✅ 프로그래머 없이 밸런스 조정
- ✅ 새 캐릭터 추가 = JSON 파일 복사
- ✅ Git으로 변경 이력 관리
- ✅ A/B 테스트 용이

</details>

<details>
<summary><b>4. 체력바 시스템 (SpriteRenderer)</b></summary>

### 구현 방식
WorldSpace Canvas 대신 **SpriteRenderer** 사용

```csharp
// Background (회색 바)
GameObject background;
SpriteRenderer bgRenderer;

// Foreground (빨간 바)
GameObject foreground;
foreground.transform.localScale = new Vector3(healthPercent, 1, 1);
```

### 장점
- ✅ Canvas 오버헤드 없음
- ✅ 가벼움 (수백 개 체력바 처리 가능)
- ✅ 픽셀 퍼펙트 유지
- ✅ 이벤트 기반 업데이트 (매 프레임 X)

</details>

<details>
<summary><b>5. 충돌 시스템 (Physics2D)</b></summary>

### 요구사항
- 적끼리 안 겹침
- 플레이어-적 안 겹침
- 플레이어끼리 겹침 OK
- 총알끼리 충돌 안 함

### 해결
**Layer Collision Matrix + RigidbodyType**

```
Layer 설정:
- Player: Kinematic (isTrigger=false)
- Enemy: Dynamic (isTrigger=false)
- Bullet: Dynamic (isTrigger=false)
- EnemyProjectile: Dynamic (isTrigger=false)

Collision Matrix:
- Player-Player: ❌
- Enemy-Enemy: ✅
- Player-Enemy: ✅
- Bullet-Bullet: ❌
- Bullet-EnemyProjectile: ❌
```

### 기술적 포인트
- Kinematic ↔ Kinematic = 충돌 안 됨
- Dynamic ↔ Dynamic = 충돌 됨
- `OnCollisionEnter2D` 사용 (OnTriggerEnter2D 아님)

</details>

<details>
<summary><b>6. 볼륨 조절 시스템</b></summary>

### 3단계 볼륨
```csharp
GameAudioSettings
  ├── masterVolume (전체)
  ├── bgmVolume (배경음악)
  └── sfxVolume (효과음)

실제 볼륨 = masterVolume * bgmVolume (또는 sfxVolume)
```

### 저장/로드
- PlayerPrefs 사용
- 게임 종료 후에도 유지

### UI
- 옵션 → 소리 설정
- 3개 슬라이더 (실시간 조절)

</details>

---

## 🎯 디자인 결정

<details>
<summary><b>주요 설계 결정 사항</b></summary>

### 1. SubManager vs MonoBehaviour
**결정**: SubManager 패턴 채택

**이유**:
- 기능별 모듈화 (단일 책임)
- JSON으로 조합 가능
- 코드 재사용성 높음

### 2. Trigger vs Collision
**결정**: `isTrigger=false` + `OnCollisionEnter2D`

**이유**:
- 물리 충돌 필요 (적끼리 안 겹침)
- OnCollision이 더 유연함

### 3. Canvas 구조
**결정**: UIManager에 통합 Canvas 1개

**이유**:
- 여러 Canvas = 드로우콜 증가
- EventSystem 중복 방지
- 계층 관리 용이

### 4. 체력바: WorldSpace Canvas vs SpriteRenderer
**결정**: SpriteRenderer

**이유**:
- Canvas 오버헤드 큼
- 적이 많으면 성능 문제
- 간단한 바 형태만 필요

### 5. 오디오: 전역 Manager vs SubManager
**결정**: SubManager (각 Pawn에 붙음)

**이유**:
- 기존 패턴과 일관성
- 이벤트 버스 활용
- JSON에서 선택적 추가

</details>

---

## 🚀 빌드 & 실행

<details>
<summary><b>빌드 방법</b></summary>

### WebGL 빌드
```
1. File > Build Settings
2. Platform: WebGL 선택
3. Build
```

### 로컬 실행
Unity 에디터에서 Play 버튼

### 필수 설정
- Unity 2022.3 LTS 이상
- TextMeshPro 임포트
- Layer 설정:
  - Player (Layer 6)
  - Enemy (Layer 7)
  - Bullet (Layer 8)
  - EnemyProjectile (Layer 9)

</details>

---

## 📊 개발 현황

<details>
<summary><b>완료된 기능</b></summary>

### 핵심 시스템
- ✅ Clean Architecture 구조
- ✅ SubManager 시스템
- ✅ 이벤트 버스
- ✅ JSON 레시피 시스템

### 게임 기능
- ✅ 캐릭터 선택 (10명)
- ✅ 캠페인 시스템
- ✅ 공격 시스템 (3가지 Method, 2가지 Pattern)
- ✅ 적 스폰 (웨이브 기반)
- ✅ 레벨업 시스템 (4가지 전략)
- ✅ 아이템 시스템 (전역/장착)
- ✅ 상점 시스템

### 시각/사운드
- ✅ 체력바 (모든 Pawn)
- ✅ 피격/사망 사운드
- ✅ BGM (스테이지별)
- ✅ 볼륨 조절

### UI
- ✅ 타이틀 화면
- ✅ 캐릭터 선택
- ✅ 캠페인 선택
- ✅ 일시정지 메뉴
- ✅ 옵션 (소리 설정)
- ✅ 상점 화면
- ✅ 게임오버/클리어

</details>

<details>
<summary><b>향후 개선 사항</b></summary>

### 성능
- [ ] Object Pooling (총알, 적)
- [ ] UI Canvas 통합 (완전히)

### 게임플레이
- [ ] 더 많은 캐릭터
- [ ] 더 많은 스테이지
- [ ] 보스 패턴
- [ ] 패시브 스킬

### 기술
- [ ] 세이브/로드 시스템
- [ ] 업적 시스템
- [ ] 멀티플레이어 (선택)

</details>

---

## 📝 코드 예시

<details>
<summary><b>새 캐릭터 추가 (JSON만)</b></summary>

```json
{
  "pawnName": "NewCharacter",
  "subManagerSetups": [
    {
      "$type": "PawnSurvivors.Data.Recipes.DamageableSubManagerSetupData, Assembly-CSharp",
      "maxHealth": 100
    },
    {
      "$type": "PawnSurvivors.Data.Recipes.AttackSubManagerSetupData, Assembly-CSharp",
      "attackMethodType": "Projectile",
      "attackPatternType": "Burst",
      "projectileRecipeName": "Bullet",
      "fireRate": 3.0,
      "damage": 15,
      "burstCount": 3,
      "burstDelay": 0.1
    },
    {
      "$type": "PawnSurvivors.Data.Recipes.MovableSubManagerSetupData, Assembly-CSharp",
      "strategySetups": [
        {
          "$type": "PawnSurvivors.Data.Recipes.KeyboardStrategySetupData, Assembly-CSharp",
          "isEnabledByDefault": true,
          "moveSpeed": 5.0
        }
      ]
    },
    {
      "$type": "PawnSurvivors.Data.Recipes.HealthBarSubManagerSetupData, Assembly-CSharp",
      "barWidth": 1.5
    },
    {
      "$type": "PawnSurvivors.Data.Recipes.AudioSubManagerSetupData, Assembly-CSharp",
      "hitSounds": ["Audio/Hit0", "Audio/Hit1"],
      "volume": 1.0
    }
  ]
}
```

**코드 수정 없이 새 캐릭터 완성!**

</details>

---

## 🧑‍💻 개발자

**포트폴리오 프로젝트**

Clean Architecture와 디자인 패턴을 실전에 적용한 Unity 2D 게임 프로젝트입니다.

---

## 📜 라이선스

개인 포트폴리오 프로젝트

---

## 🔗 참고

- Clean Architecture (Robert C. Martin)
- Game Programming Patterns (Robert Nystrom)
- Unity 공식 문서

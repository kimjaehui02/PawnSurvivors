# PlayerController 시스템

**여러 플레이어 유닛을 하나의 대열로 관리하는 시스템**

---

## 🎯 핵심 아이디어

### **기존 방식**
```
Player Pawn (직접 입력 받음)
```
- 한 명의 플레이어만 가능
- 여러 유닛 관리 복잡

### **새로운 방식**
```
PlayerController (입력 받는 중심)
├── MainCamera (자식)
├── Player1 (자식, 대열 위치)
├── Player2 (자식, 대열 위치)
└── Player3 (자식, 대열 위치)
```
- PlayerController가 움직이면 **모든 자식이 자동으로 따라옴!**
- 복잡한 추적 로직 불필요
- 대열 관리 = `localPosition` 설정

---

## 🏗️ 구조

### **PlayerController**
- **역할**: 키보드 입력 받아서 이동
- **위치**: 화면 중앙 (보이지 않음)
- **이동**: WASD로 직접 이동

### **Player Pawn들**
- **역할**: 전투 (공격, 방어, 스킬)
- **위치**: PlayerController의 **자식**
- **이동**: 부모를 따라서 자동 이동 (입력 안 받음!)

### **Camera**
- **역할**: 게임 화면 표시
- **위치**: PlayerController의 **자식**
- **이동**: 부모를 따라서 자동 이동

---

## 📝 대열 시스템

### **formationPositions 배열**

```csharp
public Vector3[] formationPositions = new Vector3[]
{
    new Vector3(0f, 0f, 0f),    // 위치 0: 중앙
    new Vector3(1f, -0.5f, 0f), // 위치 1: 오른쪽 뒤
    new Vector3(-1f, -0.5f, 0f) // 위치 2: 왼쪽 뒤
};
```

### **대열 예시**

```
      [0]        ← 첫 번째 플레이어 (중앙)
     /   \
  [1]     [2]   ← 두 번째, 세 번째 플레이어
```

### **대열 수정 방법**

Unity Inspector에서 `PlayerController` 컴포넌트의 `Formation Positions` 수정:

```
Element 0: (0, 0, 0)      // 중앙
Element 1: (1, -1, 0)     // 오른쪽 뒤
Element 2: (-1, -1, 0)    // 왼쪽 뒤
Element 3: (2, -1.5, 0)   // 더 오른쪽
Element 4: (-2, -1.5, 0)  // 더 왼쪽
```

---

## 🚀 사용 방법

### **1. 게임 시작 시 (자동)**

```csharp
// GameManager.StartStage()에서 자동 실행됨
PlayerController = new GameObject("PlayerController").AddComponent<PlayerController>();

// 카메라 붙이기
Camera.main.transform.SetParent(PlayerController.transform);

// 플레이어 폰 생성 및 추가
GameObject pawn = CreationManager.CreatePawn(playerRecipe, Vector3.zero);
PlayerController.AddPlayerPawn(pawn);
```

### **2. 런타임에 플레이어 추가**

```csharp
// 새 플레이어 유닛 획득 시
GameObject newPawn = CreationManager.CreatePawn(playerRecipe, Vector3.zero);
GameManager.Instance.PlayerController.AddPlayerPawn(newPawn);

// → 자동으로 대열에 배치됨!
```

### **3. 플레이어 제거**

```csharp
// 플레이어 유닛 제거 (사망 등)
GameManager.Instance.PlayerController.RemovePlayerPawn(pawn);

// 모든 플레이어 제거 (게임 오버 등)
GameManager.Instance.PlayerController.ClearAllPawns();
```

### **4. 대열 재정렬**

```csharp
// 플레이어 추가/제거 후 대열 재정렬
GameManager.Instance.PlayerController.RearrangeFormation();
```

---

## 🎮 Player Pawn 설정

### **중요: 이동 입력 제거!**

`Player.json`에서 `MovableSubManagerSetupData` **제거**:

```json
{
  "pawnName": "Player",
  "subManagerSetups": [
    // ❌ 제거: MovableSubManagerSetupData
    // PlayerController가 이동을 담당하므로 불필요
    
    // ✅ 유지: 전투, 시각 효과 등
    { "$type": "DamageableSubManagerSetupData" },
    { "$type": "ProjectileShooterSubManagerSetupData" },
    { "$type": "VisualSubManagerSetupData" },
    // ...
  ]
}
```

### **Player Pawn의 역할**

- ✅ **전투**: 공격, 방어, 체력
- ✅ **시각 효과**: 스프라이트, 애니메이션
- ✅ **물리**: 충돌 감지
- ❌ **이동**: PlayerController가 담당!

---

## ⚙️ 확장 방법

### **1. 플레이어 타입 추가**

```csharp
// 전사, 마법사, 궁수 등 다양한 Recipe 생성
GameObject warrior = CreationManager.CreatePawn("Warrior", Vector3.zero);
GameObject mage = CreationManager.CreatePawn("Mage", Vector3.zero);
GameObject archer = CreationManager.CreatePawn("Archer", Vector3.zero);

// 모두 같은 대열에 추가
PlayerController.AddPlayerPawn(warrior);
PlayerController.AddPlayerPawn(mage);
PlayerController.AddPlayerPawn(archer);
```

### **2. 동적 대열 변경**

```csharp
// 전투 중 대열 변경
PlayerController.formationPositions = new Vector3[]
{
    new Vector3(0, 0),    // 밀집 대형
    new Vector3(0.5f, -0.5f),
    new Vector3(-0.5f, -0.5f)
};
PlayerController.RearrangeFormation();
```

### **3. 플레이어 전환 (단일 플레이어 모드)**

```csharp
// 플레이어 1명만 표시
foreach (var pawn in PlayerController.playerPawns)
{
    pawn.SetActive(false); // 숨김
}
PlayerController.playerPawns[0].SetActive(true); // 첫 번째만 활성화
```

---

## 🎨 시각적 확인

### **Scene View에서**

PlayerController를 선택하면:
- **파란색 원**: 대열 위치 표시 (Gizmos)
- **PlayerController 위치**: 화면 중앙
- **자식 오브젝트**: Player Pawn들과 Camera

### **Game View에서**

- PlayerController는 보이지 않음
- Player Pawn들만 보임
- WASD로 이동 시 모두 함께 움직임

---

## 🔄 생명주기

```
1. 게임 시작 (GameManager.StartStage)
   ↓
2. PlayerController 생성
   ↓
3. Camera 자식으로 추가
   ↓
4. Player Pawn 생성 → 자식으로 추가
   ↓
5. 게임 중 (Update)
   - PlayerController가 입력 받아 이동
   - 자식들이 자동으로 따라옴
   ↓
6. 게임 종료
   - PlayerController 파괴
   - 자식들도 함께 파괴됨
```

---

## 📌 주의사항

### **✅ 좋은 패턴**

```csharp
// PlayerController를 통해 관리
GameManager.Instance.PlayerController.AddPlayerPawn(pawn);
```

### **❌ 나쁜 패턴**

```csharp
// Player Pawn에 직접 이동 로직 추가 ❌
// PlayerController와 충돌!
```

---

## 🚀 향후 확장 가능성

1. **다중 플레이어 파티**
   - 여러 종류의 유닛
   - 동적 대열 변경

2. **플레이어 전환 시스템**
   - 1번, 2번, 3번 키로 플레이어 전환
   - 또는 전체 파티 제어

3. **AI 동료**
   - PlayerController에 붙이면 플레이어처럼 행동
   - 별도 로직 불필요

4. **대열 전술**
   - 공격 대형, 방어 대형
   - 상황에 따라 자동 변경

---

## 🎯 핵심 장점

| 장점 | 설명 |
|-----|------|
| **✅ 간단함** | Unity 계층 구조만 사용 |
| **✅ 확장성** | 플레이어 수 제한 없음 |
| **✅ 성능** | 복잡한 추적 로직 불필요 |
| **✅ 직관적** | 부모-자식 관계로 명확 |

---

**이제 여러 플레이어 유닛을 하나의 대열로 관리할 수 있습니다!** 🎉


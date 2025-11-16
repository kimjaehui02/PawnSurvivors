# 여러 캐릭터 운용 가이드

**PlayerController로 여러 플레이어를 대열로 관리하는 방법**

---

## 🎯 빠른 시작

### **1단계: 대열 위치 설정 (Inspector)**

게임 실행 후:
1. **Hierarchy** → `PlayerController` 선택
2. **Inspector** → `Formation Positions` 펼치기
3. **Size: 5** (원하는 플레이어 수)
4. 각 Element에 좌표 입력

```
Element 0: (0, 0, 0)      ← 중앙
Element 1: (1.5, 0, 0)    ← 오른쪽
Element 2: (-1.5, 0, 0)   ← 왼쪽
Element 3: (1.5, -1.5, 0) ← 오른쪽 뒤
Element 4: (-1.5, -1.5, 0)← 왼쪽 뒤
```

### **2단계: 플레이어 추가 (코드)**

```csharp
// 단일 추가
GameManager.Instance.AddPlayerPawn("Player");
GameManager.Instance.AddPlayerPawn("Warrior");

// 또는 배열로 한 번에
string[] party = { "Player", "Warrior", "Mage" };
GameManager.Instance.AddMultiplePlayerPawns(party);
```

---

## 📐 좌표 시스템

### **로컬 좌표 (localPosition)**

PlayerController 기준 상대 좌표:

```
Y (위)
↑
│    [0]         (0, 1, 0)
│   /   \
│ [1]   [2]     (-1, 0, 0) and (1, 0, 0)
│
└──────────→ X (오른쪽)
```

### **월드 좌표 vs 로컬 좌표**

```csharp
// 로컬 좌표 (대열 위치)
pawn.transform.localPosition = new Vector3(1, 0, 0);

// 월드 좌표 (실제 화면 위치)
// → PlayerController가 (10, 5)에 있으면
// → pawn의 worldPosition은 (11, 5)
```

---

## 🎮 대형 프리셋

### **공격 대형 (V자)**

```csharp
formationPositions = new Vector3[]
{
    new Vector3(0f, 1f, 0f),    // 선봉
    new Vector3(-1f, 0f, 0f),   // 좌측
    new Vector3(1f, 0f, 0f),    // 우측
    new Vector3(-2f, -1f, 0f),  // 좌후방
    new Vector3(2f, -1f, 0f)    // 우후방
};
```

```
      [0]
     /   \
  [1]     [2]
  /         \
[3]         [4]
```

### **방어 대형 (횡대)**

```csharp
formationPositions = new Vector3[]
{
    new Vector3(-2f, 0f, 0f),
    new Vector3(-1f, 0f, 0f),
    new Vector3(0f, 0f, 0f),
    new Vector3(1f, 0f, 0f),
    new Vector3(2f, 0f, 0f)
};
```

```
[0] [1] [2] [3] [4]
```

### **밀집 대형**

```csharp
formationPositions = new Vector3[]
{
    new Vector3(0f, 0f, 0f),      // 중앙
    new Vector3(0.8f, 0f, 0f),    // 바로 옆
    new Vector3(-0.8f, 0f, 0f),
    new Vector3(0.4f, -0.8f, 0f), // 뒤쪽
    new Vector3(-0.4f, -0.8f, 0f)
};
```

```
  [1] [0] [2]
    [3] [4]
```

### **원형 대형**

```csharp
int count = 5;
float radius = 2f;
formationPositions = new Vector3[count];

for (int i = 0; i < count; i++)
{
    float angle = (360f / count) * i * Mathf.Deg2Rad;
    formationPositions[i] = new Vector3(
        Mathf.Cos(angle) * radius,
        Mathf.Sin(angle) * radius,
        0f
    );
}
```

```
    [0]
[1]    [4]
    [2]
  [3]
```

---

## 💻 코드 레시피

### **레벨업 시 플레이어 추가**

```csharp
void OnLevelUp(int level)
{
    // 레벨 3, 5, 7마다 새 유닛
    if (level % 2 == 1 && level >= 3)
    {
        string[] units = { "Warrior", "Mage", "Archer" };
        int index = (level / 2) % units.Length;
        
        GameManager.Instance.AddPlayerPawn(units[index]);
    }
}
```

### **상점에서 유닛 구매**

```csharp
void BuyUnit(string unitType, int cost)
{
    var session = GameManager.Instance.SessionData;
    
    if (session.GetInt("gold") >= cost)
    {
        session.AddInt("gold", -cost);
        GameManager.Instance.AddPlayerPawn(unitType);
        
        Debug.Log($"{unitType} 구매 완료!");
    }
}
```

### **유닛 사망 시 제거**

```csharp
// PawnDeathEvent 핸들러
void OnPlayerPawnDeath(GameObject pawn)
{
    var controller = GameManager.Instance.PlayerController;
    
    controller.RemovePlayerPawn(pawn);
    controller.RearrangeFormation(); // 대열 재정렬
    
    Debug.Log($"플레이어 폰 사망. 남은 수: {controller.playerPawns.Count}");
}
```

### **키 입력으로 대형 전환**

```csharp
void Update()
{
    var controller = GameManager.Instance.PlayerController;
    
    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
        // 1번: 공격 대형
        SetAttackFormation(controller);
    }
    if (Input.GetKeyDown(KeyCode.Alpha2))
    {
        // 2번: 방어 대형
        SetDefenseFormation(controller);
    }
}
```

---

## 🔧 고급 기능

### **플레이어별 역할 구분**

```csharp
// 탱커는 앞, 딜러는 중간, 힐러는 뒤
void ArrangeByRole()
{
    var controller = GameManager.Instance.PlayerController;
    var pawns = controller.playerPawns;
    
    // 역할별 정렬
    pawns.Sort((a, b) => {
        string roleA = GetRole(a);
        string roleB = GetRole(b);
        return CompareRoles(roleA, roleB);
    });
    
    // 재배치
    for (int i = 0; i < pawns.Count; i++)
    {
        pawns[i].transform.localPosition = controller.formationPositions[i];
    }
}
```

### **동적 간격 조정**

```csharp
// 플레이어 수에 따라 간격 자동 조정
float GetSpacing(int playerCount)
{
    if (playerCount <= 2) return 1.5f;
    if (playerCount <= 4) return 1.2f;
    return 1.0f;
}
```

### **애니메이션으로 대형 전환**

```csharp
IEnumerator SmoothFormationChange(Vector3[] newFormation)
{
    var controller = GameManager.Instance.PlayerController;
    float duration = 0.5f;
    float elapsed = 0f;
    
    Vector3[] oldPositions = new Vector3[controller.playerPawns.Count];
    for (int i = 0; i < oldPositions.Length; i++)
    {
        oldPositions[i] = controller.playerPawns[i].transform.localPosition;
    }
    
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;
        
        for (int i = 0; i < controller.playerPawns.Count; i++)
        {
            controller.playerPawns[i].transform.localPosition = 
                Vector3.Lerp(oldPositions[i], newFormation[i], t);
        }
        
        yield return null;
    }
}
```

---

## 📊 디버깅 팁

### **Scene View에서 확인**

- **파란색 원**: 대열 위치 (Gizmos)
- PlayerController 선택 시 표시됨
- Game View와 Scene View 비교

### **콘솔 로그 확인**

```csharp
// PlayerController.AddPlayerPawn()에서 자동 로그:
"[PlayerController] Pawn 추가됨: Player at localPosition (0, 0, 0)"
```

### **Inspector에서 실시간 확인**

```
PlayerController
├── Player Pawns (List)
│   ├── Element 0: Player
│   ├── Element 1: Warrior
│   └── Element 2: Mage
└── Formation Positions
    ├── [0] = (0, 0, 0)
    ├── [1] = (1, 0, 0)
    └── [2] = (-1, 0, 0)
```

---

## ⚠️ 주의사항

### **대열 위치 개수 부족**

```csharp
// 플레이어 5명인데 대열 위치가 3개만 있으면?
// → 4, 5번째 플레이어는 (0, 0, 0)에 배치됨 + 경고 로그
```

**해결**: Inspector에서 `Formation Positions` Size 증가

### **좌표 범위**

```csharp
// ✅ 적당한 간격
formationPositions[0] = new Vector3(1.5f, 0, 0);

// ❌ 너무 멀면 화면 밖으로
formationPositions[0] = new Vector3(100f, 0, 0);

// ❌ 너무 가까우면 겹침
formationPositions[0] = new Vector3(0.1f, 0, 0);
```

**권장**: 1.0f ~ 2.0f 간격

---

## 🚀 실전 예시

### **3인 파티 설정**

```csharp
// StartStage 수정
public void StartStage(string stageName)
{
    // ... PlayerController 생성 ...
    
    // 3인 파티 추가
    string[] party = { "Warrior", "Mage", "Archer" };
    AddMultiplePlayerPawns(party);
    
    // V자 대형 설정
    PlayerController.formationPositions = new Vector3[]
    {
        new Vector3(0f, 0.5f, 0f),   // 전사 (앞)
        new Vector3(-1f, -0.5f, 0f), // 마법사 (왼쪽 뒤)
        new Vector3(1f, -0.5f, 0f)   // 궁수 (오른쪽 뒤)
    };
    PlayerController.RearrangeFormation();
}
```

---

## 📌 요약

| 작업 | 방법 |
|-----|------|
| **대열 설정** | Inspector → Formation Positions |
| **플레이어 추가** | `GameManager.Instance.AddPlayerPawn()` |
| **대형 변경** | `formationPositions` 수정 + `RearrangeFormation()` |
| **좌표 확인** | Scene View의 파란색 Gizmos |
| **수동 배치** | `pawn.transform.localPosition` 직접 설정 |

---

**이제 여러 캐릭터를 자유롭게 운용할 수 있습니다!** 🎉


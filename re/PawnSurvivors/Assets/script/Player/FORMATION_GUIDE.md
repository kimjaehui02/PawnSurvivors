# 정다각형 자동 배치 시스템

**캐릭터 수에 따라 자동으로 정다각형으로 배치하는 시스템**

---

## 🔷 **정다각형 배치 (Polygon)**

### **캐릭터 수에 따른 자동 배치**

```
1명: 중앙 (●)

2명: 선분 (●―●)

3명: 삼각형 (△)
      ●
     / \
    ●   ●

4명: 사각형 (□)
    ●   ●
    
    ●   ●

5명: 오각형 (⬠)
      ●
     /  \
    ●    ●
     \  /
      ●●

6명: 육각형 (⬡)
     ●   ●
    
   ●     ●
    
     ●   ●
```

---

## ⚙️ **설정 방법**

### **Inspector에서 설정** (권장)

게임 실행 후:
1. **Hierarchy** → `PlayerController` 선택
2. **Inspector** → 설정 변경

```
Use Automatic Formation: ✓ (체크)
Formation Type: Polygon
Polygon Radius: 1.5
```

---

## 🎮 **대열 타입**

### **1. Polygon (정다각형)** ⭐ 권장
- 1명: 중앙
- 2명: 선분 (가로)
- 3명: 삼각형 (위쪽 꼭짓점부터 시계방향)
- 4명: 사각형
- 5명: 오각형
- 6명: 육각형

**특징**: 대칭적이고 균형잡힌 배치

### **2. VShape (V자)**
```
      [0]
     /   \
  [1]     [2]
  /         \
[3]         [4]
```
**특징**: 전방 집중 공격

### **3. TwoRows (2열)**
```
[0] [1] [2]

[3] [4] [5]
```
**특징**: 전후방 분산

### **4. Horizontal (횡대)**
```
[0] [1] [2] [3] [4] [5]
```
**특징**: 좌우로 넓게 분산

### **5. Circle (원형)**
```
    [0]
[1]    [5]
[2]    [4]
    [3]
```
**특징**: Polygon과 유사하지만 반지름이 더 큼

---

## 💻 **코드로 사용하기**

### **기본 사용 (자동)**

```csharp
// 플레이어 추가 - 자동으로 정다각형 배치!
GameManager.Instance.AddPlayerPawn("Player");
GameManager.Instance.AddPlayerPawn("Warrior");
GameManager.Instance.AddPlayerPawn("Mage");
// → 3명이므로 삼각형으로 자동 배치됨!
```

### **대열 타입 변경**

```csharp
var controller = GameManager.Instance.PlayerController;

// Polygon (정다각형)
controller.formationType = FormationType.Polygon;
controller.polygonRadius = 1.5f;
controller.RearrangeFormation();

// V자 대형
controller.formationType = FormationType.VShape;
controller.RearrangeFormation();

// 횡대
controller.formationType = FormationType.Horizontal;
controller.RearrangeFormation();
```

### **반지름 조정**

```csharp
var controller = GameManager.Instance.PlayerController;

// 작은 반지름 (밀집)
controller.polygonRadius = 1.0f;
controller.RearrangeFormation();

// 큰 반지름 (분산)
controller.polygonRadius = 2.5f;
controller.RearrangeFormation();
```

### **자동 배치 끄기**

```csharp
// 수동으로 위치 지정하고 싶을 때
controller.useAutomaticFormation = false;

// 직접 위치 배열 설정
controller.formationPositions = new Vector3[]
{
    new Vector3(0, 0, 0),
    new Vector3(2, 0, 0),
    new Vector3(-2, 0, 0)
};
controller.RearrangeFormation();
```

---

## 📊 **정다각형 좌표 계산**

### **수학 공식**

```csharp
// N각형의 꼭짓점 좌표
for (int i = 0; i < count; i++)
{
    // 시작 각도 90도 (위쪽부터 시작)
    float angle = (90f + (360f / count) * i) * Mathf.Deg2Rad;
    
    float x = Mathf.Cos(angle) * radius;
    float y = Mathf.Sin(angle) * radius;
    
    positions[i] = new Vector3(x, y, 0f);
}
```

### **각도 분포**

| 캐릭터 수 | 각도 간격 | 시작 위치 |
|---------|---------|---------|
| 2명 | 180° | 위쪽 (90°) |
| 3명 | 120° | 위쪽 (90°) |
| 4명 | 90° | 위쪽 (90°) |
| 5명 | 72° | 위쪽 (90°) |
| 6명 | 60° | 위쪽 (90°) |

---

## 🎯 **실전 예시**

### **예시 1: 3인 파티**

```csharp
// StartStage에서
GameManager.Instance.AddPlayerPawn("Warrior");  // 위쪽
GameManager.Instance.AddPlayerPawn("Mage");     // 오른쪽 아래
GameManager.Instance.AddPlayerPawn("Archer");   // 왼쪽 아래

// → 자동으로 삼각형 배치!
```

**결과:**
```
      Warrior
       /  \
   Archer  Mage
```

### **예시 2: 6인 파티**

```csharp
string[] party = { "Tank", "Warrior", "Mage", "Archer", "Healer", "Support" };
GameManager.Instance.AddMultiplePlayerPawns(party);

// → 자동으로 육각형 배치!
```

**결과:**
```
   Tank   Warrior
   
 Support    Mage
   
  Healer   Archer
```

### **예시 3: 동적 추가**

```csharp
// 레벨업마다 캐릭터 추가
void OnLevelUp(int level)
{
    if (level == 3)  // 1명 → 2명 (선분)
        GameManager.Instance.AddPlayerPawn("Warrior");
    
    if (level == 5)  // 2명 → 3명 (삼각형)
        GameManager.Instance.AddPlayerPawn("Mage");
    
    if (level == 7)  // 3명 → 4명 (사각형)
        GameManager.Instance.AddPlayerPawn("Archer");
    
    if (level == 9)  // 4명 → 5명 (오각형)
        GameManager.Instance.AddPlayerPawn("Healer");
    
    if (level == 11) // 5명 → 6명 (육각형)
        GameManager.Instance.AddPlayerPawn("Support");
}
```

---

## 🔧 **반지름 가이드**

### **권장 반지름 값**

| 용도 | 반지름 | 설명 |
|-----|-------|-----|
| **밀집** | 1.0f | 캐릭터들이 가깝게 |
| **기본** | 1.5f | 균형잡힌 간격 ⭐ |
| **분산** | 2.0f | 캐릭터들이 멀게 |
| **원형** | 2.5f | 큰 원 |

### **반지름별 시각적 차이**

```
반지름 1.0f (밀집):
   ●●●

반지름 1.5f (기본):
  ●   ●
     ●

반지름 2.5f (분산):
●       ●
   
      ●
```

---

## 🎨 **Scene View에서 확인**

### **Gizmos 표시**

PlayerController 선택 시:
- **파란색 원**: 각 대열 위치
- **실시간 업데이트**: 캐릭터 추가/제거 시 자동 갱신

### **디버깅 팁**

1. **Scene View와 Game View 동시 띄우기**
2. **PlayerController 선택 유지**
3. **캐릭터 추가하면서 파란색 원 관찰**

---

## ⚠️ **주의사항**

### **자동 배치 활성화 필수**

```csharp
// ✅ 자동 배치 활성화 (기본값)
controller.useAutomaticFormation = true;

// ❌ 비활성화 시 수동 설정 필요
controller.useAutomaticFormation = false;
```

### **캐릭터 제거 시**

```csharp
// 캐릭터 제거 후 재정렬 필요
GameManager.Instance.PlayerController.RemovePlayerPawn(pawn);
GameManager.Instance.PlayerController.RearrangeFormation();

// → 남은 캐릭터 수에 맞춰 다시 배치됨
// 예: 4명 → 3명으로 줄면 사각형 → 삼각형
```

---

## 📌 **빠른 참조**

### **설정 체크리스트**

- [ ] PlayerController 선택
- [ ] `Use Automatic Formation` 체크
- [ ] `Formation Type` = Polygon
- [ ] `Polygon Radius` = 1.5
- [ ] 캐릭터 추가 → 자동 배치 확인!

### **핵심 코드**

```csharp
// 캐릭터 추가 (자동 배치)
GameManager.Instance.AddPlayerPawn("Player");

// 대열 타입 변경
controller.formationType = FormationType.Polygon;
controller.RearrangeFormation();

// 반지름 조정
controller.polygonRadius = 2.0f;
controller.RearrangeFormation();
```

---

## 🚀 **결과**

### **완성!**

- ✅ 1~6명까지 자동 정다각형 배치
- ✅ 캐릭터 추가 시 자동 재배치
- ✅ Inspector에서 실시간 조정 가능
- ✅ 코드 수정 불필요!

**이제 캐릭터를 추가하기만 하면 자동으로 정다각형으로 배치됩니다!** 🎉


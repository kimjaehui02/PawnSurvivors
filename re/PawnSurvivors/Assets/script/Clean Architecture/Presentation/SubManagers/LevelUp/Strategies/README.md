# 레벨업 전략 (LevelUp Strategies)

**룬테라 스타일의 조건 기반 레벨업 시스템**

---

## 🎯 개요

각 캐릭터는 고유한 레벨업 조건을 가지며, 조건 달성 시 스탯이 강화되고 새로운 능력을 획득합니다.
대부분의 캐릭터는 1단계 레벨업(Lv1→Lv2)을 가지지만, 일부는 2단계 이상(Lv1→Lv2→Lv3)의 레벨업이 가능합니다.

---

## 📐 시스템 구조

```
LevelUpSubManager (관리자)
  └─ LevelUpStrategyBase[] (순차적 전략 배열)
      ├─ KillCountLevelUpStrategy (적 처치 수)
      ├─ SurvivalTimeLevelUpStrategy (생존 시간)
      └─ DamageDealtLevelUpStrategy (누적 데미지)
```

### **동작 방식**

1. **SubStart()**: 모든 LevelUpStrategyBase를 찾아서 `targetLevel` 순으로 정렬 후 초기화
2. **SubUpdate()**: 현재 활성 전략의 `CheckCondition()` 호출
3. **조건 달성**: `ApplyRewards()` 실행 → 다음 전략 활성화
4. **모든 조건 완료**: 최대 레벨 도달

---

## 🛠️ 기본 제공 전략

### 1. **KillCountLevelUpStrategy** (적 처치 수)
- **조건**: 전략 활성화 후 N명의 적 처치
- **설정**:
  - `requiredKills`: 필요한 처치 수

```json
{
  "targetLevel": 2,
  "conditionDescription": "적 3명 처치",
  "rewardDescription": "체력 +20, 데미지 1.5배",
  "requiredKills": 3,
  "healthIncrease": 20,
  "damageMultiplier": 1.5
}
```

### 2. **SurvivalTimeLevelUpStrategy** (생존 시간)
- **조건**: 전략 활성화 후 N초 생존
- **설정**:
  - `requiredSeconds`: 필요한 생존 시간 (초)

```json
{
  "targetLevel": 2,
  "requiredSeconds": 30,
  "speedIncrease": 2
}
```

### 3. **DamageDealtLevelUpStrategy** (누적 데미지)
- **조건**: 전략 활성화 후 총 N의 데미지 입히기
- **설정**:
  - `requiredDamage`: 필요한 누적 데미지

```json
{
  "targetLevel": 2,
  "requiredDamage": 500,
  "healthIncrease": 30,
  "fireRateMultiplier": 0.8
}
```

---

## 🎁 레벨업 보상

모든 전략에서 사용 가능한 공통 보상:

| 속성 | 설명 | 예시 |
|-----|------|-----|
| `healthIncrease` | 최대 체력 증가 | `20` = +20 HP |
| `damageMultiplier` | 데미지 배율 | `1.5` = 50% 증가 |
| `speedIncrease` | 이동 속도 증가 | `2` = +2 속도 |
| `fireRateMultiplier` | 공격 속도 배율 | `0.8` = 20% 빠르게 |

---

## 📝 JSON 레시피 예시

### **단일 레벨업 (Lv1 → Lv2)**

```json
{
  "LevelUpSubManagerSetupData": {
    "strategies": [
      {
        "type": "KillCount",
        "targetLevel": 2,
        "requiredKills": 3,
        "healthIncrease": 20,
        "damageMultiplier": 1.5
      }
    ]
  }
}
```

### **다단계 레벨업 (Lv1 → Lv2 → Lv3) - 슈리마 스타일**

```json
{
  "LevelUpSubManagerSetupData": {
    "strategies": [
      {
        "type": "SurvivalTime",
        "targetLevel": 2,
        "requiredSeconds": 30,
        "speedIncrease": 1,
        "healthIncrease": 20
      },
      {
        "type": "DamageDealt",
        "targetLevel": 3,
        "requiredDamage": 500,
        "healthIncrease": 30,
        "damageMultiplier": 2.0,
        "fireRateMultiplier": 0.7
      }
    ]
  }
}
```

**진행 순서**:
1. Lv1 상태로 시작
2. 30초 생존 → Lv2 달성 (속도+1, 체력+20)
3. 누적 500 데미지 → Lv3 달성 (체력+30, 데미지x2, 공속 30% 빠르게)

---

## 🔧 커스텀 전략 추가하기

### 1. **LevelUpStrategyBase 상속**

```csharp
public class CustomLevelUpStrategy : LevelUpStrategyBase
{
    public int customRequirement = 10;
    private int _startValue = 0;
    
    public override void Init(PawnManager pawnManager)
    {
        base.Init(pawnManager);
        _startValue = GetCustomValue();
    }
    
    public override bool CheckCondition()
    {
        return GetCustomValue() - _startValue >= customRequirement;
    }
    
    public override float GetProgress()
    {
        int progress = GetCustomValue() - _startValue;
        return Mathf.Clamp01(progress / (float)customRequirement);
    }
    
    public override string GetProgressText()
    {
        int progress = Mathf.Min(GetCustomValue() - _startValue, customRequirement);
        return $"{progress}/{customRequirement} 커스텀";
    }
    
    private int GetCustomValue()
    {
        // 커스텀 로직
        return 0;
    }
}
```

### 2. **특수 보상 추가**

```csharp
public override void ApplyRewards(PawnManager pawnManager)
{
    // 기본 보상 먼저 적용
    base.ApplyRewards(pawnManager);
    
    // 커스텀 보상 추가
    var newSubManager = pawnManager.gameObject.AddComponent<SpecialAbilitySubManager>();
    Debug.Log("  └─ 특수 능력 획득!", this);
}
```

---

## 💡 팁

### **진행도 UI 표시**
```csharp
var levelUpManager = pawnManager.GetComponent<LevelUpSubManager>();
if (levelUpManager != null)
{
    float progress = levelUpManager.GetCurrentProgress(); // 0~1
    string text = levelUpManager.GetCurrentProgressText(); // "2/3 처치"
    int level = levelUpManager.GetCurrentLevel(); // 현재 레벨
}
```

### **SessionData 통계 추적**
레벨업 조건이 제대로 작동하려면 다음 통계가 SessionData에 기록되어야 합니다:
- `enemiesKilled` (int): KillCountLevelUpStrategy
- `totalDamageDealt` (float): DamageDealtLevelUpStrategy

---

## 🎮 MovementStrategy와의 차이점

| MovementStrategy | LevelUpStrategy |
|-----------------|----------------|
| **1개만 활성** (한 번에 1가지 이동) | **순차적 활성화** (조건 달성 시 다음으로) |
| 전략 교체 (A→B) | 조건 완료 (A 완료 → B 시작) |
| 계속 반복 실행 | 한 번만 실행 후 다음 단계 |
| `ChangeMovementStrategyEvent`로 교체 | 자동으로 다음 전략 활성화 |

---

## 📚 참고

- **MovementStrategy 시스템**: `Assets/script/PawnCore/Presentation/SubManagers/Movement/Strategies/`
- **SessionData**: `Assets/script/Data/GameSessionData.cs`
- **이벤트 버스**: `Assets/script/PawnCore/Domain/Events/`


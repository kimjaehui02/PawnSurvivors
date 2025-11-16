# 게임 데이터 관리 시스템

**유연하고 확장 가능한 런타임 데이터 관리 시스템**

---

## 🎯 설계 철학

### **문제: 기획이 자주 변경됨**
- "적 처치 수 추가해주세요"
- "데미지 통계도 필요해요"
- "경험치 시스템 추가할까요?"

### **해결: Dictionary 기반 유연한 구조**
```csharp
// ❌ 기존 방식: 필드 추가할 때마다 클래스 수정
public int enemiesKilled;
public int bossesKilled;
public int eliteKilled;
// ... 끝없이 추가

// ✅ 새로운 방식: 코드 수정 없이 데이터 추가
session.AddInt("enemiesKilled");
session.AddInt("bossesKilled");
session.AddInt("eliteKilled");
// 클래스 수정 불필요!
```

---

## 📊 데이터 타입 4가지

| 타입 | 용도 | 예시 |
|-----|------|-----|
| **`intValues`** | 정수 (개수, 레벨 등) | 처치 수, 레벨, 골드 |
| **`floatValues`** | 실수 (시간, 비율 등) | 데미지, 경과 시간 |
| **`counterMaps`** | 카운터 맵 (여러 항목) | 업그레이드 레벨, 아이템 개수 |
| **`flagSets`** | 플래그 세트 (존재 여부) | 획득한 아이템, 달성한 업적 |

---

## 🚀 사용 예시

### **1. 기본 통계 (intValues, floatValues)**

```csharp
var session = GameManager.Instance.SessionData;

// 적 처치
session.AddInt("enemiesKilled");  // +1
session.AddInt("enemiesKilled", 5);  // +5

// 데미지 누적
session.AddFloat("totalDamage", 123.5f);

// 값 읽기
int kills = session.GetInt("enemiesKilled");  // 없으면 0
float damage = session.GetFloat("totalDamage");  // 없으면 0.0f
```

### **2. 업그레이드 시스템 (counterMaps)**

```csharp
// 업그레이드 획득
session.AddCounter("upgrades", "AttackSpeed");  // 레벨 1
session.AddCounter("upgrades", "AttackSpeed", 2);  // +2 → 레벨 3

// 레벨 확인
int attackLevel = session.GetCounter("upgrades", "AttackSpeed");

// 효과 적용
float attackBonus = attackLevel * 0.1f;  // 레벨당 10%
```

### **3. 아이템/업적 수집 (flagSets)**

```csharp
// 아이템 획득
session.AddFlag("collectedItems", "Sword");
session.AddFlag("collectedItems", "Shield");

// 보유 여부 확인
bool hasSword = session.HasFlag("collectedItems", "Sword");

// 모든 아이템 가져오기
HashSet<string> items = session.GetFlags("collectedItems");
// → {"Sword", "Shield"}
```

### **4. 다중 카테고리 (counterMaps)**

```csharp
// 아이템 개수 관리
session.SetCounter("items", "HealthPotion", 5);
session.AddCounter("items", "HealthPotion", -1);  // 사용

// 적 종류별 처치 수
session.AddCounter("killsByType", "Zombie");
session.AddCounter("killsByType", "Boss");

int zombieKills = session.GetCounter("killsByType", "Zombie");
```

---

## 📝 실전 통합 예시

### **적 처치 시**

```csharp
// DamageableSubManager 또는 사망 이벤트
private void OnEnemyDeath(PawnManager enemy)
{
    var session = GameManager.Instance.SessionData;
    
    // 전체 처치 수 증가
    session.AddInt("enemiesKilled");
    
    // 적 타입별 처치 수 (필요하면)
    string enemyType = enemy.PawnData.GetString("enemyType");
    session.AddCounter("killsByType", enemyType);
    
    // 골드 획득 (기획 완료 후 추가)
    // session.AddInt("gold", 10);
}
```

### **데미지 통계 (필요 시 추가)**

```csharp
// DamageableSubManager
private void HandleDamage(float damage, GameObject attacker)
{
    var session = GameManager.Instance.SessionData;
    
    if (attacker.CompareTag("Player"))
    {
        session.AddFloat("damageDealt", damage);
    }
    
    if (gameObject.CompareTag("Player"))
    {
        session.AddFloat("damageTaken", damage);
    }
}
```

### **UI 표시**

```csharp
// StageScreen 또는 통계 UI
private void UpdateUI()
{
    var session = GameManager.Instance.SessionData;
    
    killsText.text = $"Kills: {session.GetInt("enemiesKilled")}";
    timeText.text = $"Time: {session.GetSurvivalTime():F1}s";
    
    // 나중에 추가되면
    // levelText.text = $"Level: {session.GetInt("level", 1)}";
    // goldText.text = $"Gold: {session.GetInt("gold")}";
}
```

---

## 🔄 생명주기

```csharp
// 1. 게임 시작 시 자동 초기화 (GameManager.Awake)
SessionData = new GameSessionData();

// 2. 스테이지 시작 시 리셋 (GameManager.StartStage)
SessionData.Reset();
SessionData.currentStageName = stageName;

// 3. 게임 중 데이터 수집 (다양한 이벤트)
SessionData.AddInt("enemiesKilled");
SessionData.AddFloat("damageDealt", 123.5f);

// 4. 게임 종료 시 참조 (결과 화면)
int finalKills = SessionData.GetInt("enemiesKilled");
```

---

## ⚙️ 확장 방법

### **새 통계 추가 (코드 수정 불필요)**

```csharp
// 1. 데이터 수집 시작
session.AddInt("projectilesFired");

// 2. UI에서 표시
int fired = session.GetInt("projectilesFired");

// 끝! 클래스 수정 없음
```

### **새 시스템 추가 (필요 시)**

```csharp
// 경험치/레벨 시스템 (나중에)
session.AddInt("exp", 25);
if (session.GetInt("exp") >= 100)
{
    session.AddInt("level");
    session.SetInt("exp", 0);
}

// 퀘스트 시스템 (나중에)
session.AddFlag("completedQuests", "FirstKill");
session.AddFlag("completedQuests", "Survive5Minutes");
```

---

## 💾 저장/불러오기 (나중에 추가)

```csharp
// JSON 변환 (나중에 구현)
string json = JsonUtility.ToJson(SessionData);
PlayerPrefs.SetString("LastSession", json);

// 불러오기
string json = PlayerPrefs.GetString("LastSession");
SessionData = JsonUtility.FromJson<GameSessionData>(json);
```

---

## 🎯 핵심 장점

| 장점 | 설명 |
|-----|------|
| **✅ 유연성** | 기획 변경 시 코드 수정 최소화 |
| **✅ 확장성** | 새 데이터 추가가 매우 쉬움 |
| **✅ 간결성** | 복잡한 필드 관리 불필요 |
| **✅ 테스트** | Dictionary 구조라 디버깅 쉬움 |
| **✅ 저장** | JSON 직렬화 가능 (나중에) |

---

## 📌 사용 규칙

### **✅ 좋은 패턴**
```csharp
// 간결하고 명확
session.AddInt("enemiesKilled");
session.AddFloat("damageDealt", damage);
```

### **❌ 피해야 할 패턴**
```csharp
// 복잡한 로직은 SessionData에 넣지 마세요
// 대신 별도 클래스로 분리
LevelSystem.CheckLevelUp(session);
QuestSystem.CheckCompletion(session);
```

---

## 🚀 현재 상태

- ✅ 유연한 데이터 구조 완성
- ✅ 모든 헬퍼 메서드 준비
- ⏳ 구체적인 통계 (기획 후 추가)
- ⏳ 저장/불러오기 (필요 시 추가)

**→ 기획이 확정되면 코드 수정 없이 바로 데이터 추가 가능! 🎉**

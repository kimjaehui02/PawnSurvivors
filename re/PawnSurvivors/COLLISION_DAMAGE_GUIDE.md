# 충돌 데미지 시스템 가이드

## 📋 개요
적이 플레이어를 들이받아 데미지를 주거나, 발사체가 충돌 시 데미지를 주는 시스템입니다.

## 🎯 주요 기능

### 1. 충돌 시 파괴 여부 설정 (`destroyOnHit`)
- **발사체**: `destroyOnHit: true` → 충돌 후 파괴됨
- **근접 공격 유닛 (적)**: `destroyOnHit: false` → 충돌해도 살아있음

### 2. 피격 시 추가 효과
`PawnDamagedEvent`를 구독하여 다양한 피격 효과를 구현할 수 있습니다:
- 무적 (예시 구현: `InvincibilitySubManager`)
- 넉백
- 피격 이펙트
- 피격 사운드
- 카메라 쉐이크

---

## 🔧 사용 방법

### 발사체 (Bullet) 설정
```json
{
  "pawnName": "Bullet",
  "subManagerSetups": [
    {
      "$type": "PawnCore.Recipes.Json.CollisionDamageSubManagerSetupData, Assembly-CSharp",
      "damage": 10,
      "destroyOnHit": true
    }
  ]
}
```

### 근접 공격 유닛 (Enemy) 설정
```json
{
  "pawnName": "Enemy",
  "subManagerSetups": [
    {
      "$type": "PawnCore.Recipes.Json.CollisionDamageSubManagerSetupData, Assembly-CSharp",
      "damage": 5,
      "destroyOnHit": false
    },
    {
      "$type": "PawnCore.Recipes.Json.DamageableSubManagerSetupData, Assembly-CSharp",
      "maxHealth": 50
    }
  ]
}
```

---

## 📝 피격 시 추가 효과 구현 예시

### 무적 효과 (InvincibilitySubManager)
피격 후 일정 시간 동안 무적 상태가 되며, 깜빡임 효과가 나타납니다.

**레시피에 추가:**
```json
{
  "pawnName": "Player",
  "subManagerSetups": [
    {
      "$type": "PawnCore.Recipes.Json.InvincibilitySubManagerSetupData, Assembly-CSharp",
      "invincibilityDuration": 1.0,
      "blinkSpeed": 5.0
    }
  ]
}
```

**직접 코드로 사용:**
```csharp
// InvincibilitySubManager를 플레이어에 추가
var invincibility = playerObject.AddComponent<InvincibilitySubManager>();
invincibility.invincibilityDuration = 1.5f;
invincibility.blinkSpeed = 8f;
```

### 커스텀 피격 효과 만들기
새로운 SubManager를 만들고 `PawnDamagedEvent`를 구독하세요:

```csharp
using UnityEngine;
using PawnCore.Domain.Events;

public class MyCustomHitEffectSubManager : PawnSubManager
{
    public override void SubStart()
    {
        // 피격 이벤트 구독
        _pawnManager.Subscribe<PawnDamagedEvent>(HandlePawnDamaged);
    }

    private void OnDisable()
    {
        if (_pawnManager != null)
        {
            _pawnManager.Unsubscribe<PawnDamagedEvent>(HandlePawnDamaged);
        }
    }

    public override void SubUpdate()
    {
        // 필요한 업데이트 로직
    }

    private void HandlePawnDamaged(PawnDamagedEvent evt)
    {
        // 이 Pawn을 대상으로 한 피격인지 확인
        if (evt.Target != _pawnManager) return;

        // 여기에 원하는 효과 구현
        // 예: 넉백, 이펙트 재생, 사운드 재생 등
        Debug.Log($"피격! 데미지: {evt.DamageApplied}, 남은 체력: {evt.RemainingHealth}");
        
        if (evt.IsFatal)
        {
            Debug.Log("치명상!");
        }
    }
}
```

---

## 🎮 이벤트 흐름

```
충돌 발생
    ↓
CollisionDamageSubManager가 충돌 감지
    ↓
DamageEvent 발행 (데미지 적용 전)
    ↓
DamageableSubManager가 데미지 적용
    ↓
PawnDamagedEvent 발행 (데미지 적용 후) ← 여기서 추가 효과 구현!
    ↓
InvincibilitySubManager 등이 반응
    ↓
체력 0 이하? → PawnDeathEvent 발행
```

---

## 🔍 주요 클래스

### CollisionDamageSubManager
- **역할**: 충돌 시 상대방에게 데미지를 줌
- **설정**: `damage`, `destroyOnHit`
- **파일**: `Assets/script/PawnCore/Presentation/SubManagers/Combat/CollisionDamageSubManager.cs`

### DamageableSubManager
- **역할**: 데미지를 받고 체력 관리
- **설정**: `maxHealth`
- **파일**: `Assets/script/PawnCore/Presentation/SubManagers/Combat/DamageableSubManager.cs`

### InvincibilitySubManager (예시)
- **역할**: 피격 후 무적 효과 제공
- **설정**: `invincibilityDuration`, `blinkSpeed`
- **파일**: `Assets/script/PawnCore/Presentation/SubManagers/Combat/InvincibilitySubManager.cs`

---

## 📚 더 알아보기

- 이벤트 시스템 전체 설명: `Assets/script/PawnCore/Domain/Events/README.md`
- 레시피 시스템 설명: 메인 `README.md`


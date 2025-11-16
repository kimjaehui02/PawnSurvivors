# 최근 변경사항 요약 (빠른 체크용)

## 🔥 반드시 확인해야 할 파일 (우선순위 순)

### 1. CombatUsecases.cs (새로 추가된 메서드)
**파일**: `Assets/script/PawnCore/Domain/Usecases/CombatUsecases.cs`
**변경**: 
- `ApplyDamage()` - 데미지 계산 로직
- `CanDealDamageWithCooldown()` - 쿨다운 판정

**체크**: 데미지 계산 로직이 당신 의도와 같은가?
```csharp
healthData.currentHealth = Mathf.Max(healthData.currentHealth - damageAmount, 0f);
```

---

### 2. DamageableSubManager.cs (리팩토링)
**파일**: `Assets/script/PawnCore/Presentation/SubManagers/Combat/DamageableSubManager.cs`
**변경**: 
- 데미지 계산을 UseCases로 이동 (62-63행)

**체크**: SubManager가 얇아졌는가? 로직이 UseCases로 잘 분리되었는가?

---

### 3. CollisionDamageSubManager.cs (2가지 변경)
**파일**: `Assets/script/PawnCore/Presentation/SubManagers/Combat/CollisionDamageSubManager.cs`
**변경**:
- `_hasCollided` 플래그로 다중 충돌 방지 (34행)
- 쿨다운을 UseCases로 위임 (90-97행)

**체크**: 총알이 첫 적만 맞고 파괴되는가? 근접 유닛은 쿨다운이 작동하는가?

---

### 4. HitFlashSubManager.cs (Shader 기반 재구현)
**파일**: `Assets/script/PawnCore/Presentation/SubManagers/Visual/HitFlashSubManager.cs`
**변경**: 
- 2중 SpriteRenderer 방식 → Shader lerp 방식
- `Sprites/FlashEffect` 셰이더 사용

**체크**: 적이 피격 시 완전히 하얗게 변하는가?

---

### 5. Sprite-FlashEffect.shader (새 파일)
**파일**: `Assets/Resources/Shaders/Sprite-FlashEffect.shader`
**변경**: 
- 새로 생성된 커스텀 셰이더
- `_FlashAmount`로 lerp(원본색, 흰색) 제어

**체크**: Shader 코드가 당신 기준에 맞는가?

---

## 🟡 선택적으로 확인 (나중에 봐도 됨)

### 6. SeparationSubManager.cs (미사용 - JSON에서 제외됨)
**파일**: `Assets/script/PawnCore/Presentation/SubManagers/Movement/SeparationSubManager.cs`
**상태**: JSON에서 제외되어 현재 사용 안 함
**체크**: 나중에 사용할 때 확인

---

## ✅ 테스트 체크리스트

실행해서 이것만 확인하면 됩니다:

1. ✅ **총알이 첫 적만 맞고 파괴됨** (다중 관통 안 함)
2. ✅ **적이 피격 시 흰색 번쩍임** (회색→흰색 flash)
3. ✅ **적이 플레이어 충돌 시 데미지** (연속 충돌은 0.5초 쿨다운)
4. ✅ **플레이어 피격 시 무적 + 깜빡임** (1초간)
5. ✅ **데미지 계산이 정상** (체력 감소, 0 이하 시 파괴)

---

## 🔄 되돌리고 싶은 부분이 있다면?

**Shader 방식이 마음에 안 들면:**
→ "HitFlash를 원래 방식으로 되돌려줘" (2중 SpriteRenderer 또는 다른 방법)

**UseCases 분리가 과하다고 느껴지면:**
→ "데미지 계산을 SubManager에 다시 넣어줘" (CleanArchitecture 완화)

**총알 플래그 방식이 단순하다고 느껴지면:**
→ "총알 충돌 로직을 더 정교하게 해줘" (충돌 대상 추적 등)

---

## 💡 추천 체크 순서

1. **실행해서 5가지 테스트 확인** (5분)
2. **CombatUsecases.cs의 ApplyDamage() 읽기** (2분)
3. **DamageableSubManager.cs 62-65행 읽기** (1분)
4. **Sprite-FlashEffect.shader 읽기** (선택)

**총 시간: 10분 이내**

이후 문제가 있으면 말씀해주세요!


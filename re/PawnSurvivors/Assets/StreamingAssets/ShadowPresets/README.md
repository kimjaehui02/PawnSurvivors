# ShadowPresets (그림자 프리셋)

이 폴더에는 Pawn의 그림자 설정을 담은 JSON 프리셋 파일들이 저장됩니다.

---

## 🎯 목적

- **재사용성**: 여러 Pawn이 같은 그림자를 공유
- **관리 용이성**: 그림자 설정을 한 곳에서 관리
- **분리**: Pawn Recipe는 핵심 정보만, 그림자는 부가 효과로 분리
- **확장성**: 나중에 애니메이션 그림자, 동적 그림자 등 추가 가능

---

## 📁 구조

```
ShadowPresets/
├── PlayerShadow.json     (플레이어용 - 큰 그림자)
├── EnemyShadow.json      (일반 적용 - 작은 그림자)
├── BossShadow.json       (보스용 - 매우 큰 그림자)
└── (추가 프리셋...)
```

---

## 📄 JSON 형식

```json
{
  "presetName": "PlayerShadow",
  "shadowColor": {
    "r": 0,
    "g": 0,
    "b": 0,
    "a": 0.5
  },
  "shadowScale": {
    "x": 1.2,
    "y": 0.5
  },
  "shadowOffset": {
    "x": 0,
    "y": -0.6,
    "z": 0
  }
}
```

### 파라미터 설명

| 파라미터 | 타입 | 설명 |
|---------|------|------|
| **presetName** | string | 프리셋 이름 (파일명과 동일해야 함) |
| **shadowColor** | Color (RGBA) | 그림자 색상 (보통 검은색 반투명) |
| **shadowScale** | Vector2 | 타원형 크기 (x: 가로, y: 세로) |
| **shadowOffset** | Vector3 | Pawn 기준 위치 오프셋 (보통 아래쪽) |

---

## 🔧 사용 방법

### 1. ShadowPreset 생성

```json
// MyCustomShadow.json
{
  "presetName": "MyCustomShadow",
  "shadowColor": {"r": 0, "g": 0, "b": 0, "a": 0.6},
  "shadowScale": {"x": 1.0, "y": 0.3},
  "shadowOffset": {"x": 0, "y": -0.5, "z": 0}
}
```

### 2. Pawn Recipe에 적용

```json
// Enemy.json
{
  "subManagerSetups": [
    {
      "$type": "PawnCore.Recipes.Json.VisualSubManagerSetupData, Assembly-CSharp",
      "visualSpriteName": "Sprites/Circle1Sprite",
      "visualColor": {"r": 1, "g": 0, "b": 0, "a": 1},
      "shadowPreset": "MyCustomShadow"  // ✅ 프리셋 이름만 지정!
    }
  ]
}
```

### 3. 그림자 없애기

```json
// Bullet.json (그림자 필요 없음)
{
  "$type": "PawnCore.Recipes.Json.VisualSubManagerSetupData, Assembly-CSharp",
  "visualSpriteName": "Sprites/Bullet",
  "visualColor": {"r": 1, "g": 1, "b": 0, "a": 1}
  // shadowPreset 생략 또는 null → 그림자 없음
}
```

---

## 🎨 기본 제공 프리셋

### PlayerShadow
- **용도**: 플레이어 캐릭터
- **크기**: 큼 (1.2 x 0.5)
- **투명도**: 50%
- **위치**: 발 아래 약간 (-0.6)

### EnemyShadow
- **용도**: 일반 적
- **크기**: 작음 (0.8 x 0.4)
- **투명도**: 40%
- **위치**: 발 아래 약간 (-0.4)

### BossShadow
- **용도**: 보스 캐릭터
- **크기**: 매우 큼 (2.0 x 0.8)
- **투명도**: 60%
- **색상**: 약간 붉은 검정
- **위치**: 발 아래 많이 (-1.0)

---

## 💡 커스터마이징 예시

### 원형 그림자
```json
"shadowScale": {"x": 1.0, "y": 1.0}
```

### 파란 그림자
```json
"shadowColor": {"r": 0, "g": 0.3, "b": 1, "a": 0.5}
```

### 그림자 없음
```json
// Pawn Recipe에서 shadowPreset 생략
```

### 매우 납작한 타원
```json
"shadowScale": {"x": 1.5, "y": 0.2}
```

---

## 🔄 작동 원리

1. **게임 시작 시** → `CreationManager`가 모든 ShadowPreset JSON 로드
2. **Pawn 생성 시** → Recipe의 `shadowPreset` 이름으로 프리셋 찾기
3. **그림자 생성** → `VisualSubManager`가 프리셋 데이터로 Shadow GameObject 생성

---

## ⚠️ 주의사항

1. **파일명 = presetName**: 반드시 일치해야 함
   - ✅ `PlayerShadow.json` + `"presetName": "PlayerShadow"`
   - ❌ `PlayerShadow.json` + `"presetName": "Player"` (안 됨!)

2. **대소문자 구분**: JSON은 대소문자 구분
   - ✅ `"shadowPreset": "PlayerShadow"`
   - ❌ `"shadowPreset": "playershadow"` (못 찾음!)

3. **존재하지 않는 프리셋**: 경고 로그만 출력하고 그림자 없이 진행

---

## 📊 장점 (Bullet Recipe와 비교)

| 항목 | Before (인라인) | After (Preset) |
|------|----------------|----------------|
| **코드 길이** | 4줄 (4개 파라미터) | 1줄 |
| **재사용** | 매번 복붙 | 프리셋 이름만 |
| **수정** | 모든 Recipe 수정 | Preset 1개만 수정 |
| **Git 충돌** | 높음 | 낮음 |

---

## 🚀 확장 가능성

나중에 이런 기능 추가 가능:
- **애니메이션 그림자**: `shadowAnimation: "Pulse"`
- **동적 크기**: `shadowScaleByHeight: true`
- **그림자 페이드**: `fadeOnJump: true`
- **투영 방향**: `projectionAngle: 45`

모두 Preset 파일만 수정하면 됨! ✨


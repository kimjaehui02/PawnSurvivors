# UI 시스템 설정 가이드

이 가이드는 Unity 에디터에서 UI 시스템을 설정하는 방법을 단계별로 설명합니다.

## 📋 **필수 준비물**

1. TextMeshPro 패키지
   - Window → TextMeshPro → Import TMP Essential Resources

## 🎯 **설정 단계**

### 1단계: GameManager에 UIManager 추가

**중요:** UIManager는 별도 GameObject가 아닌 GameManager와 같은 GameObject에 붙입니다.

1. Hierarchy에서 GameManager GameObject 선택
2. Inspector에서 Add Component 클릭
3. `UIManager` 스크립트 추가

**GameManager GameObject 구조:**
```
GameManager
├── LifecycleManager (컴포넌트)
├── CreationManager (컴포넌트)
├── StageManager (컴포넌트)
└── UIManager (컴포넌트) ← 추가
```

### 2단계: 각 화면 Canvas 생성

각 화면마다 다음 단계를 반복합니다:

#### A. Title Screen

1. Hierarchy에서 우클릭 → UI → Canvas
2. 이름을 "TitleScreen"으로 변경
3. Canvas 설정:
   - Render Mode: Screen Space - Overlay
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920x1080
4. `TitleScreen` 스크립트 추가
5. `CanvasGroup` 컴포넌트 추가

6. UI 요소 추가:
   ```
   TitleScreen (Canvas)
   ├── Background (Image) - 배경 이미지
   ├── Title (TextMeshPro) - "PAWN SURVIVORS"
   ├── StartButton (Button)
   │   └── Text (TextMeshPro) - "START"
   └── QuitButton (Button)
       └── Text (TextMeshPro) - "QUIT"
   ```

7. Inspector에서 TitleScreen 스크립트에 버튼들 연결

#### B. Main Menu Screen

```
MainMenuScreen (Canvas)
├── Background (Image)
├── Title (TextMeshPro) - "MAIN MENU"
├── StageNameText (TextMeshPro) - "Selected Stage: Stage1"
├── StartStageButton (Button)
│   └── Text (TextMeshPro) - "START STAGE"
└── BackButton (Button)
    └── Text (TextMeshPro) - "BACK"
```

#### C. Gameplay HUD

```
GameplayHUD (Canvas)
├── TopPanel (Panel)
│   ├── HealthText (TextMeshPro) - "HP: 100/100"
│   ├── HealthSlider (Slider)
│   ├── ScoreText (TextMeshPro) - "Score: 0"
│   └── TimeText (TextMeshPro) - "Time: 00:00"
└── (기타 게임플레이 UI 요소)
```

#### D. Pause Menu Screen

```
PauseMenuScreen (Canvas)
├── Background (Image) - 반투명 검은색
├── Title (TextMeshPro) - "PAUSED"
├── ResumeButton (Button)
│   └── Text (TextMeshPro) - "RESUME"
├── MainMenuButton (Button)
│   └── Text (TextMeshPro) - "MAIN MENU"
└── QuitButton (Button)
    └── Text (TextMeshPro) - "QUIT"
```

#### E. Game Over Screen

```
GameOverScreen (Canvas)
├── Background (Image)
├── GameOverText (TextMeshPro) - "GAME OVER"
├── FinalScoreText (TextMeshPro) - "Final Score: 0"
├── RetryButton (Button)
│   └── Text (TextMeshPro) - "RETRY"
└── MainMenuButton (Button)
    └── Text (TextMeshPro) - "MAIN MENU"
```

#### F. Stage Clear Screen

```
StageClearScreen (Canvas)
├── Background (Image)
├── ClearText (TextMeshPro) - "STAGE CLEAR!"
├── ScoreText (TextMeshPro) - "Score: 0"
├── TimeText (TextMeshPro) - "Time: 00:00"
├── NextStageButton (Button)
│   └── Text (TextMeshPro) - "NEXT STAGE"
└── MainMenuButton (Button)
    └── Text (TextMeshPro) - "MAIN MENU"
```

### 3단계: UIManager에 화면 연결

1. GameManager GameObject 선택
2. Inspector에서 UIManager 컴포넌트 찾기
3. 각 화면 Canvas를 해당 필드에 드래그:
   - Title Screen → titleScreen
   - Main Menu Screen → mainMenuScreen
   - Gameplay HUD → gameplayHUD
   - Pause Menu Screen → pauseMenuScreen
   - Game Over Screen → gameOverScreen
   - Stage Clear Screen → stageClearScreen

### 4단계: 최종 확인

GameManager GameObject에 다음 컴포넌트가 모두 있는지 확인:
- LifecycleManager ✓
- CreationManager ✓
- StageManager ✓
- UIManager ✓ (새로 추가)

## 🎨 **UI 스타일 가이드 (권장)**

### 색상 팔레트
```
Primary: #2C3E50 (어두운 청회색)
Secondary: #E74C3C (빨강)
Accent: #3498DB (파랑)
Background: #34495E (어두운 회색)
Text: #ECF0F1 (밝은 회색)
```

### 버튼 스타일
- Normal: Primary 색상
- Highlighted: 약간 밝게
- Pressed: 약간 어둡게
- Disabled: 회색

### 텍스트 크기
- 타이틀: 72pt
- 부제목: 48pt
- 본문: 24pt
- 버튼 텍스트: 32pt

## ✅ **테스트 체크리스트**

- [ ] 타이틀 화면이 게임 시작 시 표시됨
- [ ] Start 버튼 클릭 시 메인 메뉴로 이동
- [ ] Start Stage 버튼 클릭 시 게임 시작
- [ ] ESC 키로 일시정지 메뉴 열기/닫기
- [ ] 플레이어 체력이 HUD에 실시간 표시됨
- [ ] 플레이어 사망 시 게임 오버 화면 표시
- [ ] 모든 버튼이 정상 작동

## 🐛 **문제 해결**

### UI가 표시되지 않음
- Canvas의 Render Mode 확인
- CanvasGroup의 Alpha가 1인지 확인
- UIManager에 화면들이 제대로 연결되었는지 확인

### 버튼이 작동하지 않음
- EventSystem이 씬에 있는지 확인
- 버튼에 Button 컴포넌트가 있는지 확인
- Inspector에서 OnClick 이벤트가 연결되었는지 확인

### TextMeshPro 텍스트가 깨짐
- Window → TextMeshPro → Import TMP Essential Resources 실행
- Font Asset이 제대로 설정되었는지 확인

## 📝 **다음 단계**

1. 각 화면의 디자인 개선
2. 화면 전환 애니메이션 추가
3. 사운드 효과 추가
4. 스테이지 선택 UI 구현
5. 설정 화면 추가


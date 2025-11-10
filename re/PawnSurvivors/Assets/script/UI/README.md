# UI 시스템 (UI System)

게임의 모든 사용자 인터페이스와 화면 전환을 관리하는 시스템입니다.

## 📋 **핵심 구성 요소**

### 1. **UIManager**
- 모든 UI 화면을 관리하는 중앙 매니저
- 게임 상태(`GameState`)에 따라 적절한 화면을 자동으로 표시
- 싱글톤 패턴으로 구현되어 어디서든 접근 가능
- **GameManager와 같은 GameObject에 컴포넌트로 추가**
- GameManager가 `GetComponent<UIManager>()`로 참조

### 2. **GameState Enum**
게임의 전체 상태를 정의:
- `Title`: 타이틀 화면
- `MainMenu`: 메인 메뉴 (스테이지 선택)
- `Playing`: 게임 플레이 중
- `Paused`: 일시정지
- `GameOver`: 게임 오버
- `StageClear`: 스테이지 클리어

### 3. **UIScreen (기반 클래스)**
모든 UI 화면의 기반 클래스:
- `Show()`: 화면 표시
- `Hide()`: 화면 숨김
- `OnShow()`: 표시 시 호출되는 가상 메서드
- `OnHide()`: 숨김 시 호출되는 가상 메서드

## 🎮 **화면 목록**

### TitleScreen
- 게임 시작 화면
- "Start" 버튼: 메인 메뉴로 이동
- "Quit" 버튼: 게임 종료

### MainMenuScreen
- 스테이지 선택 화면
- "Start Stage" 버튼: 선택한 스테이지 시작
- "Back" 버튼: 타이틀로 돌아가기

### GameplayHUD
- 게임 플레이 중 표시되는 HUD
- 플레이어 체력 표시 (텍스트 + 슬라이더)
- 점수 표시
- 플레이 시간 표시
- 실시간으로 플레이어 정보 업데이트

### PauseMenuScreen
- 일시정지 메뉴
- ESC 키로 토글 가능
- "Resume" 버튼: 게임 재개
- "Main Menu" 버튼: 메인 메뉴로 돌아가기
- "Quit" 버튼: 게임 종료

### GameOverScreen
- 게임 오버 화면
- 최종 점수 표시
- "Retry" 버튼: 스테이지 재시작
- "Main Menu" 버튼: 메인 메뉴로 돌아가기

### StageClearScreen
- 스테이지 클리어 화면
- 점수 및 클리어 시간 표시
- "Next Stage" 버튼: 다음 스테이지로
- "Main Menu" 버튼: 메인 메뉴로 돌아가기

## 🔄 **화면 전환 흐름**

```
Title Screen
    ↓ (Start 버튼)
Main Menu Screen
    ↓ (Start Stage 버튼)
Gameplay HUD
    ↓ (ESC 키)
Pause Menu
    ↓ (Resume 버튼)
Gameplay HUD
    ↓ (플레이어 사망)
Game Over Screen
    ↓ (Retry 버튼)
Gameplay HUD

또는

Gameplay HUD
    ↓ (스테이지 클리어)
Stage Clear Screen
    ↓ (Next Stage 버튼)
Gameplay HUD
```

## 💻 **사용 예시**

### 다른 스크립트에서 UI 제어하기

```csharp
// 게임 오버 표시
UIManager.Instance.ShowGameOver();

// 스테이지 클리어 표시
UIManager.Instance.ShowStageClear();

// 일시정지
UIManager.Instance.PauseGame();

// 게임 재개
UIManager.Instance.ResumeGame();

// 메인 메뉴로 돌아가기
UIManager.Instance.ReturnToMainMenu();
```

### 이벤트와 연동하기

```csharp
// PawnDeathEvent를 받아서 게임 오버 처리
public class GameOverHandler : MonoBehaviour
{
    void Start()
    {
        // 플레이어 Pawn 찾기
        var player = FindPlayerPawn();
        if (player != null)
        {
            player.Subscribe<PawnDeathEvent>(OnPlayerDeath);
        }
    }

    void OnPlayerDeath(PawnDeathEvent evt)
    {
        if (evt.DeadPawn.gameObject.CompareTag("Player"))
        {
            UIManager.Instance.ShowGameOver();
        }
    }
}
```

## 🎨 **Unity 설정 가이드**

### 1. UIManager 설정
1. 빈 GameObject 생성 → 이름: "UIManager"
2. `UIManager` 컴포넌트 추가
3. 각 화면 프리팹을 Inspector에서 할당

### 2. 각 화면 설정
1. Canvas 생성 (Screen Space - Overlay)
2. 해당 화면 스크립트 추가 (예: `TitleScreen`)
3. CanvasGroup 컴포넌트 추가 (페이드 효과용)
4. UI 요소 (버튼, 텍스트 등) 배치
5. Inspector에서 UI 요소들을 스크립트에 할당

### 3. TextMeshPro 설정
- 텍스트 표시를 위해 TextMeshPro 사용
- Window → TextMeshPro → Import TMP Essential Resources

## 📝 **TODO**

- [ ] 화면 전환 애니메이션 추가 (페이드 인/아웃)
- [ ] 점수 시스템 구현
- [ ] 스테이지 선택 UI 개선 (여러 스테이지 표시)
- [ ] 설정 화면 추가 (사운드, 그래픽 옵션)
- [ ] 업적/통계 화면 추가
- [ ] 로컬라이제이션 지원

## 🏗️ **아키텍처 노트**

이 UI 시스템은 **State Pattern**을 기반으로 설계되었습니다:
- `GameState`: 게임의 상태를 정의
- `UIManager`: 상태 전환을 관리하는 State Machine
- `UIScreen`: 각 상태에 해당하는 화면

이를 통해:
- ✅ 명확한 화면 전환 로직
- ✅ 쉬운 새 화면 추가
- ✅ 중앙화된 UI 관리
- ✅ 테스트 용이성


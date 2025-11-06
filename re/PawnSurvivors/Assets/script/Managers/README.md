# 전역 매니저 (Global Managers)

이 폴더에는 게임의 전반적인 생명주기, 상태, 객체 생성 등 핵심적인 '글로벌' 기능을 관리하는 매니저 클래스들이 위치합니다.

이 매니저들은 일반적으로 씬(Scene)에 단 하나만 존재하는 싱글톤(Singleton) 형태로 구현되며, 다른 모든 객체들이 필요로 하는 공통 기능에 대한 핵심 접근점(예: `GameManager.Instance`)을 제공합니다.

## 주요 클래스
- **GameManager:** 게임의 주 진입점(Entry Point)이자, 다른 매니저들을 총괄하는 최상위 매니저입니다. **게임 시작 시 `CreationManager`를 통해 플레이어 및 적 `Pawn`을 생성하는 임시 로직을 포함합니다.**
- **LifecycleManager:** `SubStart` 같은 초기화 액션의 실행 순서를 보장하고, `ManagedUpdate` 루프를 관리합니다. 또한, `RequestDestruction`을 통해 오브젝트의 파괴 시점을 통제하여, 업데이트 루프 중 발생하는 충돌을 방지하는 **제어된 파괴(Controlled Destruction)** 역할을 수행합니다. **`PawnSubManager`의 `SubStart()` 호출은 더 이상 `LifecycleManager`를 통하지 않고 `PawnManager`가 직접 제어합니다.**
- **CreationManager:** **"Pawn 설계도(`PawnRecipe`)" 시스템의 핵심**입니다. `Assets/StreamingAssets/Recipes/` 경로에서 **JSON 기반 레시피 파일**을 로드하고 관리하며, `PawnRecipeData` 에셋을 읽어, 그에 명시된 모든 컴포넌트를 프로그래밍 방식으로 조립하여 완전한 `Pawn` 게임 오브젝트를 생성하는 **팩토리(Factory)** 역할을 합니다. `PawnManager`의 `PawnData`가 완전히 설정된 후 `PawnManager.InitializeSubManagers()`를 호출하여 `SubManager` 초기화를 보장합니다.

## 클린 아키텍처 관점
이 매니저들은 애플리케이션을 부트스트랩하고, 각 계층(Layer)이 원활하게 동작하도록 조율하는 **인프라스트럭처(Infrastructure)** 계층의 일부로 볼 수 있습니다.
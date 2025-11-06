# 서브매니저 (SubManagers)

이 폴더에는 `Pawn`의 구체적인 기능들을 담당하는 서브매니저(SubManager) 클래스들이 기능별 하위 폴더로 나뉘어 저장됩니다. **최근 `Visual` 및 `Physics` 관련 서브매니저가 추가되어 Pawn의 시각적 요소와 물리적 특성을 동적으로 관리합니다.**

모든 서브매니저는 `PawnSubManager` 베이스 클래스를 상속받으며, `PawnManager`에 의해 생명주기가 관리됩니다.

## 클린 아키텍처 관점
서브매니저들은 클린 아키텍처의 **프레젠테이션 계층(Presentation Layer)** 에 해당합니다.

이들은 Unity 엔진의 세계(입력, 업데이트 루프, 물리 등)와 `PawnCore`의 순수한 도메인 계층(Usecases, Events 등) 사이를 연결하는 **어댑터(Adapter)** 역할을 수행합니다. 예를 들어, Unity의 입력을 받아 도메인 이벤트를 발생시키거나, `Update` 루프에 맞춰 도메인의 유스케이스를 호출하는 등의 작업을 처리합니다. **새로 추가된 `VisualSubManager`는 시각적 프리팹을 관리하고, `PhysicsSubManager`는 콜라이더, 리지드바디, 레이어, 태그 등의 물리 설정을 코드 베이스에서 동적으로 처리합니다.**

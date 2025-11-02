# 이동 서브시스템 (Movement Subsystem)

이 폴더는 폰(Pawn)의 이동과 관련된 로직을 관리합니다.

## 주요 클래스
- **MovableSubManager:** 폰의 이동을 총괄하는 메인 서브매니저입니다. 실제 이동 방식은 `Strategies` 폴더에 있는 '전략 컴포넌트'에게 위임하며, 이들의 실행을 관리하고 통제하는 역할을 합니다.

## 핵심 구조
`MovableSubManager`는 `ChangeMovementStrategyEvent` 이벤트를 구독하여, 게임 도중 언제든지 외부의 요청에 따라 이동 방식을 동적으로 변경할 수 있습니다.

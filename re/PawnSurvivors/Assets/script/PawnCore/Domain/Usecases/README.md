# 유스케이스 (Usecases)

이 폴더에는 특정 도메인(전투, 이동 등)과 관련된 순수 C# 헬퍼(Helper) 함수 또는 복잡한 계산 로직을 담는 정적(static) 클래스들이 위치합니다.

## 목적
`MonoBehaviour`의 생명주기나 상태에 종속되지 않는 순수한 로직을 별도의 클래스로 분리하여, 코드의 재사용성과 테스트 용이성을 높이는 것을 목적으로 합니다. 특히 **`PawnData`를 직접 받아 로직을 처리하여, 도메인 규칙을 명확히** 합니다.

예를 들어, 복잡한 데미지 계산 공식, 특정 경로 계산 알고리즘 등이 여기에 해당될 수 있습니다.

## 주요 클래스
- **CombatUsecases:** 데미지 적용, 발사체 발사, 충돌 처리 등 전투와 관련된 로직을 담당합니다. (`ApplyDamage`, `HandleCollisionDamage`, `HandleProjectileAttack`)
- **MovementUsecases:** 특정 방향으로 이동, 입력에 따른 이동 등 이동과 관련된 순수 계산 로직을 담당합니다. (`MoveWithInput`, `MoveInDirection`, `HandleLifetime`)
- **TargetingUsecases:** 특정 범위 내에서 조건에 맞는 대상을 찾는 로직을 담당합니다.

## 클린 아키텍처 관점
이 클래스들은 클린 아키텍처의 **유스케이스(Use Cases)** 또는 **인터랙터(Interactors)**에 해당하며, 애플리케이션의 핵심 비즈니스 규칙을 담고 있는 도메인 계층의 일부입니다.
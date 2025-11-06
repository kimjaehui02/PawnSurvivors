# Pawn 설계도 시스템 (Pawn Recipe System)

이 폴더에는 `CreationManager`가 `Pawn`을 생성할 때 사용하는 데이터 기반 "설계도" 시스템 관련 클래스들이 위치합니다. **이 시스템은 JSON 파일을 통해 Pawn의 구성을 정의합니다.**

## 핵심 목적
복잡한 구조를 가진 `Pawn` 게임 오브젝트를 프리팹에 의존하여 수동으로 조립하는 대신, **JSON 파일 기반의 데이터로 정의**하고, 이를 바탕으로 런타임에 프로그래밍 방식으로 `Pawn`을 조립하기 위해 설계되었습니다.

이를 통해, 새로운 종류의 `Pawn`을 추가하거나 기존 `Pawn`의 구성을 변경할 때, 코드 수정 없이 데이터 파일의 조합만으로 대응할 수 있어 생산성과 안정성이 크게 향상됩니다.

## 주요 구성 요소

1.  **`PawnRecipeData` (최상위 설계도):**
    *   하나의 `Pawn` 타입을 정의하는 마스터 설계도 데이터입니다.
    *   폰의 이름, 그리고 어떤 `SubManager`들을 가질지에 대한 정보(`subManagerSetups` 리스트)를 담고 있습니다.
    *   `ToPawnData()` 메서드를 통해 `PawnData` 객체를 초기화하는 데 사용됩니다.

2.  **`SubManagerSetupData` (부품 설정):**
    *   각 `SubManager`가 `PawnData`의 어떤 필드를 초기화하고 `Pawn` `GameObject`에 해당 `SubManager` 컴포넌트를 추가할지에 대한 정보를 담는 '부품' 데이터의 기반 추상 클래스입니다.
    *   `ApplyToPawnData(PawnData pawnData)`: `PawnData`의 필드를 설정합니다.
    *   `AddSubManagerComponent(GameObject pawnObject)`: 해당 `SubManager` 컴포넌트를 `Pawn` `GameObject`에 추가하고 반환합니다.
    *   예를 들어, `DamageableSubManagerSetupData`는 `maxHealth` 값을, `MovableSubManagerSetupData`는 어떤 이동 전략들을 사용할지를 정의하고, `VisualSubManagerSetupData`는 `visualPrefabName`을, `PhysicsSubManagerSetupData`는 물리 관련 설정(`colliderType`, `rigidbodyType`, `layer`, `tag` 등)을 정의합니다.

3.  **`MovementStrategySetupData` (세부 부품 설정):**
    *   `MovableSubManager`에 추가될 각 이동 전략 컴포넌트의 세부 설정 및 컴포넌트 추가를 담당하는 데이터의 기반 추상 클래스입니다.
    *   `ApplyToMovableData(MovableData movableData)`: `PawnData.movableData`의 필드를 설정합니다.
    *   `AddMovementStrategyComponent(GameObject pawnObject)`: 해당 이동 전략 컴포넌트를 `Pawn` `GameObject`에 추가하고 반환합니다.
    *   `isEnabledByDefault`: 해당 전략이 초기에 활성화될지 여부를 정의합니다.

## 사용 흐름

1.  **JSON 파일 생성:** `Assets/StreamingAssets/Recipes` 폴더에 JSON 파일을 생성하여 `PawnRecipeData` 구조에 맞춰 폰의 구성을 정의합니다.
2.  **Pawn 생성:** `CreationManager.CreatePawn(recipeName, ...)` 코드를 호출하여, JSON 파일에 정의된 대로 모든 컴포넌트가 조립되고 `PawnData`가 초기화된 `Pawn` 게임 오브젝트를 생성합니다.
# UseCase 구조 분석 결과

## ✅ 적절하게 분리된 UseCase들

### 1. ItemPoolUseCase vs ItemManagementUseCase
- **ItemPoolUseCase**: 아이템 풀(전체 아이템 목록)에서 랜덤 선택
- **ItemManagementUseCase**: 플레이어가 보유한 아이템 관리 (구매, 장착, 조회)
- **판단**: 명확히 분리되어 있음 ✅

### 2. Tracking UseCase들
- **DamageTrackingUseCase**: 데미지 추적
- **KillTrackingUseCase**: 처치 수 추적  
- **SurvivalTimeTrackingUseCase**: 생존 시간 추적
- **판단**: 각각 다른 도메인 개념이므로 분리 유지 ✅

### 3. CurrencyUseCase
- 골드 관리만 담당 (획득, 소비, 잔액 확인)
- **판단**: 단일 책임 원칙 준수 ✅

### 4. ItemManagementUseCase
- 구매, 장착, 조회, 상태 확인 등 아이템 관리 전반
- **판단**: 모두 "아이템 관리"라는 하나의 도메인 개념이므로 적절 ✅

## ⚠️ 향후 검토 필요

### ShopUseCase
- **현재 상태**: 얇은 래퍼 (GetShopItems는 ItemPoolUseCase 단순 호출)
- **의미 있는 로직**: RerollShop(), CanReroll()만 있음
- **판단**: 
  - 현재는 얇은 래퍼이지만, 향후 상점 전용 비즈니스 로직 추가 가능성 고려
  - 예: 할인 시스템, 특가 아이템, 상점 레벨, 상점 업그레이드 등
- **결정**: 현재 상태 유지, 향후 상점 전용 로직 추가 시 확장 예정

## 📋 UseCase 책임 정리

| UseCase | 주요 책임 | 상태 |
|---------|----------|------|
| ItemPoolUseCase | 아이템 풀에서 랜덤 선택 | ✅ 적절 |
| ItemManagementUseCase | 아이템 구매/장착/조회 | ✅ 적절 |
| ShopUseCase | 상점 아이템 가져오기, 리롤 | ⚠️ 향후 확장 예정 |
| CurrencyUseCase | 골드 관리 | ✅ 적절 |
| DamageTrackingUseCase | 데미지 추적 | ✅ 적절 |
| KillTrackingUseCase | 처치 수 추적 | ✅ 적절 |
| SurvivalTimeTrackingUseCase | 생존 시간 추적 | ✅ 적절 |
| SessionManagementUseCase | 세션 데이터 관리 | ✅ 적절 |
| StageManagementUseCase | 스테이지 관리 | ✅ 적절 |
| PawnStatCalculator | 스탯 계산 | ✅ 적절 |
| FloatingEffectUseCase | 플로팅 이펙트 생성 | ✅ 적절 |

## 결론

대부분의 UseCase는 적절하게 분리되어 있으며, 단일 책임 원칙을 잘 따르고 있습니다. ShopUseCase만 현재는 얇은 래퍼이지만, 향후 상점 전용 비즈니스 로직 추가를 고려하여 현재 상태로 유지합니다.


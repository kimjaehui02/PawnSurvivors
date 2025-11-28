# Domain 계층 디버깅 시스템 TODO

## 현재 상태

### 완료된 작업
- ✅ `IItemPoolRepository` 인터페이스 생성 (Domain 계층)
- ✅ `ItemPoolUseCase` 생성 (Domain 계층)
- ✅ `MockItemPoolRepository` 생성 (디버깅용 Mock - Data 계층)
- ✅ `JsonItemPoolDataSource` 생성 (디버깅용 데이터 소스 - Data 계층)
- ✅ `JsonItemPoolRepository` 생성 (디버깅용 Repository - Data 계층)
- ✅ `ItemPoolTestManager` 생성 (디버깅용 MonoBehaviour - Presentation 계층)
- ✅ 계층별 폴더 분리 (Debug/Data, Debug/Presentation)

### 진행 중
- 🔄 JSON 파일 작업 (나중에)

---

## 폴더 구조

```
Debug/
├── Data/                          # Data 계층 (디버깅용)
│   ├── MockItemPoolRepository.cs  # Mock Repository 구현체
│   ├── JsonItemPoolDataSource.cs  # JSON 데이터 소스
│   └── JsonItemPoolRepository.cs  # JSON Repository 구현체
├── Presentation/                  # Presentation 계층 (디버깅용)
│   └── ItemPoolTestManager.cs     # 테스트 MonoBehaviour
└── TODO.md                        # 이 파일
```

---

## 향후 개선 사항 (우선순위 낮음)

### Repository와 Data Source 구조 개선

**현재 문제점:**
- Loader들(`RecipeLoader`, `StageLoader`, `ItemPoolLoader` 등)이 JSON을 직접 읽고 있음
- Repository 구현체들이 데이터 소스를 직접 참조하지 않고 있음
- Loader와 Repository가 분리되어 있어 연결이 약함

**권장 구조:**
```
Data Source (JsonItemPoolDataSource)
└── JSON 읽기 → ItemData 모델 반환

Repository (ItemPoolRepository)
└── Data Source로부터 ItemData 받음 → Domain 계층에 제공
```

**개선 이유:**
1. **관심사 분리**: Repository는 비즈니스 로직, Data Source는 데이터 로딩
2. **테스트 용이**: Data Source를 Mock으로 교체 가능
3. **확장성**: Data Source 변경(JSON → DB → 서버) 시 Repository 수정 최소화
4. **Clean Architecture 준수**: Repository가 Data Source에 의존하되, 인터페이스로 분리

**적용 대상:**
- `ItemPoolLoader` → `JsonItemPoolDataSource` + `ItemPoolRepository` 구현체
- `RecipeLoader` → `JsonRecipeDataSource` + `RecipeRepository` (필요시)
- `StageLoader` → `JsonStageDataSource` + `StageRepository` (필요시)
- 기타 Loader들도 동일한 패턴 적용

**참고:**
- 현재 `ItemRepository`는 `GameSessionData`(메모리 데이터 소스)를 참조하는 구조
- 이는 런타임 데이터 저장용이므로 구조가 다름
- `ItemPoolRepository`는 정적 데이터(아이템 풀)를 다루므로 Data Source 패턴 적용 적합

---

## 참고 사항

- 디버깅 폴더의 코드들은 나중에 실제 구현 시 참고용
- Mock과 Data Source는 실제 구현의 구조를 미리 보여줌
- 실제 구현 시 Data 계층에 위치하게 됨


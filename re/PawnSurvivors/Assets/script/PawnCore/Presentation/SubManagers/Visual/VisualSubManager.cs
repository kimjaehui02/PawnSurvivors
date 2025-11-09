using UnityEngine;
using PawnCore.Domain;

namespace PawnCore.Presentation.SubManagers.Visual
{
    public class VisualSubManager : PawnSubManager
    {
        private PawnData _pawnData;
        private SpriteRenderer _spriteRenderer; // SpriteRenderer 참조 추가

        public override void SubStart()
        {
            _pawnData = _pawnManager.PawnData;

            // "Visuals" 라는 이름의 자식 게임오브젝트 생성
            GameObject visualsObject = new GameObject("Visuals");
            visualsObject.transform.SetParent(_pawnManager.transform);
            visualsObject.transform.localPosition = Vector3.zero; // 위치 초기화

            // 자식 오브젝트에 SpriteRenderer 추가 또는 가져오기
            _spriteRenderer = visualsObject.AddComponent<SpriteRenderer>();

            Sprite visualSprite = null;
            // 지정된 스프라이트 로드 시도
            if (!string.IsNullOrEmpty(_pawnData.visualSpriteName))
            {
                visualSprite = Resources.Load<Sprite>(_pawnData.visualSpriteName);
            }

            // 찾을 수 없거나 지정되지 않은 경우 기본 원 스프라이트로 대체
            if (visualSprite == null)
            {
                visualSprite = Resources.Load<Sprite>("Temporary/Circle");
                if (visualSprite == null)
                {
                    Debug.LogError("VisualSubManager: 'Resources/Temporary/Circle'에서 기본 원 스프라이트를 찾을 수 없습니다. 시각적 개체가 인스턴스화되지 않습니다.", this);
                    return; // 렌더링할 스프라이트 없음
                }
            }

            _spriteRenderer.sprite = visualSprite;
            _spriteRenderer.color = _pawnData.visualColor; // 색상 적용
        }

        public override void SubUpdate()
        {
            // 시각적 업데이트가 있는 경우 여기에 배치합니다. 간단한 스프라이트의 경우 종종 아무것도 필요하지 않습니다.
        }
    }
}
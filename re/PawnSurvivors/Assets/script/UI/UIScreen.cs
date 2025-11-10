using UnityEngine;

namespace PawnSurvivors.UI
{
    /// <summary>
    /// 모든 UI 화면의 기반 클래스입니다.
    /// 화면의 표시/숨김 및 생명주기를 관리합니다.
    /// </summary>
    public abstract class UIScreen : MonoBehaviour
    {
        [SerializeField] protected Canvas canvas;
        [SerializeField] protected CanvasGroup canvasGroup;

        protected virtual void Awake()
        {
            if (canvas == null)
                canvas = GetComponent<Canvas>();
            
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            
            // 기본적으로 숨김 상태로 시작
            Hide();
        }

        /// <summary>
        /// 화면을 표시합니다.
        /// </summary>
        public virtual void Show()
        {
            gameObject.SetActive(true);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            OnShow();
        }

        /// <summary>
        /// 화면을 숨깁니다.
        /// </summary>
        public virtual void Hide()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            gameObject.SetActive(false);
            OnHide();
        }

        /// <summary>
        /// Show 호출 시 실행되는 가상 메서드입니다.
        /// </summary>
        protected virtual void OnShow() { }

        /// <summary>
        /// Hide 호출 시 실행되는 가상 메서드입니다.
        /// </summary>
        protected virtual void OnHide() { }
    }
}


using UnityEngine;

namespace PawnSurvivors.Managers
{
    /// <summary>
    /// 화면 비율을 강제로 고정하고 레터박스를 추가하는 매니저
    /// </summary>
    public class AspectRatioManager : MonoBehaviour
    {
        [Header("Target Aspect Ratio")]
        [Tooltip("목표 화면 비율 (16:9 = 1.777...)")]
        [SerializeField] private float targetAspect = 16f / 9f;
        
        [Header("Letterbox Settings")]
        [Tooltip("레터박스 색상 (보통 검은색)")]
        [SerializeField] private Color letterboxColor = Color.black;
        
        private Camera _mainCamera;
        private Camera _letterboxCamera;
        
        private void Awake()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                Debug.LogError("[AspectRatioManager] Main Camera를 찾을 수 없습니다!");
                return;
            }
            
            CreateLetterboxCamera();
            ApplyLetterbox();
        }
        
        private void Update()
        {
            // 화면 크기가 변경되면 레터박스 재적용
            ApplyLetterbox();
        }
        
        /// <summary>
        /// 레터박스용 배경 카메라 생성
        /// </summary>
        private void CreateLetterboxCamera()
        {
            GameObject letterboxObj = new GameObject("LetterboxCamera");
            letterboxObj.transform.SetParent(transform);
            
            _letterboxCamera = letterboxObj.AddComponent<Camera>();
            _letterboxCamera.depth = _mainCamera.depth - 1; // 메인 카메라보다 뒤에
            _letterboxCamera.clearFlags = CameraClearFlags.SolidColor;
            _letterboxCamera.backgroundColor = letterboxColor;
            _letterboxCamera.cullingMask = 0; // 아무것도 렌더링하지 않음
            _letterboxCamera.orthographic = true;
            _letterboxCamera.orthographicSize = 1;
            _letterboxCamera.nearClipPlane = -1;
            _letterboxCamera.farClipPlane = 1;
        }
        
        /// <summary>
        /// 레터박스 적용
        /// </summary>
        private void ApplyLetterbox()
        {
            if (_mainCamera == null) return;
            
            // 현재 화면 비율
            float windowAspect = (float)Screen.width / Screen.height;
            
            // 목표 비율과 비교
            float scaleHeight = windowAspect / targetAspect;
            
            Rect rect = _mainCamera.rect;
            
            if (scaleHeight < 1.0f)
            {
                // 화면이 목표보다 세로로 긴 경우 (위아래에 레터박스)
                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;
            }
            else
            {
                // 화면이 목표보다 가로로 긴 경우 (좌우에 레터박스)
                float scaleWidth = 1.0f / scaleHeight;
                rect.width = scaleWidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scaleWidth) / 2.0f;
                rect.y = 0;
            }
            
            _mainCamera.rect = rect;
        }
        
        /// <summary>
        /// 목표 비율 변경
        /// </summary>
        public void SetTargetAspect(float width, float height)
        {
            targetAspect = width / height;
            ApplyLetterbox();
        }
        
        /// <summary>
        /// 레터박스 색상 변경
        /// </summary>
        public void SetLetterboxColor(Color color)
        {
            letterboxColor = color;
            if (_letterboxCamera != null)
            {
                _letterboxCamera.backgroundColor = color;
            }
        }
    }
}


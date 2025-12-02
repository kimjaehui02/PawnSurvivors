using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PawnSurvivors.Managers
{
    /// <summary>
    /// 화면 비율을 강제로 고정하고 레터박스를 추가하는 매니저
    /// Pixel Perfect Camera와 통합하여 작동합니다.
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
        private PixelPerfectCamera _pixelPerfectCamera;
        private float _lastWidth;
        private float _lastHeight;
        private bool _isInitialized = false;
        
        private void Start()
        {
            Debug.Log("[AspectRatioManager] Start() 호출됨");
            // Start에서 초기화 (다른 컴포넌트들이 Awake에서 카메라를 설정한 후)
            FindAndSetupCamera();
        }
        
        private void Update()
        {
            // 카메라를 아직 못 찾았으면 다시 시도
            if (!_isInitialized)
            {
                FindAndSetupCamera();
                return;
            }
            
            // 화면 크기가 변경되었을 때만 레터박스 재적용
            if (Screen.width != _lastWidth || Screen.height != _lastHeight)
            {
                Debug.Log($"[AspectRatioManager] 화면 크기 변경 감지: {_lastWidth}x{_lastHeight} → {Screen.width}x{Screen.height}");
                _lastWidth = Screen.width;
                _lastHeight = Screen.height;
                ApplyLetterbox();
            }
        }
        
        private void FindAndSetupCamera()
        {
            // 씬의 모든 카메라 찾기
            Camera[] allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            Debug.Log($"[AspectRatioManager] 씬에서 찾은 카메라 수: {allCameras.Length}");
            
            // Pixel Perfect Camera가 있는 카메라 찾기
            foreach (var cam in allCameras)
            {
                Debug.Log($"[AspectRatioManager] 카메라 확인 중: {cam.name}");
                
                // 카메라의 모든 컴포넌트 출력
                var components = cam.GetComponents<Component>();
                string componentList = string.Join(", ", System.Array.ConvertAll(components, c => c.GetType().Name));
                Debug.Log($"[AspectRatioManager] {cam.name}의 컴포넌트 목록: {componentList}");
                
                // 타입 이름으로 직접 찾기
                Component ppcComponent = null;
                foreach (var comp in components)
                {
                    if (comp.GetType().Name == "PixelPerfectCamera")
                    {
                        ppcComponent = comp;
                        Debug.Log($"[AspectRatioManager] PixelPerfectCamera 타입 찾음! FullName: {comp.GetType().FullName}");
                        break;
                    }
                }
                
                if (ppcComponent != null)
                {
                    _mainCamera = cam;
                    _pixelPerfectCamera = ppcComponent as PixelPerfectCamera;
                    Debug.Log($"[AspectRatioManager] Pixel Perfect Camera 찾음: {cam.name}");
                    break;
                }
                else
                {
                    Debug.Log($"[AspectRatioManager] PixelPerfectCamera 타입을 찾지 못했습니다.");
                }
            }
            
            // Pixel Perfect Camera가 없으면 메인 카메라 사용
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null)
                {
                    GameObject cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
                    if (cameraObj != null)
                    {
                        _mainCamera = cameraObj.GetComponent<Camera>();
                    }
                }
            }
            
            if (_mainCamera == null)
            {
                Debug.LogWarning("[AspectRatioManager] 카메라를 찾지 못했습니다. 다음 프레임에 재시도...");
                return;
            }
            
            Debug.Log($"[AspectRatioManager] 사용할 카메라: {_mainCamera.name}");
            
            // Pixel Perfect Camera 설정
            if (_pixelPerfectCamera != null)
            {
                _pixelPerfectCamera.cropFrame = PixelPerfectCamera.CropFrame.StretchFill;
                Debug.Log($"[AspectRatioManager] Pixel Perfect Camera Crop Frame 활성화: cropFrame={_pixelPerfectCamera.cropFrame}");
            }
            else
            {
                Debug.Log("[AspectRatioManager] Pixel Perfect Camera 없음. 수동 레터박스 모드 사용");
                // Pixel Perfect Camera가 없으면 수동 레터박스 생성
                if (_letterboxCamera == null)
                {
                    CreateLetterboxCamera();
                }
            }
            
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
            _isInitialized = true;
            
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
            
            // Pixel Perfect Camera가 있으면 Crop Frame이 자동으로 레터박스 처리
            if (_pixelPerfectCamera != null)
            {
                Debug.Log("[AspectRatioManager] Pixel Perfect Camera가 레터박스 처리 중");
                // Pixel Perfect Camera가 알아서 처리하므로 추가 작업 불필요
                return;
            }
            
            // Pixel Perfect Camera가 없을 때만 수동 레터박스 적용
            float windowAspect = (float)Screen.width / Screen.height;
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


using UnityEngine;
using PawnCore.Domain;
using PawnCore.Domain.Events;
using PawnSurvivors.UI;
using PawnSurvivors.Domain.Usecases;
using System.Collections.Generic;
using TMPro;

namespace PawnSurvivors.Managers
{
    /// <summary>
    /// 범용 FloatingEffect 시스템을 관리하는 매니저입니다.
    /// Presentation 계층의 일부로, UI 표시를 담당합니다.
    /// GameManager에 컴포넌트로 추가하세요.
    /// </summary>
    public class FloatingEffectManager : MonoBehaviour
    {
        [Header("Canvas 설정")]
        [SerializeField] private Canvas worldSpaceCanvas;

        [Header("데미지 효과 설정")]
        [SerializeField] private Color damageTextColor = Color.red;
        [SerializeField] private float damageTextFontSize = 24f;
        [SerializeField] private float damageTextDisplayScale = 0.1f; // 폰트와 별개의 표시 크기 (작게 조정)
        [SerializeField] private float damageTextMoveDistance = 1f;
        [SerializeField] private float damageTextDuration = 0.5f;
        [SerializeField] private TMP_FontAsset damageTextFont;

        [Header("파티클 효과 설정")]
        [SerializeField] private string defaultParticlePrefabPath = ""; // Resources 경로

        [Header("풀링 시스템")]
        [SerializeField] private FloatingEffectPool effectPool;

        private Camera _mainCamera;
        private List<PawnManager> _subscribedPawns = new List<PawnManager>();
        private FloatingEffectUseCase _floatingEffectUseCase;

        private void Awake()
        {
            // UseCase 초기화
            _floatingEffectUseCase = new FloatingEffectUseCase();
            
            SetupWorldSpaceCanvas();
            InitializePool();
            
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                _mainCamera = FindFirstObjectByType<Camera>();
            }
        }

        private void OnEnable()
        {
            SubscribeToDamageEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromDamageEvents();
        }

        private void SetupWorldSpaceCanvas()
        {
            if (worldSpaceCanvas != null) return;

            Canvas existingCanvas = FindFirstObjectByType<Canvas>();
            if (existingCanvas != null && existingCanvas.renderMode == RenderMode.WorldSpace)
            {
                worldSpaceCanvas = existingCanvas;
                return;
            }

            // 새로 생성
            GameObject canvasObj = new GameObject("FloatingEffectCanvas");
            worldSpaceCanvas = canvasObj.AddComponent<Canvas>();
            worldSpaceCanvas.renderMode = RenderMode.WorldSpace;
            
            var scaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.scaleFactor = 0.001f; // 월드 공간에 맞게 스케일 조정 (더 작게 조정)
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
            if (_mainCamera != null)
            {
                worldSpaceCanvas.worldCamera = _mainCamera;
            }

            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(100f, 100f);
        }

        private void InitializePool()
        {
            if (effectPool == null)
            {
                effectPool = gameObject.AddComponent<FloatingEffectPool>();
            }
            effectPool.Initialize(worldSpaceCanvas.transform);
        }

        private void SubscribeToDamageEvents()
        {
            UnsubscribeFromDamageEvents();

            foreach (var pawnManager in PawnManager.AllPawnManagers)
            {
                if (pawnManager != null)
                {
                    pawnManager.Subscribe<PawnDamagedEvent>(HandlePawnDamaged);
                    _subscribedPawns.Add(pawnManager);
                }
            }
        }

        private void UnsubscribeFromDamageEvents()
        {
            foreach (var pawnManager in _subscribedPawns)
            {
                if (pawnManager != null)
                {
                    pawnManager.Unsubscribe<PawnDamagedEvent>(HandlePawnDamaged);
                }
            }
            _subscribedPawns.Clear();
        }

        /// <summary>
        /// 특정 PawnManager를 구독 목록에 추가합니다.
        /// </summary>
        public void SubscribeToPawnManager(PawnManager pawnManager)
        {
            if (pawnManager != null && !_subscribedPawns.Contains(pawnManager))
            {
                pawnManager.Subscribe<PawnDamagedEvent>(HandlePawnDamaged);
                _subscribedPawns.Add(pawnManager);
            }
        }

        private void HandlePawnDamaged(PawnDamagedEvent evt)
        {
            // UseCase를 통해 비즈니스 로직 처리
            FloatingEffectData effectData = _floatingEffectUseCase.CreateDamageTextFromEvent(
                evt,
                damageTextColor,
                damageTextFontSize,
                damageTextDisplayScale,
                damageTextMoveDistance,
                damageTextDuration
            );

            if (effectData != null)
            {
                effectData.FontAsset = damageTextFont;
                ShowFloatingEffect(effectData);
            }
        }

        /// <summary>
        /// 범용 FloatingEffect를 표시합니다.
        /// </summary>
        public void ShowFloatingEffect(FloatingEffectData data)
        {
            if (effectPool == null)
            {
                Debug.LogError("[FloatingEffectManager] FloatingEffectPool이 초기화되지 않았습니다.");
                return;
            }
            
            FloatingEffect effect = effectPool.GetFromPool(data.EffectType, data);
            if (effect != null)
            {
                effect.Show(data);
            }
        }

        /// <summary>
        /// 데미지 텍스트를 표시합니다.
        /// (직접 호출용 - 이벤트 기반이 아닌 경우)
        /// </summary>
        public void ShowDamageText(Vector3 position, float damage)
        {
            if (damage <= 0) return;

            // UseCase를 통해 데이터 생성 (직접 호출용)
            FloatingEffectData data = FloatingEffectData.CreateDamageText(
                position,
                damage,
                damageTextColor,
                damageTextFontSize,
                damageTextDisplayScale,
                damageTextMoveDistance,
                damageTextDuration
            );

            if (data != null)
            {
                data.FontAsset = damageTextFont;
                ShowFloatingEffect(data);
            }
        }

        /// <summary>
        /// 파티클 효과를 표시합니다.
        /// </summary>
        public void ShowParticleEffect(Vector3 position, string prefabPath = null, float duration = 1f)
        {
            string path = prefabPath ?? defaultParticlePrefabPath;
            
            // UseCase를 통해 데이터 생성
            FloatingEffectData data = _floatingEffectUseCase.CreateParticleEffectData(position, path, duration);
            
            if (data != null)
            {
                ShowFloatingEffect(data);
            }
            else
            {
                Debug.LogWarning("[FloatingEffectManager] 파티클 프리팹 경로가 지정되지 않았습니다.");
            }
        }
    }
}

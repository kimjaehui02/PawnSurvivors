using UnityEngine;
using PawnCore.Domain;

namespace PawnCore.Presentation.SubManagers.Visual
{
    public class VisualSubManager : PawnSubManager
    {
        private PawnData _pawnData;
        private SpriteRenderer _spriteRenderer; // Add SpriteRenderer reference

        public override void SubStart()
        {
            _pawnData = _pawnManager.PawnData;

            // Add or get SpriteRenderer component
            _spriteRenderer = _pawnManager.gameObject.GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
            {
                _spriteRenderer = _pawnManager.gameObject.AddComponent<SpriteRenderer>();
            }

            Sprite visualSprite = null;
            // Try to load the specified sprite
            if (!string.IsNullOrEmpty(_pawnData.visualSpriteName))
            {
                visualSprite = Resources.Load<Sprite>(_pawnData.visualSpriteName);
                // if (visualSprite == null)
                // {
                //     Debug.LogWarning($"VisualSubManager: Sprite '{_pawnData.visualSpriteName}' not found. Using default circle sprite.", this);
                // }
            }

            // Fallback to default circle sprite if not found or not specified
            if (visualSprite == null)
            {
                visualSprite = Resources.Load<Sprite>("Temporary/Circle");
                if (visualSprite == null)
                {
                    Debug.LogError("VisualSubManager: Default circle sprite not found at 'Resources/Temporary/Circle'. No visual will be instantiated.", this);
                    return; // No sprite to render
                }
            }

            _spriteRenderer.sprite = visualSprite;
            _spriteRenderer.color = _pawnData.visualColor; // Apply color
        }

        public override void SubUpdate()
        {
            // Visual updates, if any, go here. For simple sprites, often nothing is needed.
        }
    }
}
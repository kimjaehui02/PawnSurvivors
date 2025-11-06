using UnityEngine;
using PawnCore.Domain;

namespace PawnCore.Presentation.SubManagers.Visual
{
    public class VisualSubManager : PawnSubManager
    {
        private PawnData _pawnData;

        public override void SubStart()
        {
            _pawnData = _pawnManager.PawnData;

            if (string.IsNullOrEmpty(_pawnData.visualPrefabName))
            {
                Debug.LogWarning("Visual prefab name is not assigned in PawnData. No visual will be instantiated.", this);
                return;
            }

            GameObject visualPrefab = Resources.Load<GameObject>(_pawnData.visualPrefabName);
            if (visualPrefab != null)
            {
                Instantiate(visualPrefab, this.transform);
            }
            else
            {
                // Debug.LogWarning($"Visual prefab not found at path: '{_pawnData.visualPrefabName}'. Attempting to load default Circle prefab.", this);
                GameObject defaultVisualPrefab = Resources.Load<GameObject>("Prefabs/Circle");
                if (defaultVisualPrefab != null)
                {
                    Instantiate(defaultVisualPrefab, this.transform);
                }
                else
                {
                    // Debug.LogError("Default Circle prefab not found at path: 'Prefabs/Circle'. No visual will be instantiated.", this);
                }
            }
        }

        public override void SubUpdate()
        {
            // Visuals do not need per-frame update by default.
        }
    }
}

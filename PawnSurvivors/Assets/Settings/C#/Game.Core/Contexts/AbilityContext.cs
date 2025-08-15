using UnityEngine;

namespace Game.Core.Contexts
{
    public class AbilityContext
    {
        public Pawn SourcePawn { get; set; }
        public Pawn TargetPawn { get; set; }
        public Vector3? InputDirection { get; set; }
        public float? DamageAmount { get; set; }
    }
}

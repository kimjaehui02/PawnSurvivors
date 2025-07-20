using UnityEngine; // Vector3, GameObject 등을 위해 필요
using System;      // Nullable 타입 (?)을 위해 필요

namespace Game.Core // 프로젝트 구조에 맞게 네임스페이스 조정
{


    /// <summary>
    /// 게임 내에서 발생하는 다양한 행동(Ability)의 맥락(Context) 정보를 담는 클래스입니다.
    /// 모든 델리게이트 시그니처에 Action<AbilityContext>를 유지하면서,
    /// 필요한 정보만 선택적으로 제공하여 정보 과다를 줄이고,
    /// SourcePawn과 TargetPawn으로 행동의 주체와 대상을 명확히 합니다.
    /// </summary>
    public class AbilityContext
    {
        // --- 핵심 정보 (대부분의 Acts에서 유용) ---
        // 누가 이 행동을 시작했는가? (주체/발신자)
        public Pawn SourcePawn { get; set; }

        // 누가 이 행동의 대상인가? (수신자)
        // Self-action의 경우 SourcePawn과 동일하거나, 해당 PawnAction이 부착된 Pawn을 의미.
        public Pawn TargetPawn { get; set; }



        // 이동 관련 입력 방향 (Acts.OnMove 등에서 사용, 없을 시 null)
        public Vector3? InputDirection { get; set; }

        // --- 단발성 이벤트 정보 (각 Acts에 따라 선택적으로 사용) ---
        // Acts.OnHit, OnDamage 등 피해 관련
        public float? DamageAmount { get; set; }
        //public float? DamageAmount { get; set; }
        //public float? DamageAmount { get; set; }



    }
}
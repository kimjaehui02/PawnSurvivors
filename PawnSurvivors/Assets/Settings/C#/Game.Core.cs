// GameCore.cs 파일
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Game.Core // 또는 당신의 게임 이름.Core
{
    public enum Acts // 델리게이트 키로 사용될 핵심 행동 정의
    {
        OnMove,
        OnDamaged,
        OnDeath,
        OnAttack,
        OnHeal,
        // ...
    }

    /// <summary>
    /// 특정 Acts에 대한 Action 델리게이트 딕셔너리를 제공하는 인터페이스.
    /// </summary>
    //public interface IActionProvider
    //{
    //    /// <summary>
    //    /// 이 제공자가 관리하는 Acts와 연결된 Action 델리게이트 딕셔너리를 가져옵니다.
    //    /// </summary>
    //    // 인터페이스 멤버는 기본적으로 public이므로, 접근자를 명시하지 않습니다.
    //    // 여기에 'public'을 붙이면 컴파일 오류가 발생할 수 있습니다 (C# 버전/설정에 따라 다름).
    //    Dictionary<Acts, Action<AbilityContext>> ActionDelegates { get; }

    //}

    /// <summary>
    /// Acts와 연결된 Action 델리게이트 딕셔너리를 관리하는 기능을 제공하는 인터페이스입니다.
    /// </summary>
    public interface IActionMapManager
    {
        /// <summary>
        /// 이 관리자가 소유한 Acts-Action 델리게이트 딕셔너리에 대한 읽기 전용 뷰를 가져옵니다.
        /// 외부에서 딕셔너리 내용을 직접 변경(추가/제거)할 수 없도록 IReadOnlyDictionary를 사용합니다.
        /// </summary>
        IReadOnlyDictionary<Acts, Action<AbilityContext>> GetActions { get; }

        /// <summary>
        /// 지정된 Acts에 대한 Action 델리게이트를 추가하거나, 이미 존재하는 경우 기존 델리게이트에 연결합니다.
        /// </summary>
        /// <param name="act">등록할 Acts Enum 값.</param>
        /// <param name="action">해당 Acts에 연결할 Action 델리게이트.</param>
        void AddAction(Acts act, Action<AbilityContext> action);

        /// <summary>
        /// 지정된 Acts에 대한 특정 Action 델리게이트를 제거하거나,
        /// action 매개변수가 null인 경우 해당 Acts에 연결된 모든 델리게이트를 제거합니다.
        /// </summary>
        /// <param name="act">제거할 대상 Acts Enum 값.</param>
        /// <param name="action">제거할 특정 Action 델리게이트. null이면 해당 Acts의 모든 델리게이트를 제거합니다.</param>
        void RemoveAction(Acts act, Action<AbilityContext> action = null);

        // 참고: InvokeAction 메서드는 델리게이트 맵 '관리' 기능이라기보다는 '사용' 기능에 가깝습니다.
        // 딕셔너리를 직접 노출하는 GetActions를 통해 외부에서 Invoke를 할 수 있으므로,
        // 이 인터페이스에 반드시 포함될 필요는 없습니다. 하지만 필요하다면 추가할 수 있습니다.
        // void InvokeAction(Acts act, AbilityContext context);
    }


    /// <summary>
    /// 델리게이트에 필요한 모든 맥락 정보를 담는 클래스.
    /// </summary>
    public class AbilityContext
    {
        public Vector3 direction;
    }



    // 여기에 나중에 다른 공통 Enum이나 인터페이스 등을 추가할 수도 있습니다.
    // public interface IHealth { float CurrentHealth { get; } }
}
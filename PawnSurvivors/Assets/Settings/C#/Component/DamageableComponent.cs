using UnityEngine;
using System;
using Game.Core; // Acts 및 AbilityContext를 위해 추가

/// <summary>
/// IDamageable 인터페이스를 실제로 구현하여 체력 관리 및 피해/회복 처리 로직을 담당하는 컴포넌트입니다.
/// 이 컴포넌트를 플레이어, 몬스터 등 체력이 필요한 GameObject에 붙여서 사용합니다.
/// </summary>
public class DamageableComponent : PawnAction
{
    #region Fields & Properties

    [SerializeField] // 유니티 에디터에서 최대 체력을 설정할 수 있도록 노출
    private float _maxHealth = 100f; // 기본값 설정 (선택 사항)
    public float MaxHealth => _maxHealth; // 최대 체력은 읽기 전용 프로퍼티로 외부 노출

    [SerializeField] // 유니티 에디터에서 현재 체력을 초기 설정할 수 있도록 노출
    private float _currentHealth; // 현재 체력의 실제 값을 저장하는 백킹 필드

    /// <summary>
    /// 현재 체력입니다. 외부에서 읽기만 가능합니다.
    /// 체력 변경은 TakeDamage 또는 별도의 힐 메서드를 통해서만 이루어집니다.
    /// </summary>
    public float CurrentHealth // 현재 체력 프로퍼티
    {
        get { return _currentHealth; }
        // private set을 사용하여 외부에서는 값을 직접 변경할 수 없고,
        // 클래스 내부 메서드(TakeDamage, InitializeHealth 등)를 통해서만 변경 가능하도록 제한합니다.
        private set
        {
            _currentHealth = Mathf.Clamp(value, 0, _maxHealth); // 체력을 0과 MaxHealth 사이로 클램프
            // TODO: 체력이 변경될 때마다 UI 업데이트 등의 이벤트를 발생시킬 수 있습니다.
            // OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
    }

    // OnDamaged 이벤트는 RequestAction 시스템을 사용하므로 직접적인 Action 이벤트는 불필요합니다.
    // 하지만, 만약 이 컴포넌트 외부에서만 구독할 수 있는 고유한 이벤트를 원한다면 활성화할 수 있습니다.
    // public event Action<float> OnDamaged;

    #endregion

    #region Ability Registration

    /// <summary>
    /// PawnAction의 RegisterAbilities를 오버라이드하여
    /// 이 컴포넌트의 능력을 Pawn의 델리게이트 시스템에 등록합니다.
    /// </summary>
    public override void RegisterAbilities()
    {
        // Acts.OnDamaged 이벤트가 발생했을 때 TakeDamage 메서드를 호출하도록 등록합니다.
        // AbilityContext를 통해 피해량 정보를 받아 처리합니다.
        AddAction(Acts.OnDamaged, TakeDamage);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 이 컴포넌트의 체력을 초기화합니다.
    /// 예를 들어 Pawn이 생성될 때 호출될 수 있습니다.
    /// 이 메서드는 현재 체력을 최대 체력으로 설정합니다.
    /// </summary>
    public void InitializeHealth()
    {
        // 초기화 시 현재 체력을 최대 체력으로 설정합니다.
        // 에디터에서 _currentHealth에 직접 값을 설정했다면, 그 값이 초기값으로 사용됩니다.
        // 하지만 일반적으로는 게임 시작 시 _maxHealth로 초기화하는 경우가 많습니다.
        CurrentHealth = _maxHealth;
    }

    /// <summary>
    /// 피해를 입었을 때 호출되는 메서드입니다.
    /// Acts.OnDamaged 델리게이트에 연결됩니다.
    /// </summary>
    /// <param name="abilityContext">피해량 정보(damageAmount)를 포함하는 컨텍스트.</param>
    public void TakeDamage(AbilityContext abilityContext)
    {
        // TODO: AbilityContext에 damageAmount 필드가 있다고 가정하고 피해를 적용합니다.
         CurrentHealth -= abilityContext.damageAmount;

        // 디버그: 전달받은 AbilityContext 값을 로그로 출력합니다.
        Debug.Log($"DamageableComponent: {name}이(가) AbilityContext로부터 피해를 받았습니다. Context: {abilityContext}");

        // TODO: 체력이 0 이하가 되었을 때 사망 처리 로직을 트리거할 수 있습니다.
        // if (CurrentHealth <= 0) { RequestAction(Acts.OnDeath, new AbilityContext()); }
    }

    #endregion
}
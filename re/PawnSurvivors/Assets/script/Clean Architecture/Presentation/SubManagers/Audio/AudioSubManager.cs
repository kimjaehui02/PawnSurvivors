using UnityEngine;
using PawnSurvivors.Domain.Events;
using PawnSurvivors.Utilities;

/// <summary>
/// Pawn의 사운드를 관리하는 SubManager입니다.
/// 이벤트를 구독하여 적절한 사운드를 재생합니다.
/// </summary>
public class AudioSubManager : PawnSubManager
{
    [Header("Sound Settings")]
    [Tooltip("피격 사운드 (여러 개면 랜덤 재생)")]
    public string[] hitSounds = new string[] { "Audio/Hit0", "Audio/Hit1" };
    
    [Tooltip("사망 사운드")]
    public string deathSound = "Audio/Dead";
    
    [Tooltip("공격 사운드 (근접)")]
    public string meleeSounds = "Audio/Melee0";
    
    [Tooltip("공격 사운드 (원거리)")]
    public string rangeSound = "Audio/Range";
    
    [Tooltip("레벨업 사운드")]
    public string levelUpSound = "Audio/LevelUp";
    
    [Tooltip("사운드 볼륨")]
    public float volume = 1f;

    public override void SubStart()
    {
        // 피격 이벤트 구독
        _pawnManager.Subscribe<PawnDamagedEvent>(OnPawnDamaged);
        
        // 사망 이벤트 구독
        _pawnManager.Subscribe<PawnDeathEvent>(OnPawnDeath);
    }

    private void OnDisable()
    {
        if (_pawnManager != null)
        {
            _pawnManager.Unsubscribe<PawnDamagedEvent>(OnPawnDamaged);
            _pawnManager.Unsubscribe<PawnDeathEvent>(OnPawnDeath);
        }
    }

    public override void SubUpdate()
    {
        // 사운드는 이벤트 기반으로 재생되므로 Update 불필요
    }

    /// <summary>
    /// Pawn이 데미지를 받았을 때 호출됩니다.
    /// </summary>
    private void OnPawnDamaged(PawnDamagedEvent evt)
    {
        if (evt.Target == _pawnManager)
        {
            // 피격 사운드 재생 (여러 개면 랜덤)
            if (hitSounds != null && hitSounds.Length > 0)
            {
                AudioHelper.PlayRandomSound(hitSounds, transform.position, volume);
            }
        }
    }

    /// <summary>
    /// Pawn이 사망했을 때 호출됩니다.
    /// </summary>
    private void OnPawnDeath(PawnDeathEvent evt)
    {
        if (evt.DeadPawn == _pawnManager)
        {
            // 사망 사운드 재생
            if (!string.IsNullOrEmpty(deathSound))
            {
                AudioHelper.PlaySound(deathSound, transform.position, volume);
            }
        }
    }
}


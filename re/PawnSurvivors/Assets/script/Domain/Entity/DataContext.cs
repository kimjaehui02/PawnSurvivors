using System.Collections.Generic;
using UnityEngine;

public class DataContext 
{
    public GameObject Activator { get; set; }  // 발동자
    public GameObject Target { get; set; }     // 대상
    public Vector3? InputDirection { get; set; }  // 입력 방향
    public float MoveSpeed { get; set; }  // 이동 속도
    
    public Dictionary<FloatKeys, float> FloatData { get; set; } = new();
    public Dictionary<Vector3Keys, Vector3> Vector3Data { get; set; } = new();
    public Dictionary<BoolKeys, bool> BoolData { get; set; } = new();
}

public enum FloatKeys
{
    MoveSpeed,
    Damage,
    Health,
    AttackRange
}

public enum Vector3Keys
{
    InputDirection,
    TargetPosition,
    SpawnPosition
}

public enum BoolKeys
{
    IsMoving,
    IsAttacking,
    IsAlive
}

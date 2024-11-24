using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    public float Health = 100.0f;
    public float Damage = 1.0f;
    public float Speed = 5.0f;
    public float AttackCooldown = 0.5f;
    public float AttackRange = 2.0f;
}

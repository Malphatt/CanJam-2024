using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    public float health = 100.0f;
    public float speed = 5.0f;
}

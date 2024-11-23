using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyReferences : MonoBehaviour
{
    public NavMeshAgent NavMesh;
    public Animator Animator;

    public GameObject UpsideEnemy;
    public GameObject DownsideEnemy;

    public EnemyData EnemyData;
}

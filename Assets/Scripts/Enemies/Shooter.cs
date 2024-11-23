using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Shooter : Enemy
{
    [SerializeField]
    private NavMeshAgent _navMeshAgent;
    [SerializeField]
    private Animator _animator;

    private StateMachine _stateMachine;

    private void Start()
    {
        _navMeshAgent.speed = _enemyData.speed;

        _stateMachine = new StateMachine();

        // States

        // Transitions

        // Set initial state

        // Funtions & Conditions
        //void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        //void Any(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition(to, condition);
    }

    private new void Update()
    {
        base.Update();

        _stateMachine.Tick();
    }

    private void OnDrawGizmos()
    {
        if (_stateMachine != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 3.0f, 0.4f);
        }
    }
}

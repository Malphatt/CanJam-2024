using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Shooter : Enemy
{
    [SerializeField]
    private NavMeshAgent _navMeshAgent;

    private Transform _player;

    [SerializeField]
    private LayerMask _whatIsGround, _whatIsPlayer;

    // Patroling
    [SerializeField]
    private Vector3 _walkPoint;
    private bool _walkPointSet;
    [SerializeField]
    private float _walkPointRange;

    // Attacking
    [SerializeField]
    private float _timeBetweenAttacks;
    private bool _alreadyAttacked;

    // States
    [SerializeField]
    private float _sightRange, _attackRange;
    [SerializeField]
    private bool _playerInSightRange, _playerInAttackRange;

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<PlayerController>().NormalPlayer.transform;

        _navMeshAgent.speed = _enemyData.Speed;
        _timeBetweenAttacks = _enemyData.AttackCooldown;
        _attackRange = _enemyData.AttackRange;
    }

    private new void Update()
    {
        base.Update();

        // Check for sight and attack range
        _playerInSightRange = Physics.CheckSphere(_UpsideEnemy.transform.position, _sightRange, _whatIsPlayer);
        _playerInAttackRange = Physics.CheckSphere(_UpsideEnemy.transform.position, _attackRange, _whatIsPlayer);

        if (!_playerInSightRange && !_playerInAttackRange) Patroling();
        if (_playerInSightRange && !_playerInAttackRange) ChasePlayer();
        if (_playerInSightRange && _playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!_walkPointSet) SearchWalkPoint();

        if (_walkPointSet)
            _navMeshAgent.SetDestination(_walkPoint);

        Vector3 distanceToWalkPoint = _UpsideEnemy.transform.position - _walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            _walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-_walkPointRange, _walkPointRange);
        float randomX = Random.Range(-_walkPointRange, _walkPointRange);

        _walkPoint = new Vector3(_UpsideEnemy.transform.position.x + randomX, _UpsideEnemy.transform.position.y, _UpsideEnemy.transform.position.z + randomZ);

        if (Physics.Raycast(_walkPoint, -_UpsideEnemy.transform.up, 2f, _whatIsGround))
            _walkPointSet = true;
    }

    private void ChasePlayer()
    {
        _navMeshAgent.SetDestination(_player.position);
    }

    private void AttackPlayer()
    {
        _navMeshAgent.SetDestination(_UpsideEnemy.transform.position);

        _UpsideEnemy.transform.LookAt(_player);

        if (!_alreadyAttacked)
        {
            // Attack code here


            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack), _timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        _alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_UpsideEnemy.transform.position, _attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_UpsideEnemy.transform.position, _sightRange);
    }
}

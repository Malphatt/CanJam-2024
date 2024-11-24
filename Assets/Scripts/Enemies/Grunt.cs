using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : Enemy
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


    [SerializeField]
    private Animator _animator;

    public GameObject Beebop;

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<PlayerController>().NormalPlayer.transform;

        _navMeshAgent.speed = _enemyData.Speed;
        _timeBetweenAttacks = _enemyData.AttackCooldown;
        _attackRange = _enemyData.AttackRange;
        _animator = Beebop.GetComponent<Animator>();
    }

    private new void Update()
    {
        base.Update();

        Debug.DrawRay(_UpsideEnemy.transform.position, _UpsideEnemy.transform.forward * 5.0f, Color.red);

        // Check for sight and attack range
        _playerInSightRange = Physics.CheckSphere(_UpsideEnemy.transform.position, _sightRange, _whatIsPlayer);
        _playerInAttackRange = Physics.CheckSphere(_UpsideEnemy.transform.position, _attackRange, _whatIsPlayer);

        if (!_playerInSightRange && !_playerInAttackRange) Patroling();
        if (_playerInSightRange && !_playerInAttackRange) ChasePlayer();
        if (_playerInSightRange && _playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        _animator.SetBool("Chasing", true);
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

    private void ChasePlayer() => _navMeshAgent.SetDestination(_player.position);

    private void AttackPlayer()
    {
        _animator.SetBool("Chasing", true); ;
        _navMeshAgent.SetDestination(_UpsideEnemy.transform.position);

        _UpsideEnemy.transform.LookAt(_player);

        if (!_alreadyAttacked)
        {
            // Attack code here
            // Raycast forward
            if (Physics.Raycast(_UpsideEnemy.transform.position, _UpsideEnemy.transform.forward, out RaycastHit hit, 5.0f, _whatIsPlayer))
            {
                if (hit.collider.GetComponent<PlayerController>()
                    || hit.collider.transform.parent.GetComponent<PlayerController>())
                {
                    hit.collider.GetComponent<PlayerController>()?.TakeDamage(_enemyData.Damage);
                    hit.collider.transform.parent.GetComponent<PlayerController>()?.TakeDamage(_enemyData.Damage);
                }
            }

            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack), _timeBetweenAttacks);
        }
    }

    private void ResetAttack() => _alreadyAttacked = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_UpsideEnemy.transform.position, _attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_UpsideEnemy.transform.position, _sightRange);
    }
}

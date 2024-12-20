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
    private float _bulletSpread = 0.15f;

    // States
    [SerializeField]
    private float _sightRange, _attackRange;
    [SerializeField]
    private bool _playerInSightRange, _playerInAttackRange;

    [SerializeField]
    private Transform _muzzlePoint;

    [SerializeField]
    private GameObject _bulletPrefab;

    public AudioSource audioSource;
    public AudioClip beep1;
    public AudioClip shoot1;

    [SerializeField]
    private Animator _animator;

    public GameObject mech;


    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<PlayerController>().NormalPlayer.transform;
        audioSource = GetComponent<AudioSource>();

        _navMeshAgent.speed = _enemyData.Speed;
        _timeBetweenAttacks = _enemyData.AttackCooldown;
        _attackRange = _enemyData.AttackRange;

        _animator = mech.GetComponent<Animator>();
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
        _animator.SetBool("inRange", false);
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
        audioSource.clip = beep1;
        audioSource.Play();
        _navMeshAgent.SetDestination(_player.position);
    }

    private void AttackPlayer()
    {
        _animator.SetBool("inRange", true);
        _navMeshAgent.SetDestination(_UpsideEnemy.transform.position);

        _UpsideEnemy.transform.LookAt(_player);

        if (!_alreadyAttacked)
        {
            audioSource.clip = shoot1;
            audioSource.Play();
            // Attack code here
            float distance = Vector3.Distance(_player.position, _UpsideEnemy.transform.position);
            Vector3 shootLocation = _player.position + new Vector3(
                Random.Range(-_bulletSpread, _bulletSpread) * distance,
                Random.Range(-_bulletSpread, _bulletSpread) * distance,
                Random.Range(-_bulletSpread, _bulletSpread) * distance
            );

            Debug.DrawRay(_UpsideEnemy.transform.position, shootLocation - _UpsideEnemy.transform.position, Color.red, 1.0f);
            Debug.DrawRay(_muzzlePoint.position, shootLocation - _muzzlePoint.position, Color.yellow, 1.0f);

            Quaternion shootDirection = Quaternion.LookRotation(shootLocation - _muzzlePoint.position);
            Instantiate(_bulletPrefab, _muzzlePoint.position, shootDirection);

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

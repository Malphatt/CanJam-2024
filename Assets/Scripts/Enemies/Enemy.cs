using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected EnemyData _enemyData;

    [SerializeField]
    private GameObject _UpsideEnemy;

    [SerializeField]
    private GameObject _DownsideEnemy;

    private float _remainingHealth;

    private Rigidbody _rb;

    private void Awake()
    {
        _remainingHealth = _enemyData.health;

        _rb = _UpsideEnemy.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Move and rotate the DownsideEnemy
        _DownsideEnemy.transform.position = new Vector3(
            _UpsideEnemy.transform.position.x,
            -_UpsideEnemy.transform.position.y,
            _UpsideEnemy.transform.position.z
        );

        _DownsideEnemy.transform.rotation = Quaternion.Euler(
            _UpsideEnemy.transform.rotation.eulerAngles.x,
            _UpsideEnemy.transform.rotation.eulerAngles.y,
            _UpsideEnemy.transform.rotation.eulerAngles.z + 180.0f
        );
    }

    public float TakeDamage(float damage)
    {
        _remainingHealth -= damage;

        if (_remainingHealth <= 0.0f)
            Destroy(gameObject);

        return Mathf.Clamp(_remainingHealth, 0.0f, _enemyData.health);
    }
}
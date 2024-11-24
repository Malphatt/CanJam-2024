using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected EnemyData _enemyData;

    [SerializeField]
    protected GameObject _UpsideEnemy;

    [SerializeField]
    protected GameObject _DownsideEnemy;

    public float CurrentHealth;

    protected Rigidbody _rb;

    private void Awake()
    {
        CurrentHealth = _enemyData.Health;

        _rb = _UpsideEnemy.GetComponent<Rigidbody>();
    }

    protected void Update()
    {
        // Move and rotate the DownsideEnemy
        _DownsideEnemy.transform.SetPositionAndRotation(new Vector3(
            _UpsideEnemy.transform.position.x,
            -_UpsideEnemy.transform.position.y,
            _UpsideEnemy.transform.position.z
        ), Quaternion.Euler(
            _UpsideEnemy.transform.rotation.eulerAngles.x,
            _UpsideEnemy.transform.rotation.eulerAngles.y,
            _UpsideEnemy.transform.rotation.eulerAngles.z + 180.0f
        ));
    }

    public float TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0.0f)
            Destroy(gameObject);

        return Mathf.Clamp(CurrentHealth, 0.0f, _enemyData.Health);
    }
}

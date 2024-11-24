using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Enemy
{
    [SerializeField]
    private GameObject _player;

    private bool _diveBombing = false;

    private float _explosionSize = 3.0f;
    private float _explosionForce = 10.0f;
    private float _explosionDamage = 15.0f;


    private new void Update()
    {
        base.Update();

        if (_diveBombing)
            DiveBomb();
        else
            FollowEnemy(_player.transform);
    }

    private void DiveBomb()
    {
        float distanceToPlayer = Vector3.Distance(_UpsideEnemy.transform.position, _player.transform.position);

        if (distanceToPlayer < 5.0f)
        {
            // Explode (Create a physics sphere and apply an explosion force)
            Collider[] colliders = Physics.OverlapSphere(_UpsideEnemy.transform.position, _explosionSize);

            foreach (Collider collider in colliders)
            {
                if (collider.GetComponent<Rigidbody>())
                {
                    Rigidbody rb = collider.GetComponent<Rigidbody>();

                    if (rb != null)
                        rb.AddExplosionForce(_explosionForce, _UpsideEnemy.transform.position, _explosionSize);
                }

                if (collider.GetComponent<PlayerController>() || collider.GetComponent<Enemy>())
                {
                    if (collider.GetComponent<PlayerController>())
                        collider.GetComponent<PlayerController>().TakeDamage(_explosionDamage);
                    else if (collider.GetComponent<Enemy>())
                        collider.GetComponent<Enemy>().TakeDamage(_explosionDamage);
                }
            }
        }
    }

    private void FollowEnemy(Transform target)
    {
        _UpsideEnemy.transform.position = Vector3.MoveTowards(
            _UpsideEnemy.transform.position,
            new Vector3(
                target.position.x,
                target.position.y + 2.0f,
                target.position.z
            ),
            _enemyData.Speed * Time.deltaTime
        );
    }
}

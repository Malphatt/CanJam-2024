using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _bulletSpeed = 100f;
    [SerializeField]
    private float _bulletLifeTime = 2f;
    [SerializeField]
    private float _bulletDamage = 2.0f;
    [SerializeField]
    private float _killRange = 0.75f;
    [SerializeField]
    private LayerMask _whatIsPlayer;

    private GameObject _player;

    void Awake()
    {
        _player = GameObject.Find("Player");

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = transform.forward * _bulletSpeed;

        Destroy(gameObject, _bulletLifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the bullet position is within an acceptable range of the player
        if (Physics.CheckSphere(transform.position, _killRange, _whatIsPlayer))
        {
            _player.GetComponent<PlayerController>().TakeDamage(_bulletDamage);
            Destroy(gameObject);
        }
    }
}

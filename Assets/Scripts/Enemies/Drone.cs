using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Enemy
{
    [SerializeField]
    private GameObject _drone;

    [SerializeField]
    private GameObject _player;

    private float _distanceToPlayer;

    private void Update()
    {
        _distanceToPlayer = Vector3.Distance(_drone.transform.position, _player.transform.position);

        if (_distanceToPlayer < 10.0f)
        {
            _drone.transform.position = Vector3.MoveTowards(
                _drone.transform.position,
                _player.transform.position,
                _enemyData.speed * Time.deltaTime
            );
        }
    }
}

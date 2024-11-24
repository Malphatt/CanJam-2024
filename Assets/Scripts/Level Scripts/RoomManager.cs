using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] upDoors = new GameObject[0];
    [SerializeField]
    private GameObject[] rightDoors = new GameObject[0];
    [SerializeField]
    private GameObject[] downDoors = new GameObject[0];
    [SerializeField]
    private GameObject[] leftDoors = new GameObject[0];

    private int entranceDoorDirection = -1;
    private int exitDoorDirection = -1;

    private GameObject[][] doors;

    void Awake()
    {
        // Create a 2D array of doors.
        doors = new GameObject[4][];
        // Assign the doors to the 2D array.

        doors[0] = upDoors;
        doors[1] = rightDoors;
        doors[2] = downDoors;
        doors[3] = leftDoors;
    }

    public void RemoveDoors(int entrance, int exit)
    {
        entranceDoorDirection = entrance;
        exitDoorDirection = exit;

        Debug.Log("Entrance: " + entranceDoorDirection + " Exit: " + exitDoorDirection);

        if (entranceDoorDirection != -1)
        {
            GameObject[] entranceDoors = doors[entranceDoorDirection];

            Debug.Log("Entrance Doors: " + entranceDoors.Length);

            for (int i = 0; i < entranceDoors.Length; i++)
            {
                entranceDoors[i].SetActive(false);
            }
        }

        if (exitDoorDirection != -1)
        {
            GameObject[] exitDoors = doors[exitDoorDirection];

            for (int i = 0; i < exitDoors.Length; i++)
            {
                exitDoors[i].SetActive(false);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEditor.AI;
using UnityEngine;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{
    // 2D array to store procedural generation (PCG) points.
    private PcgPoints[,] pointsArray;

    //create a private array for the available neighbours
    private PcgPoints[] availableNeighbours;

    private List<PcgPoints> roomsPath;

    // PCG grid settings.
    [Header("Level Generation Settings")]
    [SerializeField]
    [Tooltip("The width of the level")]
    private int gridWidth = 10;
    [SerializeField]
    [Tooltip("The height of the level")]
    private int gridHeight = 10;

    private PcgPoints currentPoint;
    private int lastDirection;

    // Room Settings
    [Header("Room Settings")]
    [SerializeField]
    //Number of rooms to generate
    private int numberOfRooms = 10;

    // Room prefabs.
    [Header("Room Prefabs")]
    public GameObject StartRoom;
    public GameObject EndRoom;

    public GameObject[] Rooms; 

    void Start()
    {
        roomsPath = new List<PcgPoints>();

        GenerateLevel();
    }

    void GenerateLevel()
    {
        CreatePCGPoints();

        SetStartPoint();

        CreateRooms();
    }
    void CreatePCGPoints()
    {
        // Initialize the points array.
        pointsArray = new PcgPoints[gridWidth, gridHeight];

        // Initialize the available neighbours array
        availableNeighbours = new PcgPoints[4];

        // Iterate through the grid and create a point for each position.
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Create a data point for each grid position, with each point 10 units apart.
                Vector3 point = new Vector3(x * 100 - (gridWidth/2) * 100, 0, y * 100 - (gridHeight/2) * 100);

                // Initialize the PCG point.
                PcgPoints newPoint = new PcgPoints(point, x, y);

                // Store the point in the array.
                pointsArray[x, y] = newPoint;
            }
        }
    }

    void SetStartPoint()
    {
        // This can be removed later, it is just to visualize the points.
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                PcgPoints point = pointsArray[x, y];

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = pointsArray[x, y].point;
            }
        }

        // Select the center point of the grid
        int centerPointX = gridWidth / 2;
        int centerPointY = gridHeight / 2;

        //create a room at the center point
        pointsArray[centerPointX, centerPointY].room = Instantiate(StartRoom, pointsArray[centerPointX, centerPointY].point, Quaternion.identity);

        //set the current point to true
        currentPoint = pointsArray[centerPointX, centerPointY];
        pointsArray[centerPointX, centerPointY].isBlocked = true;

        //add the center point to the rooms path
        roomsPath.Add(pointsArray[centerPointX, centerPointY]);

    }

    List<PcgPoints> GetNeighbours(PcgPoints point)
    {
        List<PcgPoints> neighbours = new List<PcgPoints>();

        Vector2[] directions = new Vector2[4]
        {
            new Vector2(1, 0),
            new Vector2(-1, 0),
            new Vector2(0, 1),
            new Vector2(0, -1)
        };

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2 dir = directions[i];
            Vector2 neighbourPos = new Vector2(point.X + dir.x, point.Y + dir.y);

            if (neighbourPos.x >= 0 && neighbourPos.x < gridWidth && neighbourPos.y >= 0 && neighbourPos.y < gridHeight)
            {
                PcgPoints neighbour = pointsArray[(int)neighbourPos.x, (int)neighbourPos.y];

                if (!neighbour.isBlocked)
                {
                    neighbours.Add(neighbour);
                }
            }
        }

        return neighbours;
    }

    void CreateRooms()
    {
        // run these two functions in a loop until the number of rooms is reached
        for (int i = 0; i < numberOfRooms; i++)
        {
            List<PcgPoints> neighbours = GetNeighbours(currentPoint);

            int randomDirection = Random.Range(0, neighbours.Count);

            while (randomDirection == lastDirection)
            {
                randomDirection = Random.Range(0, neighbours.Count);
            }

            //select a random neighbour from the available neighbours array
            PcgPoints randomNeighbour = neighbours[randomDirection];

            //create a room at the random neighbour's position
            pointsArray[randomNeighbour.X, randomNeighbour.Y].room = Instantiate(Rooms[Random.Range(0, Rooms.Length)], randomNeighbour.point, Quaternion.identity);

            //get the direction of the entrance door
            int exitDoorDirection = GetExitDoorDirection(currentPoint, randomNeighbour);

            pointsArray[currentPoint.X, currentPoint.Y].exitDoorDirection = exitDoorDirection;
            pointsArray[randomNeighbour.X, randomNeighbour.Y].entranceDoorDirection = (exitDoorDirection + 2) % 4;

            //set the current point
            currentPoint = randomNeighbour;
            if ( i % 2 == 0)
            {
            lastDirection = randomDirection;
            }

            //add the current room to the rooms path
            roomsPath.Add(pointsArray[currentPoint.X, currentPoint.Y]);

            //set all of the available neighbours to blocked
            for (int j = 0; j < neighbours.Count; j++)
                pointsArray[neighbours[j].X, neighbours[j].Y].isBlocked = true;
        }

        //for (int i = 0; i < roomsPath.Count; i++)
        //{
        //    roomsPath[i].room.GetComponent<RoomManager>()?.RemoveDoors(
        //        pointsArray[roomsPath[i].X, roomsPath[i].Y].exitDoorDirection,
        //        pointsArray[roomsPath[i].X, roomsPath[i].Y].entranceDoorDirection
        //    );
        //}

        NavMeshBuilder.BuildNavMesh();
    }

    int GetExitDoorDirection(PcgPoints currentPoint, PcgPoints randomNeighbour)
    {
        int entranceDoorDirection = -1;

        if (currentPoint.Y < randomNeighbour.Y)
        {
            entranceDoorDirection = (int)DoorDirections.Up;
        }
        else if (currentPoint.X < randomNeighbour.X)
        {
            entranceDoorDirection = (int)DoorDirections.Right;
        }
        else if (currentPoint.Y > randomNeighbour.Y)
        {
            entranceDoorDirection = (int)DoorDirections.Down;
        }
        else if (currentPoint.X > randomNeighbour.X)
        {
            entranceDoorDirection = (int)DoorDirections.Left;
        }

        return entranceDoorDirection;
    }

    private enum DoorDirections
    {
        Up,
        Right,
        Down,
        Left
    }

    struct PcgPoints
    {
        public readonly int X;
        public readonly int Y;
        public readonly Vector3 point;
        public bool isBlocked;
        public int entranceDoorDirection;
        public int exitDoorDirection;
        public GameObject room;

        public PcgPoints(Vector3 _point, int _x, int _y)
        {
            this.X = _x;
            this.Y = _y;
            point = _point;
            isBlocked = false;
            entranceDoorDirection = -1;
            exitDoorDirection = -1;
            room = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // 2D array to store procedural generation (PCG) points.
    private PcgPoints[,] pointsArray;

    //create a private array for the available neighbours
    private PcgPoints[] availableNeighbours;

    // PCG grid settings.
    [Header("Level Generation Settings")]
    [SerializeField]
    [Tooltip("The width of the level")]
    private int gridWidth = 10;
    [SerializeField]
    [Tooltip("The height of the level")]
    private int gridHeight = 10;

    private PcgPoints currentPoint;

    // Room Settings
    [Header("Room Settings")]
    [SerializeField]
    //Number of rooms to generate
    private int numberOfRooms = 10;

    // Room prefabs.
    [Header("Room Prefabs")]
    public GameObject StartRoom;
    public GameObject Room1;
    public GameObject Room2;
    public GameObject Room3;
    public GameObject EndRoom;

    public GameObject[] Rooms; 

    void Start()
    {
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
        GameObject room = Instantiate(Room1, pointsArray[centerPointX, centerPointY].point, Quaternion.identity);

        //set the current point to true
        currentPoint = pointsArray[centerPointX, centerPointY];
        pointsArray[centerPointX, centerPointY].isBlocked = true;

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

            //select a random neighbour from the available neighbours array
            PcgPoints randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];

            //create a room at the random neighbour's position
            GameObject room = Instantiate(Rooms[Random.Range (0,Rooms.Length)], randomNeighbour.point, Quaternion.identity);

            //set the current point
            currentPoint = randomNeighbour;

            //set all of the available neighbours to blocked
            for (int j = 0; j < neighbours.Count; j++)
                pointsArray[neighbours[j].X, neighbours[j].Y].isBlocked = true;
        }
    }

    struct PcgPoints
    {
        public readonly int X;
        public readonly int Y;
        public readonly Vector3 point;
        public bool isBlocked;

        public PcgPoints(Vector3 _point, int _x, int _y)
        {
            this.X = _x;
            this.Y = _y;
            point = _point;
            isBlocked = false;
        }
    }
}

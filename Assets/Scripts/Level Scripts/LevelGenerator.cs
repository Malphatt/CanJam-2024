// Ignore Spelling: Pcg

using System.Collections;
using System.Collections.Generic;
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

    // Room prefabs.
    [Header("Room Prefabs")]
    public GameObject StartRoom;
    public GameObject Room1;
    public GameObject Room2;
    public GameObject Room3;
    public GameObject EndRoom;

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        CreatePCGPoints();

        SetStartPoint();

        CheckNeighbours();

        CreateRooms();
    }

    void CreatePCGPoints()
    {
        // Initialize the points array.
        pointsArray = new PcgPoints[gridWidth, gridHeight];

        //Initialize the available neighbours array
        availableNeighbours = new PcgPoints[4];

        // Iterate through the grid and create a point for each position.
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Create a data point for each grid position, with each point 10 units apart.
                Vector3 point = new Vector3(x * 10, 0, y * 10);

                // Initialize the PCG point.
                PcgPoints newPoint = new PcgPoints(point);

                // Store the point in the array.
                pointsArray[x, y] = newPoint;
            }
        }
    }

    void CheckNeighbours()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                PcgPoints point = pointsArray[x, y];

                // Check if this is the current point.
                if (point.isCurrentPoint)
                {
                    // Check if the point has a neighbour to the right.
                    if (x < gridWidth - 1)
                    {
                        PcgPoints rightNeighbour = pointsArray[x + 1, y];
                        // Check if the right neighbour is not blocked.
                        if (!rightNeighbour.isBlocked)
                        {
                            // Instantiate room 2 at the right neighbour's position.
                            //GameObject room = Instantiate(Room2, rightNeighbour.point, Quaternion.identity);

                            //add the right neighbour to the available neighbours array
                            availableNeighbours[0] = rightNeighbour;
                        }
                    }

                    // Check if the point has a neighbour to the left.
                    if (x > 0)
                    {
                        PcgPoints leftNeighbour = pointsArray[x - 1, y];

                        if (!leftNeighbour.isBlocked)
                        {
                            // Instantiate room 2 at the right neighbour's position.
                            //GameObject room = Instantiate(Room2, leftNeighbour.point, Quaternion.identity);

                            //add the left neighbour to the available neighbours array
                            availableNeighbours[1] = leftNeighbour;
                        }
                    }

                    // Check if the point has a neighbour above.
                    if (y < gridHeight - 1)
                    {
                        PcgPoints topNeighbour = pointsArray[x, y + 1];

                        if (!topNeighbour.isBlocked)
                        {
                            // Instantiate room 2 at the right neighbour's position.
                            //GameObject room = Instantiate(Room2, topNeighbour.point, Quaternion.identity);

                            //add the top neighbour to the available neighbours array
                            availableNeighbours[2] = topNeighbour;
                        }
                    }

                    // Check if the point has a neighbour below.
                    if (y > 0)
                    {
                        PcgPoints bottomNeighbour = pointsArray[x, y - 1];

                        if (!bottomNeighbour.isBlocked)
                        {
                            // Instantiate room 2 at the right neighbour's position.
                            //GameObject room = Instantiate(Room2, bottomNeighbour.point, Quaternion.identity);

                            //add the bottom neighbour to the available neighbours array
                            availableNeighbours[3] = bottomNeighbour;
                        }
                    }

                    //set the current point to blocked
                    point.isBlocked = true;
                    //set the current point to not current
                    point.isCurrentPoint = false;

                    // Exit the loops once the current point is found and its neighbours are checked.
                    return;
                }
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
        pointsArray[centerPointX, centerPointY].isCurrentPoint = true;

        
    }

    void CreateRooms()
    {

            Debug.Log("Creating rooms");

            //select a random neighbour from the available neighbours array
            PcgPoints randomNeighbour = availableNeighbours[Random.Range(0, availableNeighbours.Length)];

            //create a room at the random neighbour's position
            GameObject room = Instantiate(Room1, randomNeighbour.point, Quaternion.identity);

            //set the current point to true
            Debug.Log("Current point is " + randomNeighbour.point + "isCurrentPoint" + randomNeighbour.isCurrentPoint);

            randomNeighbour.isCurrentPoint = true;
            Debug.Log("Current point is " + randomNeighbour.point + "isCurrentPoint" + randomNeighbour.isCurrentPoint);

            //set all of the available neighbours to blocked
            for (int i = 0; i < availableNeighbours.Length; i++)
            {
                availableNeighbours[i].isBlocked = true;
            }

            for (int i = 0; i < availableNeighbours.Length; i++)
        {
            Debug.Log("Available neighbour " + i + " is " + availableNeighbours[i].point + "isBlocked" + availableNeighbours[i].isBlocked);
        }

        //remove all elements of the available neighbours array
        availableNeighbours = new PcgPoints[4];

    }

    struct PcgPoints
    {
        public readonly Vector3 point;
        public bool isCurrentPoint;
        public bool isBlocked;

        public PcgPoints(Vector3 _point) 
        {
            point = _point;
            isCurrentPoint = false;
            isBlocked = false;
        }
    }
}

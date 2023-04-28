using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct waterFlow
{
    public Vector3 direction;
    public float speed;

    public waterFlow(Vector3 initDirection, float initSpeed)
    {
        direction = initDirection;
        speed = initSpeed;
    }
}

public class WaterCurrent : MonoBehaviour
{
    [Header("Water Current Properties")]
    [Tooltip("The water current mode. Simple means the water current is the same no matter where you are," +
        " but it will change every chosen interval. Advanced will have a flow speed and direction for each grid value.")]
    [SerializeField] bool simpleWaterCurrent;

    [Tooltip("Current flow direction of water (Only applies if simple mode is activated)")]
    [SerializeField] float waterCurrentDirection;

    [Header("Ocean Current Properties")]
    [Tooltip("The size of the starting water current grid (e.g 100, 100)")]
    [SerializeField] Vector2 waterCurrentGridSize;

    /// <summary>
    /// The 2D list that stores the water current grid
    /// </summary>
    List<List<waterFlow>> waterFlowGrid = new List<List<waterFlow>>();


    [Tooltip("The bounds of the grid")]
    int lowerBoundX, upperBoundX, lowerBoundZ, upperBoundZ;

    // Start is called before the first frame update
    void Start()
    {
        initWaterCurrentGrid();
    }

    /// <summary>
    /// Takes the grid sizes set by the user then generates a 2D list.
    /// populates each index in the grid with random values for current speed and direction.
    /// </summary>
    void initWaterCurrentGrid()
    {
        //Purposly cutting off the decimals to get usable dimensions
        lowerBoundX = 0;
        upperBoundX = (int)waterCurrentGridSize.x;
        lowerBoundZ = 0;
        upperBoundZ = (int)waterCurrentGridSize.y;


        //Generate the top line of dictionaries that will act as the holder for the Y values
        for (int a = lowerBoundX; a < upperBoundX; a++)
        {
            waterFlowGrid.Add(new List<waterFlow>());
        }


        for (int i = lowerBoundX, j = lowerBoundZ; i < upperBoundX && j < upperBoundZ; i++)
        {
            for (int k = lowerBoundX; k < upperBoundX; k++)
            {
                waterFlowGrid[i].Add(new waterFlow(new Vector3(0, Random.Range(0, 360), Random.Range(0, 20)), 0));
            }
        }
    }

    /// <summary>
    /// Iterates through the list of floating points and applies a force to them based on the direction and speed of the water current.
    /// The ship Controller Script will call this function then this function will apply the appropriate forces
    /// </summary>
    /// <param name="vesselRigidbody">The rigidbody attached to the ship calling this function</param>
    /// <param name="floatingPoints">The floating points attached to the ship calling this function</param>
    public void applyWaterCurrent(Rigidbody vesselRigidbody, List<GameObject> floatingPoints)
    {

        foreach (GameObject floatingPoint in floatingPoints)
        {
            Vector3 xyz = floatingPoint.transform.position;
            int floatingPointX = (int)xyz.x/100;
            int floatingPointZ = (int)xyz.z/100;

            if (floatingPointX < 0)
            {
                floatingPointX *= -1;
            }
            if (floatingPointZ < 0)
            {
                floatingPointZ *= -1;
            }

            //Make sure it doesn't try to access an out of bounds location
            if (floatingPointX > upperBoundX || floatingPointX < lowerBoundX || floatingPointZ > upperBoundZ || floatingPointZ < lowerBoundZ)
            {
                Debug.Log("Not in bounds");
                break;
            }
            else
            {
                vesselRigidbody.AddForceAtPosition((Vector3.forward * waterFlowGrid[floatingPointX][floatingPointZ].speed)
                    + waterFlowGrid[floatingPointX][floatingPointZ].direction, floatingPoint.transform.position, ForceMode.Force);
            }
        }
    }
}
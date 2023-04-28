using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Adds a rigidbody if there isn't already one
[RequireComponent(typeof(Rigidbody))]

public class FloatingScript : MonoBehaviour
{
    [Tooltip("The list of floating points that will keep the boat floating")]
    [SerializeField] List<GameObject> floatingPoints = new List<GameObject>();
    /// <summary>
    /// The parent of all the floating points attached to this ship. Helps organise the floating points
    /// </summary>
    private GameObject FloatingPointsLocalObject = null;

    [SerializeField] public float totalFloatingPointsSubmerged = 0;

    /// <summary>
    /// The drag applied to the boat when it is in the water
    /// </summary>
    public float underWaterDrag = 3f;

    /// <summary>
    /// The angular drag applied to the boat when it is in the water
    /// </summary>
    public float underWaterAngularDrag = 1f;

    /// <summary>
    /// The drag applied to the boat when it isn't in the water
    /// </summary>
    public float airDrag = 0.0f;
    
    /// <summary>
    /// The angular drag applied to the boat when it isn't in the water
    /// </summary>
    public float airAngularDrag = 0.05f;

    /// <summary>
    /// The strength of the bouyancy force pushing the ship back towards the surface of the water
    /// </summary>
    public float bouyancyStrength = 200f;

    [Tooltip("Is the boat submerged?")]
    bool isSubmerged;

    [Tooltip("The distance between the floating point and the nearest water mesh vertex")]
    [SerializeField] float difference;

    /// <summary>
    /// The rigidbody attached to the vessel that this floating point is part of
    /// </summary>
    Rigidbody vesselRigidbody;

    /// <summary>
    /// The GameObject for the water, this is how the water mesh will be accessed to enable accurate floating
    /// </summary>
    GameObject waterObject = null;

    /// <summary>
    /// The script that controls the water, this controls the density and temperature of the water
    /// </summary>
    WaterControlScript waterControlScript = null;

    /// <summary>
    /// The script respponsible for controlling the water current, refer to this to access the water current grid
    /// so that you can apply it to the ship however you want.
    /// </summary>
    WaterCurrent waterCurrentScript;

    

    

    



    // Start is called before the first frame update
    void Start()
    {
        vesselRigidbody = this.GetComponent<Rigidbody>();

        waterObject = GameObject.Find("waterPlane");
        waterControlScript = waterObject.GetComponent<WaterControlScript>();
        waterCurrentScript = waterObject.GetComponent<WaterCurrent>();


        isSubmerged = false;

        //Find the floating points within this object so that they don't have to be manually added.
        FloatingPointsLocalObject = this.transform.Find("FloatingPoints").gameObject;
        for (int i = 0; i < FloatingPointsLocalObject.transform.childCount; i++)
        {
            floatingPoints.Add(FloatingPointsLocalObject.transform.GetChild(i).gameObject);
        }

        underWaterDrag = 3f;
        underWaterAngularDrag = 1f;

        airDrag = 0.0f;
        airAngularDrag = 0.05f;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (waterCurrentScript == null)
        {
            waterCurrentScript = waterObject.GetComponent<WaterCurrent>();
        }


        totalFloatingPointsSubmerged = 0;
        for (int i = 0; i < floatingPoints.Count; i++)
        {
            //float difference = floatingPoints[i].transform.position.y - getYPosOfOcean(floatingPoints[i].transform.position);
            difference = floatingPoints[i].transform.position.y - getClosestVertexOfWater(waterObject, floatingPoints[i].transform.position).y;
            if (difference < 0)
            {
                totalFloatingPointsSubmerged += 1;
                //Add foce to the floating point that is submerged. The deeper the floating point is the stronger ther force will be.
                vesselRigidbody.AddForceAtPosition(Vector3.up * bouyancyStrength * (Mathf.Abs(difference)*0.1f), floatingPoints[i].transform.position, ForceMode.Force);


                waterCurrentScript.applyWaterCurrent(vesselRigidbody, floatingPoints);

                //If the vessel isn't already set as submerged then set is as true
                if (!isSubmerged)
                {
                    isSubmerged = true;
                    switchDrag();
                }
            }
            else
            {
                totalFloatingPointsSubmerged -= 1;
            }
        }

        if (totalFloatingPointsSubmerged <= 0)
        {
            isSubmerged = false;
            switchDrag();
            totalFloatingPointsSubmerged = 0;
        }
        
    }

    /// <summary>
    /// Switches the drag values around depending on if the ship is submerged or not.
    /// </summary>
    void switchDrag()
    {
        if (isSubmerged)
        {
            vesselRigidbody.drag = underWaterDrag;
            vesselRigidbody.angularDrag = underWaterAngularDrag;
        }
        else
        {
            vesselRigidbody.drag = airDrag;
            vesselRigidbody.angularDrag = airAngularDrag;
        }
    }

    /// <summary>
    /// Gets the closest vertex of the water mesh to the current floating point
    /// </summary>
    /// <param name="targetMesh">The mesh you want to search for the closest vertex</param>
    /// <param name="point">The point you are trying to find out which vertex is closest to</param>
    /// <returns>The X Y Z coordinates of the cloesest vertex to the point</returns>
    public Vector3 getClosestVertexOfWater(GameObject targetMesh, Vector3 point)
    {
        point = targetMesh.transform.InverseTransformPoint(point);
        float minDistance = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;

        // scan all vertices to find nearest
        foreach (Vector3 vertex in targetMesh.GetComponent<MeshFilter>().mesh.vertices)
        {
            float difference = Vector3.Distance(vertex, point);
            
            if (difference < minDistance)
            {
                minDistance = difference;
                nearestVertex = vertex;
            }
        }

        // convert nearest vertex back to world space
        return targetMesh.transform.TransformPoint(nearestVertex);
    }
}


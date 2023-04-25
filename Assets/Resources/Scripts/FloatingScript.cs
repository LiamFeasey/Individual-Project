using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Adds a rigidbody if there isn't already one
[RequireComponent(typeof(Rigidbody))]

public class FloatingScript : MonoBehaviour
{
    [SerializeField] List<GameObject> floatingPoints = new List<GameObject>();
    private GameObject FloatingPointsLocalObject = null;

    public float totalFloatingPointsSubmerged = 0;


    public float underWaterDrag = 3f;

    public float underWaterAngularDrag = 1f;

    [SerializeField] public float airDrag = 0.0f;

    public float airAngularDrag = 0.05f;

    public float bouyancyStrength = 200f;

    Rigidbody vesselRigidbody;

    GameObject waterObject = null;

    //The world position of the ocean vertices
    [SerializeField] public Vector3[] waterVerticePositionsW;
    //Scripts used to control different aspects of the simulation
    WaterControlScript waterControlScript = null;
    WaterCurrent waterCurrentScript;

    bool isSubmerged;


    Vector3 contactPoint;

    [SerializeField] float difference;



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

        if (waterObject != null)
        {
            waterVerticePositionsW = waterObject.GetComponent<MeshFilter>().mesh.vertices;
        }


        totalFloatingPointsSubmerged = 0;
        for (int i = 0; i < floatingPoints.Count; i++)
        {
            //float difference = floatingPoints[i].transform.position.y - getYPosOfOcean(floatingPoints[i].transform.position);
            difference = floatingPoints[i].transform.position.y - getClosestVectorOfWater(waterObject, floatingPoints[i].transform.position).y;
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

    //Switches the drag values around depending on if the ship is submerged or not.
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

    public Vector3 getClosestVectorOfWater(GameObject targetMesh, Vector3 point)
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


        //Debug.DrawLine(point, targetMesh.transform.TransformPoint(nearestVertex), Color.red);
        // convert nearest vertex back to world space
        return targetMesh.transform.TransformPoint(nearestVertex);
    }
}


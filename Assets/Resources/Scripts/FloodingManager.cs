using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodingManager : MonoBehaviour
{
    /// <summary>
    /// The local game object that is attached to the ship the holes are being attached to,
    /// this is for organisation purposes
    /// </summary>
    GameObject holesObject;

    /// <summary>
    /// The script that holds the information about the water, such as density and temperature.
    /// This can affect how much water flows through a hole.
    /// </summary>
    WaterControlScript waterControlScript;

    [Tooltip("The current list of holes in the ship. The less the better!")]
    [SerializeField] List<GameObject> holes = new List<GameObject>();

    /// <summary>
    /// Used to restrict the flooding to a set cycle to prevent excessive flooding
    /// </summary>
    float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        holesObject = gameObject.transform.Find("Holes").gameObject;
        waterControlScript = GameObject.Find("waterPlane").GetComponent<WaterControlScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (waterControlScript == null)
        {
            waterControlScript = GameObject.Find("waterPlane").GetComponent<WaterControlScript>();
        }


        timer += Time.deltaTime;
        
        if (holesObject == null)
        {
            holesObject = gameObject.transform.Find("Holes").GetComponent<GameObject>();
        }
    }

    private void FixedUpdate()
    {
        if (timer > 1.0f)
        {
            foreach (GameObject hole in holes)
            {
                if (hole.transform.position.y <= 0.0f)//Make sure the hole is actually below the water!!
                {
                    hole.GetComponent<Rigidbody>().mass += calculateTheoreticalDischarge(hole.transform.position.y, hole.GetComponent<HoleDetails>().getHoleRadius()) * (waterControlScript.density / 1000);
                }
            }
            timer = 0.0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 10)//Impact was hard enough to put a hole in the hull!!
        {
            Debug.Log("BANG!! That crash was a magnitude of: " + collision.relativeVelocity.magnitude);


            holes.Add(new GameObject());

            holes[holes.Count-1].transform.parent = holesObject.transform;
            holes[holes.Count-1].transform.position = collision.contacts[0].point;


            FixedJoint newHoleJoint = holes[holes.Count - 1].AddComponent<FixedJoint>();
            newHoleJoint.connectedBody = gameObject.GetComponent<Rigidbody>();


            //holes[holes.Count - 1].AddComponent<Rigidbody>();
            Rigidbody newHoleRigid = holes[holes.Count - 1].GetComponent<Rigidbody>();


            newHoleRigid.mass = 0.0f;
            newHoleRigid.useGravity = true;


            holes[holes.Count - 1].name = "Hole";


            holes[holes.Count - 1].AddComponent<HoleDetails>();
            holes[holes.Count - 1].GetComponent<HoleDetails>().setHoleRadius(collision.relativeVelocity.magnitude/10);


        }
        else
        {
            Debug.Log("That crash wasn't strong enough to form a hole in the hull. Magnitude: " + collision.relativeVelocity.magnitude);
        }
    }

    /// <summary>
    /// Calculate the theoretical discharge of water flowing through a hole based on the depth and size of the hole.
    /// </summary>
    /// <param name="y">The hydraulic head (depth) of the hole</param>
    /// <param name="crossSection">The area of the cross section of the hole</param>
    /// <returns>Total amount of water that's flowed through the hole during this flooding cycle</returns>
    float calculateTheoreticalDischarge(float y, float crossSection)
    {
        if (y < 0)
        {
            y = Mathf.Abs(y);
        }
        float hydraulicHead = y;
        float gravity = 9.81f;
        float resultant = hydraulicHead * gravity;
        resultant *= 2;
        resultant = Mathf.Sqrt(resultant);
        resultant *= (Mathf.PI * Mathf.Pow(crossSection, 2));

        return resultant;
    }
}


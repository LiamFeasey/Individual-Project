using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodingManager : MonoBehaviour
{
    MeshFilter shipMeshFilter;
    GameObject holesObject;//Where the holes will be placed to keep them in one place

    [Tooltip("The current list of holes in the ship. The less the better!")]
    [SerializeField] List<GameObject> holes = new List<GameObject>();


    float timer = 0.0f;//Used to prevent the flooding from running at excessive speeds

    // Start is called before the first frame update
    void Start()
    {
        shipMeshFilter = gameObject.GetComponent<MeshFilter>();
        holesObject = gameObject.transform.Find("Holes").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
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
                    hole.GetComponent<Rigidbody>().mass += calculateTheoreticalDischarge(hole.transform.position.y, hole.GetComponent<HoleDetails>().getHoleRadius());
                }
            }
            timer = 0.0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            //Debug.DrawRay(contact.point, contact.normal, Color.red, 10.0f);
            //Debug.LogWarning("Collision! " + contact.point);
            Debug.LogWarning("Collision! Other collider: " + contact.point);
        }
        if (collision.relativeVelocity.magnitude > 2)//Impact was hard enough to put a hole in the hull!!
        {
            Debug.LogWarning("BANG");


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
    }

    //Calculate the theoretical Discharge to use in place of actual discharge
    float calculateTheoreticalDischarge(float y, float crossSection)
    {
        if (y < 0)
        {
            y = Mathf.Abs(y);
        }
        float hydraulicHead = y;
        float gravity = 9.81f;
        float resultant = hydraulicHead * gravity;
        resultant = Mathf.Sqrt(resultant);
        resultant *= (Mathf.PI * Mathf.Pow(crossSection, 2));

        return resultant;
    }
}


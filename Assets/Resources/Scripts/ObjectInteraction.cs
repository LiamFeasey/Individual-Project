using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles how other objects will interact with the ship. Such as attaching to the upwards surface of the ship.
public class ObjectInteraction : MonoBehaviour
{
    [Header("Objects")]
    [Tooltip("Objects currently attached to the boat")]
    [SerializeField] List<GameObject> attachedObjects = new List<GameObject>();

    [Tooltip("Nearby objects that are touching the boat that can be attached if so desired")]
    [SerializeField] List<GameObject> nearbyObjects = new List<GameObject>();

    [Space(25)]
    [Header("Scripts")]
    [Tooltip("The script that controls the ship, used for controlling throttle," +
        " steering, camera angles, and object attachment")]
    [SerializeField] ShipControllerScript shipControllerScript;

    /// <summary>
    /// The game object for the boat that this script will be attached to.
    /// Used to attach objects to the hull of the boat.
    /// </summary>
    GameObject shipHull;


    // Start is called before the first frame update
    void Start()
    {
        shipControllerScript = gameObject.GetComponent<ShipControllerScript>();
        shipHull = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (shipControllerScript == null)
        {
            shipControllerScript = gameObject.GetComponent<ShipControllerScript>();
        }
        if (shipHull == null)
        {
            shipHull = gameObject;
        }

        //If the ship rolls too far then release every attached object
        if (shipHull)
        {
            float x, z;

            //Just makes it easier to read the code!
            x = shipHull.transform.rotation.x;
            z = shipHull.transform.rotation.z;

            if (x > 0.44f || x < -0.44f || z > 0.44f || z < -0.44f)
            {
                releaseObjects();
            }
        }
        
    }

    ///<summary>
    ///Add objects to the nearby objects list, unless they're terrain or another ship
    /// </summary>
    /// <param name="collision">The object that has entered the collider</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Terrain" && collision.gameObject.tag != "Ship")
        {
            if (!nearbyObjects.Contains(collision.gameObject))
            {
                nearbyObjects.Add(collision.gameObject);
            }
            else
            {
                Debug.LogWarning("Object already in nearby list!");
            }
        }
    }

    /// <summary>
    /// Remove the object from the nearbyObjects list if it leaves the ship
    /// </summary>
    /// <param name="collision">The object that left the collider</param>
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "Terrain" && collision.gameObject.tag != "Ship")
        {
            if (nearbyObjects.Contains(collision.gameObject))
            {
                nearbyObjects.Remove(collision.gameObject);
            }
            else
            {
                Debug.LogWarning("Object already removed from nearby list!");
            }
        }
    }

    /// <summary>
    /// Attaches the objects, to the ship, that are in the nearbyObjects list.
    /// </summary>
    public void attachObjects()
    {
        for (int i = 0; i < nearbyObjects.Count; i++)
        {
            nearbyObjects[i].AddComponent<FixedJoint>();
            nearbyObjects[i].GetComponent<FixedJoint>().connectedBody = gameObject.GetComponent<Rigidbody>();
            attachedObjects.Add(nearbyObjects[i]);
            nearbyObjects.Remove(nearbyObjects[i]);
        }
    }


    /// <summary>
    /// Release the currently attached objects, and place them back into the nearbyObjects list
    /// </summary>
    public void releaseObjects()
    {
        for (int i = 0; i < attachedObjects.Count; i++)
        {
            Destroy(attachedObjects[i].GetComponent<FixedJoint>());
            nearbyObjects.Add(attachedObjects[i]);
            attachedObjects.Remove(attachedObjects[i]);
        }
    }
}

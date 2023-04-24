using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles how other objects will interact with the ship. Such as attaching to the upwards surface of the ship.
public class ObjectInteraction : MonoBehaviour
{
    [Header("Objects")]
    [Tooltip("Currently attached objects")]
    [SerializeField] List<GameObject> attachedObjects = new List<GameObject>();

    [Tooltip("Nearby objects that are touching the ship that can be attached if so desired")]
    [SerializeField] List<GameObject> nearbyObjects = new List<GameObject>();

    [Space(25)]
    [Header("Properties")]
    [Tooltip("When turned on objects will automatically attach to the ship on contact (Their X and Z rotations will be set to match the ships too)")]
    [SerializeField] bool automaticAttachment;


    [Space(25)]
    [Header("Scripts")]
    [SerializeField] ShipControllerScript shipControllerScript;


    // Start is called before the first frame update
    void Start()
    {
        shipControllerScript = gameObject.GetComponent<ShipControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shipControllerScript == null)
        {
            shipControllerScript = gameObject.GetComponent<ShipControllerScript>();
        }
    }

    ///<summary>
    ///Add objects to the nearby objects list, unless they're terrain or another ship
    /// </summary>
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
    /// Attaches the objects in the nearbyObjects list.
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
    /// Release the currently attached objects
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

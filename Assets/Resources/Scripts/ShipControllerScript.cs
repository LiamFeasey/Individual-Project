using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControllerScript : MonoBehaviour
{

    //Array of propulsion points.
    [Header("Component Arrays")]
    [SerializeField] List<GameObject> PropulsionPoints = new List<GameObject>();
    private GameObject localPropulsionPointsObject;

    //Array of steering points.
    [SerializeField] List<GameObject> SteeringPoints = new List<GameObject>();
    private GameObject localSteeringPointsObject;

    //Array of engines
    [Tooltip("Engines haven't been implemented yet!")]
    [SerializeField] List<GameObject> Engines = new List<GameObject>();

    //Array of bouyancy compartments (acts like floating points but can be flooded to pull the ship down instead of providing bouyancy)
    [Tooltip("Bouyancy Compartments haven't been implemented yet!")]
    [SerializeField] List<GameObject> BouyancyCompartments = new List<GameObject>();

    //Array of camera positions
    [SerializeField] List<GameObject> CameraPoints = new List<GameObject>();
    private GameObject localCameraPointsObject;



    [Space(25)]

    //Total Horse Power
    [Tooltip("The total combined horsepower of all the ships engines")]
    [Header("Ship Statistics")]
    [SerializeField] int totalHorsePower = 100;

    //Throttle Increase Speed
    [Tooltip("The speed at which the throttle increases/decreases when either the increase or decrease button is held (w and up arrow for increase, s and down arrow for decrease)")]
    [SerializeField] float throttleIncreaseSpeed = 0.1f;

    //Current Throttle
    [Tooltip("Throttle of all the engines, must be between -100 and 100")]
    [Range(-100f, 100f)]
    [SerializeField] float currentThrottle = 0;


    //Throttle Increase Speed
    [Tooltip("The speed at which the steering increases/decreases when either the increase or decrease button is held (A and left arrow for increase, D and right arrow for decrease)")]
    [SerializeField] float steeringIncreaseSpeed = 0.1f;

    [Tooltip("Holds the current value of the steering with a range of -80 to 80")]
    [Range(-80.0f, 80.0f)]
    [SerializeField] float currentSteering = 0;

    //Engine ignition on?
    [SerializeField] bool engineIgnitionOn = false;

    [Tooltip("How fast the ship is moving based on how fair it's moved since the last update")]
    [SerializeField] float speed = 0;
    private Vector3 lastPos;

    [Tooltip("Trim applied to steering in the case the ship is pulling to one side or the other without steering input")]
    [SerializeField] float steeringTrim;



    [Space(25)]

    [Header("Scripts")]
    //Floating Script associated with current ship
    [Tooltip("The floating script that is being used by the current ship/GameObject")]
    [SerializeField] FloatingScript currentShipFloatingScript = null;

    //Water Control Script
    [Tooltip("The water control sript being used by the current scene")]
    [SerializeField] WaterControlScript waterControlScript = null;

    /// <summary>
    /// Object Interaction Script. Controls how objects are attached and released from the ship
    /// </summary>
    [Tooltip("The object interaction script being used by the current ship")]
    [SerializeField] ObjectInteraction objectInteractionScript;



    [Space(25)]

    [Header("Player Components")]

    [Tooltip("The camera object that the player sees through so that it can be manipulated as needed by the script")]
    [SerializeField] Camera playerCamera = null;


    [Space(25)]

    [Header("Camera Options")]

    [Tooltip("The currently selected camera index/ID6 used when accessing the camera points array")]
    [SerializeField] int currentCameraIndex;

    [Tooltip("Sets whether the cameras roll matches the ships roll or not")]
    [SerializeField] bool cameraMatchesShipRoll;


    // Start is called before the first frame update
    void Start()
    {
        //Find the FloatingScript attached to the current ship and set the floating scripts value to it
        currentShipFloatingScript = gameObject.GetComponent<FloatingScript>();
        
        //Find the WaterControlScript and save a reference to it in the water contron script variable
        waterControlScript = gameObject.GetComponent<WaterControlScript>();

        //Find the objectInteraction script and save a reference to it
        objectInteractionScript = gameObject.GetComponent<ObjectInteraction>();


        //populate propulsion points array by finding objects in the propulsion section of a ship.
        localPropulsionPointsObject = gameObject.transform.Find("PropulsionPoints").gameObject;
        for (int i = 0; i < localPropulsionPointsObject.transform.childCount; i++)
        {
            PropulsionPoints.Add(localPropulsionPointsObject.transform.GetChild(i).gameObject);
        }
        //populate steering points array by finding objects in the steering points section of a ship.
        localSteeringPointsObject = gameObject.transform.Find("SteeringPoints").gameObject;
        for (int i = 0; i < localSteeringPointsObject.transform.childCount; i++)
        {
            SteeringPoints.Add(localSteeringPointsObject.transform.GetChild(i).gameObject);
        }
        //populate engines array by finding objects in the engines section of a ship.
        //populate bouyancy compartments array by finding objects in the bouyancy compartments section of a ship.
        //populate camera positions array by finding objects in the camera positions section of a ship.
        localCameraPointsObject = gameObject.transform.Find("CameraPositions").gameObject;
        for(int i = 0; i < localCameraPointsObject.transform.childCount; i++)
        {
            CameraPoints.Add(localCameraPointsObject.transform.GetChild(i).gameObject);
        }


        lastPos = this.gameObject.transform.position;

        playerCamera.transform.position = CameraPoints[0].transform.position;
        playerCamera.transform.rotation = CameraPoints[0].transform.rotation;
    }

    //Using FixedUpdate for the speed because it kept jumping between the speed and 0 making the speed hard to read.
    //Updates at a fixed interval.
    private void FixedUpdate()
    {
        //Calculates the speed based of the distance between the current and last position
        speed = Vector3.Distance(transform.position, lastPos) / Time.deltaTime;
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if the required scripts are found, if not then try to find them again.
        if (currentShipFloatingScript == null)
        {
            currentShipFloatingScript = this.gameObject.GetComponent<FloatingScript>();
        }
        if (waterControlScript == null)
        {
            waterControlScript = this.gameObject.GetComponent<WaterControlScript>();
        }
        if (objectInteractionScript == null)
        {
            objectInteractionScript = gameObject.GetComponent<ObjectInteraction>();
        }

        //Set the players camera to the current camera position chosen.
        playerCamera.transform.position = CameraPoints[currentCameraIndex].transform.position;
        if (cameraMatchesShipRoll)
        {
            playerCamera.transform.rotation = CameraPoints[currentCameraIndex].transform.rotation;
        }
        else if (cameraMatchesShipRoll != true)
        {
            playerCamera.transform.rotation = updatePlayerCameraRotation(CameraPoints[currentCameraIndex].transform.rotation);
        }
        


        //Check for any user input
        if (Input.anyKey)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                currentThrottle += throttleIncreaseSpeed;
                if (currentThrottle > 100.0f)
                {
                    currentThrottle = 100.0f;
                }
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                currentThrottle -= throttleIncreaseSpeed;
                if (currentThrottle < -100.0f)
                {
                    currentThrottle = -100.0f;
                }
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                currentSteering -= steeringIncreaseSpeed;
                if (currentSteering < -80.0f)
                {
                    currentSteering = -80.0f;
                }
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                currentSteering += steeringIncreaseSpeed;
                if (currentSteering > 80.0f)
                {
                    currentSteering = 80.0f;
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                engineIgnitionOn = engineIgnitionOn == false ? true : false;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                cameraMatchesShipRoll = cameraMatchesShipRoll == false ? true : false;
            }

            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                if (currentCameraIndex - 1 >= 0)
                {
                    currentCameraIndex -= 1;
                }
                else
                {
                    currentCameraIndex = CameraPoints.Count - 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                if (currentCameraIndex + 1 <= CameraPoints.Count - 1)
                {
                    currentCameraIndex += 1;
                }
                else
                {
                    currentCameraIndex = 0;
                }
            }
            
            if (Input.GetKeyDown(KeyCode.K))
            {
                objectInteractionScript.attachObjects();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                objectInteractionScript.releaseObjects();
            }


        }
        //If w or s (could include up and down arrow) is pressed then increase or decrease current throttle level accordingly.
        //If a or d (could include left and right arrow) is pressed then adjust the steering position of everything in the steering points array accordingly.

        //If ignition button is pressed then toggle the engines accordingly.

        //If camera change button is pressed then switch between available cameras accordingly.

        //Update UI elements such as speedometer, bouyancy compartment gauges, and engine status etc...


        if (engineIgnitionOn)
        {
            //Add propulsion based on throttle and steering
            //for (int i = 0; i < PropulsionPoints.Count; i++)
            //{
            //    PropulsionPoints[i].GetComponent<Rigidbody>().AddRelativeForce(Vector3.down * (currentThrottle * totalHorsePower), ForceMode.Force);
            //}
            for (int i = 0; i < SteeringPoints.Count; i++)
            {
                SteeringPoints[i].GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * ((currentSteering + steeringTrim) * (speed * 0.5f)), ForceMode.Force);
            }
        }


    }

    //Used when the user doesn't want the camera to roll and pitch with the ship.
    Quaternion updatePlayerCameraRotation(Quaternion inputQuaternion)
    {
        float x = inputQuaternion.x;
        float y = inputQuaternion.y;
        float z = inputQuaternion.z;
        float w = inputQuaternion.w;

        Quaternion updatedQuaternion = new Quaternion();
        updatedQuaternion.Set(0, y, 0, w);

        return updatedQuaternion;
    }

    public float getCurrentThrottle()
    {
        return currentThrottle;
    }

    public float getSpeed()
    {
        return speed;
    }

    public bool getIgnition()
    {
        return engineIgnitionOn;
    }    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Used to store information about different fuel types. 
/// These different fuel types will be used in the engines for the boats.
/// </summary>
[System.Serializable] public struct Fuel
{
    /// <summary>
    /// The fuels name
    /// </summary>
    public string name;
    /// <summary>
    /// the specific energy of the fuel in MJ/kg
    /// </summary>
    public float specificEnergy;
    /// <summary>
    /// The energy density of the fuel in MJ/
    /// </summary>
    public float energyDensity;

    public Fuel(string initName, float initEnergy, float initDensity)
    {
        name = initName;
        specificEnergy = initEnergy;
        energyDensity = initDensity;
    }
};

public class EngineController : MonoBehaviour
{
    [Header("Component Arrays")]
    [Tooltip("The list of propulsion points attached to this boat")]
    [SerializeField] List<GameObject> PropulsionPoints = new List<GameObject>();
    /// <summary>
    /// The game object that the propulsion points are a child of for organisation
    /// </summary>
    private GameObject localPropulsionPointsObject;

    [Header("Fuels")]
    public static Fuel Diesel = new("Diesel", 45.6f, 38.6f);//Diesel fuel
    public static Fuel Petrol = new("Petrol", 46.4f, 34.3f);//Petrol fuel
    [Tooltip("The total amount of fuel in all the tanks")]
    [SerializeField] List<float> fuelTanks = new List<float> { };
    [Tooltip("The total amount of individual fuel tanks")]
    [SerializeField] int totalFuelTanks;
    [Tooltip("The size of each fuel tank")]
    [SerializeField] float fuelTankSize;

    /// <summary>
    /// The ship controller script for the ship this engine controller is attached to.
    /// </summary>
    ShipControllerScript shipController;

    // Start is called before the first frame update
    void Start()
    {
        localPropulsionPointsObject = this.gameObject;
        for (int i = 0; i < localPropulsionPointsObject.transform.childCount; i++)
        {
            PropulsionPoints.Add(localPropulsionPointsObject.transform.GetChild(i).gameObject);
        }

        shipController = gameObject.transform.parent.gameObject.GetComponent<ShipControllerScript>();

        for (int i = 0; i < totalFuelTanks; i++)
        {
            fuelTanks.Add(fuelTankSize);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!shipController)
        {
            shipController = gameObject.transform.parent.gameObject.GetComponent<ShipControllerScript>();
        }

        if (shipController && shipController.getIgnition())
        {
            //Add propulsion based on throttle and steering
            for (int i = 0; i < PropulsionPoints.Count; i++)
            {
                bool engineRunning;
                engineRunning = PropulsionPoints[i].GetComponent<DefaultEngine>().getIgnition();
                if (engineRunning)
                {
                    switch (PropulsionPoints[i].GetComponent<DefaultEngine>().getIsBelowWater())
                    {
                        case true:
                            PropulsionPoints[i].GetComponent<Rigidbody>().AddRelativeForce(Vector3.down * (shipController.getCurrentThrottle() 
                                * (PropulsionPoints[i].GetComponent<DefaultEngine>().getHorsePower())), ForceMode.Force);
                            break;
                        case false:
                            PropulsionPoints[i].GetComponent<Rigidbody>().AddRelativeForce(Vector3.down * ((shipController.getCurrentThrottle() 
                                * (PropulsionPoints[i].GetComponent<DefaultEngine>().getHorsePower())) * 0.01f), ForceMode.Force);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Engines call this function to request fuel from the engine controller.
    /// Fuel will be subtracted from the tanks based on the engines calculated fuel usage
    /// </summary>
    /// <param name="fuelUsage">The calling engines fuel usage</param>
    /// <returns>A boolean stating whether there is enough fuel for the engine or not</returns>
    public bool requestFuel(float fuelUsage)
    {
        float averegedFuelUse = fuelUsage / fuelTanks.Count;

        for (int i = 0; i < fuelTanks.Count; i++)
        {
            if (fuelTanks[i] > averegedFuelUse)
            {
                fuelTanks[i] -= averegedFuelUse;
            }
            else
            { 
                return false; 
            }
        }
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable] public struct Fuel
{
    public string name;//The fuels name
    public float specificEnergy;//the specific energy of the fuel in MJ/kg
    public float energyDensity;//The energy density of the fuel in MJ/L

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
    [SerializeField] List<GameObject> PropulsionPoints = new List<GameObject>();
    private GameObject localPropulsionPointsObject;

    [Header("Fuels")]
    public static Fuel Diesel = new("Diesel", 45.6f, 38.6f);//Diesel fuel
    public static Fuel Petrol = new("Petrol", 46.4f, 34.3f);//Petrol fuel
    [Tooltip("The total amount of fuel in the tanks")]
    [SerializeField] public float fuelTank1 = 100.0f;
    [SerializeField] public float fuelTank2 = 100.0f;


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
                switch (PropulsionPoints[i].GetComponent<DefaultEngine>().getIsBelowWater())
                {
                    case true:
                        PropulsionPoints[i].GetComponent<Rigidbody>().AddRelativeForce(Vector3.down * (shipController.getCurrentThrottle() * (PropulsionPoints[i].GetComponent<DefaultEngine>().getHorsePower())), ForceMode.Force);
                        break;
                    case false:
                        PropulsionPoints[i].GetComponent<Rigidbody>().AddRelativeForce(Vector3.down * ((shipController.getCurrentThrottle() * (PropulsionPoints[i].GetComponent<DefaultEngine>().getHorsePower())) * 0.01f), ForceMode.Force);
                        break;
                }
                
            }
        }
    }
}

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
    public static Fuel Diesel = new("Diesel", 45.6f, 38.6f);//Diesel fuel
    public static Fuel Petrol = new("Petrol", 46.4f, 34.3f);//Petrol fuel
    [Tooltip("The total amount of fuel in the tanks")]
    [SerializeField] public float fuelTank1 = 100.0f;
    [SerializeField] public float fuelTank2 = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

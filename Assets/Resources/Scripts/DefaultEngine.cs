using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;




enum FuelType
{
    Diesel,
    Petrol
};

public class DefaultEngine : MonoBehaviour
{
    [Header("Engine Properties")]
    [Tooltip("[Warning!] Increasing Cylinders and Cylinder Displacement too far with a light ship will cause flipping!")]
    [Range(1, 20f)]
    [SerializeField] int cylinders;
    [Tooltip("[Warning!] Increasing Cylinders and Cylinder Displacement too far with a light ship will cause flipping!")]
    [Range(1, 20f)]
    [SerializeField] float cylinderDisplacement;
    [SerializeField] float engineDisplacement;
    [SerializeField] float torque;
    [Range(0, 5000)]
    [SerializeField] int RPM;
    [SerializeField] float fuelUsage;
    [SerializeField] FuelType engineFuelType;
    [Tooltip("The type of fuel that's being used and its properties (Name, Specific Energy, and Energy Density)")]
    [SerializeField] Fuel fuelType;//The fuel type struct that will contain all the relevant details about the fuel for this engine.
    float throttle;


    [Header("Propeller Properties")]
    [SerializeField] float propellerSize;
    [SerializeField] float resistance;//The resistance of the propeller, this will affect how quick the RPM will climb


    [Header("Propulsion Properties")]
    [SerializeField] float propulsionForce;
    [SerializeField] bool isBelowWater;


    [SerializeField] ShipControllerScript shipController;
    WaterControlScript waterControlScript;

    // Start is called before the first frame update
    void Start()
    {
        switch (engineFuelType)
        {
            case FuelType.Diesel:
                fuelType = EngineController.Diesel;//Specific Energy = 45.6, Energy Density = 38.6
                break;
            case FuelType.Petrol:
                fuelType = EngineController.Petrol;//Specific Energy = 46.4, Energy Density = 34.3
                break;
        }
        shipController = gameObject.transform.parent.parent.gameObject.GetComponent<ShipControllerScript>();
        waterControlScript = GameObject.Find("waterPlane").GetComponent<WaterControlScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if the propulsion object is below the water.
        checkIsBelowWater();


        resistance = waterControlScript.density / propellerSize * (1.0f - waterControlScript.dynamicViscosity) * shipController.getSpeed();
        throttle = shipController.getCurrentThrottle();
        if (!shipController)
        {
            shipController = gameObject.transform.parent.parent.gameObject.GetComponent<ShipControllerScript>();
        }
        else
        {
            if (RPM > (int)Mathf.Lerp(0, 5000.0f, throttle / 100))
            {
                RPM += (int)throttle - (int)(throttle * resistance);
                if (RPM > 5000)
                {
                    RPM = 5000;
                }
            }
            else if (RPM < (int)Mathf.Lerp(0, 5000.0f, throttle / 100))
            {
                RPM -= (int)throttle - (int)(throttle * resistance);
                if (RPM < 0)
                {
                    RPM = 0;
                }
            }
            if (RPM > 5000)
            {
                RPM = 5000;
            }
            else if (RPM < 0)
            {
                RPM = 0;
            }

            
            
            propulsionForce = calculateHorsePower(cylinders, cylinderDisplacement, RPM, fuelType);
            print("Propulsion Force" + propulsionForce);
        }

        
    }

    //Gets the horsepower of the engine using the engines properties
    float calculateHorsePower(int cylinders, float cylinderDisplacement, int RPM, Fuel fuelType)
    {
        float result;

        result = (fuelType.energyDensity * (cylinderDisplacement * cylinders)) / 5252;
        result = result * RPM;

        return result;
    }

    void checkIsBelowWater()
    {
        var meshFilter = GetComponent<MeshFilter>();

        var list = meshFilter.mesh.vertices.Select(transform.TransformPoint).OrderBy(v => v.y).ToList();

        Debug.Log(list[0]); // lowest position

        Debug.Log(list.Last()); // highest position

        if (list[0].y < 0.0f)
        {
            isBelowWater = true;
        }
        else
        {
            isBelowWater = false;
        }
    }

    public float getHorsePower()
    {
        //print(propulsionForce);
        return propulsionForce;
    }

    public bool getIsBelowWater()
    {
        return isBelowWater;
    }

}

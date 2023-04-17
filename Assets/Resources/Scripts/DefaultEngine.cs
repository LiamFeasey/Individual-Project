using System.Collections;
using System.Collections.Generic;
using UnityEngine;





enum FuelType
{
    Diesel,
    Petrol
};

public class DefaultEngine : MonoBehaviour
{
    [Header("Engine Properties")]
    [SerializeField] int cylinders;
    [SerializeField] float cylinderDisplacement;
    [SerializeField] float engineDisplacement;
    [SerializeField] float torque;
    [SerializeField] float RPM;
    [SerializeField] float fuelUsage;
    [SerializeField] FuelType engineFuelType;
    [Tooltip("The type of fuel that's being used and its properties (Name, Specific Energy, and Energy Density)")]
    [SerializeField] Fuel fuelType;//The fuel type struct that will contain all the relevant details about the fuel for this engine.

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

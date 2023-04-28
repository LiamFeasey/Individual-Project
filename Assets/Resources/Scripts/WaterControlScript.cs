using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterControlScript : MonoBehaviour
{
    [Tooltip("The dynamic viscosity of the water, effects kinematic viscosity. (Resistance to external forces exlcuding gravity)")]
    [SerializeField] public float dynamicViscosity;

    [Tooltip("The temperature of the water, effects density")]
    [Range(-100f, 100f)]
    [SerializeField] public float temperature;

    [Tooltip("Density of the water, effects the dynamic and kinematic viscosity, as well as the proppeler resistance.")]
    [SerializeField] public float density;

    // Start is called before the first frame update
    void Start()
    {
        dynamicViscosity = 0.9775f;

        density = 998f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        updateWaterDensity(temperature);
    }

    /// <summary>
    /// Calculate the density of the water based on the current temperature
    /// This runs every update as the temperature might change during runtime
    /// </summary>
    /// <param name="temperature">The current temperature of the water</param>
    void updateWaterDensity(float temperature)
    {
        float H20 = 998.2f;// Nominal density of water
        float B = 0.0002f;// Volumetric temperature expansion coefficient for water.
        float T = 20.0f;// Standard temperature
        
        density = H20 / (1 + B * (temperature - T));
    }
}

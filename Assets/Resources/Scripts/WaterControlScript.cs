using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterControlScript : MonoBehaviour
{

    ////The kinematic viscosity of the water, will affect how quick water flows into a hole in a floating point.
    //[Tooltip("The kinematic viscosity of the water, effects how quick water flows into a hole in a floating point.")]
    //[SerializeField] public float kinematicViscosity;

    

    //kinematicViscosity = 0.9795f;

    //The dynamic viscosity of the water, will affect kinematic viscosity. Resistance to external forces exlcuding gravity.
    [Tooltip("The dynamic viscosity of the water, effects kinematic viscosity. (Resistance to external forces exlcuding gravity)")]
    [SerializeField] public float dynamicViscosity;
    //The temperature of the water, will affect density
    [Tooltip("The temperature of the water, effects density")]
    [Range(-100f, 100f)]
    [SerializeField] public float temperature;
    //Density of the water, will affect the dynamic and kinematic viscosity.
    [Tooltip("Density of the water, effect the dynamic and kinematic viscosity.")]
    [SerializeField] public float density;
    //The murkiness of the water, will affect how well you can see through it
    [Tooltip("The murkiness of the water, effects how well you can see through it")]
    [SerializeField] public float murkiness;
    // Start is called before the first frame update

    private Material waterMaterial = null;
    private GameObject waterPlane = null;


    void Start()
    {
        dynamicViscosity = 0.9775f;

        density = 998f;
        //Default murkiness is 200, with a max of 255 and minimum of 0
        murkiness = 200f;


        waterPlane = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //this.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(43f, 101f, 236f, (murkiness/255f)));
        //murkiness += 0.1f;

        updateWaterDensity(temperature);
    }

    void updateWaterDensity(float temperature)
    {
        float H20 = 998.2f;// Nominal density of water
        float B = 0.0002f;// Volumetric temperature expansion coefficient for water.
        float T = 20.0f;// Standard temperature
        
        density = H20 / (1 + B * (temperature - T));
    }



    //========================================[This isn't working, the colour won't Lerp properly]========================================

    //changeWaterColour(Color.Lerp(coldWater, warmWater, (temperature+100)/200));

    //Change the colour of the ocean, could be used to make it grey like off the coast of the UK, or a nice tropical blue.
    //void changeWaterColour(Color waterColourRGB)
    //{
    //    waterPlane.GetComponent<MeshRenderer>().material.color = waterColourRGB;
    //    waterColour = waterColourRGB;
    //    //waterPlane.GetComponent<MeshRenderer>().material.color = new Color(0.0f, 74.0f, 246.0f, 220.0f);
    //    //waterPlane.GetComponent<MeshRenderer>().material.color = new Color(0.0f, 157.0f, 196.0f, 220.0f);
    //}
}

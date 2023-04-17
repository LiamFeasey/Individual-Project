using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterControlScript : MonoBehaviour
{
    //The temperature of the water, will affect density
    [SerializeField] public float temperature;
    //The dynamic viscosity of the water, will affect kinematic viscosity. Resistance to external forces exlcuding gravity.
    [SerializeField] public float dynamicViscosity;
    //The kinematic viscosity of the water, will affect how quick water flows into a hole in a floating point.
    [SerializeField] public float kinematicViscosity;
    //Density of the water, will affect the dynamic and kinematic viscosity.
    [SerializeField] public float density;
    //The murkiness of the water, will affect how well you can see through it
    [SerializeField] public float murkiness;
    // Start is called before the first frame update

    private Material waterMaterial = null;
    void Start()
    {
        temperature = 21.0f;

        dynamicViscosity = 0.9775f;

        kinematicViscosity = 0.9795f;

        density = 998f;
        //Default murkiness is 200, with a max of 255 and minimum of 0
        murkiness = 200f;

    }

    // Update is called once per frame
    void Update()
    {
        //this.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(43f, 101f, 236f, (murkiness/255f)));
        //murkiness += 0.1f;
    }
}

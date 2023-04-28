using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleDetails : MonoBehaviour
{
    [Tooltip("The size of the hole, the larger this is the quicker you will flood here!")]
    [SerializeField] float holeRadius;

    /// <summary>
    /// Set the value of the holes radius
    /// </summary>
    /// <param name="initRadius"></param>
    public void setHoleRadius(float initRadius)
    {
        holeRadius = initRadius;
    }

    /// <summary>
    /// Returns the value of the holes radius
    /// </summary>
    /// <returns></returns>
    public float getHoleRadius()
    {
        return holeRadius;
    }
}

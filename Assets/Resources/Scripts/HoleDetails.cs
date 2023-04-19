using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleDetails : MonoBehaviour
{
    [Tooltip("The size of the hole, the larger this is the quicker you will flood here!")]
    [SerializeField] float holeRadius;
    // Start is called before the first frame update


    public void setHoleRadius(float initRadius)
    {
        holeRadius = initRadius;
    }
    public float getHoleRadius()
    {
        return holeRadius;
    }
}

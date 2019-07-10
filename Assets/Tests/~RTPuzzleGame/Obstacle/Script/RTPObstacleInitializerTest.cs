using RTPuzzle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTPObstacleInitializerTest : MonoBehaviour
{
    private bool hasInit = false;


    private void FixedUpdate()
    {
        if (!hasInit)
        {
            GameObject.FindObjectOfType<RangeTypeObject>().Init(new RangeTypeObjectInitializer(99999f));
            hasInit = true;
        }
    }
}

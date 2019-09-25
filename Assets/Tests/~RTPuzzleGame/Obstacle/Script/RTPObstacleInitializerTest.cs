using GameConfigurationID;
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
            GameObject.FindObjectOfType<RangeTypeObject>().Init(
                RangeTypeObjectDefinitionConfigurationInherentDataBuilder.SphereRangeWithObstacleListener(99999f, RangeTypeID.ATTRACTIVE_OBJECT ,withRangeColliderTracker: false),
                new RangeTypeObjectInitializer());
            hasInit = true;
        }
    }
}

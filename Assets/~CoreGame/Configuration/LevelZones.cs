using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelZones
{

    public static Dictionary<LevelZonesID, string> LevelZonesSceneName = new Dictionary<LevelZonesID, string>()
    {
        {LevelZonesID.LEVEL1, "Level1EnvironmentScene" },
        {LevelZonesID.SEWER, "SewerScene" },
        {LevelZonesID.SEWER_RTP, "RTPuzzle_LV1" }
    };

}

public enum LevelZonesID
{
    LEVEL1, SEWER, SEWER_RTP
}
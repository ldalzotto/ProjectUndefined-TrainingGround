using System.Collections.Generic;

public class LevelZones
{

    public static Dictionary<LevelZonesID, string> LevelZonesSceneName = new Dictionary<LevelZonesID, string>()
    {
        {LevelZonesID.LEVEL1, "SampleScene" },
        {LevelZonesID.SEWER, "SewerScene" }
    };

}

public enum LevelZonesID
{
    LEVEL1, SEWER
}
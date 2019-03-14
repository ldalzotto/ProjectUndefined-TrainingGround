using System.Collections.Generic;

public class LevelZones
{

    public static Dictionary<LevelZonesID, string> LevelZonesSceneName = new Dictionary<LevelZonesID, string>()
    {
        {LevelZonesID.LEVEL1, "Level1EnvironmentScene" },
        {LevelZonesID.SEWER, "SewerScene" },
        {LevelZonesID.SEWER_RTP, "RTPuzzle_LV1" },
        {LevelZonesID.RTP_TEST, "TestPuzzle" }
    };

}

public enum LevelZonesID
{
    LEVEL1 = 0,
    SEWER = 1,
    SEWER_RTP = 2,
    RTP_TEST = 3
}
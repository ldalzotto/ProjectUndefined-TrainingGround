using System.Collections.Generic;

public class LevelZones
{

    public static Dictionary<LevelZonesID, string> LevelZonesSceneName = new Dictionary<LevelZonesID, string>()
    {
        {LevelZonesID.LEVEL1, "Level1EnvironmentScene" },
        {LevelZonesID.SEWER, "SewerScene" },
        {LevelZonesID.SEWER_RTP, "RTPuzzle_LV1" },
        {LevelZonesID.RTP_TEST, "TestPuzzle" },
        {LevelZonesID.SEWER_ADVENTURE, "Adventure_LV1" }
    };

    public static Dictionary<LevelZoneChunkID, string> LevelZonesChunkScenename = new Dictionary<LevelZoneChunkID, string>()
    {
        {LevelZoneChunkID.SEWER_RTP_1, "Sewers_LV1_Chunk" },
        {LevelZoneChunkID.SEWER_RTP_2, "Sewers_LV2_Chunk" }
    };

    public static Dictionary<LevelZonesID, List<LevelZoneChunkID>> LevelHierarchy = new Dictionary<LevelZonesID, List<LevelZoneChunkID>>()
        {
            {LevelZonesID.SEWER_ADVENTURE, new List<LevelZoneChunkID>(){ LevelZoneChunkID.SEWER_RTP_1, LevelZoneChunkID.SEWER_RTP_2} },
            {LevelZonesID.SEWER_RTP, new List<LevelZoneChunkID>(){ LevelZoneChunkID.SEWER_RTP_1} }
        };
}

public enum LevelZonesID
{
    LEVEL1 = 0,
    SEWER = 1,
    SEWER_RTP = 2,
    RTP_TEST = 3,
    RTP_EDITOR_TEST = 4,
    SEWER_ADVENTURE = 5
}

public enum LevelZoneChunkID
{
    SEWER_RTP_1 = 0,
    SEWER_RTP_2 = 1
}
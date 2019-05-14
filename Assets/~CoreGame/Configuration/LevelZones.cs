using System.Collections.Generic;

public class LevelZones
{
    /*
    public static Dictionary<LevelZonesID, List<LevelZoneChunkID>> LevelHierarchy = new Dictionary<LevelZonesID, List<LevelZoneChunkID>>()
        {
            {LevelZonesID.SEWER_ADVENTURE, new List<LevelZoneChunkID>(){ LevelZoneChunkID.SEWER_RTP_1, LevelZoneChunkID.SEWER_RTP_2} },
            {LevelZonesID.SEWER_RTP, new List<LevelZoneChunkID>(){ LevelZoneChunkID.SEWER_RTP_1} }
        };
        */
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
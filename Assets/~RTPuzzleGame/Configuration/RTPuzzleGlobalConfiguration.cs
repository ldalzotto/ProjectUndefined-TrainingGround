using System.Collections.Generic;

public class RTPPlayerActionConfiguration
{
    public static Dictionary<LevelZonesID, List<RTPPlayerAction>> conf = new Dictionary<LevelZonesID, List<RTPPlayerAction>>() {
        {   LevelZonesID.SEWER_RTP, new List<RTPPlayerAction>(){ new LaunchProjectileRTPAction() } }
    };
}
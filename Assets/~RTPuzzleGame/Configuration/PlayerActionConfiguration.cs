using System.Collections.Generic;

namespace RTPuzzle
{
    public class PlayerActionConfiguration
    {
        public static Dictionary<LevelZonesID, List<RTPPlayerAction>> conf = new Dictionary<LevelZonesID, List<RTPPlayerAction>>() {
        {
                LevelZonesID.SEWER_RTP, new List<RTPPlayerAction>(){ new LaunchProjectileAction(LaunchProjectileId.STONE, 0.5f) }
        }
    };
    }
}

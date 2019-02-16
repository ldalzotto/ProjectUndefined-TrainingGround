using System.Collections.Generic;

namespace RTPuzzle
{
    public class LevelConfiguration
    {

        public static Dictionary<LevelZonesID, LevelConfigurationData> conf = new Dictionary<LevelZonesID, LevelConfigurationData>()
        {
            { LevelZonesID.SEWER_RTP, new LevelConfigurationData(20f) }
        };

    }

    public class LevelConfigurationData
    {
        private float availableTimeAmount;

        public LevelConfigurationData(float availableTimeAmount)
        {
            this.availableTimeAmount = availableTimeAmount;
        }

        public float AvailableTimeAmount { get => availableTimeAmount; }
    }
}

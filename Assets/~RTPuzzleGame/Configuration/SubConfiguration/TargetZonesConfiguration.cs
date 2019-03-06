using System.Collections.Generic;

namespace RTPuzzle
{
    public class TargetZoneConfigurationData
    {
        private float escapeMinDistance;
        private float escapeFOVSemiAngle;

        public TargetZoneConfigurationData(float escapeMinDistance, float escapeFOVSemiAngle)
        {
            this.escapeMinDistance = escapeMinDistance;
            this.escapeFOVSemiAngle = escapeFOVSemiAngle;
        }

        public float EscapeMinDistance { get => escapeMinDistance; }
        public float EscapeFOVSemiAngle { get => escapeFOVSemiAngle; }
    }

    public class TargetZonesConfiguration
    {
        public static Dictionary<TargetZoneID, TargetZoneConfigurationData> conf = new Dictionary<TargetZoneID, TargetZoneConfigurationData>()
        {
             {TargetZoneID.LEVEL1_TARGET_ZONE, new TargetZoneConfigurationData(10f, 110f) }
        };
    }
}

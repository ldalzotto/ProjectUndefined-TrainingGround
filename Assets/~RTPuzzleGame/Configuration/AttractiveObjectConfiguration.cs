using System.Collections.Generic;

namespace RTPuzzle
{
    public class AttractiveObjectConfiguration
    {
        public static Dictionary<AttractiveObjectId, AttractiveObjectInherentConfigurationData> conf = new Dictionary<AttractiveObjectId, AttractiveObjectInherentConfigurationData>()
        {
            {AttractiveObjectId.CHEESE, new AttractiveObjectInherentConfigurationData(30f,1f) }
        };
    }

    public class AttractiveObjectInherentConfigurationData
    {
        public float EffectRange;
        public float EffectiveTime;

        public AttractiveObjectInherentConfigurationData(float effectRange, float effectiveTime)
        {
            EffectRange = effectRange;
            EffectiveTime = effectiveTime;
        }
    }

    public enum AttractiveObjectId
    {
        CHEESE
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class AttractiveObjectConfiguration
    {
        public static Dictionary<AttractiveObjectId, AttractiveObjectInherentConfigurationData> conf = new Dictionary<AttractiveObjectId, AttractiveObjectInherentConfigurationData>()
        {
            {AttractiveObjectId.CHEESE, new AttractiveObjectInherentConfigurationData(30f) }
        };
    }

    public class AttractiveObjectInherentConfigurationData
    {
        public float EffectRange;

        public AttractiveObjectInherentConfigurationData(float effectRange)
        {
            EffectRange = effectRange;
        }
    }

    public enum AttractiveObjectId
    {
        CHEESE
    }
}

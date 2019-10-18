using System;
using UnityEngine;

namespace RangeObjects
{
    [Serializable]
    public class LevelRangeEffectInherentData
    {
        public float DeltaIntensity = 0;
        [Range(-0.5f, 0.5f)] public float DeltaMixFactor = 0;
    }
}
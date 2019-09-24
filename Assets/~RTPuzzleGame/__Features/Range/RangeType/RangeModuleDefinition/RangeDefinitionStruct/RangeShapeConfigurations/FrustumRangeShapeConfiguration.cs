using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    public class FrustumRangeShapeConfiguration : RangeShapeConfiguration
    {
        [WireFrustum(R = 0, G = 1, B = 1)]
        public FrustumV2 frustum;
    }
}


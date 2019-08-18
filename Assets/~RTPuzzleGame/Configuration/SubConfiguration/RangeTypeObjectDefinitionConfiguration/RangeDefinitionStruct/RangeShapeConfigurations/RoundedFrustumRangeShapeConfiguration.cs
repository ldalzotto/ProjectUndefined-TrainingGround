using UnityEngine;
using System.Collections;
using CoreGame;

namespace RTPuzzle
{
    [System.Serializable]
    public class RoundedFrustumRangeShapeConfiguration : RangeShapeConfiguration
    {
        public FrustumV2 frustum;
    }
}

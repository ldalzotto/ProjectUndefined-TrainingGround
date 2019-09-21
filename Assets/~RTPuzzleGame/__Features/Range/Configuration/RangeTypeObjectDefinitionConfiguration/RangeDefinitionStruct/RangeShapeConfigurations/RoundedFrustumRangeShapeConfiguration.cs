using CoreGame;

namespace RTPuzzle
{
    [System.Serializable]
    public class RoundedFrustumRangeShapeConfiguration : RangeShapeConfiguration
    {
        [WireRoundedFrustum(R = 0, G = 1, B = 1)]
        public FrustumV2 frustum;
    }
}
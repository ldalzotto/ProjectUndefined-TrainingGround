using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    public class BoxRangeShapeConfiguration : RangeShapeConfiguration
    {
        [WireBox(CenterFieldName = nameof(BoxRangeShapeConfiguration.Center), SizeFieldName = nameof(BoxRangeShapeConfiguration.Size))]
        public Vector3 Center;
        public Vector3 Size;
    }

}

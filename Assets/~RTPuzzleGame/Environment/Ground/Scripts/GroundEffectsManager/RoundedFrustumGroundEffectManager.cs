using UnityEngine;

namespace RTPuzzle
{
    public class RoundedFrustumGroundEffectManager : AbstractGroundEffectManager<RoundedFrustumRangeType>
    {
        public RoundedFrustumGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData) : base(rangeTypeInherentConfigurationData)
        {
        }

        public RoundedFrustumRangeBufferData ToFrustumBuffer()
        {
            var RoundedFrustumRangeBufferData = ((RoundedFrustumRangeType)this.GetAssociatedRangeObject().RangeType).GetRoundedFrustumRangeBufferData();

            if (this.rangeTypeInherentConfigurationData.RangeColorProvider != null)
            {
                RoundedFrustumRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeColorProvider.Invoke();
            }
            else
            {
                RoundedFrustumRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeBaseColor;
            }
            
            if (this.GetAssociatedRangeObject().IsOccludedByFrustum())
            {
                RoundedFrustumRangeBufferData.OccludedByFrustums = 1;
            }
            else
            {
                RoundedFrustumRangeBufferData.OccludedByFrustums = 0;
            }

            return RoundedFrustumRangeBufferData;
        }
    }

    public struct RoundedFrustumRangeBufferData
    {
        public Vector3 FC1;
        public Vector3 FC2;
        public Vector3 FC3;
        public Vector3 FC4;
        public Vector3 FC5;

        public Vector3 normal1;
        public Vector3 normal2;
        public Vector3 normal3;
        public Vector3 normal4;
        public Vector3 normal5;
        public Vector3 normal6;

        public Vector3 BoundingBoxMax;
        public Vector3 BoundingBoxMin;

        public float RangeRadius;
        public Vector3 CenterWorldPosition;
        
        public int OccludedByFrustums;
        public Vector4 AuraColor;

        public static int GetByteSize()
        {
            return (((3 * 5) + (3 * 6) + (3 * 2) + 4 + 1 + 3) * sizeof(float)) + ((1) * sizeof(int));
        }
    };
}
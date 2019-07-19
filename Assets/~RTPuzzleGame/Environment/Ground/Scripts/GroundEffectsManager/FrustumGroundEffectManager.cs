using UnityEngine;

namespace RTPuzzle
{
    public class FrustumGroundEffectManager : AbstractGroundEffectManager<FrustumRangeType>
    {
        public FrustumGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData) : base(rangeTypeInherentConfigurationData)
        {
        }

        public FrustumRangeBufferData ToFrustumBuffer()
        {
            var FrustumRangeBufferData = ((FrustumRangeType)this.GetAssociatedRangeObject().RangeType).GetFrustumRangeBufferData();

            if (this.rangeTypeInherentConfigurationData.RangeColorProvider != null)
            {
                FrustumRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeColorProvider.Invoke();
            }
            else
            {
                FrustumRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeBaseColor;
            }

            FrustumRangeBufferData.AuraTextureAlbedoBoost = 0.1f;
            FrustumRangeBufferData.AuraAnimationSpeed = 20f;

            if (this.GetAssociatedRangeObject().IsOccludedByFrustum())
            {
                FrustumRangeBufferData.OccludedByFrustums = 1;
            }
            else
            {
                FrustumRangeBufferData.OccludedByFrustums = 0;
            }

            return FrustumRangeBufferData;
        }
    }

    public struct FrustumRangeBufferData
    {
        public Vector3 FC1;
        public Vector3 FC2;
        public Vector3 FC3;
        public Vector3 FC4;
        public Vector3 FC5;
        public Vector3 FC6;
        public Vector3 FC7;
        public Vector3 FC8;

        public int OccludedByFrustums;
        public Vector4 AuraColor;
        public float AuraTextureAlbedoBoost;
        public float AuraAnimationSpeed;

        public static int GetByteSize()
        {
            return (((3 * 8) + 4 + 1 + 1) * sizeof(float)) + ((1) * sizeof(int));
        }
    };
}
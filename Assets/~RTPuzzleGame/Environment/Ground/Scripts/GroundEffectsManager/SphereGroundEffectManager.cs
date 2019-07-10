using UnityEngine;

namespace RTPuzzle
{
    public class SphereGroundEffectManager : AbstractGroundEffectManager<SphereRangeType>
    {

        public SphereGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData) : base(rangeTypeInherentConfigurationData)
        { }

        public CircleRangeBufferData ToSphereBuffer()
        {
            CircleRangeBufferData CircleRangeBufferData = new CircleRangeBufferData();
            CircleRangeBufferData.CenterWorldPosition = this.AssociatedRangeObject.RangeType.GetCenterWorldPos();
            CircleRangeBufferData.Radius = this.rangeAnimation.CurrentValue;
            if (this.rangeTypeInherentConfigurationData.RangeColorProvider != null)
            {
                CircleRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeColorProvider.Invoke();
            }
            else
            {
                CircleRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeBaseColor;
            }
            CircleRangeBufferData.AuraTextureAlbedoBoost = 0.1f;
            CircleRangeBufferData.AuraAnimationSpeed = 20f;

            if (this.AssociatedRangeObject.IsOccludedByFrustum())
            {
                CircleRangeBufferData.OccludedByFrustums = 1;
            }
            else
            {
                CircleRangeBufferData.OccludedByFrustums = 0;
            }

            return CircleRangeBufferData;
        }

    }

    public struct CircleRangeBufferData
    {
        public Vector3 CenterWorldPosition;
        public float Radius;
        public Vector4 AuraColor;
        public float AuraTextureAlbedoBoost;
        public float AuraAnimationSpeed;

        public int OccludedByFrustums;

        public static int GetByteSize()
        {
            return ((3 + 1 + 4 + 1 + 1) * sizeof(float)) + ((1) * sizeof(int));
        }
    }
}


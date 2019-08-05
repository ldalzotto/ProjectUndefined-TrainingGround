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
            CircleRangeBufferData.CenterWorldPosition = this.GetAssociatedRangeObject().RangeType.GetCenterWorldPos();
            CircleRangeBufferData.WorldRangeForward = this.GetAssociatedRangeObject().transform.forward;

            CircleRangeBufferData.Radius = this.rangeAnimation.CurrentValue;
            if (this.rangeTypeInherentConfigurationData.RangeColorProvider != null)
            {
                CircleRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeColorProvider.Invoke();
            }
            else
            {
                CircleRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeBaseColor;
            }

            if (this.GetAssociatedRangeObject().IsOccludedByFrustum())
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
        public Vector3 WorldRangeForward;
        public float Radius;
        public Vector4 AuraColor;
        public int OccludedByFrustums;

        public static int GetByteSize()
        {
            return ((3 + 3 + 1 + 4 ) * sizeof(float)) + ((1) * sizeof(int));
        }
    }
}


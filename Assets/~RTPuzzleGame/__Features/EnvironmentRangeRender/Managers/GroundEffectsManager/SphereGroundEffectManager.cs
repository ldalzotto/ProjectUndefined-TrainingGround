using UnityEngine;

namespace RTPuzzle
{
    public class SphereGroundEffectManager : AbstractGroundEffectManager<SphereRangeType>
    {
        private SphereRangeObjectRenderingDataProvider SphereRangeObjectRenderingDataProvider;

        public SphereGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData,
            SphereRangeObjectRenderingDataProvider SphereRangeObjectRenderingDataProvider) : base(rangeTypeInherentConfigurationData)
        {
            this.SphereRangeObjectRenderingDataProvider = SphereRangeObjectRenderingDataProvider;
        }

        public CircleRangeBufferData ToSphereBuffer()
        {
            CircleRangeBufferData CircleRangeBufferData = new CircleRangeBufferData();
            CircleRangeBufferData.CenterWorldPosition = this.SphereRangeObjectRenderingDataProvider.GetWorldPositionCenter();

            CircleRangeBufferData.Radius = this.SphereRangeObjectRenderingDataProvider.GetRadius();
            if (this.rangeTypeInherentConfigurationData.RangeColorProvider != null)
            {
                CircleRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeColorProvider.Invoke();
            }
            else
            {
                CircleRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeBaseColor;
            }

            if (this.rangeObjectRenderingDataProvider.IsTakingObstacleIntoConsideration())
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
        public int OccludedByFrustums;

        public static int GetByteSize()
        {
            return ((3 + 1 + 4) * sizeof(float)) + ((1) * sizeof(int));
        }
    }
}


using UnityEngine;

namespace RTPuzzle
{
    public class FrustumGroundEffectManager : AbstractGroundEffectManager<FrustumRangeType>
    {
        private FrustumRangeObjectRenderingDataprovider FrustumRangeObjectRenderingDataprovider;
        public FrustumGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData, FrustumRangeObjectRenderingDataprovider FrustumRangeObjectRenderingDataprovider) : base(rangeTypeInherentConfigurationData)
        {
            this.FrustumRangeObjectRenderingDataprovider = FrustumRangeObjectRenderingDataprovider;
        }

        public FrustumRangeBufferData ToFrustumBuffer()
        {
            var frustumPointsLocalPositions =  this.FrustumRangeObjectRenderingDataprovider.Frustum.FrustumPointsPositions;

            var FrustumRangeBufferData = new FrustumRangeBufferData();
            FrustumRangeBufferData.FC1 = this.FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC1);
            FrustumRangeBufferData.FC2 = this.FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC2);
            FrustumRangeBufferData.FC3 = this.FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC3);
            FrustumRangeBufferData.FC4 = this.FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC4);
            FrustumRangeBufferData.FC5 = this.FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC5);
            FrustumRangeBufferData.FC6 = this.FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC6);
            FrustumRangeBufferData.FC7 = this.FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC7);
            FrustumRangeBufferData.FC8 = this.FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC8);
            
            if (this.rangeTypeInherentConfigurationData.RangeColorProvider != null)
            {
                FrustumRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeColorProvider.Invoke();
            }
            else
            {
                FrustumRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeBaseColor;
            }
            
            if (this.rangeObjectRenderingDataProvider.IsTakingObstacleIntoConsideration())
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

        public static int GetByteSize()
        {
            return (((3 * 8) + 4) * sizeof(float)) + ((1) * sizeof(int));
        }
    };
}
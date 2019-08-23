using CoreGame;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    public class FrustumRangeType : AbstractFrustumRangeType
    {
        public override float GetRadiusRange()
        {
            float diagonalDistance = Vector3.Distance(Vector3.zero, new Vector3(0, this.frustumBoundingBox.size.y / 2f, this.frustumBoundingBox.size.z));
            return Mathf.Max(this.frustumBoundingBox.size.z, this.frustumBoundingBox.size.x / 2f, this.frustumBoundingBox.size.y / 2f, diagonalDistance);
        }

        public FrustumRangeBufferData GetFrustumRangeBufferData()
        {
            if (!this.frustumPointsLocalPositions.HasValue)
            {
                this.DoFrustumCalculation();
            }

            var FrustumRangeBufferData = new FrustumRangeBufferData();
            FrustumRangeBufferData.FC1 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC1);
            FrustumRangeBufferData.FC2 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC2);
            FrustumRangeBufferData.FC3 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC3);
            FrustumRangeBufferData.FC4 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC4);
            FrustumRangeBufferData.FC5 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC5);
            FrustumRangeBufferData.FC6 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC6);
            FrustumRangeBufferData.FC7 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC7);
            FrustumRangeBufferData.FC8 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC8);
            return FrustumRangeBufferData;
        }


        public override bool IsInside(BoxCollider boxCollider)
        {
            return Intersection.FrustumBoxIntersection(this.GetFrustumPointsWorldPositions(), boxCollider) || Intersection.BoxEntirelyContainedInFrustum(this.GetFrustumPointsWorldPositions(), boxCollider);
        }

        public override bool IsInside(Vector3 worldPointComparison)
        {
            return Intersection.PointInsideFrustum(this.frustumPointsWorldPositions, worldPointComparison);
        }

#if UNITY_EDITOR

        public override void HandlesDraw()
        {
        }
#endif
    }
}


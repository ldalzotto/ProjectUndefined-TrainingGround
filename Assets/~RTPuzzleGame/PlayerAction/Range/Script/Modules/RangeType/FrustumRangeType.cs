using CoreGame;
using System;
using UnityEngine;

namespace RTPuzzle
{
    public class FrustumRangeType : RangeType
    {
        [SerializeField]
        private FrustumV2 frustum;

        private BoxCollider boxCollider;

        private Nullable<FrustumPointsPositions> frustumPointsLocalPositions;
        private FrustumPointsPositions frustumPointsWorldPositions;

        public FrustumPointsPositions GetFrustumPointsWorldPositions()
        {
            if (!this.frustumPointsLocalPositions.HasValue)
            {
                this.DoCalculation();
            }

            this.frustumPointsWorldPositions = new FrustumPointsPositions(
                    this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC1),
                    this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC2),
                    this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC3),
                    this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC4),
                    this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC5),
                    this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC6),
                    this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC7),
                    this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC8)
             );
            return this.frustumPointsWorldPositions;
        }

        public FrustumRangeBufferData GetFrustumRangeBufferData()
        {
            if (!this.frustumPointsLocalPositions.HasValue)
            {
                this.DoCalculation();
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

        public override void Init(RangeTypeObjectInitializer RangeTypeObjectInitializer, RangeTypeObject RangeTypeObjectRef)
        {
            base.Init(RangeTypeObjectInitializer, RangeTypeObjectRef);
            this.boxCollider = GetComponent<BoxCollider>();
            this.boxCollider.center = new Vector3(0, 0, this.frustum.F2.FaceOffsetFromCenter.z / 4f);
            this.boxCollider.size = new Vector3(Mathf.Max(this.frustum.F1.Width, this.frustum.F2.Width), Math.Max(this.frustum.F1.Height, this.frustum.F2.Height), this.frustum.F2.FaceOffsetFromCenter.z / 2f);
        }

        public override Vector3 GetCenterWorldPos()
        {
            return this.transform.position;
        }

        public override Collider GetCollider()
        {
            return this.boxCollider;
        }

        public override float GetRadiusRange()
        {
            float diagonalDistance = Vector3.Distance(Vector3.zero, new Vector3(0, this.boxCollider.size.y / 2f, this.boxCollider.size.z));
            return Mathf.Max(this.boxCollider.size.z, this.boxCollider.size.x / 2f, this.boxCollider.size.y / 2f, diagonalDistance);
        }

        public override bool IsInside(Vector3 worldPointComparison)
        {
            return Intersection.PointInsideFrustum(this.frustumPointsWorldPositions, worldPointComparison);
        }

        private void DoCalculation()
        {
            this.frustum.SetCalculationDataForFaceBasedCalculation(Vector3.zero, Quaternion.identity, Vector3.one);
            this.frustum.CalculateFrustumPoints(out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8);
            this.frustumPointsLocalPositions = new FrustumPointsPositions(C1, C2, C3, C4, C5, C6, C7, C8);

        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            this.DoCalculation();
            var worldPosFrustum = GetFrustumPointsWorldPositions();

            this.DrawFace(worldPosFrustum.FC1, worldPosFrustum.FC2, worldPosFrustum.FC3, worldPosFrustum.FC4);
            this.DrawFace(worldPosFrustum.FC1, worldPosFrustum.FC5, worldPosFrustum.FC6, worldPosFrustum.FC2);
            this.DrawFace(worldPosFrustum.FC2, worldPosFrustum.FC6, worldPosFrustum.FC7, worldPosFrustum.FC3);
            this.DrawFace(worldPosFrustum.FC3, worldPosFrustum.FC7, worldPosFrustum.FC8, worldPosFrustum.FC4);
            this.DrawFace(worldPosFrustum.FC4, worldPosFrustum.FC8, worldPosFrustum.FC5, worldPosFrustum.FC1);
            this.DrawFace(worldPosFrustum.FC5, worldPosFrustum.FC6, worldPosFrustum.FC7, worldPosFrustum.FC8);

        }

        private void DrawFace(Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4)
        {
            Gizmos.DrawLine(C1, C2);
            Gizmos.DrawLine(C2, C3);
            Gizmos.DrawLine(C3, C4);
            Gizmos.DrawLine(C4, C1);
        }
#endif
    }
}


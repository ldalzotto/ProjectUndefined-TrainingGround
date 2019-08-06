using CoreGame;
using System;
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

        private void OnDrawGizmos()
        {
            this.HandlesDraw();
        }

        public override void HandlesDraw()
        {
            this.DoFrustumCalculation();
            var worldPosFrustum = GetFrustumPointsWorldPositions();

            var oldGizmoColor = Handles.color;
            Handles.color = MyColors.HotPink;
            this.DrawFace(worldPosFrustum.FC1, worldPosFrustum.FC2, worldPosFrustum.FC3, worldPosFrustum.FC4);
            this.DrawFace(worldPosFrustum.FC1, worldPosFrustum.FC5, worldPosFrustum.FC6, worldPosFrustum.FC2);
            this.DrawFace(worldPosFrustum.FC2, worldPosFrustum.FC6, worldPosFrustum.FC7, worldPosFrustum.FC3);
            this.DrawFace(worldPosFrustum.FC3, worldPosFrustum.FC7, worldPosFrustum.FC8, worldPosFrustum.FC4);
            this.DrawFace(worldPosFrustum.FC4, worldPosFrustum.FC8, worldPosFrustum.FC5, worldPosFrustum.FC1);
            this.DrawFace(worldPosFrustum.FC5, worldPosFrustum.FC6, worldPosFrustum.FC7, worldPosFrustum.FC8);
            Handles.color = oldGizmoColor;
        }

        private void DrawFace(Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4)
        {
            Handles.DrawLine(C1, C2);
            Handles.DrawLine(C2, C3);
            Handles.DrawLine(C3, C4);
            Handles.DrawLine(C4, C1);
        }
#endif
    }
}


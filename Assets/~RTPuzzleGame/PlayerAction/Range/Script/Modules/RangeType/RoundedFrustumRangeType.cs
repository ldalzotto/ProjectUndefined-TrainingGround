using CoreGame;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    public class RoundedFrustumRangeType : AbstractFrustumRangeType
    {
        private float rangeRadius;
        
        public override void Init(RangeTypeObjectInitializer RangeTypeObjectInitializer, RangeTypeObject RangeTypeObjectRef)
        {
            base.Init(RangeTypeObjectInitializer, RangeTypeObjectRef);
            this.rangeRadius = this.frustum.F2.FaceOffsetFromCenter.z / 2f;
        }

        public override float GetRadiusRange()
        {
            return this.rangeRadius;
        }

        public RoundedFrustumRangeBufferData GetRoundedFrustumRangeBufferData()
        {
            if (!this.frustumPointsLocalPositions.HasValue)
            {
                this.DoFrustumCalculation();
            }

            var RoundedFrustumRangeBufferData = new RoundedFrustumRangeBufferData();
            RoundedFrustumRangeBufferData.FC1 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC1);
            RoundedFrustumRangeBufferData.FC2 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC2);
            RoundedFrustumRangeBufferData.FC3 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC3);
            RoundedFrustumRangeBufferData.FC4 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC4);
            RoundedFrustumRangeBufferData.FC5 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC5);
            RoundedFrustumRangeBufferData.FC6 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC6);
            RoundedFrustumRangeBufferData.FC7 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC7);
            RoundedFrustumRangeBufferData.FC8 = this.transform.TransformPoint(frustumPointsLocalPositions.Value.FC8);

            RoundedFrustumRangeBufferData.RangeRadius = this.rangeRadius;
            RoundedFrustumRangeBufferData.CenterWorldPosition = this.transform.position;

            return RoundedFrustumRangeBufferData;
        }

        public override bool IsInside(Vector3 worldPointComparison)
        {
            return Vector3.Distance(this.transform.position, worldPointComparison) <= rangeRadius && Intersection.PointInsideFrustum(this.frustumPointsWorldPositions, worldPointComparison);
        }
        
        public override bool IsInside(BoxCollider boxCollider)
        {
            return Intersection.BoxIntersectsSphereV2(boxCollider, this.transform.position, this.rangeRadius)
                && (Intersection.FrustumBoxIntersection(this.GetFrustumPointsWorldPositions(), boxCollider) || Intersection.BoxEntirelyContainedInFrustum(this.GetFrustumPointsWorldPositions(), boxCollider));
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
            Handles.DrawWireDisc(this.transform.position, Vector3.up, this.frustum.F2.FaceOffsetFromCenter.z / 2f);
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

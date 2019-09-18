using CoreGame;
using UnityEngine;

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

            var RoundedFrustumRangeBufferData = new RoundedFrustumRangeBufferData();
            var frustumPointsWorldPositions = this.GetFrustumPointsWorldPositions();
            RoundedFrustumRangeBufferData.FC1 = frustumPointsWorldPositions.FC1;
            RoundedFrustumRangeBufferData.FC2 = frustumPointsWorldPositions.FC2;
            RoundedFrustumRangeBufferData.FC3 = frustumPointsWorldPositions.FC3;
            RoundedFrustumRangeBufferData.FC4 = frustumPointsWorldPositions.FC4;
            RoundedFrustumRangeBufferData.FC5 = frustumPointsWorldPositions.FC5;
            RoundedFrustumRangeBufferData.normal1 = frustumPointsWorldPositions.normal1;
            RoundedFrustumRangeBufferData.normal2 = frustumPointsWorldPositions.normal2;
            RoundedFrustumRangeBufferData.normal3 = frustumPointsWorldPositions.normal3;
            RoundedFrustumRangeBufferData.normal4 = frustumPointsWorldPositions.normal4;
            RoundedFrustumRangeBufferData.normal5 = frustumPointsWorldPositions.normal5;
            RoundedFrustumRangeBufferData.normal6 = frustumPointsWorldPositions.normal6;

            RoundedFrustumRangeBufferData.BoundingBoxMax = this.frustumBoundingBox.bounds.max;
            RoundedFrustumRangeBufferData.BoundingBoxMin = this.frustumBoundingBox.bounds.min;

            RoundedFrustumRangeBufferData.RangeRadius = this.rangeRadius;
            RoundedFrustumRangeBufferData.CenterWorldPosition = this.transform.position;

            return RoundedFrustumRangeBufferData;
        }

        public override bool IsInside(Vector3 worldPointComparison)
        {
            return Vector3.Distance(this.transform.position, worldPointComparison) <= rangeRadius && Intersection.PointInsideFrustum(this.GetFrustumPointsWorldPositions(), worldPointComparison);
        }

        public override bool IsInside(BoxCollider boxCollider)
        {
            return Intersection.BoxIntersectsOrEntirelyContainedInSphere(boxCollider, this.transform.position, this.rangeRadius)
                && (Intersection.FrustumBoxIntersection(this.GetFrustumPointsWorldPositions(), boxCollider) || Intersection.BoxEntirelyContainedInFrustum(this.GetFrustumPointsWorldPositions(), boxCollider));
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            var oldCol = Gizmos.color;
            Gizmos.color = Color.blue;
            Gizmos.color = oldCol;
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

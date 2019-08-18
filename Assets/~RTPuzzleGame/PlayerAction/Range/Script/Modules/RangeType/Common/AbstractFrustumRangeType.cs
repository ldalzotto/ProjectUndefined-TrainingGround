using CoreGame;
using System;
using UnityEngine;

namespace RTPuzzle
{
    public abstract class AbstractFrustumRangeType : RangeType
    {
        [SerializeField]
        protected FrustumV2 frustum;

        protected BoxCollider frustumBoundingBox;

        protected Nullable<FrustumPointsPositions> frustumPointsLocalPositions;
        protected FrustumPointsPositions frustumPointsWorldPositions;

        public FrustumPointsPositions GetFrustumPointsWorldPositions()
        {
            if (!this.frustumPointsLocalPositions.HasValue)
            {
                this.DoFrustumCalculation();
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


        public override void PopulateFromDefinition(RangeTypeDefinition RangeTypeDefinition)
        {
            base.PopulateFromDefinition(RangeTypeDefinition);
            if (RangeTypeDefinition.RangeShapeConfiguration != null)
            {
                if (RangeTypeDefinition.RangeShapeConfiguration.GetType() == typeof(FrustumRangeShapeConfiguration))
                {
                    this.frustum = ((FrustumRangeShapeConfiguration)RangeTypeDefinition.RangeShapeConfiguration).frustum.Clone();
                }
                else if (RangeTypeDefinition.RangeShapeConfiguration.GetType() == typeof(RoundedFrustumRangeShapeConfiguration))
                {
                    this.frustum = ((RoundedFrustumRangeShapeConfiguration)RangeTypeDefinition.RangeShapeConfiguration).frustum.Clone();
                }
            }
        }

        public override void Init(RangeTypeObjectInitializer RangeTypeObjectInitializer, RangeTypeObject RangeTypeObjectRef)
        {
            base.Init(RangeTypeObjectInitializer, RangeTypeObjectRef);
            this.frustumBoundingBox = GetComponent<BoxCollider>();
            this.frustumBoundingBox.center = new Vector3(0, 0, this.frustum.F2.FaceOffsetFromCenter.z / 4f);
            this.frustumBoundingBox.size = new Vector3(Mathf.Max(this.frustum.F1.Width, this.frustum.F2.Width), Math.Max(this.frustum.F1.Height, this.frustum.F2.Height), this.frustum.F2.FaceOffsetFromCenter.z / 2f);
        }

        public override Vector3 GetCenterWorldPos()
        {
            return this.transform.position;
        }

        public override Collider GetCollider()
        {
            return this.frustumBoundingBox;
        }

        protected void DoFrustumCalculation()
        {
            this.frustum.SetCalculationDataForFaceBasedCalculation(Vector3.zero, Quaternion.identity, Vector3.one);
            this.frustum.CalculateFrustumPoints(out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8);
            this.frustumPointsLocalPositions = new FrustumPointsPositions(C1, C2, C3, C4, C5, C6, C7, C8);
        }
    }
}


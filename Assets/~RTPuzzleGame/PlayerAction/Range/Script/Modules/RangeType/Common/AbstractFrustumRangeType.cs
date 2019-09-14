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

        protected Nullable<FrustumPointsPositions> frustumPointsWorldPositions;

        public FrustumPointsPositions GetFrustumPointsWorldPositions()
        {
            if(this.frustumPointsWorldPositions == null || !this.frustumPointsWorldPositions.HasValue)
            {
                this.DoFrustumCalculation();
            }
            return this.frustumPointsWorldPositions.Value;
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

        public override void EndOfFrameTick()
        {
            this.frustumPointsWorldPositions = null;
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
            this.frustum.SetCalculationDataForFaceBasedCalculation(this.transform.position, this.transform.rotation, this.transform.lossyScale);
            this.frustumPointsWorldPositions = this.frustum.CalculateFrustumPointsWorldPosV2();
        }
    }
}


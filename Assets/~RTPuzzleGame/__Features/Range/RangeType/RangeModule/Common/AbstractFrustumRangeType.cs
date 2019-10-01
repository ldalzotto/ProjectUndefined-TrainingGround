using CoreGame;
using System;
using UnityEngine;

namespace RTPuzzle
{
    public abstract class AbstractFrustumRangeType : RangeType, TransformChangeListener
    {
        [SerializeField]
        protected FrustumV2 frustum;

        protected BoxCollider frustumBoundingBox;

        private BlittableTransformChangeListenerManager positionChangeListener;
        private bool calculateFrustumWorldPos;

        public FrustumPointsPositions GetFrustumPointsWorldPositions()
        {
            if(this.calculateFrustumWorldPos)
            {
                this.DoFrustumCalculation();
                this.calculateFrustumWorldPos = false;
            }
            return this.frustum.FrustumPointsPositions;
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
            this.positionChangeListener = new BlittableTransformChangeListenerManager(true, true, this);
            this.frustumBoundingBox = GetComponent<BoxCollider>();
            this.frustumBoundingBox.center = new Vector3(0, 0, this.frustum.F2.FaceOffsetFromCenter.z / 4f);
            this.frustumBoundingBox.size = new Vector3(Mathf.Max(this.frustum.F1.Width, this.frustum.F2.Width), Math.Max(this.frustum.F1.Height, this.frustum.F2.Height), this.frustum.F2.FaceOffsetFromCenter.z / 2f);
        }

        public override void Tick(float d)
        {
            this.positionChangeListener.Tick(this.transform.position, this.transform.rotation);
        }

        private void ClearFrustumWorldPositions()
        {
            this.calculateFrustumWorldPos = true;
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
        }

        public void onPositionChange()
        {
            this.ClearFrustumWorldPositions();
        }

        public void onRotationChange()
        {
            this.ClearFrustumWorldPositions();
        }
    }
}


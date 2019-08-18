using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class BoxRangeType : RangeType
    {
        #region Internal Component
        private BoxCollider BoxCollider;
        #endregion

        private Vector3 localSize;

        public Vector3 LocalSize { get => localSize; }

        public override void PopulateFromDefinition(RangeTypeDefinition rangeTypeDefinition)
        {
            base.PopulateFromDefinition(rangeTypeDefinition);
            if(rangeTypeDefinition.RangeShapeConfiguration != null && rangeTypeDefinition.RangeShapeConfiguration.GetType() == typeof(BoxRangeShapeConfiguration))
            {
                var BoxRangeShapeConfiguration = (BoxRangeShapeConfiguration)rangeTypeDefinition.RangeShapeConfiguration;
                this.BoxCollider = GetComponent<BoxCollider>();
                this.BoxCollider.size = BoxRangeShapeConfiguration.Size;
                this.BoxCollider.center = BoxRangeShapeConfiguration.Center;
            }
        }

        public override void Init(RangeTypeObjectInitializer RangeTypeObjectInitializer, RangeTypeObject RangeTypeObjectRef)
        {
            this.BoxCollider = GetComponent<BoxCollider>();
            this.localSize = this.BoxCollider.size;
            base.Init(RangeTypeObjectInitializer, RangeTypeObjectRef);
        }
        
        public override Collider GetCollider()
        {
            return this.BoxCollider;
        }

        public override float GetRadiusRange()
        {
            return Mathf.Max(this.BoxCollider.bounds.size.x, this.BoxCollider.bounds.size.y, this.BoxCollider.bounds.size.z);
        }

        public override bool IsInside(BoxCollider boxCollider)
        {
            return false;
        }

        public override bool IsInside(Vector3 worldPointComparison)
        {
            return false;
        }

        public override Vector3 GetCenterWorldPos()
        {
            return this.BoxCollider.center + this.transform.position;
        }
    }

}

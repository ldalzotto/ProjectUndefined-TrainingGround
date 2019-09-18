using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class BoxRangeType : RangeType
    {
        #region Internal Component
        private BoxCollider boxCollider;
        #endregion

        private Vector3 localSize;

        public BoxCollider BoxCollider { get => boxCollider; }

        public override void PopulateFromDefinition(RangeTypeDefinition rangeTypeDefinition)
        {
            base.PopulateFromDefinition(rangeTypeDefinition);
            if(rangeTypeDefinition.RangeShapeConfiguration != null && rangeTypeDefinition.RangeShapeConfiguration.GetType() == typeof(BoxRangeShapeConfiguration))
            {
                var BoxRangeShapeConfiguration = (BoxRangeShapeConfiguration)rangeTypeDefinition.RangeShapeConfiguration;
                this.boxCollider = GetComponent<BoxCollider>();
                this.boxCollider.size = BoxRangeShapeConfiguration.Size;
                this.boxCollider.center = BoxRangeShapeConfiguration.Center;
            }
        }

        public override void Init(RangeTypeObjectInitializer RangeTypeObjectInitializer, RangeTypeObject RangeTypeObjectRef)
        {
            this.boxCollider = GetComponent<BoxCollider>();
            this.localSize = this.boxCollider.size;
            base.Init(RangeTypeObjectInitializer, RangeTypeObjectRef);
        }
        
        public override Collider GetCollider()
        {
            return this.boxCollider;
        }

        public override float GetRadiusRange()
        {
            return Mathf.Max(this.boxCollider.bounds.size.x, this.boxCollider.bounds.size.y, this.boxCollider.bounds.size.z);
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
            return this.boxCollider.center + this.transform.position;
        }
    }

}

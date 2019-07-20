using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class BoxRangeType : RangeType
    {
        #region Internal Component
        private BoxCollider BoxCollider;
        #endregion

        public Vector3 LocalSize;

        public override void Init(RangeTypeObjectInitializer RangeTypeObjectInitializer, RangeTypeObject RangeTypeObjectRef)
        {
            this.BoxCollider = GetComponent<BoxCollider>();
            this.LocalSize = this.BoxCollider.size;
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

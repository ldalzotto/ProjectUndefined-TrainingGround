using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class BoxRangeType : RangeType
    {
        #region Internal Component
        private BoxCollider BoxCollider;
        #endregion

        public Vector3 WorldPositionCenter;
        public Vector3 LocalSize;

        public override void Init()
        {
            this.BoxCollider = GetComponent<BoxCollider>();
            this.WorldPositionCenter = this.BoxCollider.center + this.transform.position;
            this.LocalSize = this.BoxCollider.size;
            base.Init();
        }
        
        public override Collider GetCollider()
        {
            return this.BoxCollider;
        }

        public override float GetRadiusRange()
        {
            return Mathf.Max(this.BoxCollider.bounds.size.x, this.BoxCollider.bounds.size.y, this.BoxCollider.bounds.size.z);
        }

        public override bool IsInside(Vector3 worldPointComparison)
        {
            return false;
        }
    }

}

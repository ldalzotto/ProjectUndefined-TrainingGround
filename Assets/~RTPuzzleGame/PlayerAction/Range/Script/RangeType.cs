using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public abstract class RangeType : MonoBehaviour
    {
        [SerializeField]
        private RangeTypeID rangeTypeID;

        protected RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData;

        #region External dependencies
        protected RangeTypeContainer rangeTypeContainer;

        #endregion

        public RangeTypeID RangeTypeID { get => rangeTypeID; set => rangeTypeID = value; }

        public virtual void Init()
        {
            this.rangeTypeContainer = GameObject.FindObjectOfType<RangeTypeContainer>();
            this.rangeTypeInherentConfigurationData = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().RangeTypeConfiguration()[this.rangeTypeID];
            rangeTypeContainer.AddRange(this);
        }

        private void OnDestroy()
        {
            rangeTypeContainer.RemoveRange(this);
        }

        public virtual void Tick(float d) { }

        public abstract bool IsInside(Vector3 worldPointComparison);
        public abstract Collider GetCollider();

        public static RangeType RetrieveFromCollisionType(CollisionType collisionType)
        {
            if (collisionType != null && collisionType.IsRange)
            {
                return collisionType.GetComponent<RangeType>();
            }
            return null;
        }

        #region Logical conditions
        public bool IsInRangeEffectEnabled()
        {
            return this.IsRangeConfigurationDefined() && this.rangeTypeInherentConfigurationData.InRangeEffectMaterial != null;
        }
        public bool IsRangeConfigurationDefined()
        {
            return this.rangeTypeInherentConfigurationData != null;
        }
        #endregion
    }

}
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public abstract class RangeType : MonoBehaviour
    {
        [SerializeField]
        private RangeTypeID rangeTypeID;

        protected RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData;
        protected RangeTypeObject RangeTypeObjectRef;

        public RangeTypeID RangeTypeID { get => rangeTypeID; set => rangeTypeID = value; }

        public virtual void PopulateFromDefinition(RangeTypeDefinition rangeTypeDefinition)
        {
            this.rangeTypeID = rangeTypeDefinition.RangeTypeID;
        }

        public virtual void Init(RangeTypeObjectInitializer RangeTypeObjectInitializer, RangeTypeObject RangeTypeObjectRef)
        {
            this.rangeTypeInherentConfigurationData = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().RangeTypeConfiguration()[this.rangeTypeID];
            this.RangeTypeObjectRef = RangeTypeObjectRef;
        }

        public virtual void Tick(float d) { }

#if UNITY_EDITOR
        public virtual void HandlesDraw() { }
#endif
        public abstract bool IsInside(Vector3 worldPointComparison);
        public abstract bool IsInside(BoxCollider boxCollider);
        public abstract Collider GetCollider();
        public abstract float GetRadiusRange();
        public abstract Vector3 GetCenterWorldPos();

        public static RangeType RetrieveFromCollisionType(CollisionType collisionType)
        {
            if (collisionType != null && collisionType.IsRange)
            {
                return collisionType.GetComponent<RangeType>();
            }
            return null;
        }

        private void OnTriggerEnter(Collider other)
        {
            this.RangeTypeObjectRef.OnRangeTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            this.RangeTypeObjectRef.OnRangeTriggerExit(other);
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
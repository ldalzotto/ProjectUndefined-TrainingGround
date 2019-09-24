using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public abstract class RangeType : MonoBehaviour
    {
        [SerializeField]
        private RangeTypeID rangeTypeID;

        private CollisionType associatedCollisionType;
        protected RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData;
        protected RangeTypeObject RangeTypeObjectRef;

        public RangeTypeID RangeTypeID { get => rangeTypeID; set => rangeTypeID = value; }

        public virtual void PopulateFromDefinition(RangeTypeDefinition rangeTypeDefinition)
        {
            this.rangeTypeID = rangeTypeDefinition.RangeTypeID;
        }

        public virtual void Init(RangeTypeObjectInitializer RangeTypeObjectInitializer, RangeTypeObject RangeTypeObjectRef)
        {
            this.rangeTypeInherentConfigurationData = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.RangeTypeConfiguration()[this.rangeTypeID];
            this.RangeTypeObjectRef = RangeTypeObjectRef;
            this.associatedCollisionType = GetComponent<CollisionType>();
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

        protected void SetWorldPosition(Vector3 worldPosition)
        {
            this.RangeTypeObjectRef.transform.position = worldPosition;
        }

        public static RangeType RetrieveFromCollisionType(CollisionType collisionType)
        {
            if (collisionType != null && collisionType.IsRange)
            {
                return collisionType.GetComponent<RangeType>();
            }
            return null;
        }

        public CollisionType GetCollisionType()
        {
            return this.associatedCollisionType;
        }

        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            this.RangeTypeObjectRef.OnRangeTriggerEnter(collisionType);
        }

        private void OnTriggerExit(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            this.RangeTypeObjectRef.OnRangeTriggerExit(collisionType);
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

        public bool IsInsideAndNotOccluded(BoxCollider boxCollider,bool forceObstacleOcclusionIfNecessary)
        {
            return this.RangeTypeObjectRef.IsInsideAndNotOccluded(boxCollider, forceObstacleOcclusionIfNecessary);
        }

        #endregion
    }

}
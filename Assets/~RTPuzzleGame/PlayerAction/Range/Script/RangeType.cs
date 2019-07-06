using UnityEngine;
using System.Collections;
using GameConfigurationID;

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

        #region Internal Managers
        protected ObstacleListener obstacleListener;
        #endregion

        public RangeTypeID RangeTypeID { get => rangeTypeID; set => rangeTypeID = value; }
        public ObstacleListener ObstacleListener { get => obstacleListener;  }

        public virtual void Init()
        {
            this.rangeTypeContainer = GameObject.FindObjectOfType<RangeTypeContainer>();
            this.rangeTypeInherentConfigurationData = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().RangeTypeConfiguration()[this.rangeTypeID];
            rangeTypeContainer.AddRange(this);
            this.obstacleListener = GetComponent<ObstacleListener>();
        }

        private void OnDestroy()
        {
            rangeTypeContainer.RemoveRange(this);
        }

        public virtual void Tick(float d) { }

        public abstract bool IsInside(Vector3 worldPointComparison);
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
        

        #region Logical conditions
        public bool IsInRangeEffectEnabled()
        {
            return this.IsRangeConfigurationDefined() && this.rangeTypeInherentConfigurationData.InRangeEffectMaterial != null;
        }
        public bool IsRangeConfigurationDefined()
        {
            return this.rangeTypeInherentConfigurationData != null;
        }
        public bool IsOccludedByFrustum()
        {
            return this.obstacleListener != null && this.obstacleListener.IsListenerHaveObstaclesNearby();
        }
        #endregion
    }

}
using GameConfigurationID;
using System;
using UnityEngine;

namespace RTPuzzle
{
    public class RangeTypeObject : MonoBehaviour
    {
        #region Modules
        private RangeType rangeType;
        private RangeObstacleListener rangeObstacleListener;
        #endregion

        #region Data Retrieval
        public RangeType RangeType { get => rangeType; }
        public RangeObstacleListener RangeObstacleListener { get => rangeObstacleListener; }
        #endregion

        #region External Dependencies 
        protected RangeTypeObjectContainer rangeTypeContainer;
        #endregion

        public void Init(RangeTypeObjectInitializer RangeTypeObjectInitializer)
        {
            #region External Dependencies
            this.rangeTypeContainer = GameObject.FindObjectOfType<RangeTypeObjectContainer>();
            #endregion

            this.PopulateModules();

            this.rangeType.Init(RangeTypeObjectInitializer, this);
            if (this.rangeObstacleListener != null)
            {
                this.rangeObstacleListener.Init(this.rangeType);
            }

            this.rangeTypeContainer.AddRange(this);
        }

        private void PopulateModules()
        {
            #region Modules
            this.rangeType = GetComponentInChildren<RangeType>();
            this.rangeObstacleListener = GetComponentInChildren<RangeObstacleListener>();
            #endregion
        }

        public void Tick(float d)
        {
            this.rangeType.Tick(d);
        }

        #region Internal Events
        public void OnRangeTriggerEnter(Collider other)
        {
            if (this.rangeObstacleListener != null)
            {
                this.rangeObstacleListener.OnRangeTriggerEnter(other);
            }
        }
        public void OnRangeTriggerExit(Collider other)
        {
            if (this.rangeObstacleListener != null)
            {
                this.rangeObstacleListener.OnRangeTriggerExit(other);
            }
        }
        #endregion

        #region Data Setter
        public void SetRangeID(RangeTypeID rangeTypeID)
        {
            this.rangeType.RangeTypeID = rangeTypeID;
        }
        #endregion

        public static RangeTypeObject Instanciate(RangeTypeID rangeTypeID, float sphereRadius, Func<Vector3> originPositionProvider = null, Func<Color> rangeColorProvider = null)
        {
            var rangeTypeContainer = GameObject.FindObjectOfType<RangeTypeObjectContainer>();
            var sphereRangeTypeObject = MonoBehaviour.Instantiate(PrefabContainer.Instance.BaseSphereRangePrefab, rangeTypeContainer.transform);
            sphereRangeTypeObject.PopulateModules();
            sphereRangeTypeObject.SetRangeID(rangeTypeID);
            sphereRangeTypeObject.Init(new RangeTypeObjectInitializer(sphereRadius, originPositionProvider, rangeColorProvider));
            return sphereRangeTypeObject;
        }

        private void OnDestroy()
        {
            this.rangeTypeContainer.RemoveRange(this);
        }

        #region Logical Conditions
        public bool IsOccludedByFrustum()
        {
            return this.rangeObstacleListener != null && this.rangeObstacleListener.ObstacleListener.IsListenerHaveObstaclesNearby();
        }
        public bool IsRangeConfigurationDefined()
        {
            return this.rangeType.IsRangeConfigurationDefined();
        }
        public bool IsInside(Vector3 worldPointComparison)
        {
            bool isInsideRange = this.rangeType.IsInside(worldPointComparison);
            if (this.rangeObstacleListener != null)
            {
                isInsideRange = isInsideRange && !this.rangeObstacleListener.IsPointOccludedByObstacles(worldPointComparison);
            }
            return isInsideRange;
        }
        #endregion

    }

    public class RangeTypeObjectInitializer
    {
        public float SphereRadius;
        public Func<Vector3> OriginPositionProvider;
        public Func<Color> RangeColorProvider;

        public RangeTypeObjectInitializer(float sphereRadius = 0f, Func<Vector3> originPositionProvider = null, Func<Color> rangeColorProvider = null)
        {
            SphereRadius = sphereRadius;
            OriginPositionProvider = originPositionProvider;
            RangeColorProvider = rangeColorProvider;
        }
    }
}
